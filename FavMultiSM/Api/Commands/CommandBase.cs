using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Commands
{
    public abstract class CommandBase: ICommandProcider
    {        
        private IEnumerable<string> Aliases { get; set; }
        private int ArgsLength;
        public CommandBase(IEnumerable<string> aliases, int argsLength = 1)
        {
            Aliases = aliases;
            ArgsLength = argsLength;
        }      
        
        //mb use async?
        public virtual bool VaildateArgs(ReSendMessage message)
        {            
            var args = message.Text.Split(' ');
            var cmd = args.First().Remove(0,1);
            var len = args.Length;
            return Aliases.Contains(cmd) && len <= ArgsLength + 1;           
        }

        public abstract Task<CommandResult> ProcideCmd(ReSendMessage message, UserAccountData user);
        
    }
}
