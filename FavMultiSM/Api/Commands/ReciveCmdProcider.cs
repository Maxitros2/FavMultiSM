using FavMultiSM.Api.Users.Security;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Commands
{
    public class ReciveCmdProcider : CommandBase, ITrancientService
    {
        public ReciveCmdProcider(TokenManager manager, IConfiguration configuration) : base(new List<string>() { "active", "активировать", "а", "a" })
        {
            Manager = manager;
            Configuration = configuration;           
        }

        public TokenManager Manager { get; }
        public IConfiguration Configuration { get; }        

        public async override Task<CommandResult> ProcideCmd(ReSendMessage message, UserAccountData user)
        {
            long? userId = await Manager.CheckChellange(message.Text, GetDataByUser(user), user, user.CurrentSocial);
            if (userId != null)
            {

                return new CommandResult()
                {
                    Message = new ReSendMessage() { Text = Configuration["lang:code:success"] },
                    Executed = true
                };
            }
            else
            {
                return new CommandResult()
                {
                    Message = new ReSendMessage() { Text = Configuration["lang:code:unsuccess"] },
                    Executed = false
                };
            }
        }
        string GetDataByUser(UserAccountData data)
        {
            switch (data.CurrentSocial)
            {
                case Socials.SocialEnum.VK:
                    return data.VKId;                    
                case Socials.SocialEnum.Telegram:
                    return data.TelegtamId;
                case Socials.SocialEnum.Instagram:
                    return data.InstagramId;
            }
            return String.Empty;
        }
    }
}