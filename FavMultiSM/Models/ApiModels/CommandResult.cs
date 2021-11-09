using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models.ApiModels
{
    public class CommandResult
    {
        public ReSendMessage Message { get; set; } 
        public bool Executed { get; set; }
    }
}
