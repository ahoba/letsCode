using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class ItemDTO
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public ItemDTO()
        {

        }

        public ItemDTO(Item item)
        {
            Name = item.Template.Name;
            Quantity = item.Quantity;
        }
    }
}
