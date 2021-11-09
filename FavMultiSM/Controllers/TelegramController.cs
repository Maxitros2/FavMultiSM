using FavMultiSM.Api.Telegram;
using FavMultiSM.Api.Users;
using FavMultiSM.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
                case Telegram.Bot.Types.Enums.MessageType.Photo: 
                    resendMsg.Text = message.Text;
                    var stream = await (await TelegramBot.GetBotClientAsync()).GetFileAsync(message.Photo.Last().FileId);
                    using (var saveImageStream = new FileStream(stream.FilePath, FileMode.Create))
                    {
                        await(await TelegramBot.GetBotClientAsync()).DownloadFileAsync(stream.FilePath, saveImageStream);
                    }                              
                    resendMsg.Attachments = new List<string>() { stream.FilePath }; break;
            }
            if(message.Text == null)
            {
                resendMsg.Text = "";
            }
            await MessageProceeder.ProceedTelegramMessage(resendMsg, message.Chat.Id.ToString());
            return Ok();
        }
    }
}
