using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class JOModel
    {
        public int Id { get; set; }
        public string? JobDescription { get; set; }
        public string? YOS { get; set; }
        public decimal? MonthlySalary { get; set; }
        public string? OtherSOC { get; set; }
        public int? BO_Status { get; set; }
        public string? MemId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyID { get; set; }
        public string? Status { get; set; }
        public string? Emp_Status { get; set; }
    }
}
