using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class RebelDTO
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public Location Base { get; set; }

        public IEnumerable<ItemDTO> Inventory { get; set; }

        public IEnumerable<string> ReportedBy { get; set; }

        public bool Traitor { get; set; }

        public RebelDTO(Rebel rebel)
        {
            Name = rebel.Name;
            Age = rebel.Age;
            Gender = rebel.Gender;
            Base = rebel.Location;
            Inventory = rebel.Inventory.Select(x => new ItemDTO(x));
            ReportedBy = rebel.TreasonReports.Select(x => x.AccuserId);
            Traitor = rebel.IsTraitor;
        }
    }
}
