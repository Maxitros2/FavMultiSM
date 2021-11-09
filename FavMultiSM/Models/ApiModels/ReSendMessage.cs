using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models.ApiModels
{
    public class ReSendMessage
    {
        public string Text { get; set; }
        public IEnumerable<string> Attachments { get; set; }
    }
}
