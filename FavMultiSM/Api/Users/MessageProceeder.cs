using FavMultiSM.Api.Commands;
using FavMultiSM.Api.NSFW_Corrector;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Users
{
    public class MessageProceeder : ITrancientService
    {
        private readonly NSFWCorrectorMiddleware correctorMiddleware;

        public MessageProceeder(CommandsMiddleware commandsMiddleware, UserSender userSender, IConfiguration configuration, UserConverter userConverter, NSFWCorrectorMiddleware correctorMiddleware)
        {
            CommandsMiddleware = commandsMiddleware;
            UserSender = userSender;
            Configuration = configuration;
            UserConverter = userConverter;
            this.correctorMiddleware = correctorMiddleware;
        }

        CommandsMiddleware CommandsMiddleware { get; }
        UserSender UserSender { get; }
        IConfiguration Configuration { get; }
        UserConverter UserConverter { get; }
        public async Task<bool> ProceedTelegramMessage(ReSendMessage message, string name)
        {
            return await ProceedMessage(message, await UserConverter.ConvertTelegramUser(name));
        }
        public async Task<bool> ProceedVkMessage(ReSendMessage message, string peerId)
        {
            return await ProceedMessage(message, await UserConverter.ConvertVkUser(peerId));
        }

        public async Task<bool> ProceedInstagramMessage(ReSendMessage message, string userId)
        {
            return await ProceedMessage(message, await UserConverter.ConvertInstagramUser(userId));
        }
        private async Task<bool> ProceedMessage(ReSendMessage message, UserAccountData user)
        {
            if(user.IsNew)
            {
                var cmdresult = await CommandsMiddleware.ExecuteStart(message, user);
                if (cmdresult != null)
                {
                    return await UserSender.SendBackMessage(cmdresult.Message, user);
                }
            }
            if (message.Text.StartsWith("!")|| message.Text.StartsWith("/"))
            {
                var cmdresult = await CommandsMiddleware.TryToExecute(message, user);
                if(cmdresult!=null)
                {
                    return await UserSender.SendBackMessage(cmdresult.Message, user);
                }
                else
                {
                    return await UserSender.SendBackMessage(new ReSendMessage() {Text = Configuration["lang:cmd:wrongalias"] }, user);
                }
            }
            else
            {
                if(message.Attachments!=null)
                {
                    message.Attachments = await correctorMiddleware.GetAttachmentsAsync(message.Attachments);
                }
                return await UserSender.SendToOthersMessage(message, user);
            }
        }       
    }
}
