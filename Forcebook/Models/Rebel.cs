using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.Models
{
    public class Rebel
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public Location Location { get; set; }

        [Required]
        public virtual ICollection<Item> Inventory { get; } = new List<Item>();

        [Required]
        public virtual ICollection<TreasonReport> TreasonReports { get; set; } = new List<TreasonReport>();

        public bool IsTraitor => TreasonReports.Count() >= 3;
    }

    public enum Gender
    {
        Undefined = 0,
        Male = 1,
        Female = 2,
        Droid = 3,
        Other = 4
    }
}
