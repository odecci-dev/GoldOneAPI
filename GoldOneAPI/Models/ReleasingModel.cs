using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class ReleasingModel
    {
        public string? LDID { get; set; }
        public string? MemId { get; set; }
        public string? Note { get; set; }
        public string? Approvedby { get; set; }
        public string? NAID { get; set; }
        public decimal? ApprovedReleasingAmount { get; set; }
        public decimal? ApprovedNotarialFee { get; set; }
        public decimal? ApprovedAdvancePayment { get; set; }
        public decimal? ApproveedInterest { get; set; }
        public decimal? ApprovedDailyAmountDue { get; set; }
        public decimal? LoanAmount { get; set; }
        public string? TopId { get; set; }
        public string? Courier { get; set; }
        public string? CourierName { get; set; }
        public string? CourierCno { get; set; }
        public string? ModeOfRelease { get; set; }
        public string? ModeOfReleaseReference { get; set; }
        public decimal? TotalSavingsUsed { get; set; }
    }
}
