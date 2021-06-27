using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.DTO
{
    public class ReportTreasonRequest
    {
        [Required]
        public string Accuser { get; set; }

        [Required]
        public string Accused { get; set; }
    }
}
