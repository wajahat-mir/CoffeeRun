using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.ViewModels
{
    [NotMapped]
    public class FriendViewModel
    {
        [Key]
        [ScaffoldColumn(false)]
        public int FriendId { get; set; }
        [Required]
        [Remote(action: "VerifyUserName", controller: "Friends", ErrorMessage = "User does not exist.")]
        [Display(Name="Your Friend's Username")]
        public string FriendUniqueName { get; set; }
        [Display(Name="Friend Confirmed")]
        public bool isConfirmed { get; set; }
    }
}
