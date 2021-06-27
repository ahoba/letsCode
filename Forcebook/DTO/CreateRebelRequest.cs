using Forcebook.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class CreateRebelRequest
    {
        [Required]
        public string Name { get; set; }

        [Range(0, double.MaxValue)] // Only droids allowed with age == 0!
        public int? Age { get; set; }

        [Required]
        public Gender? Gender { get; set; }

        public Location? Location { get; set; }

        public IEnumerable<ItemDTO> Inventory { get; set; }

        public static Rebel FromRequest(CreateRebelRequest request)
        {
            Rebel rebel = new Rebel()
            {
                Name = request.Name,
                Age = request.Age.Value,
                Gender = request.Gender.Value,
                Location = request.Location.Value
            };

            return rebel;
        }
    }
}
