using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Text;
using static GoldOneAPI.Controllers.MemberController;
using static GoldOneAPI.Controllers.FieldOfficerController;
using System.Data.SqlClient;
using GoldOneAPI.Manager;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoanSummaryController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        public class LoanSummaryVM
        {
            public string Fname { get; set; }
            public string Mname { get; set; }
            public string Lname { get; set; }
            public string Suffix { get; set; }
            public string Cno { get; set; }
            public string Borrower { get; set; }
            public string Profile_Pic { get; set; }
            public string AreaName { get; set; }
            public string AreaID { get; set; }
            public string City { get; set; }
            public string Date { get; set; }
            public string DueDate { get; set; }
            public string NAID { get; set; }
            public string MemId { get; set; }
            public string? Days { get; set; }
            public string? PrincipalLoan { get; set; }
            public string? LoanAmount { get; set; }
            public string? LoanAmount_Min { get; set; }
            public string? InterestRate { get; set; }
            public string? Total_InterestAmount { get; set; }
            public string? DailyCollectibles { get; set; }
            public string? AdvancePayment { get; set; }
            //public string AmountReceived { get; set; }
            //public string InterestType { get; set; }
            public string NotarialFee { get; set; }
            public string LoanInsurance { get; set; }
            //public string Total_LoanInsuraneAmount { get; set; }
            public string Total_LoanReceivable { get; set; }
            public string LoanI_Type { get; set; }
            public string LifeInsurance { get; set; }
            public string Savings { get; set; }
            public string TotalSavingsAmount { get; set; }
            public string TotalSavingUsed { get; set; }
            public string LoanBalance { get; set; }
            //public string LifeInsuranceType { get; set; }

            //public string OldFormula { get; set; }
            public string NoAdvancePayment { get; set; }
            //public string InterestApplied { get; set; }
            //public string DeductInterest { get; set; }
            public string TypeOfCollection { get; set; }
            public string CollectionLapses { get; set; }

            public string HolidayAmount { get; set; }
            public string HolidayDaysCount { get; set; }
            public string DayOfHoliday { get; set; }
            public string DateOfHoliday { get; set; }


            public string CI_ApprovedBy { get; set; }
            public string CI_ApprovedBy_UserId { get; set; }
            public string CI_ApprovalDate { get; set; }
            public string DeclinedBy { get; set; }
            public string DeclinedBy_UserId { get; set; }
            public string DeclineDate { get; set; }
            public string App_ApprovedBy_1 { get; set; }
            public string App_ApprovedBy_1_UserId { get; set; }
            public string App_ApprovalDate_1 { get; set; }
            public string App_ApprovedBy_2 { get; set; }
            public string App_ApprovedBy_2_UserId { get; set; }
            public string App_ApprovalDate_2 { get; set; }
            public string App_Notedby { get; set; }
            public string App_Notedby_UserId { get; set; }
            public string App_NotedDate { get; set; }
            public string CreatedBy { get; set; }
            public string CreatedBy_UserId { get; set; }
            public string DateCreated { get; set; }
            public string SubmittedBy { get; set; }
            public string SubmittedBy_UserId { get; set; }
            public string DateSubmitted { get; set; }
            public string ReleasedBy { get; set; }
            public string ReleasedBy_UserId { get; set; }
            public string ReleasingDate { get; set; }
            //public string Formula { get; set; }
            public string APFID { get; set; }
            //public string Loan_amount_GreaterEqual_Value { get; set; }
            //public string Loan_amount_GreaterEqual { get; set; }
            //public string LAGEF_Type { get; set; }
            //public string Loan_amount_Lessthan_Value { get; set; }
            //public string Loan_amount_Lessthan { get; set; }
            //public string LALV_Type { get; set; }
            public string Co_Fname { get; set; }
            public string Co_Mname { get; set; }
            public string Co_Lname { get; set; }
            public string Co_Borrower { get; set; }
            public string Co_Suffix { get; set; }
            public string Co_Cno { get; set; }

            public string ApprovedNotarialFee { get; set; }
            public string ApprovedAdvancePayment { get; set; }
            public string ApproveedInterest { get; set; }
            public string ApprovedReleasingAmount { get; set; }
            public string ApprovedDailyAmountDue { get; set; }
            public string ApprovedTermsOfPayment { get; set; }
            public string ApprovedLoanAmount { get; set; }
            public string ModeOfRelease { get; set; }
            public string ModeOfReleaseReference { get; set; }
            public string Status { get; set; }
            public string Penalty { get; set; }
            public string DeductInterest { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> GetLoanSummary(string naid)
        {

            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetLoanSummary(naid).ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLoanBalance()
        {
            try
            {

                string sql = "";
                string areafilter = $@"select NAID,MemId from tbl_Application_Model where status = 14";
                DataTable area_table = db.SelectDb(areafilter).Tables[0];
                foreach (DataRow dr in area_table.Rows)
                {
                    var loan_bal = dbmet.GetLoanSummary(dr["NAID"].ToString()).FirstOrDefault();
                    sql += $@"update tbl_LoanHistory_Model set OutstandingBalance ='" + loan_bal.LoanBalance + "' where MemId='" + loan_bal.MemId + "'";

                }
                return Ok(db.AUIDB_WithParam(sql));
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLoanSummaryComputation(string naid, string? loanamount, string? loantype)
        {


            //var result = new List<CreditModel>();
            try
            {

                return Ok(dbmet.LoanSummaryRecompute(naid, loanamount, loantype).ToList());
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

    }
}