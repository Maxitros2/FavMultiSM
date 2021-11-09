using FavMultiSM.Api.Socials;
using FavMultiSM.Models;
using FavMultiSM.Registration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Users.Security
{
    public class TokenManager : ITrancientService
    {
        AppDbContext AppContext { get; }
        UserCreatorBase UserCreator { get; }
        public TokenManager(AppDbContext appContext, UserCreatorBase userCreator)
        {
            AppContext = appContext;
            UserCreator = userCreator;
        }
        //TODO Tests
        public async Task<long?> CheckChellange(string code, string data, UserAccountData dataToDelete, SocialEnum socialId)
        {
            var challenge = await AppContext.Challenges.FirstOrDefaultAsync(x => x.Code == code && DateTime.Now <= x.ExpirationTime);
            if (challenge != null)
            {
                if (dataToDelete.Id != challenge.UserId)
                    AppContext.UserAccountDatas.Remove(await AppContext.UserAccountDatas.FindAsync(dataToDelete.Id));
                else
                    return null;
                UserCreator.AddUserData(socialId, data, challenge.UserId);
                return challenge.UserId;
            }
            return null;                
        }
        public async Task<string> CreateChallange(long userId)
        {
            var record = new Challenge() { ExpirationTime = DateTime.Now.AddMinutes(10), UserId = userId };
            record.Code = generateToken().ToString();
            await AppContext.Challenges.AddAsync(record);
            await AppContext.SaveChangesAsync();
            return record.Code;
        }
        int generateToken()
        {
            Random r = new Random();
            return r.Next(10000, 99999);
        }
    }
}
