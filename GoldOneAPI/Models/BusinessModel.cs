using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class BusinessModel
    {
        public string? BusinessName { get; set; }
        public string? BusinessType { get; set; }
        public string? BusinessAddress { get; set; }
        public int? B_status { get; set; }
        public int? YOB { get; set; }
        public int? NOE { get; set; }
        public decimal? Salary { get; set; }
        public decimal? VOS { get; set; }
        public decimal? AOS { get; set; }
        public List<FileModel>? BusinessFiles { get; set; }
    }
}
