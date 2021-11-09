using FavMultiSM.Api.Instagram;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Socials
{
    public class InstagramSender : ISender, ITrancientService
    {
        public InstagramSender(InstagramApi instagramApi)
        {
            InstagramApi = instagramApi;
        }

        InstagramApi InstagramApi { get; }

        public async Task<bool> SendMessage(ReSendMessage message, UserAccountData data)
        {
            if (InstagramApi.HasCode)
            {
                if (!String.IsNullOrEmpty(message.Text))
                    await (await InstagramApi.GetInstaApi()).MessagingProcessor.SendDirectTextAsync(data.InstagramId, null, message.Text);
                if (message.Attachments != null)
                    foreach (var media in message.Attachments)
                        await (await InstagramApi.GetInstaApi()).MessagingProcessor.SendDirectPhotoToRecipientsAsync(new InstaImage() { ImageBytes = await new WebClient().DownloadDataTaskAsync(new Uri(media)) }, data.InstagramId) ;
            }
            return true;
        }

        public Task<bool> SendMessage(ReSendMessage message, UserAccountData data, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
