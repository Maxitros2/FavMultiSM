using FavMultiSM.Api.Socials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models
{
    [Table("user_account_datas")]
    public class UserAccountData
    {
        public long Id { get; set; }
        public string TelegtamId { get; set; }
        public string InstagramId { get; set; }
        public string VKId { get; set; }
        [NotMapped]
        public SocialEnum CurrentSocial { get; set; }
        [NotMapped]
        public bool IsNew { get; set; }
    }
}
