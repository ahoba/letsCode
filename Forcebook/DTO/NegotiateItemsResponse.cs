using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class NegotiateItemsResponse
    {
        [Required]
        public NegotiationLeg LegA { get; set; }

        [Required]
        public NegotiationLeg LegB { get; set; }
    }
}
