using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Commands
{
    public interface ICommandProcider
    {
        abstract Task<CommandResult> ProcideCmd(ReSendMessage message, UserAccountData user);
        abstract bool VaildateArgs(ReSendMessage message);
    }
}
