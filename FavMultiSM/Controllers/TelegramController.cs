using FavMultiSM.Api.Telegram;
using FavMultiSM.Api.Users;
using FavMultiSM.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FavMultiSM.Controllers
{
    [Route("api/tg/callback")]
    public class TelegramController : ControllerBase, IReciver
    {
        public TelegramController(MessageProceeder messageProceeder, TelegramBot telegramBot)
        {
            MessageProceeder = messageProceeder;
            TelegramBot = telegramBot;            
        }

        MessageProceeder MessageProceeder { get; }
        TelegramBot TelegramBot { get; }

        [HttpPost]
        public async Task<OkResult> Post([FromBody] Update update)
        {
            if (update == null) return Ok();           
            var message = update.Message;
            var resendMsg = new ReSendMessage();
            switch (message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text: resendMsg.Text = message.Text;  break;
                case Telegram.Bot.Types.Enums.MessageType.Photo: resendMsg.Text = message.Text; resendMsg.Attachments = new List<string>() { (await (await TelegramBot.GetBotClientAsync()).GetFileAsync(message.Photo.Last().FileId)).FilePath }; break;
            }
            await MessageProceeder.ProceedTelegramMessage(resendMsg, message.Chat.Id.ToString());
            return Ok();
        }
    }
}
