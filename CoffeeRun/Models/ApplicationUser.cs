using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Models
{
    public class ApplicationUser: IdentityUser
    {
        public virtual ICollection<Friend> FriendList { get; set; }

        public virtual ICollection<Run> CoffeeRuns { get; set; }
    }
}
