using FavMultiSM.Api.Users.Security;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Commands
{
    public class GetCodeCmd : CommandBase, ITrancientService
    {

        public GetCodeCmd(TokenManager manager, IConfiguration configuration) : base(new List<string>() { "code", "код", "k", "к" }, 0)
        {
            Manager = manager;
            Configuration = configuration;
        }

        TokenManager Manager { get; }
        IConfiguration Configuration { get; }

        public override async Task<CommandResult> ProcideCmd(ReSendMessage message, UserAccountData user)
        {
            return new CommandResult() 
            { Message = new ReSendMessage() { Text = Configuration["lang:code:usethis1"] +  await Manager.CreateChallange(user.Id) + Configuration["lang:code:usethis2"] }, 
                Executed = true };
        }
        
    }
}
