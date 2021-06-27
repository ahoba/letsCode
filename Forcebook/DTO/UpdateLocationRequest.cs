using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class UpdateLocationRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Location Location { get; set; }
    }
}
