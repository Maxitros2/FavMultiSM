using FavMultiSM.Api.Socials;
using FavMultiSM.Models;
using FavMultiSM.Registration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Users
{
    public class UserCreatorBase : ITrancientService
    {
        AppDbContext AppContext { get; }
        public UserCreatorBase(AppDbContext appContext)
        {
            AppContext = appContext;
        }
        public async void AddUserData(SocialEnum socialType, string data, long userId)
        {
            var userData = await AppContext.UserAccountDatas.FindAsync(userId);
            addData(socialType, ref userData, data);           
            await AppContext.SaveChangesAsync();
        }
        public async Task<UserAccountData> InitUserData(SocialEnum socialType, string data)
        {            
            var userData = new UserAccountData();
            addData(socialType, ref userData, data);
            await AppContext.UserAccountDatas.AddAsync(userData);
            await AppContext.SaveChangesAsync();
            return userData;
        }
        void addData(SocialEnum socialType, ref UserAccountData userData, string data)
        {
            switch (socialType)
            {
                case SocialEnum.VK: userData.VKId = data; break;
                case SocialEnum.Instagram: userData.InstagramId = data; break;
                case SocialEnum.Telegram: userData.TelegtamId = data; break;
            }
        }
        
    }
}
