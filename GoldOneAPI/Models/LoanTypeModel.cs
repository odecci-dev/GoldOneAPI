using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class LoanTypeModel
    {
        
        public string? LoanTypeName { get; set; }
        public string? LoanTypeID { get; set; }
        public decimal? Savings { get; set; }
        public decimal? LoanAmount_Min { get; set; }
        public decimal? LoanAmount_Max { get; set; }
      
        public List<TermsOfPaymentModel> Terms { get; set; }



    }
}
