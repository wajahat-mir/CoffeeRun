using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public Size size { get; set; }
        public Options HowWouldYouLikeIt { get; set; }
        public uint Quantity { get; set; }
        public int RunId { get; set; }
        public virtual Run run { get; set; }
        [ForeignKey("OrderUser")]
        public string OrderUserId { get; set; }
        public virtual ApplicationUser OrderUser { get; set; }
    }

    public enum Size
    {
        Small,
        Medium, 
        Large,
        [Display(Name = "X-Large")]
        XLarge
    }

    public enum Options
    {
        Black,
        Regular,
        [Display(Name="Double-Double")]
        DoubleDouble,
        [Display(Name = "Triple-Triple")]
        TripleTriple
    }
}
