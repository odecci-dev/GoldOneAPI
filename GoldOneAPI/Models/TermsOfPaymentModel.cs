using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class TermsOfPaymentModel
    {
        public string? TopId { get; set; }

        public string? NameOfTerms { get; set; }
        public decimal? InterestRate { get; set; }
        public string? InterestType { get; set; }
        public int? Status { get; set; }
        public string? LoanTypeID { get; set; }
        public string? Formula { get; set; }
        public string? InterestApplied { get; set; }
        public int? Terms { get; set; }
        public int? OldFormula { get; set; }
        public int? NoAdvancePayment { get; set; }
        public string? NotarialFeeOrigin { get; set; }
        public decimal? LessThanNotarialAmount { get; set; }
        public int? LessThanAmountTYpe { get; set; }
        public decimal? GreaterThanEqualNotarialAmount { get; set; }
        public int? GreaterThanEqualAmountType { get; set; }
        public decimal? LoanInsuranceAmount { get; set; }
        public int? LoanInsuranceAmountType { get; set; }
        public decimal? LifeInsuranceAmount { get; set; }
        public int? LifeInsuranceAmountType { get; set; }
        public int? DeductInterest { get; set; }
        public int? CollectionTypeId { get; set; }

    }
}
