using CoffeeRun.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.ViewModels
{
    [NotMapped]
    public class OrderViewModel
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Please select a Coffee size")]
        public Size size { get; set; }
        [Required(ErrorMessage = "Please let us know how you would like your coffee")]
        public Options HowWouldYouLikeIt { get; set; }
        [Required(ErrorMessage = "Please let us know how many coffee(s) you would like")]
        public uint Quantity { get; set; }
        [ScaffoldColumn(false)]
        public string OrderUserId { get; set; }
        public bool ableToModify { get; set; }
    }
}
