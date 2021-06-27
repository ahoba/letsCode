using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forcebook.Models
{
    public class TreasonReport
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string AccuserId { get; set; }

        public Rebel Accuser { get; set; }

        [Required]
        public string AccusedId { get; set; }

        public Rebel Accused { get; set; }

        public int ReportCount { get; set; }
    }
}
