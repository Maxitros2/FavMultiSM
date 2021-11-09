using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.Attachments;

namespace FavMultiSM.Api.Socials
{
    public class VKSender : ISender, ITrancientService
    {
        public VKSender(IVkApi vkApi)
        {
            VkApi = vkApi;
        }

        IVkApi VkApi { get; }

        public async Task<bool> SendMessage(ReSendMessage message, UserAccountData data)
        {              
            await VkApi.Messages.SendAsync(new VkNet.Model.RequestParams.MessagesSendParams()
            {
                PeerId = Convert.ToInt64(data.VKId),
                RandomId = new DateTime().Millisecond,
                Message = message.Text,
                Attachments = message.Attachments.Select(x => new Photo() { PhotoSrc = new Uri(x) })
            });
            return true;
        }

        public Task<bool> SendMessage(ReSendMessage message, UserAccountData data, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
