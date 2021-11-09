using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models
{
    [Table("challenges")]
    public class Challenge
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationTime { get; set; }
    }   
}
