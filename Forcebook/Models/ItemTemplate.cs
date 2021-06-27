using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.Models
{
    public class ItemTemplate
    {
        [Key]
        public string Name { get; set; }

        [Required]
        public int Points { get; set; }
    }
}
