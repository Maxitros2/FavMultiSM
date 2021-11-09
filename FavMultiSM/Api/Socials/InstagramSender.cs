using FavMultiSM.Api.Instagram;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Socials
{
    public class InstagramSender : ISender, ITrancientService
    {
        public InstagramSender(InstagramApi instagramApi, IWebHostEnvironment hostEnvironment)
        {
            InstagramApi = instagramApi;
            HostEnvironment = hostEnvironment;
        }

        InstagramApi InstagramApi { get; }
        IWebHostEnvironment HostEnvironment { get; }

        public async Task<bool> SendMessage(ReSendMessage message, UserAccountData data)
        {
            if (InstagramApi.HasCode)
            {
                if (!String.IsNullOrEmpty(message.Text))
                    await (await InstagramApi.GetInstaApi()).MessagingProcessor.SendDirectTextAsync(null, data.InstagramId, message.Text);
                if (message.Attachments != null)
                {
                    string path = HostEnvironment.ContentRootPath;
                    foreach (var media in message.Attachments)
                    {
                        string filepath = Path.Combine(path, media.Split("/").Last());                        
                        await new WebClient().DownloadFileTaskAsync(new Uri(media), filepath);                        
                        var send = await (await InstagramApi.GetInstaApi()).MessagingProcessor.SendDirectDisappearingPhotoAsync(new InstaImage() { Uri = filepath, Width = 591, Height = 1280 }, InstagramApiSharp.Enums.InstaViewMode.Permanent, data.InstagramId);                        
                        if (send.Succeeded == false)
                        {
                            if (send.Info.ResponseType == ResponseType.ChallengeRequired)
                            {
                                var challengeData = await (await InstagramApi.GetInstaApi()).GetLoggedInChallengeDataInfoAsync();                    

                                var acceptChallenge = await (await InstagramApi.GetInstaApi()).AcceptChallengeAsync();
                                var send2 = await(await InstagramApi.GetInstaApi()).MessagingProcessor.SendDirectDisappearingPhotoAsync(new InstaImage() { Uri = filepath, Width = 591, Height = 1280  }, InstagramApiSharp.Enums.InstaViewMode.Permanent, data.InstagramId);
                            }
                        }
                        File.Delete(filepath);
                    }
                }
            }
            return true;
        }

        public Task<bool> SendMessage(ReSendMessage message, UserAccountData data, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
