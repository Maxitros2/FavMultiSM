using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models
{
    [Table("user_pictures")]
    public class UserPicture
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Href { get; set; }
    }
}
