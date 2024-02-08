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
using static GoldOneAPI.Controllers.FieldAreaController;
using System.Linq;
using static GoldOneAPI.Controllers.LoanSummaryController;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.Json;
using static GoldOneAPI.Controllers.CollectionController;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();


        public class AreaRefno
        {
            public string AreaRefNo { get; set; }
            public string AreaID { get; set; }
        }
        public class CollectionVM
        {
            public string Fname { get; set; }
            public string Mname { get; set; }
            public string Lname { get; set; }
            public string Suffix { get; set; }
            public string MemId { get; set; }
            public string Cno { get; set; }
            public string Borrower { get; set; }
            public string FilePath { get; set; }
            public string NAID { get; set; }
            public string Co_Fname { get; set; }
            public string Co_Mname { get; set; }
            public string Co_Lname { get; set; }
            public string Co_Suffix { get; set; }
            public string Co_Borrower { get; set; }
            public string Co_Cno { get; set; }
            public string DailyCollectibles { get; set; }
            public string AmountDue { get; set; }
            public string PastDue { get; set; }
            public string DueDate { get; set; }
            public string DateOfFullPayment { get; set; }
            public string TotalSavingsAmount { get; set; }
            public string ApprovedAdvancePayment { get; set; }
            public string LoanPrincipal { get; set; }
            public string ReleasingDate { get; set; }
            public string TypeOfCollection { get; set; }
            public string CollectedAmount { get; set; }
            public string LapsePayment { get; set; }
            public string AdvancePayment { get; set; }
            public string Payment_Status_Id { get; set; }
            public string Payment_Status { get; set; }
            public string Collection_Status { get; set; }
            public string Collection_Status_Id { get; set; }
            public string Payment_Method { get; set; }
            public string AreaName { get; set; }
            public string AreaID { get; set; }
            public string Area_RefNo { get; set; }
            public string Collection_RefNo { get; set; }
            public string FOID { get; set; }
            public string FieldOfficer { get; set; }
            public string DateCreated { get; set; }
            public string DateCollected { get; set; }
            public string Remarks { get; set; }
            public string Penalty { get; set; }
            public string LoanInsurance { get; set; }
            public string LifeInsurance { get; set; }
            public string FieldExpenses { get; set; }
            public string? TotalItems { get; set; }
            public string? Status { get; set; }


        }
        public class CollectionVM2
        {


            public string MemId { get; set; }
            public string Cno { get; set; }
            public string Borrower { get; set; }
            public string FilePath { get; set; }
            public string NAID { get; set; }
            public string Co_Borrower { get; set; }
            public string Co_Cno { get; set; }
            public string DailyCollectibles { get; set; }
            public string AmountDue { get; set; }
            public string PastDue { get; set; }
            public string DueDate { get; set; }
            public string DateOfFullPayment { get; set; }
            public string TotalSavingsAmount { get; set; }
            public string ApprovedAdvancePayment { get; set; }
            public string LoanPrincipal { get; set; }
            public string ReleasingDate { get; set; }
            public string AreaName { get; set; }
            public string AreaID { get; set; }
            public string FOID { get; set; }
            public string FieldOfficer { get; set; }
            public string DateCreated { get; set; }
            public string TypeOfCollection { get; set; }


            public string CollectedAmount { get; set; }
            public string LapsePayment { get; set; }
            public string AdvancePayment { get; set; }
            public string Payment_Status_Id { get; set; }
            public string Payment_Status { get; set; }
            public string Payment_Method { get; set; }
            public string DateCollected { get; set; }

        }

        public class CollectionPrintedVM
        {
            public double? TotalCollectible { get; set; }
            public double? Total_Balance { get; set; }
            public double? Total_savings { get; set; }
            public double? Total_advance { get; set; }
            public double? Total_lapses { get; set; }
            public double? Total_collectedAmount { get; set; }
            public string? AreaName { get; set; }
            public string? FieldOfficer { get; set; }
            public string? FOID { get; set; }
            public string? TotalItems { get; set; }
            public double? ExpectedCollection { get; set; }
            public double? Total_FieldExpenses { get; set; }

            public string RefNo { get; set; }
            public string DateCreated { get; set; }
            public string Area_RefNo { get; set; }
            public string AreaID { get; set; }
            public string Denomination { get; set; }
            public string FieldExpenses { get; set; }
            public string Remarks { get; set; }
            public string NAID { get; set; }
            public string AdvancePayment { get; set; }
            public string LapsePayment { get; set; }
            public string CollectedAmount { get; set; }
            public string Savings { get; set; }
            public string DateCollected { get; set; }
            public string AdvanceStatus { get; set; }
            public string PrintedStatus { get; set; }
            public string Collection_Status { get; set; }
            public string Payment_Status { get; set; }
            public string Payment_Method { get; set; }
            public string MemId { get; set; }
            public string OutstandingBalance { get; set; }
            public string Penalty { get; set; }
            public string DateReleased { get; set; }
            public string DueDate { get; set; }
            public string DateOfFullPayment { get; set; }
            public string UsedSavings { get; set; }
            public string ApprovedTermsOfPayment { get; set; }
            public string ApprovedLoanAmount { get; set; }
            public string ApprovedNotarialFee { get; set; }
            public string ApprovedAdvancePayment { get; set; }
            public string ApprovedReleasingAmount { get; set; }
            public string ApproveedInterest { get; set; }
            public string ApprovedDailyAmountDue { get; set; }
            public string ModeOfRelease { get; set; }
            public string LoanTypeID { get; set; }

        }
        public class CollectionDetailsVM
        {
            public string? RefNo { get; set; }
            public string? Borrower { get; set; }
            public string? Collectible { get; set; }
            public string? AmountDue { get; set; }
            public string? Balance { get; set; }
            public string? OverAllSavings { get; set; }
            public string? Advance { get; set; }
            public string? Status { get; set; }

        }
        public class RemitVM
        {
            public string? RefNo { get; set; }
            public string? Borrower { get; set; }
            public string? Collectible { get; set; }
            public string? Savings { get; set; }
            public string? Lapses { get; set; }
            public string? Advance { get; set; }
            public string? MofeofPayment { get; set; }
            public string? PaymentStatus { get; set; }
            public string? PaymentStatusID { get; set; }
            public string? Area_RefNo { get; set; }
            public string? AreaID { get; set; }

        }
        public class AreaDetailsVM2
        {
            //public double? TotalCollectible { get; set; }
            //public double? Total_Balance { get; set; }
            //public double? Total_savings { get; set; }
            //public double? Total_advance { get; set; }
            //public double? Total_lapses { get; set; }
            //public double? Total_collectedAmount { get; set; }
            //public string? AreaName { get; set; }
            //public string? AreaID { get; set; }
            //public string? FieldOfficer { get; set; }
            //public string? FOID { get; set; }
            //public string? Area_RefNo { get; set; }
            //public string? Collection_RefNo { get; set; }
            //public string? TotalItems { get; set; }
            //public double? ExpectedCollection { get; set; }
            //public double? AdvancePayment { get; set; }
            //public string DateCreated { get; set; }

            public double? TotalCollectible { get; set; }
            public double? Total_Balance { get; set; }
            public double? Total_savings { get; set; }
            public double? Total_advance { get; set; }
            public double? Total_lapses { get; set; }
            public double? Total_collectedAmount { get; set; }
            public double? Total_FieldExpenses { get; set; }
            public string? TotalItems { get; set; }
            public double? ExpectedCollection { get; set; }
            public List<CollectionVM> Collection { get; set; }
        }


    

        public class AreaDetailsVM
        {
            public double? TotalCollectible { get; set; }
            public double? Total_Balance { get; set; }
            public double? Total_savings { get; set; }
            public double? Total_advance { get; set; }
            public double? Total_lapses { get; set; }
            public double? Total_collectedAmount { get; set; }
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public string? FieldOfficer { get; set; }
            public string? FOID { get; set; }
            public string? Area_RefNo { get; set; }
            public string? Collection_RefNo { get; set; }
            public string? TotalItems { get; set; }
            public double? ExpectedCollection { get; set; }
            public double? AdvancePayment { get; set; }
            public string DateCreated { get; set; }
            public string DateCollected { get; set; }
            public double? Total_FieldExpenses { get; set; }

        }
        public class CollectionTotals
        {
            public double? TotalCollectible { get; set; }
            public double? Total_Balance { get; set; }
            public double? Total_savings { get; set; }
            public double? Total_advance { get; set; }
            public double? Total_lapses { get; set; }
            public double? Total_collectedAmount { get; set; }
            public double? Total_FieldExpenses { get; set; }
            public List<CollectionVM> Collection { get; set; }
        }

        public class ColVM
        {
            public string? CollectionRef { get; set; }
            public string? DateCreated { get; set; }
            public List<ColAreaVM> ColArea { get; set; }
        }


        public class AreaRef
        {
            public string? CollectionRef { get; set; }
            public string? Area_RefNo { get; set; }
        }
        public class ColAreaVM
        {
            public string? AreaId { get; set; }
            public string? Area_RefNo { get; set; }
            public string? AreaName { get; set; }
            public string? Printed_Status { get; set; }
            public string? Collection_Status { get; set; }
            public string? Denomination { get; set; }
            public string? FieldExpenses { get; set; }
            public string? CollectionRefNo { get; set; }
            public string? Remarks { get; set; }
           public List<CollectionVM2> Collection { get; set; }
        }
      
            public class CollectionModel
        {
            public string? TotalItems { get; set; }
            public string? AreaID { get; set; }
            public string? RefNumber { get; set; }
            public string? MemId{ get; set; }
            public string? Borrower { get; set; }
            public string? Daily_Collectible { get; set; }
            public string? AmountDue { get; set; }
            public string? Balance { get; set; }
            public string? OverAllSavings { get; set; }
            public string? Status { get; set; }
            public string? Borrower_Cno { get; set; }
            public string? Co_Borrower { get; set; }
            public string? Co_Borrower_Cno { get; set; }
            public string? PrincipalLoan { get; set; }
            public string? DateReleased { get; set; }
            public string? DueDate { get; set; }
            public string? CollectionDate { get; set; }
            public string? Savings { get; set; }
            public string? Min_Daily_Savings { get; set; }
            public string? Collection_Lapses { get; set; }
            public string? Advance_Collection { get; set; }

        }

        [HttpGet]
        public async Task<IActionResult> GetAreaReferenceNo(string FOID)
        {
            var result = new List<AreaRefno>();
            string sql_count = $@"SELECT [tbl_CollectionArea_Model].[Id]
                              ,[tbl_CollectionArea_Model].AreaId
                              ,[Area_RefNo]
                              ,[Printed_Status]
                              ,[Collection_Status]
                              ,[Denomination]
                              ,[FieldExpenses]
                              ,[CollectionRefNo]
                              ,[Remarks]
                          FROM [dbo].[tbl_CollectionArea_Model] inner join
						  tbl_Area_Model on tbl_Area_Model.AreaID = tbl_CollectionArea_Model.AreaId and tbl_Area_Model.FOID is not null left JOIN
						  tbl_FieldOfficer_Model on tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
						  where tbl_FieldOfficer_Model.FOID = '"+FOID+"' " +
                          "group by [tbl_CollectionArea_Model].[Id]" +
                          ",[tbl_CollectionArea_Model].AreaId" +
                          ",[Area_RefNo]" +
                              ",[Printed_Status]" +
                              ",[Collection_Status]" +
                              ",[Denomination]" +
                              ",[FieldExpenses]" +
                              ",[CollectionRefNo]" +
                              ",[Remarks]";
            DataTable table_ = db.SelectDb(sql_count).Tables[0];
            foreach (DataRow dr in table_.Rows)
            {
                var item = new AreaRefno();
                item.AreaRefNo = dr["Area_RefNo"].ToString();
                item.AreaID = dr["AreaId"].ToString();
                result.Add(item);
            }

                return Ok(result);
        }

         [HttpGet]
        public async Task<IActionResult> Areas(bool? showprev)
        {
            try
            {
                if (showprev != null)
                {
                    if (showprev == true)
                    {
                        return Ok(dbmet.ShowArea().ToList());
                    }
                    else
                    {
                        return Ok(dbmet.ShowArea().ToList().OrderByDescending(a=>a.DateCreated).FirstOrDefault());
                    }
                }
                else
                {
                    return Ok(dbmet.ShowArea().ToList());
                }

            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }



        }

        [HttpGet]
        public async Task<IActionResult> MakeCollection()
        {
            try
            {
                return Ok(dbmet.getMAkeCollectionList());
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CollectionDetailsList(string areaid , string? arearefno)
        {
            try
            {
            
                string pastamount = "0";
                var res = new List<CollectionTotals>();
                if (arearefno != null)
                {
                    var areas = dbmet.getAreaLoanSummary_2(areaid,  arearefno).GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.FOID, a.Area_RefNo }).ToList();
                    var list_validate = dbmet.getAreaLoanSummary_2(areaid, arearefno).ToList();
                    if (list_validate.Count != 0)
                    {
                        var list = dbmet.getAreaLoanSummary_2(areaid, arearefno).ToList();

                      

                        var dailyCollectiblesSum = list.Select(a => double.Parse(a.DailyCollectibles)).Sum();
                        var savings = list.Select(a => double.Parse(a.TotalSavingsAmount)).Sum();
                        var balance = list.Select(a => double.Parse(a.AmountDue)).Sum();
                        var advance = list.Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                        var lapses = list.Select(a => double.Parse(a.LapsePayment)).Sum();
                        var collectedamount = list.Select(a => double.Parse(a.CollectedAmount)).Sum();
                        var fieldexpenses = list.Select(a => double.Parse(a.FieldExpenses)).Sum();

                        var items = new CollectionTotals();
                        items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                        items.Total_FieldExpenses = Math.Ceiling(double.Parse(fieldexpenses.ToString()));
                        items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
                        items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
                        items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
                        items.Total_lapses = Math.Ceiling(lapses);
                        items.Total_collectedAmount = Math.Ceiling(collectedamount);
                        DateTime dueDate = Convert.ToDateTime(Convert.ToDateTime(list[0].DateCollected).ToString("yyyy-MM-dd"));// Get the due date from your database
                        string payment_status = "";
                        // Get the current date and time
                        DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        var collection_list = new List<CollectionVM>();
                        if (list.Count != 0)
                        {
                            if (dueDate < currentDate)
                            {
                                payment_status = "No Payment";
                            }
                            else
                            {
                                payment_status = "Paid";
                            }

                            for (int x = 0; x < list.Count; x++)
                            {
                                var item = new CollectionVM();
                                item.Borrower = list[x].Borrower;
                                item.Cno = list[x].Cno;
                                item.Co_Borrower = list[x].Co_Fname + " " + list[x].Co_Mname + " " + list[x].Co_Lname;
                                item.Co_Cno = list[x].Co_Cno;
                                item.LoanPrincipal = list[x].LoanPrincipal;
                                item.ReleasingDate = list[x].ReleasingDate;
                                item.DueDate = list[x].DueDate;
                                item.TypeOfCollection = list[x].TypeOfCollection;
                                item.TotalSavingsAmount = list[x].TotalSavingsAmount;
                                item.DailyCollectibles = list[x].DailyCollectibles;
                                item.LapsePayment = list[x].LapsePayment;
                                item.AdvancePayment = list[x].AdvancePayment;
                                item.Payment_Status = list[x].Payment_Status;
                                item.Collection_Status = list[x].Collection_Status;
                                item.MemId = list[x].MemId;
                                item.FilePath = list[x].FilePath;
                                item.NAID = list[x].NAID;
                                item.AmountDue = list[x].AmountDue;
                                item.DateOfFullPayment = list[x].FilePath == null ? "Ongoing Collection" : list[x].DateOfFullPayment;
                                item.CollectedAmount = list[x].CollectedAmount;
                                item.AreaID = list[x].AreaID;
                                item.AreaName = list[x].AreaName;
                                item.Area_RefNo = list[x].Area_RefNo;
                                item.Collection_RefNo = list[x].Collection_RefNo;
                                item.FOID = list[x].FOID;
                                item.FieldOfficer = list[x].FieldOfficer;
                                item.Payment_Method = payment_status;
                                item.ApprovedAdvancePayment = list[x].ApprovedAdvancePayment;
                                item.DateCreated = list[x].DateCreated;
                             
                                item.PastDue = list[x].PastDue;
                                collection_list.Add(item);

                            }

                        }
                        items.Collection = collection_list;
                        res.Add(items);
                    }
                    else
                    {
                        return BadRequest("No Data Found!");
                    }
                }
                else
                {
                    var list = dbmet.getAreaLoanSummary_2(areaid, arearefno).ToList();



                    var dailyCollectiblesSum = dbmet.getAreaLoanSummary_2(areaid,  arearefno).Select(a => double.Parse(a.DailyCollectibles)).Sum();
                    var savings = dbmet.getAreaLoanSummary_2(areaid, arearefno).Select(a => double.Parse(a.TotalSavingsAmount)).Sum();
                    var balance = dbmet.getAreaLoanSummary_2(areaid, arearefno).Select(a => double.Parse(a.AmountDue)).Sum();
                    var advance = dbmet.getAreaLoanSummary_2(areaid, arearefno).Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                    var lapses = dbmet.getAreaLoanSummary_2(areaid, arearefno).Select(a => double.Parse(a.LapsePayment)).Sum();
                    var collectedamount = dbmet.getAreaLoanSummary_2(areaid, arearefno).Select(a => double.Parse(a.CollectedAmount)).Sum();
                    var fieldexpenses = dbmet.getAreaLoanSummary_2(areaid, arearefno).Select(a => double.Parse(a.FieldExpenses)).Sum();

                    double bal = list.Select(a => double.Parse(a.LoanPrincipal)).Sum();
                    double col = list.Select(a => double.Parse(a.CollectedAmount)).Sum();
                    double total_bal = Math.Abs(bal - col);
                    var items = new CollectionTotals();
                    items.Total_FieldExpenses = Math.Ceiling(double.Parse(fieldexpenses.ToString()));
                    items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                    items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
                    items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
                    items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
                    items.Total_lapses = Math.Ceiling(lapses);
                    items.Total_collectedAmount = Math.Ceiling(collectedamount);
             
                    var collection_list = new List<CollectionVM>();
                    if (list.Count != 0)
                    {
                        DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        string payment_status = "";
                        if (list[0].DateCollected != "")
                        {
                            DateTime dueDate = Convert.ToDateTime(Convert.ToDateTime(list[0].DateCollected).ToString("yyyy-MM-dd"));// Get the due date from your database
                        
                            // Get the current date and time
                     
                            if (dueDate < currentDate)
                            {
                                payment_status = "No Payment";
                            }
                            else
                            {
                                payment_status = "Paid";
                            }

                        }
                        else
                        {
                            payment_status = "No Payment";
                        }
                        for (int x = 0; x < list.Count; x++)
                        {
                            var item = new CollectionVM();
                            item.Borrower = list[x].Borrower;
                            item.Cno = list[x].Cno;
                            item.Co_Borrower = list[x].Co_Fname + " " + list[x].Co_Mname + " " + list[x].Co_Lname;
                            item.Co_Cno = list[x].Co_Cno;
                            item.LoanPrincipal = list[x].LoanPrincipal;
                            item.ReleasingDate = list[x].ReleasingDate;
                            item.DueDate = list[x].DueDate;
                            item.TypeOfCollection = list[x].TypeOfCollection;
                            item.TotalSavingsAmount = list[x].TotalSavingsAmount;
                            item.DailyCollectibles = list[x].DailyCollectibles;
                            item.LapsePayment = list[x].LapsePayment;
                            item.AdvancePayment = list[x].AdvancePayment;
                            item.Payment_Status = list[x].Payment_Status;
                            item.Collection_Status = list[x].Collection_Status;
                            item.MemId = list[x].MemId;
                            item.FilePath = list[x].FilePath;
                            item.NAID = list[x].NAID;
                            item.AmountDue = list[x].AmountDue;
                            item.DateOfFullPayment = list[x].FilePath == null ? "Ongoing Collection" : list[x].DateOfFullPayment;
                            item.CollectedAmount = list[x].CollectedAmount;
                            item.AreaID = list[x].AreaID;
                            item.AreaName = list[x].AreaName;
                            item.Area_RefNo = list[x].Area_RefNo;
                            item.Collection_RefNo = list[x].Collection_RefNo;
                            item.FOID = list[x].FOID;
                            item.FieldOfficer = list[x].FieldOfficer;
                            item.Payment_Method = payment_status;
                            item.ApprovedAdvancePayment = list[x].ApprovedAdvancePayment;
                            item.DateCreated = list[x].DateCreated;
                            item.PastDue = list[x].PastDue;
                            collection_list.Add(item);

                        }
                        items.Collection = collection_list;
                        res.Add(items);
                    }
                    else
                    {
                        return BadRequest("No Data Found!");
                    }
                  
                }
               
                
                return Ok(res);
             }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }



        }

        [HttpGet]
        public async Task<IActionResult> Collection_AreaReferenceList(string collection_refno)
        {
            try
            {
                var result = new List<AreaRef>();
                string sql_count = $@"SELECT        tbl_CollectionArea_Model.Area_RefNo, tbl_CollectionArea_Model.CollectionRefNo
FROM            tbl_CollectionArea_Model INNER JOIN
                         tbl_CollectionModel ON tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo
GROUP BY tbl_CollectionArea_Model.Area_RefNo, tbl_CollectionArea_Model.CollectionRefNo where tbl_CollectionArea_Model.CollectionRefNo = '"+collection_refno+"'";
                DataTable table_ = db.SelectDb(sql_count).Tables[0];
                foreach (DataRow dr in table_.Rows)
                {
                    var item = new AreaRef();
                    item.CollectionRef = dr["CollectionRefNo"].ToString();
                    item.Area_RefNo = dr["Area_RefNo"].ToString();
                    result.Add(item);


                }
                return Ok(result);

            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }



        }

        [HttpGet]
        public async Task<IActionResult> CollectionDetailsViewbyRefno(string colrefno)
        {
            try
            {
                var res = new List<CollectionPrintedVM>();
                double advance_payment = 0;
                double dailyCollectiblesSum = 0;
                double savings = 0;
                double balance = 0;
                double advance = 0;
                double collectedamount = 0;
                double lapses = 0;
                var areas = dbmet.CollectionGroupby().Where(a => a.RefNo == colrefno).ToList();
                var printed_area = dbmet.Collection_PrintedResult().Where(a => a.RefNo == colrefno ).ToList();
                var raw_list = dbmet.Collection_NotPrintedResult().ToList();
                //IEnumerable<AreaDetailsVM> notInListB = printed_area.Except(raw_list);
                var list1 = raw_list.Where(p2 => !areas.Any(p1 => p1.AreaID == p2.AreaID)).ToList();

                // Concatenate the result with listB to get the desired list.
                //List<AreaDetailsVM> resultList = 
                List<CollectionPrintedVM> resultList = areas.Concat(list1).ToList();

                for (int x = 0; x < resultList.Count; x++)
                {
                    var items = new CollectionPrintedVM();
                    items.TotalCollectible = Math.Ceiling(double.Parse(resultList[x].TotalCollectible.ToString()));
                    items.Total_Balance = Math.Ceiling(double.Parse(resultList[x].OutstandingBalance.ToString()));
                    items.Total_savings = Math.Ceiling(double.Parse(resultList[x].Savings.ToString()));
                    items.Total_advance = Math.Ceiling(double.Parse(resultList[x].AdvancePayment.ToString()));
                    items.Total_lapses = Math.Ceiling(double.Parse(resultList[x].LapsePayment.ToString()));
                    items.Total_collectedAmount = Math.Ceiling(double.Parse(resultList[x].CollectedAmount.ToString()));
                    items.Total_FieldExpenses = Math.Ceiling(double.Parse(resultList[x].FieldExpenses.ToString()));
                    items.AreaName = resultList[x].AreaName;
                    items.AreaID = resultList[x].AreaID;
                    items.FieldOfficer = resultList[x].FieldOfficer;
                    items.FOID = resultList[x].FOID;
                    items.Collection_Status = resultList[x].Collection_Status;

                    //string col_ref = "";
                    //string area_ref = "";
                    items.Area_RefNo = resultList[x].Area_RefNo == string.Empty ? "PENDING" : resultList[x].Area_RefNo;
                    items.RefNo = resultList[x].RefNo == string.Empty ? "PENDING" : resultList[x].RefNo;
                    //items.DateCreated = datec;
                    items.TotalItems = resultList[x].TotalItems;
                    items.ExpectedCollection = Math.Ceiling(double.Parse(resultList[x].ExpectedCollection.ToString()));
                    items.AdvancePayment = Math.Ceiling(double.Parse(resultList[x].LapsePayment.ToString())).ToString();
                    res.Add(items);

                }


                return Ok(res);
            }

            catch (Exception ex)
            {
                return BadRequest(dbmet.getAmount("AREA-053").ToList().ToString());
            }



        }
        [HttpGet]
        public async Task<IActionResult> CollectionDetailsViewbyAreaRefno(string areaid, string area_refno)
        {
            try
            {

                var result = dbmet.GetCollectionDetailListFilterbyAreaRefno(areaid, area_refno).ToList();
                return Ok(result);

            }

            catch (Exception ex)
             {
                return BadRequest("ERROR");
            }



        }
        [HttpGet]
        public async Task<IActionResult> GetFieldExpensesByAreaRef(string area_refno , string foid)
        {
            try
            {

                return Ok(dbmet.GetFieldExpenses(area_refno,foid).ToList());

            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }



        }
        [HttpPost]
        public async Task<IActionResult> Collect(AreaCollectionUpdate data)
        {

            try
            {
                string result = "";
                string Update = "";
                string filePath = @"C:\data\collect.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                Update = $@"
                                UPDATE [dbo].[tbl_CollectionArea_Model]
                                   SET [Collection_Status] = '7' ," +
                                      "[Denomination] = '" + data.Denomination + "' " +
                                 "WHERE AreaID = '" + data.AreaID + "' and Area_RefNo = '"+data.AreaRefno+"'";
                result = db.AUIDB_WithParam(Update) + " Updated";
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }
        [HttpPost]
        public async Task<IActionResult> Reject(AreaIDVM data)
        {

            try
            {
                string Update = "";
                string result = "";
                string filePath = @"C:\data\reject.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                Update = $@"
                                UPDATE [dbo].[tbl_Collection_AreaMember_Model]
                                   SET [Payment_Status] = '2' ," +
                                      "[Payment_Method] = 'No Payment' " +
                                 "WHERE  Area_RefNo = '"+data.AreaRefno+"'";
                result = db.AUIDB_WithParam(Update) + " Updated";


                Update += $@"UPDATE [dbo].[tbl_CollectionArea_Model]
                           SET [Collection_Status]  = NULL, " +
                             "[Remarks] ='" + data.Remarks + "' " +
                         "WHERE AreaID = '" + data.AreaID + "' and Area_RefNo = '"+data.AreaRefno+"'";
                result = db.AUIDB_WithParam(Update) + " Updated";


                string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                DataTable username_tbl = db.SelectDb(username).Tables[0];
                foreach (DataRow dr in username_tbl.Rows)
                {
                    string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                    dbmet.InsertNotification("Rejected Remitteed  " + data.AreaID + " ",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Remittance Module", name, dr["UserId"].ToString(), "2",data.AreaID);
                }
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }
        [HttpPost]
        public async Task<IActionResult> FieldExpenses(List<Fieldexpenses> data)
        {
            string result = "";
            try
            {
              
                string fieldexpenses = "";
            if (data.Count != 0)
            {
                for (int x = 0; x < data.Count; x++)
                {
                    fieldexpenses += data[x].ExpensesDescription + ", " + data[x].FieldExpenses + " |";
                }
            }

            string Updates = $@"
                                        UPDATE [dbo].[tbl_CollectionArea_Model]
                                           SET [Collection_Status] = '1' ," +
                                        "[FieldExpenses] ='" + fieldexpenses.Substring(0, fieldexpenses.Length - 1) + "' " +
                                    "WHERE  AreaId = '" + data[0].AreaId + "' and Area_RefNo = '" + data[0].AreaRefno +"'";
                result = db.AUIDB_WithParam(Updates) + " updated";
            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }

            return Ok(result);
        }
            [HttpPost]
        public async Task<IActionResult> Remit(Remit data)
        {

            try
            {
                string Update = "";
                string filePath = @"C:\data\remit.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                string result = "";
                string payment_status = "";
                double lapses = 0;
                double advance = 0;
                var memid = dbmet.getAreaLoanSummary().Where(a => a.Area_RefNo == data.AreaRefno && a.MemId == data.MemId).FirstOrDefault();
                if (memid != null)
                {


                    if (double.Parse(data.AmountCollected.ToString()) >= double.Parse(memid.DailyCollectibles))
                    {
                        payment_status = "1";
                    }
                    else if (double.Parse(data.AmountCollected.ToString()) < double.Parse(memid.DailyCollectibles))
                    {
                        payment_status = "2";
                    }
                   

                }
               if (memid != null)
                {
                    if (data.Savings != 0)
                    {
                        string sql_count = $@"
                                            SELECT [Id]
                                                  ,[MemId]
                                              FROM [dbo].[tbl_MemberSavings_Model] where MemId = '"+memid.MemId+"'";
                        DataTable table_ = db.SelectDb(sql_count).Tables[0];
                        if (table_.Rows.Count != 0)
                        {
                            var total_savings_amount = dbmet.getAreaLoanSummary().Where(a => a.MemId == memid.MemId).FirstOrDefault();
                            double total_sum_saving = Math.Ceiling(double.Parse(total_savings_amount.TotalSavingsAmount) + double.Parse(data.Savings.ToString()) );
                             Update += $@"
                                            UPDATE [dbo].[tbl_MemberSavings_Model]
                                               SET TotalSavingsAmount] =  '" + total_sum_saving + "' ," +
                                                  "[DateUpdated] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "', " +
                                                  "[UpdatedFrom] = 'Update from Collection Remitance Module' ," +
                                                  "[UpdateBy] = '" + data.UserId + "' " +
                                         "WHERE  LDID ='" + data.MemId + "'";

                      

                        }
                        else
                        {
                            string Insert = $@"INSERT INTO [dbo].[tbl_MemberSavings_Model]
                                                       ([MemId]
                                                       ,[TotalSavingsAmount]
                                                       ,[DateUpdated]
                                                       ,[UpdatedFrom]
                                                       ,[UpdateBy])
                                                 VALUES
                                                       ('" + memid.MemId + "'," +
                                                       "'" + data.Savings + "'," +
                                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                                                        "'Deposit a new Savings in Collection Remittance Module'," +
                                                        "'" + data.UserId + "') ";
                            db.AUIDB_WithParam(Insert);
                        }
                    }


               
                    string sql_areamember = $@"
                                            SELECT [Id]
                                                  ,[NAID]
                                              FROM [dbo].[tbl_Collection_AreaMember_Model] where NAID = '" + memid.NAID + "'";
                    DataTable tables = db.SelectDb(sql_areamember).Tables[0];

                    string sql_loanhistory = $@"
                                            SELECT 
                                          [OutstandingBalance]
                                      FROM [GoldOne].[dbo].[tbl_LoanHistory_Model] where MemId = '" + memid.MemId + "'";
                    DataTable sql_loanhistory_tbl = db.SelectDb(sql_loanhistory).Tables[0];

                    double total_outstanding_bal = Math.Abs (double.Parse(data.AmountCollected.ToString()) - double.Parse(sql_loanhistory_tbl.Rows[0]["OutstandingBalance"].ToString()));
                    string Update_history = $@"
                                        UPDATE [dbo].[tbl_LoanHistory_Model]
                                           SET [OutstandingBalance] = '" + total_outstanding_bal + "' " +
                                          "where MemId = '" + memid.MemId + "'";
                    db.AUIDB_WithParam(Update_history);
                    if (tables.Rows.Count != 0)
                    {
                        //update
                         Update += $@"
                                        UPDATE [dbo].[tbl_Collection_AreaMember_Model]
                                           SET [AdvancePayment] = '" + data.AdvancePayment + "' ," +
                                              "[LapsePayment] ='" + data.Lapses + "' ," +
                                              "[CollectedAmount] = '" + data.AmountCollected + "' ," +
                                              "[Savings] ='" + data.Savings + "' ," +
                                              "[Payment_Status] = '"+ payment_status + "' ," +
                                              "[Payment_Method] = '" + data.ModeOfPayment + "' ," +
                                              "[DateCollected] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "' " +
                                          "WHERE  NAID = '" + memid.NAID + "' and tbl_Collection_AreaMember_Model.Area_RefNo = '"+data.AreaRefno+"'";
                

                        Update += $@"
                                UPDATE [dbo].[tbl_CollectionArea_Model]
                                   SET [Collection_Status] = '6' " +
                         "WHERE AreaID = '" + data.AreaID + "' and Area_RefNo='"+ data.AreaRefno + "'";
                        db.AUIDB_WithParam(Update);
                    }
                    else
                    {
                        //insert
                        string Insert = $@"INSERT INTO [dbo].[tbl_Collection_AreaMember_Model]
                                           ([NAID]
                                           ,[AdvancePayment]
                                           ,[LapsePayment]
                                           ,[CollectedAmount]
                                           ,[Savings]
                                           ,[Payment_Status]
                                           ,[Payment_Method]
                                           ,[DateCollected])
                                     VALUES
                                           ('" + memid.NAID + "'," +
                                            "'" + data.AdvancePayment + "'," +
                                              "'" + data.Lapses + "'," +
                                              "'" + data.AmountCollected + "'," +
                                              "'" + data.Savings + "'," +
                                               "'"+payment_status+"'," +
                                               "'"+data.ModeOfPayment+"'," +
                                                   "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        db.AUIDB_WithParam(Insert);


                    }
                    string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                    DataTable username_tbl = db.SelectDb(username).Tables[0];
                    foreach (DataRow dr in username_tbl.Rows)
                    {
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        dbmet.InsertNotification("Successfully Remitteed  " + data.AreaRefno + " ",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Remittance Module", name, dr["UserId"].ToString(), "2",data.AreaRefno);
                    }
                    return Ok("Successfully Remitteed");
           


                }
                else
                {
                    return BadRequest("No Records Found!");
                }
                


                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }

        [HttpPost]
        public async Task<IActionResult> RemitAmountCollectedComputation(RemitCalcu data)
        {

            try
            {
                string result = "";
                double lapses = 0;
                double advance = 0;
                var res = new RemitAmountResult();
                var memid = dbmet.getAreaLoanSummary().Where(a => a.Area_RefNo == data.AreaRefno && a.MemId == data.MemId && a.Payment_Status != "Paid").FirstOrDefault();
                if (memid != null)
                {
                  
                    if (double.Parse(memid.DailyCollectibles) != data.AmountCollected)
                    {

                       
                        if (double.Parse(data.AmountCollected.ToString()) > double.Parse(memid.DailyCollectibles))
                        {
                            advance = Math.Abs(double.Parse(data.AmountCollected.ToString()) - double.Parse(memid.DailyCollectibles));
                        }
                        else
                        {
                            advance = 0;
                        }

                        if (double.Parse(data.AmountCollected.ToString()) <= double.Parse(memid.DailyCollectibles))
                        {
                            lapses = Math.Abs(double.Parse(data.AmountCollected.ToString()) - double.Parse(memid.DailyCollectibles));
                        }
                        else
                        {
                            lapses = 0;
                        }
                        res.advance = String.Format("{0:0.00}", advance);
                        res.lapses = String.Format("{0:0.00}", lapses);
                    }

                    

                    return Ok(res);

                }
                else
                {
                    return BadRequest("ERROR");
                }

     

                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }

        public class ReturnValue
        {
            public string? AreaRef { get; set; }
            public string? ColRef { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> PrintCollection(AreaIDVM data)
        {

            try
            {
                string collection_refno = "";
                var promtres = new ReturnValue();
                string filePath = @"C:\data\Printdata.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                //checking if collection model has already has a exisiting col-refno in datetime now
                string insert = "";
                string paymentstatus = "2";
                string status = "";
                string area_refno = "";
                string datecreated = "";
                string sql = $@"SELECT [Id]
                          ,[RefNo]
                          ,[DateCreated]
                      FROM [GoldOne].[dbo].[tbl_CollectionModel] where DateCreated = '"+DateTime.Now.ToString("yyyy-MM-dd")+"'";
                var result = new List<PaymentHistory>();
                DataTable table = db.SelectDb(sql).Tables[0];
                if (table.Rows.Count != 0) // if YES
                {
                    string sql_count = $@"SELECT [Id]
                          ,[RefNo]
                          ,[DateCreated]
                      FROM [GoldOne].[dbo].[tbl_CollectionModel]";
                    DataTable table_ = db.SelectDb(sql_count).Tables[0];
                    int rows_count = table_.Rows.Count == 0 ? 1 : table_.Rows.Count + 1;
                    string refno = "COL-" + data.AreaID + DateTime.Now.ToString("yyyyMMdd") + "-0" + rows_count;
                    sql = $@"select * from tbl_CollectionModel order by id desc";

                    DataTable table_refno = db.SelectDb(sql).Tables[0];
                    collection_refno = table_refno.Rows[0]["RefNo"].ToString();
                    datecreated = Convert.ToDateTime(table_refno.Rows[0]["DateCreated"].ToString()).ToString("yyyy-MM-dd");
                    //arearefno
                    string sql_count_area = $@"SELECT [Id]
                                              ,[AreaId]
                                              ,[Area_RefNo]
                                              ,[Printed_Status]
                                              ,[Collection_Status]
                                              ,[Denomination]
                                              ,[FieldExpenses]
                                              ,[CollectionRefNo]
                                               FROM [dbo].[tbl_CollectionArea_Model]";
                    DataTable table__ = db.SelectDb(sql_count_area).Tables[0];
                    int rows_count_area = table__.Rows.Count == 1 ? 0 : table__.Rows.Count + 1;
                     area_refno = data.AreaID + DateTime.Now.ToString("yyyyMMdd") + "-0" + rows_count;

                    string validate_arearef = $@"SELECT [Id]
                                              ,[AreaId]
                                              ,[Area_RefNo]
                                              ,[Printed_Status]
                                              ,[Collection_Status]
                                              ,[Denomination]
                                              ,[FieldExpenses]
                                              ,[CollectionRefNo]
                                               FROM [dbo].[tbl_CollectionArea_Model] where CollectionRefNo ='" + collection_refno + "'";
                    DataTable area_ref = db.SelectDb(validate_arearef).Tables[0];
                    if (area_ref.Rows.Count == 0)
                    {
                        insert += $@"INSERT INTO [dbo].[tbl_CollectionArea_Model]
                                       ([AreaId]
                                       ,[Area_RefNo]
                                       ,[Printed_Status]
                                       ,[Collection)Status]
                                       ,[CollectionRefNo])
                                 VALUES
                                       ('" + data.AreaID + "'," +
                                   "'" + area_refno + "'," +
                                  "'5'," +
                                  "'5'," +
                                   "'" + collection_refno + "') ";
                        var list = dbmet.getAreaLoanSummary().Where(a => a.AreaID == data.AreaID).ToList();
                        db.AUIDB_WithParam(insert);


                        string collectionarea_validate_date = $@"select * from tbl_Collection_AreaMember_Model where DateCollected  = '" + datecreated + "' and Area_RefNo = '"+area_refno+"' ";
                        DataTable tbl_collectionarea_validate_date = db.SelectDb(collectionarea_validate_date).Tables[0];
                        if (tbl_collectionarea_validate_date.Rows.Count == 0)
                        {

                            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
                         FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
                        where tbl_Area_Model.AreaID ='" + data.AreaID + "'";
                            DataTable area_table = db.SelectDb(areafilter).Tables[0];
                            foreach (DataRow dr_area in area_table.Rows)
                            {
                                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                                for (int x = 0; x < area_city.Count; x++)
                                {
                                    var spliter = area_city[x].Split(",");
                                    string barangay = spliter[0].Trim();
                                    string city = spliter[1].Trim();

                                    string select_update = $@"select tbl_Application_Model.NAID from 
                                tbl_Application_Model left JOIN tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                where tbl_Member_Model.Barangay ='" + barangay + "' and tbl_Member_Model.City ='" + city + "' ";
                                    DataTable tbl_select_update = db.SelectDb(select_update).Tables[0];
                                    if (tbl_select_update.Rows.Count != 0)
                                    {

                                      
                                        foreach (DataRow drt in tbl_select_update.Rows)
                                        {
                                 
                                                //    string Update = $@"UPDATE [dbo].[tbl_Collection_AreaMember_Model]
                                                //SET [Area_RefNo] = '" + area_refno + "' " +
                                                //    "WHERE  NAID ='" + drt["NAID"].ToString() + "'";

                                                //    db.AUIDB_WithParam(Update);
                                                string CollectedAmount = "0";
                                            string Advancepayment = "0";
                                            string date_now = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                                            //var insert_item = dbmet.getAreaLoanSummary_2(data.AreaID).Where(a => a.NAID == drt["NAID"].ToString()).FirstOrDefault();
                                            string sql_insert_item = $@"select top (1) tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,
                                                case when tbl_MemberSavings_Model.TotalSavingsAmount is null then 0 
                                                else tbl_MemberSavings_Model.TotalSavingsAmount end as TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection
                                                ,tbl_Application_Model.ReleasingDate
                                                 FROM
                                                tbl_Application_Model inner JOIN
                                                tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner JOIN
                                                tbl_LoanType_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID inner JOIN
                                                tbl_TermsOfPayment_Model on tbl_TermsOfPayment_Model.LoanTypeId = tbl_LoanType_Model.LoanTypeID left JOIN
                                                tbl_MemberSavings_Model on tbl_LoanDetails_Model.MemId = tbl_MemberSavings_Model.MemId inner join
                                                tbl_TermsTypeOfCollection_Model on tbl_TermsOfPayment_Model.CollectionTypeId = tbl_TermsTypeOfCollection_Model.Id
                                                 where tbl_Application_Model.NAID = '" + drt["NAID"].ToString() + "' and tbl_Application_Model.Status = 14 group by tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,tbl_MemberSavings_Model.TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection,tbl_Application_Model.ReleasingDate";
                                            DataTable insert_item = db.SelectDb(sql_insert_item).Tables[0];

                                            if (insert_item.Rows.Count != 0)
                                            {
                                              

                                                string col_Typedate = "";
                                                var typeofcollection = insert_item.Rows[0]["TypeOfCollection"].ToString();
                                                string totaldays = "";
                                                switch (typeofcollection)
                                                {
                                                    case "Daily":
                                                        totaldays = "0";
                                                        col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                        break;
                                                    case "7 Days":
                                                        totaldays = dbmet.datecomputation(date_now, 7).totaldays.ToString();
                                                        col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                        break;
                                                    case "15 Days":
                                                        totaldays = dbmet.datecomputation(date_now, 15).totaldays.ToString();
                                                        col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                        break;
                                                    case "Monthly":

                                                        totaldays = dbmet.datecomputation(date_now, 31).totaldays.ToString();
                                                        col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                        break;
                                                    default:
                                                        totaldays = "0";
                                                        col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                        break;
                                                }
                                                string adv_payment = "";
                                                string date_create1 = datecreated == "" ? col_Typedate : datecreated;

                                                string areamember  = $@"SELECT * FROM [dbo].[tbl_Collection_AreaMember_Model] where NAID = '"+ drt["NAID"].ToString() + "'  and AdvanceStatus=1";
                                                DataTable tbl_areamember = db.SelectDb(areamember).Tables[0];
                                                if (tbl_areamember.Rows.Count != 0)
                                                {
                                                    adv_payment = "0.00";
                                                    paymentstatus = "2";
                                                    Advancepayment = "0";

                                                }
                                                else
                                                {
                                                    adv_payment = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString() == "" ? "0" : insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();

                                                    Advancepayment = insert_item.Rows[0]["NoAdvancePayment"].ToString() == "1" ? "1" : insert_item.Rows[0]["NoAdvancePayment"].ToString();
                                                }


                                                if (adv_payment == "0.00")
                                                {

                                                    CollectedAmount = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();
                                                    paymentstatus = "2";
                                                }
                                                else
                                                {
                                                    if (date_create1 == DateTime.Now.ToString("yyyy-MM-dd"))
                                                    {
                                                        paymentstatus = "1";

                                                    }


                                                }
                                                string _insert = $@"INSERT INTO [dbo].[tbl_Collection_AreaMember_Model]
                                                       ([NAID]
                                                       ,[AdvancePayment]
                                                       ,[LapsePayment]
                                                       ,[CollectedAmount]
                                                       ,[Savings]
                                                       ,[Area_RefNo]
                                                       ,[Payment_Status]
                                                       ,[AdvanceStatus]
                                                       ,[Payment_Method]
                                                       ,[DateCollected])
                                                 VALUES
                                                       ('" + drt["NAID"].ToString() + "'," +
                                                               "'0'," +
                                                              "'0'," +
                                                               "'" + adv_payment + "'," +
                                                              "' 0'," +
                                                              "'" + area_refno + "'," +
                                                          "'" + paymentstatus + "'," +
                                                               "'" + Advancepayment + "'," +
                                                              "'No Payment'," +
                                                          "'" + Convert.ToDateTime(DateTime.Now).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd") + "') ";

                                                db.AUIDB_WithParam(_insert);

                                            }
                                        }
                                    }

                                }


                            }
                            promtres.AreaRef = area_refno;
                            promtres.ColRef = collection_refno;
                            return Ok(promtres);
                        }
                    
                        promtres.AreaRef = area_refno;
                        promtres.ColRef = collection_refno;
                        return Ok(promtres);
                    }
                    else
                    {
                        string validate_if_exist = $@"SELECT [Id]
                                              ,[AreaId]
                                              ,[Area_RefNo]
                                              ,[Printed_Status]
                                              ,[Collection_Status]
                                              ,[Denomination]
                                              ,[FieldExpenses]
                                              ,[CollectionRefNo]
                                               FROM [dbo].[tbl_CollectionArea_Model] where CollectionRefNo ='" + collection_refno + "' " +
                                               "and AreaId='"+data.AreaID+"'";
                        DataTable check = db.SelectDb(validate_if_exist).Tables[0];
                        if (check.Rows.Count == 0)
                        {
                            insert += $@"INSERT INTO [dbo].[tbl_CollectionArea_Model]
                                       ([AreaId]
                                       ,[Area_RefNo]
                                       ,[Printed_Status]
                                       ,[CollectionRefNo])
                                 VALUES
                                       ('" + data.AreaID + "'," +
                                   "'" + area_refno + "'," +
                                  "'5'," +
                                   "'" + collection_refno + "') ";
                            var list = dbmet.getAreaLoanSummary().Where(a => a.AreaID == data.AreaID).ToList();
                            db.AUIDB_WithParam(insert);

                            string collectionarea_validate_date = $@"select * from tbl_Collection_AreaMember_Model where DateCollected  = '" + datecreated + "' and Area_RefNo = '" + area_refno + "' ";
                            DataTable tbl_collectionarea_validate_date = db.SelectDb(collectionarea_validate_date).Tables[0];
                            if (tbl_collectionarea_validate_date.Rows.Count == 0)
                            {

                                string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
                         FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
                        where tbl_Area_Model.AreaID ='" + data.AreaID + "'";
                                DataTable area_table = db.SelectDb(areafilter).Tables[0];
                                foreach (DataRow dr_area in area_table.Rows)
                                {
                                    var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                                    for (int x = 0; x < area_city.Count; x++)
                                    {
                                        var spliter = area_city[x].Split(",");
                                        string barangay = spliter[0].Trim();
                                        string city = spliter[1].Trim();

                                        string select_update = $@"select tbl_Application_Model.NAID from 
                                tbl_Application_Model left JOIN tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                where tbl_Member_Model.Barangay ='" + barangay + "' and tbl_Member_Model.City ='" + city + "' ";
                                        DataTable tbl_select_update = db.SelectDb(select_update).Tables[0];
                                        if (tbl_select_update.Rows.Count != 0)
                                        {
                                            foreach (DataRow drt in tbl_select_update.Rows)
                                            {
                                                if (drt["NAID"].ToString() == "NA-027")
                                                {
                                                    var test = "";
                                                }
                                                //    string Update = $@"UPDATE [dbo].[tbl_Collection_AreaMember_Model]
                                                //SET [Area_RefNo] = '" + area_refno + "' " +
                                                //    "WHERE  NAID ='" + drt["NAID"].ToString() + "'";

                                                //    db.AUIDB_WithParam(Update);
                                                string CollectedAmount = "0";
                                                string Advancepayment = "0";
                                                string date_now = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                                                //var insert_item = dbmet.getAreaLoanSummary_2(data.AreaID).Where(a => a.NAID == drt["NAID"].ToString()).FirstOrDefault();
                                                string sql_insert_item = $@"select top (1) tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,
                                                case when tbl_MemberSavings_Model.TotalSavingsAmount is null then 0 
                                                else tbl_MemberSavings_Model.TotalSavingsAmount end as TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection
                                                ,tbl_Application_Model.ReleasingDate
                                                 FROM
                                                tbl_Application_Model inner JOIN
                                                tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner JOIN
                                                tbl_LoanType_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID inner JOIN
                                                tbl_TermsOfPayment_Model on tbl_TermsOfPayment_Model.LoanTypeId = tbl_LoanType_Model.LoanTypeID left JOIN
                                                tbl_MemberSavings_Model on tbl_LoanDetails_Model.MemId = tbl_MemberSavings_Model.MemId inner join
                                                tbl_TermsTypeOfCollection_Model on tbl_TermsOfPayment_Model.CollectionTypeId = tbl_TermsTypeOfCollection_Model.Id
                                                 where tbl_Application_Model.NAID = '" + drt["NAID"].ToString() + "' and tbl_Application_Model.Status = 14 group by tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,tbl_MemberSavings_Model.TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection,tbl_Application_Model.ReleasingDate";
                                                DataTable insert_item = db.SelectDb(sql_insert_item).Tables[0];

                                                if (insert_item.Rows.Count != 0)
                                                {


                                                    string col_Typedate = "";
                                                    var typeofcollection = insert_item.Rows[0]["TypeOfCollection"].ToString();
                                                    string totaldays = "";
                                                    switch (typeofcollection)
                                                    {
                                                        case "Daily":
                                                            totaldays = "0";
                                                            col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                            break;
                                                        case "7 Days":
                                                            totaldays = dbmet.datecomputation(date_now, 7).totaldays.ToString();
                                                            col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                            break;
                                                        case "15 Days":
                                                            totaldays = dbmet.datecomputation(date_now, 15).totaldays.ToString();
                                                            col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                            break;
                                                        case "Monthly":

                                                            totaldays = dbmet.datecomputation(date_now, 31).totaldays.ToString();
                                                            col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                            break;
                                                        default:
                                                            totaldays = "0";
                                                            col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                            break;
                                                    }
                                                    string adv_payment = "";
                                                    string date_create1 = datecreated == "" ? col_Typedate : datecreated;

                                                    string areamember = $@"SELECT * FROM [dbo].[tbl_Collection_AreaMember_Model] where NAID = '" + drt["NAID"].ToString() + "'  and AdvanceStatus=1";
                                                    DataTable tbl_areamember = db.SelectDb(areamember).Tables[0];
                                                    if (tbl_areamember.Rows.Count != 0)
                                                    {
                                                        adv_payment = "0.00";
                                                        paymentstatus = "2";
                                                        Advancepayment = "0";

                                                    }
                                                    else
                                                    {
                                                        adv_payment = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString() == "" ? "0" : insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();

                                                        Advancepayment = insert_item.Rows[0]["NoAdvancePayment"].ToString() == "1" ? "1" : insert_item.Rows[0]["NoAdvancePayment"].ToString();
                                                    }


                                                    if (adv_payment == "0.00")
                                                    {

                                                        CollectedAmount = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();
                                                        paymentstatus = "2";
                                                    }
                                                    else
                                                    {
                                                        if (date_create1 == DateTime.Now.ToString("yyyy-MM-dd"))
                                                        {
                                                            paymentstatus = "1";

                                                        }


                                                    }
                                                    string _insert = $@"INSERT INTO [dbo].[tbl_Collection_AreaMember_Model]
                                                       ([NAID]
                                                       ,[AdvancePayment]
                                                       ,[LapsePayment]
                                                       ,[CollectedAmount]
                                                       ,[Savings]
                                                       ,[Area_RefNo]
                                                       ,[Payment_Status]
                                                       ,[AdvanceStatus]
                                                       ,[Payment_Method]
                                                       ,[DateCollected])
                                                 VALUES
                                                       ('" + drt["NAID"].ToString() + "'," +
                                                                   "'0'," +
                                                                  "'0'," +
                                                                   "'" + adv_payment + "'," +
                                                                  "' 0'," +
                                                                  "'" + area_refno + "'," +
                                                              "'" + paymentstatus + "'," +
                                                                   "'" + Advancepayment + "'," +
                                                                  "'No Payment'," +
                                                              "'" + Convert.ToDateTime(DateTime.Now).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd") + "') ";

                                                    db.AUIDB_WithParam(_insert);

                                                }
                                            }
                                        }

                                    }


                                }
                                promtres.AreaRef = area_refno;
                                promtres.ColRef = collection_refno;
                                return Ok(promtres);
                            }
                          
                            promtres.AreaRef = area_refno;
                            promtres.ColRef = collection_refno;
                            return Ok(promtres);
                        }
                        else
                        {
                            promtres.AreaRef = area_refno;
                            promtres.ColRef = collection_refno;
                            return Ok(promtres);
                        }
                          
                    }
                   
                }
                else //if not equal to datetime now col-ref no
                {
                    //create a new refno
                    string sql_count = $@"SELECT [Id]
                          ,[RefNo]
                          ,[DateCreated]
                      FROM [GoldOne].[dbo].[tbl_CollectionModel]";
                    DataTable table_ = db.SelectDb(sql_count).Tables[0];
                    int rows_count = table_.Rows.Count == 0 ? 1 : table_.Rows.Count + 1;
                    string refno = "COL-" + data.AreaID + DateTime.Now.ToString("yyyyMMdd") + "-0" + rows_count;
                    //check if area_collection is print status is null or rejected
                    var check_print_status = dbmet.getAreaLoanSummary_2(data.AreaID, area_refno).FirstOrDefault();
                    if (check_print_status != null)
                    {

           
                    if (check_print_status.DateCreated != DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        //insert
                        
                        string inserts = $@"INSERT INTO [dbo].[tbl_CollectionModel]
                                           ([RefNo]
                                           ,[DateCreated])
                                            VALUES
                                           ('" + refno + "'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        db.AUIDB_WithParam(inserts);
                        sql = $@"select RefNo from tbl_CollectionModel order by id desc";

                        DataTable table_refno = db.SelectDb(sql).Tables[0];
                        collection_refno = table_refno.Rows[0]["RefNo"].ToString();

                        //arearefno
                        string sql_count_area = $@"SELECT [Id]
                                              ,[AreaId]
                                              ,[Area_RefNo]
                                              ,[Printed_Status]
                                              ,[Collection_Status]
                                              ,[Denomination]
                                              ,[FieldExpenses]
                                              ,[CollectionRefNo]
                                               FROM [dbo].[tbl_CollectionArea_Model]";
                        DataTable table__ = db.SelectDb(sql_count_area).Tables[0];
                        int rows_count_area = table__.Rows.Count == 1 ? 0 : table__.Rows.Count + 1;
                         area_refno = data.AreaID + DateTime.Now.ToString("yyyyMMdd") + "-0" + rows_count;

                        string validate_arearef = $@"SELECT [Id]
                                              ,[AreaId]
                                              ,[Area_RefNo]
                                              ,[Printed_Status]
                                              ,[Collection_Status]
                                              ,[Denomination]
                                              ,[FieldExpenses]
                                              ,[CollectionRefNo]
                                               FROM [dbo].[tbl_CollectionArea_Model] where Area_RefNo ='" + area_refno + "'";
                        DataTable area_ref = db.SelectDb(validate_arearef).Tables[0];
                        if (area_ref.Rows.Count == 0)
                        {
                            insert += $@"INSERT INTO [dbo].[tbl_CollectionArea_Model]
                                       ([AreaId]
                                       ,[Area_RefNo]
                                       ,[Printed_Status]
                                       ,[CollectionRefNo])
                                 VALUES
                                       ('" + data.AreaID + "'," +
                                 "'" + area_refno + "'," +
                                "'5'," +
                                 "'" + collection_refno + "') ";
                            var list = dbmet.getAreaLoanSummary().Where(a => a.AreaID == data.AreaID).ToList();
                            db.AUIDB_WithParam(insert);
                            string collectionarea_validate_date = $@"select * from tbl_Collection_AreaMember_Model where DateCollected  = '" + datecreated + "' and Area_RefNo = '" + area_refno + "' ";
                            DataTable tbl_collectionarea_validate_date = db.SelectDb(collectionarea_validate_date).Tables[0];
                            if (tbl_collectionarea_validate_date.Rows.Count == 0)
                            {

                                string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
                         FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
                        where tbl_Area_Model.AreaID ='" + data.AreaID + "'";
                                DataTable area_table = db.SelectDb(areafilter).Tables[0];
                                foreach (DataRow dr_area in area_table.Rows)
                                {
                                    var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                                    for (int x = 0; x < area_city.Count; x++)
                                    {
                                        var spliter = area_city[x].Split(",");
                                        string barangay = spliter[0].Trim();
                                        string city = spliter[1].Trim();

                                        string select_update = $@"select tbl_Application_Model.NAID from 
                                tbl_Application_Model left JOIN tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                where tbl_Member_Model.Barangay ='" + barangay + "' and tbl_Member_Model.City ='" + city + "' ";
                                        DataTable tbl_select_update = db.SelectDb(select_update).Tables[0];
                                        if (tbl_select_update.Rows.Count != 0)
                                        {
                                            foreach (DataRow drt in tbl_select_update.Rows)
                                            {
                                                if (drt["NAID"].ToString() == "NA-027")
                                                {
                                                    var test = "";
                                                }
                                                //    string Update = $@"UPDATE [dbo].[tbl_Collection_AreaMember_Model]
                                                //SET [Area_RefNo] = '" + area_refno + "' " +
                                                //    "WHERE  NAID ='" + drt["NAID"].ToString() + "'";

                                                //    db.AUIDB_WithParam(Update);
                                                string CollectedAmount = "0";
                                                string Advancepayment = "0";
                                                string date_now = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                                                //var insert_item = dbmet.getAreaLoanSummary_2(data.AreaID).Where(a => a.NAID == drt["NAID"].ToString()).FirstOrDefault();
                                                string sql_insert_item = $@"select top (1) tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,
                                                case when tbl_MemberSavings_Model.TotalSavingsAmount is null then 0 
                                                else tbl_MemberSavings_Model.TotalSavingsAmount end as TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection
                                                ,tbl_Application_Model.ReleasingDate
                                                 FROM
                                                tbl_Application_Model inner JOIN
                                                tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner JOIN
                                                tbl_LoanType_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID inner JOIN
                                                tbl_TermsOfPayment_Model on tbl_TermsOfPayment_Model.LoanTypeId = tbl_LoanType_Model.LoanTypeID left JOIN
                                                tbl_MemberSavings_Model on tbl_LoanDetails_Model.MemId = tbl_MemberSavings_Model.MemId inner join
                                                tbl_TermsTypeOfCollection_Model on tbl_TermsOfPayment_Model.CollectionTypeId = tbl_TermsTypeOfCollection_Model.Id
                                                 where tbl_Application_Model.NAID = '" + drt["NAID"].ToString() + "' and tbl_Application_Model.Status = 14 group by tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,tbl_MemberSavings_Model.TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection,tbl_Application_Model.ReleasingDate";
                                                DataTable insert_item = db.SelectDb(sql_insert_item).Tables[0];
                                                    if (insert_item.Rows.Count != 0)
                                                    {


                                                        string col_Typedate = "";
                                                        var typeofcollection = insert_item.Rows[0]["TypeOfCollection"].ToString();
                                                        string totaldays = "";
                                                        switch (typeofcollection)
                                                        {
                                                            case "Daily":
                                                                totaldays = "0";
                                                                col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                                break;
                                                            case "7 Days":
                                                                totaldays = dbmet.datecomputation(date_now, 7).totaldays.ToString();
                                                                col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                                break;
                                                            case "15 Days":
                                                                totaldays = dbmet.datecomputation(date_now, 15).totaldays.ToString();
                                                                col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                                break;
                                                            case "Monthly":

                                                                totaldays = dbmet.datecomputation(date_now, 31).totaldays.ToString();
                                                                col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                                break;
                                                            default:
                                                                totaldays = "0";
                                                                col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                                break;
                                                        }
                                                        string adv_payment = "";
                                                        string date_create1 = datecreated == "" ? col_Typedate : datecreated;

                                                        string areamember = $@"SELECT * FROM [dbo].[tbl_Collection_AreaMember_Model] where NAID = '" + drt["NAID"].ToString() + "'  and AdvanceStatus=1";
                                                        DataTable tbl_areamember = db.SelectDb(areamember).Tables[0];
                                                        if (tbl_areamember.Rows.Count != 0)
                                                        {
                                                            adv_payment = "0.00";
                                                            paymentstatus = "2";
                                                            Advancepayment = "0";

                                                        }
                                                        else
                                                        {
                                                            adv_payment = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString() == "" ? "0" : insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();

                                                            Advancepayment = insert_item.Rows[0]["NoAdvancePayment"].ToString() == "1" ? "1" : insert_item.Rows[0]["NoAdvancePayment"].ToString();
                                                        }


                                                        if (adv_payment == "0.00")
                                                        {

                                                            CollectedAmount = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();
                                                            paymentstatus = "2";
                                                        }
                                                        else
                                                        {
                                                            if (date_create1 == DateTime.Now.ToString("yyyy-MM-dd"))
                                                            {
                                                                paymentstatus = "1";

                                                            }


                                                        }
                                                        string _insert = $@"INSERT INTO [dbo].[tbl_Collection_AreaMember_Model]
                                                       ([NAID]
                                                       ,[AdvancePayment]
                                                       ,[LapsePayment]
                                                       ,[CollectedAmount]
                                                       ,[Savings]
                                                       ,[Area_RefNo]
                                                       ,[Payment_Status]
                                                       ,[AdvanceStatus]
                                                       ,[Payment_Method]
                                                       ,[DateCollected])
                                                 VALUES
                                                       ('" + drt["NAID"].ToString() + "'," +
                                                                   "'0'," +
                                                                  "'0'," +
                                                                   "'" + adv_payment + "'," +
                                                                  "' 0'," +
                                                                  "'" + area_refno + "'," +
                                                              "'" + paymentstatus + "'," +
                                                                   "'" + Advancepayment + "'," +
                                                                  "'No Payment'," +
                                                              "'" + Convert.ToDateTime(DateTime.Now).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd") + "') ";

                                                    db.AUIDB_WithParam(_insert);

                                                }
                                            }
                                        }

                                    }


                                }
                                promtres.AreaRef = area_refno;
                                promtres.ColRef = collection_refno;
                                return Ok(promtres);
                            }
                          
                            promtres.AreaRef = area_refno;
                            promtres.ColRef = collection_refno;
                            return Ok(promtres);
                        }
                        else
                        {

                            string validate_if_exist = $@"SELECT [Id]
                                              ,[AreaId]
                                              ,[Area_RefNo]
                                              ,[Printed_Status]
                                              ,[Collection_Status]
                                              ,[Denomination]
                                              ,[FieldExpenses]
                                              ,[CollectionRefNo]
                                               FROM [dbo].[tbl_CollectionArea_Model] where CollectionRefNo ='" + collection_refno + "' " +
                                              "and AreaId='" + data.AreaID + "'";
                            DataTable check = db.SelectDb(validate_if_exist).Tables[0];
                            if (check.Rows.Count == 0)
                            {
                                insert += $@"INSERT INTO [dbo].[tbl_CollectionArea_Model]
                                       ([AreaId]
                                       ,[Area_RefNo]
                                       ,[Printed_Status]
                                       ,[CollectionRefNo])
                                 VALUES
                                       ('" + data.AreaID + "'," +
                                       "'" + area_refno + "'," +
                                      "'5'," +
                                       "'" + collection_refno + "') ";
                                var list = dbmet.getAreaLoanSummary().Where(a => a.AreaID == data.AreaID).ToList();
                                db.AUIDB_WithParam(insert);

                                string collectionarea_validate_date = $@"select * from tbl_Collection_AreaMember_Model where DateCollected  = '" + datecreated + "' and Area_RefNo = '" + area_refno + "' ";
                                DataTable tbl_collectionarea_validate_date = db.SelectDb(collectionarea_validate_date).Tables[0];
                                if (tbl_collectionarea_validate_date.Rows.Count == 0)
                                {

                                    string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
                         FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
                        where tbl_Area_Model.AreaID ='" + data.AreaID + "'";
                                    DataTable area_table = db.SelectDb(areafilter).Tables[0];
                                    foreach (DataRow dr_area in area_table.Rows)
                                    {
                                        var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                                        for (int x = 0; x < area_city.Count; x++)
                                        {
                                            var spliter = area_city[x].Split(",");
                                            string barangay = spliter[0].Trim();
                                            string city = spliter[1].Trim();

                                            string select_update = $@"select tbl_Application_Model.NAID from 
                                tbl_Application_Model left JOIN tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                where tbl_Member_Model.Barangay ='" + barangay + "' and tbl_Member_Model.City ='" + city + "' ";
                                            DataTable tbl_select_update = db.SelectDb(select_update).Tables[0];
                                            if (tbl_select_update.Rows.Count != 0)
                                            {
                                                foreach (DataRow drt in tbl_select_update.Rows)
                                                {
                                                    if (drt["NAID"].ToString() == "NA-027")
                                                    {
                                                        var test = "";
                                                    }
                                                    //    string Update = $@"UPDATE [dbo].[tbl_Collection_AreaMember_Model]
                                                    //SET [Area_RefNo] = '" + area_refno + "' " +
                                                    //    "WHERE  NAID ='" + drt["NAID"].ToString() + "'";

                                                    //    db.AUIDB_WithParam(Update);
                                                    string CollectedAmount = "0";
                                                    string Advancepayment = "0";
                                                    string date_now = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                                                    //var insert_item = dbmet.getAreaLoanSummary_2(data.AreaID).Where(a => a.NAID == drt["NAID"].ToString()).FirstOrDefault();
                                                    string sql_insert_item = $@"select top (1) tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,
                                                case when tbl_MemberSavings_Model.TotalSavingsAmount is null then 0 
                                                else tbl_MemberSavings_Model.TotalSavingsAmount end as TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection
                                                ,tbl_Application_Model.ReleasingDate
                                                 FROM
                                                tbl_Application_Model inner JOIN
                                                tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner JOIN
                                                tbl_LoanType_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID inner JOIN
                                                tbl_TermsOfPayment_Model on tbl_TermsOfPayment_Model.LoanTypeId = tbl_LoanType_Model.LoanTypeID left JOIN
                                                tbl_MemberSavings_Model on tbl_LoanDetails_Model.MemId = tbl_MemberSavings_Model.MemId inner join
                                                tbl_TermsTypeOfCollection_Model on tbl_TermsOfPayment_Model.CollectionTypeId = tbl_TermsTypeOfCollection_Model.Id
                                                 where tbl_Application_Model.NAID = '" + drt["NAID"].ToString() + "' and tbl_Application_Model.Status = 14 group by tbl_TermsOfPayment_Model.NoAdvancePayment,tbl_Application_Model.NAID, tbl_LoanDetails_Model.ApprovedAdvancePayment,tbl_MemberSavings_Model.TotalSavingsAmount,tbl_TermsTypeOfCollection_Model.TypeOfCollection,tbl_Application_Model.ReleasingDate";
                                                    DataTable insert_item = db.SelectDb(sql_insert_item).Tables[0];
                                                        if (insert_item.Rows.Count != 0)
                                                        {


                                                            string col_Typedate = "";
                                                            var typeofcollection = insert_item.Rows[0]["TypeOfCollection"].ToString();
                                                            string totaldays = "";
                                                            switch (typeofcollection)
                                                            {
                                                                case "Daily":
                                                                    totaldays = "0";
                                                                    col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                                    break;
                                                                case "7 Days":
                                                                    totaldays = dbmet.datecomputation(date_now, 7).totaldays.ToString();
                                                                    col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                                    break;
                                                                case "15 Days":
                                                                    totaldays = dbmet.datecomputation(date_now, 15).totaldays.ToString();
                                                                    col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                                    break;
                                                                case "Monthly":

                                                                    totaldays = dbmet.datecomputation(date_now, 31).totaldays.ToString();
                                                                    col_Typedate = Convert.ToDateTime(insert_item.Rows[0]["ReleasingDate"].ToString()).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd");
                                                                    break;
                                                                default:
                                                                    totaldays = "0";
                                                                    col_Typedate = DateTime.Now.ToString("yyyy-MM-dd");
                                                                    break;
                                                            }
                                                            string adv_payment = "";
                                                            string date_create1 = datecreated == "" ? col_Typedate : datecreated;

                                                            string areamember = $@"SELECT * FROM [dbo].[tbl_Collection_AreaMember_Model] where NAID = '" + drt["NAID"].ToString() + "'  and AdvanceStatus=1";
                                                            DataTable tbl_areamember = db.SelectDb(areamember).Tables[0];
                                                            if (tbl_areamember.Rows.Count != 0)
                                                            {
                                                                adv_payment = "0.00";
                                                                paymentstatus = "2";
                                                                Advancepayment = "0";

                                                            }
                                                            else
                                                            {
                                                                adv_payment = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString() == "" ? "0" : insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();

                                                                Advancepayment = insert_item.Rows[0]["NoAdvancePayment"].ToString() == "1" ? "1" : insert_item.Rows[0]["NoAdvancePayment"].ToString();
                                                            }


                                                            if (adv_payment == "0.00")
                                                            {

                                                                CollectedAmount = insert_item.Rows[0]["ApprovedAdvancePayment"].ToString();
                                                                paymentstatus = "2";
                                                            }
                                                            else
                                                            {
                                                                if (date_create1 == DateTime.Now.ToString("yyyy-MM-dd"))
                                                                {
                                                                    paymentstatus = "1";

                                                                }


                                                            }
                                                            string _insert = $@"INSERT INTO [dbo].[tbl_Collection_AreaMember_Model]
                                                       ([NAID]
                                                       ,[AdvancePayment]
                                                       ,[LapsePayment]
                                                       ,[CollectedAmount]
                                                       ,[Savings]
                                                       ,[Area_RefNo]
                                                       ,[Payment_Status]
                                                       ,[AdvanceStatus]
                                                       ,[Payment_Method]
                                                       ,[DateCollected])
                                                 VALUES
                                                       ('" + drt["NAID"].ToString() + "'," +
                                                                       "'0'," +
                                                                      "'0'," +
                                                                       "'" + adv_payment + "'," +
                                                                      "' 0'," +
                                                                      "'" + area_refno + "'," +
                                                                  "'" + paymentstatus + "'," +
                                                                       "'" + Advancepayment + "'," +
                                                                      "'No Payment'," +
                                                                  "'" + Convert.ToDateTime(DateTime.Now).AddDays(int.Parse(totaldays)).ToString("yyyy-MM-dd") + "') ";

                                                        db.AUIDB_WithParam(_insert);

                                                    }
                                                }
                                            }

                                        }


                                    }
                                    promtres.AreaRef = area_refno;
                                    promtres.ColRef = collection_refno;
                                    return Ok(promtres);
                                }
                            
                                promtres.AreaRef = area_refno;
                                promtres.ColRef = collection_refno;
                                return Ok(promtres);
                            }
                            else
                            {
                                promtres.AreaRef = area_refno;
                                promtres.ColRef = collection_refno;
                                return Ok(promtres);
                            }
                        }

                    }
                    }
                    promtres.AreaRef = area_refno;
                    promtres.ColRef = collection_refno;
                    return Ok(promtres);
                }





            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }
        
    }
}