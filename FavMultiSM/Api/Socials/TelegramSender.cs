using FavMultiSM.Api.Telegram;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace FavMultiSM.Api.Socials
{
    public class TelegramSender : ISender, ITrancientService
    {

        public TelegramSender(TelegramBot telegramBot)
        {
            TelegramBot = telegramBot;
        }

        TelegramBot TelegramBot { get; }

        public async Task<bool> SendMessage(ReSendMessage message, UserAccountData data)
        {
            var bot = await TelegramBot.GetBotClientAsync();
            if(!String.IsNullOrEmpty(message.Text))
                await bot.SendTextMessageAsync(new ChatId(Convert.ToInt64(data.TelegtamId)), message.Text);
            if (message.Attachments!=null)
                foreach(var attach in message.Attachments)
                    switch (attach.Type)
                    {
                        case AttachmentType.LocalPhoto:
                            using (var stream = System.IO.File.OpenRead(attach.Url))
                            {
                                await bot.SendPhotoAsync(new ChatId(Convert.ToInt64(data.TelegtamId)), new InputOnlineFile(stream));                                
                            }
                            await Task.Run(() => System.IO.File.Delete(attach.Url));
                            break;
                        case AttachmentType.WebPhoto:
                            await bot.SendPhotoAsync(new ChatId(Convert.ToInt64(data.TelegtamId)), new InputOnlineFile(attach.Url));
                            break;
                    }           
            return true;
        }

        public Task<bool> SendMessage(ReSendMessage message, UserAccountData data, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
