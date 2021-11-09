using FavMultiSM.Api.Socials;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Model;

namespace FavMultiSM.Api.Users
{
    public class UserSender : ITrancientService
    {
        public UserSender(VKSender vKSender, TelegramSender telegramSender, InstagramSender instagramSender, AppDbContext context)
        {
            VKSender = vKSender;
            TelegramSender = telegramSender;
            InstagramSender = instagramSender;
            DbContext = context;
            SenderDictionary = new Dictionary<SocialEnum, ISender>() { { SocialEnum.VK, VKSender }, { SocialEnum.Instagram, InstagramSender }, { SocialEnum.Telegram, TelegramSender } };
        }

        Dictionary<SocialEnum, ISender> SenderDictionary { get; }
        VKSender VKSender { get; }
        TelegramSender TelegramSender { get; }
        InstagramSender InstagramSender { get; }
        AppDbContext DbContext { get; }
        
        public async Task<bool> SendToOthersMessage(ReSendMessage message, UserAccountData data)
        {           
            foreach (var social in ((SocialEnum[])Enum.GetValues(typeof(SocialEnum))).Where(x => x != data.CurrentSocial))
            {
                if(data.HasSocial(social))
                {
                    await SenderDictionary[social].SendMessage(message, data);
                }
            }
            return true;
        }  
        public async Task<bool> SendBackMessage(ReSendMessage message, UserAccountData data)
        {
            return await SenderDictionary[data.CurrentSocial].SendMessage(message, data);
        }
    }
}
