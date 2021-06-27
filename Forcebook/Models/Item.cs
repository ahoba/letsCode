using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.Models
{
    public class Item
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public Rebel Owner { get; set; }

        public string TemplateId { get; set; }

        public ItemTemplate Template { get; set; }

        public int Quantity { get; set; }
    }
}
