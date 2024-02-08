using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class PaymentModel
    {
        public string? Id { get; set; }
        public string? LoanAmount { get; set; }
        public string? OutStandingBalance { get; set; }
        public string? PaidAmount { get; set; }
        public string? Collector { get; set; }
        public string? PaymentDate { get; set; }
        public string? PaymentType { get; set; }
        public string? Penalty { get; set; }
        public string? DateCreated { get; set; }
        public string? MemId { get; set; }


    }
}
