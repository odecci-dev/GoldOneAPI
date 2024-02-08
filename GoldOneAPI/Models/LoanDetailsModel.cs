using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class LoanDetailsModel
    {
        public string? LoanAmount { get; set; }
        public string? LoanType { get; set; }
        public string? TermsofPayment { get; set; }
        public string? Purpose { get; set; }
        public string? MemId { get; set; }
        public string? DateCreated { get; set; }
        public string? DateUpdated { get; set; }
        public string? Status { get; set; }
        public string? GroupId { get; set; }
        public string ? TermsOfPayment { get; set; }
        public string ? Days { get; set; }
        public string ? InterestRate { get; set; }
        public string ? InterestType { get; set; }
    }
}
