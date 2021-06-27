using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class UpdateLocationResponse
    {
        public string Name { get; set; }

        public Location Location { get; set; }

        public UpdateLocationResponse(Rebel rebel)
        {
            Name = rebel.Name;
            Location = rebel.Location;
        }
    }
}
