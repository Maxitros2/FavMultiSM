using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models
{
    [Table("user_texts")]
    public class UserText
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
    }
}

