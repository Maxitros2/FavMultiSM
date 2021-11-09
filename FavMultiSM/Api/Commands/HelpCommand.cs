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
    public class HelpCommand : CommandBase, ITrancientService
    {
        public HelpCommand(IConfiguration configuration) : base(new List<string>() { "help", "помощь", "?", "h" }, 0)
        {
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }

        public override async Task<CommandResult> ProcideCmd(ReSendMessage message, UserAccountData user)
        {
            return new CommandResult()
            {
                Message = new ReSendMessage() { Text = Configuration["lang:help:helpmessage"] },
                Executed = true
            };
        }
    }
}
