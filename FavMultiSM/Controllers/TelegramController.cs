using FavMultiSM.Api.Telegram;
using FavMultiSM.Api.Users;
using FavMultiSM.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public TelegramController(MessageProceeder messageProceeder, TelegramBot telegramBot, IConfiguration configuration)
        {
            MessageProceeder = messageProceeder;
            TelegramBot = telegramBot;
            Configuration = configuration;
        }

        MessageProceeder MessageProceeder { get; }
        TelegramBot TelegramBot { get; }
        IConfiguration Configuration { get; }

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
                    var stream = (await (await TelegramBot.GetBotClientAsync()).GetFileAsync(message.Photo.Last().FileId)).FilePath;                                          
                    resendMsg.Attachments = new List<Attachment>() { new Attachment("https://api.telegram.org/file/bot"+Configuration["telegram:token"] +"/"+ stream)}; 
                    break;
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
