using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Commands
{
    public class CommandsMiddleware : ITrancientService
    {
        public CommandsMiddleware(GetCodeCmd getCodeCmd, ReciveCmdProcider reciveCmd, StartCommand startCommand, HelpCommand helpCommand )
        {
            Commands = new List<CommandBase>() { getCodeCmd, reciveCmd, startCommand, helpCommand };
            StartCommand = startCommand;
        }

        IEnumerable<CommandBase> Commands { get; }
        CommandBase StartCommand { get; }
        public async Task<CommandResult> TryToExecute(ReSendMessage message, UserAccountData user)
        {
            foreach(var cmd in Commands)
            {
                if(cmd.VaildateArgs(message))
                {
                    return await cmd.ProcideCmd(message, user);
                }
            }
            return null;
        }
        public async Task<CommandResult> ExecuteStart(ReSendMessage message, UserAccountData user)
        {
            return await StartCommand.ProcideCmd(message, user);
        }
    }
}
