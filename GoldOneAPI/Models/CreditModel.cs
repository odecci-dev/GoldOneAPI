using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class CreditModel
    {
       
            public string DateCreated { get; set; }
            public string DateApproval { get; set; }
            public string CI_ApprovalDate { get; set; }
            public string ReleasingDate { get; set; }
            public string Remarks { get; set; }
            public string NAID { get; set; }
            public string CI_Status { get; set; }
            public string Fname { get; set; }
            public string Mname { get; set; }
            public string Lname { get; set; }
            public string Suffix { get; set; }
            public string Fullname { get; set; }
            public string Cno { get; set; }
            public string LoanAmount { get; set; }
            public string LoanTypeID { get; set; }
          
            public string LAL_Type { get; set; }
            public string Loan_amount_Lessthan { get; set; }
            public string LAG_Type { get; set; }
            public string Loan_amount_Greater { get; set; }
            public string LoanInsurance { get; set; }
            public string LoanI_Type { get; set; }
            public string LifeInsurance { get; set; }
            public string LifeI_Type { get; set; }
            public string NameOfTerms { get; set; }
            public string Days { get; set; }
            public string InterestRate { get; set; }
            public string IR_Type { get; set; }
            public string MemId { get; set; }
            public string LoanTypeName { get; set; }
            public string Formula { get; set; }
            public string InterestAmount { get; set; }
        
    }
}
