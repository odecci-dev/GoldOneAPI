using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class ApplicationModel
    {
        public string? NAID { get; set; }
        public string? Remarks { get; set; }
        public string? CI_ApprovedBy { get; set; }
        public string? CI_ApprovalDate { get; set; }
        public string? ReleasingDate { get; set; }
        public string? DeclineDate { get; set; }
        public string? DeclinedBy { get; set; }
        public double? LoanAmount { get; set; }
        public string? App_ApprovedBy_1 { get; set; }
        public string? App_ApprovalDate_1 { get; set; }
        public string? App_ApprovedBy_2 { get; set; }
        public string? App_ApprovalDate_2 { get; set; }
        public string? App_Note { get; set; }
        public string? App_Notedby { get; set; }
        public string? App_NotedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? SubmittedBy { get; set; }
        public string? DateSubmitted { get; set; }
        public string? UserId { get; set; }
    }
}
