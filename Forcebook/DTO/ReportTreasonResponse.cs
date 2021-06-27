using Forcebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class ReportTreasonResponse
    {
        public string Name { get; set; }
        
        public IEnumerable<string> ReportedBy { get; set; }
        
        public bool IsTraitor { get; set; }

        public ReportTreasonResponse(Rebel rebel)
        {
            Name = rebel.Name;
            ReportedBy = rebel.TreasonReports.Select(x => x.AccuserId);
            IsTraitor = rebel.IsTraitor;
        }
    }
}
