using FavMultiSM.Api.Users;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using InstagramApiSharp;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Instagram
{
    public class InstagramChecker : BackgroundService, IHostedServiceDI
    {
        private readonly ILogger<InstagramChecker> _logger;
        

        public InstagramChecker(ILogger<InstagramChecker> logger, InstagramApi instagramApi, IServiceProvider services)
        {
            _logger = logger;
            InstagramApi = instagramApi;
            Services = services;
        }
        IServiceProvider Services { get; }
        InstagramApi InstagramApi { get; }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
           
            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {          

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriod task doing background work.");
                var instaApi = await InstagramApi.GetInstaApi();
                if (InstagramApi.HasCode && !InstagramApi.IsBusy)
                {                    
                    var pendingDirect = await instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
                    if(pendingDirect.Value!=null && pendingDirect.Value.Inbox.Threads!=null)
                        await ProceedMessages(pendingDirect.Value.Inbox.Threads);
                }
                await Task.Delay(10000, stoppingToken);
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
        }
        private async Task<bool> ProceedMessages(List<InstagramApiSharp.Classes.Models.InstaDirectInboxThread> threads)
        {
            foreach(var thread in threads)
            {
                if(!thread.IsGroup)
                {
                    foreach(var message in thread.Items)
                    {
                        if (message.UserId == InstagramApi.CurrentUserId)
                            break;
                        ReSendMessage sendMessage = null;                       
                        switch (message.ItemType)
                        {
                            case InstaDirectThreadItemType.Text:
                                sendMessage = new Models.ApiModels.ReSendMessage() { Text = message.Text }; break;
                            case InstaDirectThreadItemType.Media:
                                sendMessage = new Models.ApiModels.ReSendMessage() { Attachments = message.Media.Images.Where(x=>!Regex.Match(x.Uri, "[p,s][0-9]{1,}[x][0-9]{1,}").Success).Select(x => new Attachment(new StringBuilder(x.Uri).ToString())) }; break;
                            case InstaDirectThreadItemType.MediaShare:
                                sendMessage = new Models.ApiModels.ReSendMessage() { Attachments = message.MediaShare.Images.Select(x => new Attachment(new StringBuilder(x.Uri).ToString())) }; break;
                        }
                        if (sendMessage != null)
                        {
                            if (sendMessage.Text == null)
                                sendMessage.Text = "";
                            using (var scope = Services.CreateScope())
                            {
                                var messageProceeder =
                                    scope.ServiceProvider
                                        .GetRequiredService<MessageProceeder>();

                                await messageProceeder.ProceedInstagramMessage(sendMessage, thread.ThreadId);
                            }
                        }
                        await (await InstagramApi.GetInstaApi()).MessagingProcessor.MarkDirectThreadAsSeenAsync(thread.ThreadId, message.ItemId);
                        await (await InstagramApi.GetInstaApi()).MessagingProcessor.DeleteSelfMessageAsync(thread.ThreadId, message.ItemId);
                    }                    
                }
               
            }
            return true;
        }
    
}
}
