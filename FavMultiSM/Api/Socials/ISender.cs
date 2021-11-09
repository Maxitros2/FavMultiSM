using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Socials
{
    interface ISender
    {
        Task<bool> SendMessage(ReSendMessage message, UserAccountData data);
        Task<bool> SendMessage(ReSendMessage message, UserAccountData data, DateTime start, DateTime end);
    }
}
