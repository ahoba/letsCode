using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.Models
{
    public struct Location
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public long? Latitude { get; set; }

        [Required]
        public long? Longitude { get; set; }
    }
}
