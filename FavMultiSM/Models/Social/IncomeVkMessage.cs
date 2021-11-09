using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models
{
    [Serializable]
    public class IncomeVkMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("object")]
        public JObject Object { get; set; }
        [JsonProperty("group_id")]
        public long GroupId { get; set; }
    }
}
