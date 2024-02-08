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
using Microsoft.VisualBasic;
using GoldOneAPI.Manager;
using System.Text.Json;
using static GoldOneAPI.Controllers.FieldAreaController;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReleasingController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();

        [HttpPost]
        public async Task<IActionResult> ComppleteTransaction(ReleasingModel data)
        {
            try{
                string filePath = @"C:\data\complete.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                string result1 = "";
                string result = "";
                result1 += $@"UPDATE [dbo].[tbl_Application_Model]
                                  SET  [Status] = '14', " +
                    "  [ReleasingDate] = '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "', " +
                     "[ReleasedBy] ='" + data.Approvedby + "' " +
             " where NAID ='" + data.NAID + "'";
                result1 += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                                  SET [Status] = '14' " +
                                "WHERE  LDID ='" + data.LDID + "'";
                db.AUIDB_WithParam(result1);

                string savings = "";
                string duedate = "";
                string outstanding_bal = "";
                var loan_details = dbmet.GetLoanSummary(data.NAID).FirstOrDefault();
                //var payment_status = loan_details.AdvancePayment != "0" ? "2" : loan_details.AdvancePayment;
                //var payment_method = loan_details.AdvancePayment != "0" ? null : loan_details.AdvancePayment;
                //if (loan_details.ApprovedAdvancePayment != "" || loan_details.ApprovedAdvancePayment != "0")
                //{
                //    int adv_stats = 0;
                //    if (loan_details.ApprovedAdvancePayment != "0")
                //    {
                //        adv_stats = 1;
                //    }
                //   savings = loan_details.TotalSavingsAmount == "" ? "0.00" : loan_details.TotalSavingsAmount;
                //    duedate = loan_details.DueDate == "" ? null : loan_details.DueDate;
                //    string insert = $@"INSERT INTO [dbo].[tbl_Collection_AreaMember_Model]
                //                   ([NAID]
                //                   ,[AdvancePayment]
                //                   ,[LapsePayment]
                //                   ,[CollectedAmount]
                //                   ,[Savings]
                //                   ,[Payment_Status]
                //                   ,[Payment_Method]
                //                   ,[AdvanceStatus]
                //                   ,[DateCollected])
                //             VALUES
                //                   ('" + data.NAID + "'," +
                //                    "'" + loan_details.ApprovedAdvancePayment + "'," +
                //                   "'0'," +
                //                    "'" + loan_details.ApprovedAdvancePayment + "'," +
                //                   "'" + savings + "'," +
                //                    "'"+payment_status+"'," +
                //                   "'"+ payment_method + "'," +
                //                   "'"+ adv_stats + "'," +
                //               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                //    db.AUIDB_WithParam(insert);

                //}
                result += $@"INSERT INTO [dbo].[tbl_LoanHistory_Model]
                           ([LoanAmount]
                           ,[DateReleased]
                           ,[Savings]
                           ,[OutstandingBalance]
                           ,[UsedSavings]
                           ,[DueDate]
                           ,[DateCreated]
                           ,[MemId])
                            VALUES
                        ('" + loan_details.LoanBalance + "'," +
                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                          "'" + data.TotalSavingsUsed + "'," +
                          "'" + loan_details.LoanBalance + "'," +
                          "'" + data.TotalSavingsUsed+ "'," +
                          "'" + loan_details.DueDate + "'," +
                            "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                   "'" + loan_details.MemId + "') ";
                result = db.AUIDB_WithParam(result) + " Updated";


                if (data.TotalSavingsUsed != 0)
                {
                    double remaining_savings = 0;
                    string sql = $@"SELECT  [Id]
                              ,[MemId]
                              ,[TotalSavingsAmount]
                              ,[DateUpdated]
                              ,[UpdatedFrom]
                              ,[UpdateBy]
                          FROM [GoldOne].[dbo].[tbl_MemberSavings_Model]
                          WHERE MemId = '" + loan_details.MemId + "'";
                    DataTable get_memid = db.SelectDb(sql).Tables[0];
                    double get_total_savings_amount = double.Parse(get_memid.Rows[0]["TotalSavingsAmount"].ToString());
                    remaining_savings = get_total_savings_amount - double.Parse(data.TotalSavingsUsed.ToString());

                    string updatesavings = "";
                    updatesavings += $@"UPDATE [dbo].[tbl_Application_Model]
                                      SET  [Status] = '14', " +
                        "  [ReleasingDate] = '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "', " +
                         "[ReleasedBy] ='" + data.Approvedby + "' " +
                 " where NAID ='" + data.NAID + "'";
                    string updatedfrom = "Updated from Application " + data.NAID;
                    updatesavings += $@"UPDATE [dbo].[tbl_MemberSavings_Model]
                                      SET [TotalSavingsAmount] = '"+ remaining_savings + "', " +
                                            "  [DateUpdated] = '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "', " +
                                            "  [UpdateBy] = '" + data.Approvedby + "', " +
                                             "[UpdatedFrom] ='" + updatedfrom + "' " +
                                     "WHERE MemId = '" + loan_details.MemId + "'";
                    db.AUIDB_WithParam(updatesavings);

                }
                string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                DataTable username_tbl = db.SelectDb(username).Tables[0];
                foreach (DataRow dr in username_tbl.Rows)
                {
                    string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    dbmet.InsertNotification("Application Approved Complete  " + data.NAID + " ",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Releasing Module", name,dr["UserId"].ToString(), "2",data.NAID);
                }
                return Ok(result);

                //string username = $@"SELECT  Fname,Lname,Mname FROM [dbo].[tbl_User_Model] where UserId = '" + data.Approvedby + "'";
                //DataTable username_tbl = db.SelectDb(username).Tables[0];
                //string name = username_tbl.Rows[0]["Fname"].ToString() + " " + username_tbl.Rows[0]["Mname"].ToString() + " " + username_tbl.Rows[0]["Lname"].ToString();
                //dbmet.InsertNotification("Application Approved Complete" + data.NAID + "",
                //      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Releasing Module", name, "2");

             

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }

        [HttpPost]
        public async Task<IActionResult> ReleasingComplete(ReleasingModel data)
        {
            string result = "";
            string messageresult = "";
            try
            {
                string m_ref_no = "";
                string Update = "";
                string save_collect = "";
                int counter = 0;
                string filePath = @"C:\data\ApproveReleasing.json"; // Replace with your desired file path

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));



                save_collect += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [Status] = '15' ," +

                                       "  [ModeOfRelease] = '" + data.ModeOfRelease + "', " +
                                       "  [Courerier] = '" + data.Courier + "', " +
                                       "  [ModeOfReleaseReference] = '" + data.ModeOfReleaseReference + "', " +
                                               "  [CourerierName] = '" + data.CourierName + "', " +
                                                       "  [CourierCNo] = '" + data.CourierCno + "' " +
                                            "WHERE  LDID ='" + data.LDID + "'";
                    //application update
                    save_collect += $@"UPDATE [dbo].[tbl_Application_Model]
                               SET  [Status] = '15' ," +
                                 "[ReleasedBy] ='" + data.Approvedby+ "', " +
                                 "[ReleasingDate] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "' " +
                 " where NAID ='" + data.NAID + "'";

                result = db.AUIDB_WithParam(save_collect) + " Added";
                string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                DataTable username_tbl = db.SelectDb(username).Tables[0];
                foreach (DataRow dr in username_tbl.Rows)
                {
                    string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    dbmet.InsertNotification("Approved For Releasing   " + data.NAID + " ",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Releasing Module", name, dr["UserId"].ToString(), "2",data.NAID);
                }

                string Update_history = $@"
                                        UPDATE [dbo].[tbl_LoanHistory_Model]
                                           SET [UsedSavings] = '
                                        " + data.TotalSavingsUsed + "' " +
                                      "where MemId = '" + data.MemId + "'";
                db.AUIDB_WithParam(Update_history);
                return Ok(result);

                //string username = $@"SELECT  Fname,Lname,Mname FROM [dbo].[tbl_User_Model] where UserId = '" + data.Approvedby + "'";
                //DataTable username_tbl = db.SelectDb(username).Tables[0];
                //string name = username_tbl.Rows[0]["Fname"].ToString() + " " + username_tbl.Rows[0]["Mname"].ToString() + " " + username_tbl.Rows[0]["Lname"].ToString();
                //dbmet.InsertNotification("Approved For Releasing " + data.NAID + "",
                //      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Releasing Module", name, "2");

          
            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }

        }

    }
}