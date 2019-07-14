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
    public class DashboardViewModel
    {
        [Key]
        [ScaffoldColumn(false)]
        public int RunId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TimeToRun { get; set; }
        [EnumDataType(typeof(RunStatus))]
        public RunStatus runStatus { get; set; }
        public string Runner { get; set; }
    }
}
