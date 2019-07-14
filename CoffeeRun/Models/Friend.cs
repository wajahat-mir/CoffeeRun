using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Models
{
    public class Friend
    {
        public int FriendId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string FriendUserId { get; set; }
        public string FriendUniqueName { get; set; }
        public bool isConfirmed { get; set; }
    }
}
