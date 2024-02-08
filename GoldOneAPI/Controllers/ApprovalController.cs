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
using static GoldOneAPI.Controllers.HolidayController;
using GoldOneAPI.Manager;
using System.Text.Json;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        public class LoanDetailsVM2
        {
            public string? MemId { get; set; }
            public string? DateCreated { get; set; }
            public string? DateApproval { get; set; }
            public string? Remarks { get; set; }
            public string? NAID { get; set; }
            public string? Status { get; set; }
            public string? LoanAmount { get; set; }
            public string? No_of_Loans { get; set; }
            public string? No_of_Payment { get; set; }
            public string? LoanType { get; set; }
            public string? TermsOfPayment { get; set; }
            public string? Purpose { get; set; }
            public string? LDID { get; set; }
            public string? RefNo { get; set; }

        }
        public class LoanDetailsUpdateModel
        {
            public decimal? ApprovedLoanAmount { get; set; }
            public string? TopId { get; set; }
            public string? LDID { get; set; }
            public string? UserId { get; set; }
        }
        public class DeclineLoan
        {
            public string? LDID { get; set; }
            public string? Remarks { get; set; }
            public string? DeclinedBy { get; set; }
            public string? NAID { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> LoanDetailsApproval()
        {
            try
            {
                var result = dbmet.GetLoanDetailsApproval().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        [HttpGet]
        public async Task<IActionResult> getTermsListByLoanType(string loantypeid)
        {
            try
            {
                var result = dbmet.getTermsList().Where(a=>a.LoanTypeId == loantypeid).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeLoanAmount(LoanDetailsUpdateModel data)
        {
            string result = "";
            try
            {
                string Update = "";

              
                sql = $@"select * from tbl_LoanDetails_Model inner JOIN
                     tbl_LoanType_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID where LDID ='" + data.LDID + "'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Update = $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [ApprovedLoanAmount] = '" + data.ApprovedLoanAmount + "' ," +
                               " [ApprovedLoanBy] = '" + data.UserId + "' ," +
                               "[ApprovedTermsOfPayment] = '" + data.TopId + "' " +
                             "WHERE  LDID ='" + data.LDID + "'";


                    results = db.AUIDB_WithParam(Update) + " Updated";
                    return Ok(results);

                 
                }
                else
                {
                    return BadRequest("Invalid");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }


        [HttpPost]
        public async Task<IActionResult> DeclineLoans(DeclineLoan data)
        {
            string result = "";
            try
            {
                string Update = "";
                string filePath = @"C:\data\decline-approval.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"select MemId from tbl_LoanDetails_Model where LDID ='" + data.LDID + "'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [Status] = '11', " +
                               "[DeclineDate] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                               "[DeclinedBy] ='" + data.DeclinedBy + "' " +
                             "WHERE  LDID ='" + data.LDID + "'";



                    Update += $@"UPDATE [dbo].[tbl_Member_Model]
                               SET [Status] = '2'" +
                      " where MemId ='" + table1.Rows[0]["MemId"].ToString() + "'";

                    Update += $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [Status] = '11' ,  " +
                                  " Remarks='" + data.Remarks + "'" +
                      " where NAID ='" + data.NAID + "'";
                    string results = db.AUIDB_WithParam(Update) + " Updated";
                    string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                    DataTable username_tbl = db.SelectDb(username).Tables[0];
                    foreach (DataRow dr in username_tbl.Rows)
                    {
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        dbmet.InsertNotification("Declined Application  " + data.NAID + " ",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Approval Module", name, dr["UserId"].ToString(), "2" , data.NAID);
                    }
                    return Ok(results);


                
                }
                else
                {
                    return BadRequest("Invalid");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
      
        }
        [HttpPost]
        public async Task<IActionResult> ApproveReleasing(ReleasingModel data)
        {
            string result = "";
            try
            {
                string Update = "";
                string filePath = @"C:\data\approvereleasing.json"; // Replace with your desired file path

                //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));
                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"select * from tbl_LoanDetails_Model where LDID ='" + data.LDID + "'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    var amount_result = dbmet.LoanSummaryRecompute(data.NAID, data.LoanAmount.ToString(), "").FirstOrDefault();

                    //string username = $@"SELECT  Fname,Lname,Mname FROM [dbo].[tbl_User_Model] where UserId = '" + data.Approvedby + "'";
                    //DataTable username_tbl = db.SelectDb(username).Tables[0];
                    //string name = username_tbl.Rows[0]["Fname"].ToString() + " " + username_tbl.Rows[0]["Mname"].ToString() + " " + username_tbl.Rows[0]["Lname"].ToString();
                    //dbmet.InsertNotification("Approved Application For Releasing " + data.NAID + "",
                    //      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Approval Module", name, "2");
                    string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                    DataTable username_tbl = db.SelectDb(username).Tables[0];
                    foreach (DataRow dr in username_tbl.Rows)
                    {
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        dbmet.InsertNotification("Declined Application  " + data.NAID + " ",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Approval Module", name, dr["UserId"].ToString(), "2", data.NAID);
                    }
                    // check if approval_1 has already data if yes update approving_2
                    if (data.Note != "")
                    {
                        Update += $@"UPDATE [dbo].[tbl_Application_Model]
                               SET  [Status] = '10', " +
                                 "[App_Note] ='" + data.Note + "', " +
                                 "[App_Notedby] ='" + data.Approvedby + "', " +
                                 "[App_ApprovedBy_2] ='" + data.Approvedby + "' ," +
                                 "[App_NotedDate] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                 "[App_ApprovalDate_2] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                         " where NAID ='" + data.NAID + "'";

                        Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [Status] = '10' " +
                             "WHERE  LDID ='" + data.LDID + "'";

                        //var getformula = dbmet.GetLoanTypeDetails().Where(a => a.TermsofPayment[0].TopId == data.TopId).FirstOrDefault();
                    

                        Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [ApprovedReleasingAmount] = '" + data.ApprovedReleasingAmount + "' ," +
                                   " [ApprovedNotarialFee] = '" + data.ApprovedNotarialFee + "' ," +
                                   " [ApprovedLoanAmount] = '" + data.LoanAmount + "' ," +
                                   //" [LoanAmount] = '" + data.LoanAmount + "' ," +
                                   " [ApprovedAdvancePayment] = '" + data.ApprovedAdvancePayment + "' ," +
                                   " [ApproveedInterest] = '" + data.ApproveedInterest + "' ," +
                                   " [ApprovedDailyAmountDue] = '" + data.ApprovedDailyAmountDue + "' ," +
                                   " [ApprovedLoanBy] = '" + data.Approvedby + "' ," +
                                   "[ApprovedTermsOfPayment] = '" + data.TopId + "' " +
                                 "WHERE  LDID ='" + data.LDID + "'";


                        result = db.AUIDB_WithParam(Update) + " Updated";

                    
                    }
                    else
                    {
                        var validate = dbmet.GetApplicationList().Where(a => a.NAID == data.NAID).FirstOrDefault();
                        if (validate.App_ApprovalDate_1 != "" && validate.App_ApprovedBy_1 != "")
                        {
                            Update += $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [Status] = '10' ," +
                            "[App_ApprovedBy_2] ='" + data.Approvedby + "' ," +
                               "[App_ApprovalDate_2] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                            " where NAID ='" + data.NAID + "'";


                            //Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                            //   SET [ApprovedReleasingAmount] = '" + data.ApprovedReleasingAmount + "' ," +
                            //      " [ApprovedNotarialFee] = '" + data.ApprovedNotarialFee + "' ," +
                            //            " [ApprovedLoanAmount] = '" + amount_result.LoanAmount + "' ," +
                            //      " [ApprovedAdvancePayment] = '" + data.ApprovedAdvancePayment + "' ," +
                            //      " [ApproveedInterest] = '" + data.ApproveedInterest + "' ," +
                            //      " [ApprovedDailyAmountDue] = '" + data.ApprovedDailyAmountDue + "' ," +
                            //      " [ApprovedLoanBy] = '" + data.Approvedby + "' ," +
                            //      "[ApprovedTermsOfPayment] = '" + data.TopId + "' " +
                            //    "WHERE  LDID ='" + data.LDID + "'";
                            Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [ApprovedReleasingAmount] = '" + data.ApprovedReleasingAmount + "' ," +
                            " [ApprovedNotarialFee] = '" + data.ApprovedNotarialFee + "' ," +
                              " [ApprovedLoanAmount] = '" + data.LoanAmount + "' ," +
                                  //" [LoanAmount] = '" + data.LoanAmount + "' ," +
                            " [ApprovedAdvancePayment] = '" + data.ApprovedAdvancePayment + "' ," +
                            " [ApproveedInterest] = '" + data.ApproveedInterest + "' ," +
                            " [ApprovedDailyAmountDue] = '" + data.ApprovedDailyAmountDue + "' ," +
                            " [ApprovedLoanBy] = '" + data.Approvedby + "' ," +
                            "[ApprovedTermsOfPayment] = '" + data.TopId + "' " +
                          "WHERE  LDID ='" + data.LDID + "'";
                            result = db.AUIDB_WithParam(Update) + " Updated";
                        }
                        //
                        else
                        {
                           
                                Update += $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [App_ApprovedBy_1] ='" + data.Approvedby + "' ," +
                                   "[App_ApprovalDate_1] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                " where NAID ='" + data.NAID + "'";


                                Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [ApprovedReleasingAmount] = '" + data.ApprovedReleasingAmount + "' ," +
                                      " [ApprovedNotarialFee] = '" + data.ApprovedNotarialFee + "' ," +
                                " [ApprovedLoanAmount] = '" + data.LoanAmount + "' ," +
                                  //" [LoanAmount] = '" + data.LoanAmount + "' ," +
                                      " [ApprovedAdvancePayment] = '" + data.ApprovedAdvancePayment + "' ," +
                                      " [ApproveedInterest] = '" + data.ApproveedInterest + "' ," +
                                      " [ApprovedDailyAmountDue] = '" + data.ApprovedDailyAmountDue + "' ," +
                                      " [ApprovedLoanBy] = '" + data.Approvedby + "' ," +
                                      "[ApprovedTermsOfPayment] = '" + data.TopId + "' " +
                                    "WHERE  LDID ='" + data.LDID + "'";
                                result = db.AUIDB_WithParam(Update) + " Updated";
                          
    
                           // result = db.AUIDB_WithParam(Update) + " Updated";
                        }
             
                    }

                    
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Invalid");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }

        }

    }
}