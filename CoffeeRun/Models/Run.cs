using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Models
{
    public class Run
    {
        public int RunId {get; set;}
        public string Name { get; set; }
        public DateTime TimeToRun { get; set; }
        public string Runner { get; set; }
        public RunStatus runStatus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        [ForeignKey("Owner")]
        public string OwnerUserId { get; set; }
        public ApplicationUser Owner { get; set; }
    }

    public enum RunStatus
    { 
        [Display(Name="Need a Runner")]
        NoRunner,
        [Display(Name="No Runner :(")]
        Expired,
        [Display(Name="Death by Delay")]
        Death,
        [Display(Name="Prepped to Run")]
        Prepped,
        [Display(Name="On The Run")]
        OnTheRun,
        Delayed,
        [Display(Name="Complete :)")]
        Complete
    }

}
