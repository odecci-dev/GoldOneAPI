using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class LoanHistoryModel
    {
       
        public int Id { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal Savings { get; set; }
        public decimal Penalty { get; set; }
        public decimal OutStandingBalance { get; set; }
        public DateTime DateReleased { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateOfFullPayment { get; set; }
        public string MemId { get; set; }
        public int NoPayment { get; set; }
        public string RefNo { get; set; }
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Lname { get; set; }
        public string Suffix { get; set; }
        
    }
}
