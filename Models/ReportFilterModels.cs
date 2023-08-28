using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Models
{
    public class LedgerFilter
    {
        public int? AccountId { get; set; } 
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        public string? CommandType { get; set; }
        public int? Id { get; set; }
    }
}
