using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class TradeItemsRequest
    {
        [Required]
        public string Rebel1Name { get; set; }

        [Required]
        public IEnumerable<Item> Rebel1Offer { get; set; }

        [Required]
        public string Rebel2Name { get; set; }

        [Required]
        public IEnumerable<Item> Rebel2Offer { get; set; }
    }
}
