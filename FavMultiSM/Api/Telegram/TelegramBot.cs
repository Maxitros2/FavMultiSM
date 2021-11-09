using FavMultiSM.Controllers;
using FavMultiSM.Registration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FavMultiSM.Api.Telegram
{
    public class TelegramBot : ISingletoneService
    {
        private TelegramBotClient botClient;       

        public TelegramBot(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        IConfiguration Configuration { get; }        

        public async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }
            botClient = new TelegramBotClient(Configuration["telegram:token"]);
            string hook = string.Format(Configuration["telegram:url"], "api/tg/callback");
            await botClient.SetWebhookAsync(hook);            
            return botClient;
        }
    }
}
