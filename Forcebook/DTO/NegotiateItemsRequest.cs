using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class NegotiateItemsRequest
    {
        [Required]
        public NegotiationLeg LegA { get; set; }

        [Required]
        public NegotiationLeg LegB { get; set; }
    }

    public struct NegotiationLeg
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IEnumerable<ItemDTO> Items { get; set; }
    }
}
