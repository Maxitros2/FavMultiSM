using FavMultiSM.Models;
using FavMultiSM.Registration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Users
{
    public class UserConverter : ITrancientService
    {
        public UserConverter(AppDbContext context, UserCreatorBase creatorBase)
        {
            DbContext = context;
            CreatorBase = creatorBase;
        }
        AppDbContext DbContext { get; }
        public UserCreatorBase CreatorBase { get; }
        public async Task<UserAccountData> ConvertTelegramUser(string name)
        {
            var user = await DbContext.UserAccountDatas.FirstOrDefaultAsync(x => x.TelegtamId == name);
            if (user == null)
            {
                return (await CreatorBase.InitUserData(Socials.SocialEnum.Telegram, name)).SetSocialEnum(Socials.SocialEnum.Telegram).SetNew();
            }
            else
            {
                return user.SetSocialEnum(Socials.SocialEnum.Telegram).SetNotNew();
            }
        }
        public async Task<UserAccountData> ConvertVkUser(string peerId)
        {
            var user = await DbContext.UserAccountDatas.FirstOrDefaultAsync(x => x.VKId == peerId);
            if(user == null)
            {
                return (await CreatorBase.InitUserData(Socials.SocialEnum.VK, peerId)).SetSocialEnum(Socials.SocialEnum.VK).SetNew();
            }   
            else
            {
                return user.SetSocialEnum(Socials.SocialEnum.VK).SetNotNew();
            }
        }
        public async Task<UserAccountData> ConvertInstagramUser(string userId)
        {
            try
            {
                var user = await DbContext.UserAccountDatas.FirstOrDefaultAsync(x => x.InstagramId == userId);
                if (user == null)
                {
                    return (await CreatorBase.InitUserData(Socials.SocialEnum.Instagram, userId)).SetSocialEnum(Socials.SocialEnum.Instagram).SetNew();
                }
                else
                {
                    return user.SetSocialEnum(Socials.SocialEnum.Instagram).SetNotNew();
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
