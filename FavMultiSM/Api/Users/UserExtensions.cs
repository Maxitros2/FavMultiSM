using FavMultiSM.Api.Socials;
using FavMultiSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Users
{
    public static class UserExtensions
    {
        public static UserAccountData SetSocialEnum(this UserAccountData data, SocialEnum social)
        {
            data.CurrentSocial = social;
            return data;
        }
        public static bool HasSocial(this UserAccountData data, SocialEnum social)
        {
            switch (social)
            {
                case SocialEnum.VK:
                    return !String.IsNullOrEmpty(data.VKId);
                case SocialEnum.Telegram:
                    return !String.IsNullOrEmpty(data.TelegtamId);
                case SocialEnum.Instagram:
                    return !String.IsNullOrEmpty(data.InstagramId);
            }
            return false;
        }
        public static UserAccountData SetNew(this UserAccountData data)
        {
            data.IsNew = true;
            return data;
        }
        public static UserAccountData SetNotNew(this UserAccountData data)
        {
            data.IsNew = false;
            return data;
        }
    }
}
