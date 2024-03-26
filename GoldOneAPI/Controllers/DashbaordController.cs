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
using Microsoft.VisualBasic;
using static GoldOneAPI.Controllers.CollectionController;
using Newtonsoft.Json;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json.Linq;
using static GoldOneAPI.Controllers.FieldAreaController;
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashbaordController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();


        public class DashboardVM
        {
          
            public int? ActiveMemberCount { get; set; }
            public double? TotalLoanBalance { get; set; }
            public double? TotalInterest { get; set; }
            public double? TotalLoanCollection { get; set; }
            public double? TotaolAdvancePayment { get; set; }
            public double? TotalOtherDeductions { get; set; }
            public int? TotalActiveStanding { get; set; }
            public double? TotalFullPayment { get; set; }
            public double? TotalCR { get; set; }
            public int? TotalEndingActiveMember { get; set; }
            public double? TotalSvaingsOutstanding { get; set; }
            public double? TotalDailyOverallCollection { get; set; }
            public double? TotalNewAccountsOverall { get; set; }
            public double? TotalApplicationforApproval { get; set; }
            public double? TotalIncome { get; set; }
            public double? TotalIncomePercentage { get; set; }
            public double? TotalDailyCollection { get; set; }
            public int? TotalDaysLeft { get; set; }
            public double? TotalPercentOfLastEntry { get; set; }
            public int? TargetStatus { get; set; }
            public int ActiveMember { get; set; }
            public List<TotalLapsesArea2>? TotalLapsesArea { get; set; }
            public List<TopCollectiblesArea2>? TopCollectiblesAreas { get; set; }
            public List<AreaActiveCollection>? AreaActiveCollection { get; set; }



        }
        public class TopCollectiblesArea2
        {

            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public double? Amount { get; set; }
            public double? DailyCollectibles { get; set; }
            public int? SundayCount { get; set; }
            public int? HolidayCount { get; set; }

        }
        public class LapsesDaily
        {

            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public double? Amount { get; set; }
            public double? DailyLapses { get; set; }
            public int? SundayCount { get; set; }
            public int? HolidayCount { get; set; }

        }
        public class TopCollectiblesArea
        {
        
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public double? Amount { get; set; }

        }
        public class TotalLapsesArea
        {

            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public double? Amount { get; set; }

        } public class TotalLapsesArea2
        {

            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public double? Amount { get; set; }

        }
        public class ActiveMemberVM
        {
            public int? ActiveMemberCount { get; set; }
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public string? DateCreated { get; set; }

        }
        public class TotalLapses
        {
            public string? AreaId { get; set; }
            public string? AreaName { get; set; }
            public double? TotalLapsesPayment { get; set; }
        }
        public class TopMembersPenalty //top3
        {
            public string? BorrowerName { get; set; }
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public string? DateCreated { get; set; }
            public double? LoanAmount { get; set; }
            public double? Penalty { get; set; }

        }
        public class AreaActiveCollection //top3
        {
            public string? AreaId { get; set; }
            public string? Area { get; set; }
            public double? ActiveCollection { get; set; }
            public int? NewAccount { get; set; }
            public string? MemId { get; set; }
            public int? NoPayment { get; set; }
            public double? PastDueCollection { get; set; }

        }

        public class TopTenMembersDueDates
        {
            public string? BorrowerName { get; set; }
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public string? FOID { get; set; }
            public string? FieldOfficer { get; set; }
            public string? DueDate { get; set; }
            public double? Balance { get; set; }
            public double? Penalty { get; set; }

        }
        public class ActiveMember
        {
            public string? AreaName { get; set; }
            public string? Date { get; set; }
            public int? count { get; set; }

       

        }
        public class monthsdate
        {
            public string label { get; set; }

        }
        public static IEnumerable<dynamic> MonthsBetween(
            DateTime startDate,
            DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {

                var firstDayOfMonth = new DateTime(iterator.Year, iterator.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                yield return new
                {
                    label = dateTimeFormat.GetMonthName(iterator.Month)

                };

                iterator = iterator.AddMonths(1);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DashboardGraph(int days, string? category)
        {
            int total = 0;
            int total_count = 0;
            int daysLeft = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear - DateTime.Now.DayOfYear;
            int day = days == 1 ? 334 : days;
            string datecreated = "";
            int count_ = 0;
            var result = new List<ActiveMember>();
            DateTime startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(-day);

            DateTime endDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

            var months = MonthsBetween(startDate, endDate).ToList();
            var items = new List<monthsdate>();
            var mo = JsonConvert.SerializeObject(months);
            var list = JsonConvert.DeserializeObject<List<monthsdate>>(mo);
           
            string sqls = $@"select Count(*) as count from tbl_Member_Model where status=1";
            DataTable dts = db.SelectDb(sqls).Tables[0];
            try
            {

                string sqlarea = $@"select  * from tbl_Area_Model where status=1 and Area ='"+category+"'";
                DataTable sqlarea_dtl = db.SelectDb(sqls).Tables[0];
                if (sqlarea_dtl.Rows.Count != 0)
                {
                    string sql1 = "";
                    string areas = "";
                    if (days == 1)
                    {

                        for (int i = 0; i < list.Count; i++)
                        {
                            string query = "";
                            if (category == null)
                            {
                                sql1 = $@"SELECT  DATENAME(month,DateCreated)  AS month , count(*) as count from tbl_Member_Model where status = 1   and DATENAME(month,DateCreated) = '" + list[i].label + "'  group by   DATENAME(month,DateCreated)   ";
                                areas = "ALL AREAS";

                            }
                            else
                            {

                                sql1 = $@"select DATENAME(month,tbl_Member_Model.DateCreated)  AS month , count(*) as count ,tbl_Area_Model.Area  from tbl_Application_Model inner join
                                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID inner join
                                tbl_Member_Model on tbl_Member_Model.MemId = tbl_LoanDetails_Model.MemId inner join
                                tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.City + '%'
                                where tbl_Member_Model.status = 1  and  DATENAME(month,tbl_Member_Model.DateCreated) = '" + list[i].label + "' and tbl_Area_Model.Area ='" + category + "'  group by   DATENAME(month,tbl_Member_Model.DateCreated) ,tbl_Area_Model.Area   ";
                                areas = category;
                            }

                            var item = new ActiveMember();
                            //string sql1 = $@"SELECT  DATENAME(month,DateCreated)  AS month , count(*) as count from tbl_Member_Model where status = 1 and DATENAME(month,DateCreated) = '" + list[i].label + "' and  tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "'    group by   DATENAME(month,DateCreated)   ";
                            DataTable dt1 = db.SelectDb(sql1).Tables[0];
                            query += sql1;

                            if (dt1.Rows.Count != 0)
                            {
                                datecreated = dt1.Rows[0]["month"].ToString();
                                total_count = int.Parse(dt1.Rows[0]["count"].ToString());

                            }
                            else
                            {

                                datecreated = list[i].label;
                                total_count = 0;
                            }



                            item.AreaName = areas;
                            item.count = total_count;
                            item.Date = datecreated;
                            result.Add(item);




                        }
                    }
                    else
                    {
                        List<DateTime> allDates = new List<DateTime>();

                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            var dategen = date.Date.ToString("yyyy-MM-dd");

                            if (category == null)
                            {
                                sql1 = $@"select  CONVERT(VARCHAR, tbl_Member_Model.DateCreated, 23)  as DateCreated , Count(*) as count from tbl_Member_Model 
                        where tbl_Member_Model.status = 1  and CONVERT(VARCHAR, tbl_Member_Model.DateCreated, 23)='" + dategen + "'  group by CONVERT(VARCHAR, tbl_Member_Model.DateCreated, 23) order by  CONVERT(VARCHAR, tbl_Member_Model.DateCreated, 23)   ";
                                areas = "ALL AREAS";

                            }
                            else
                            {

                                sql1 = $@"select  CONVERT(VARCHAR, tbl_Member_Model.DateCreated, 23) as DateCreated ,Count(*) as count ,tbl_Area_Model.Area  from tbl_Application_Model inner join
                        tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID inner join
                        tbl_Member_Model on tbl_Member_Model.MemId = tbl_LoanDetails_Model.MemId inner join
                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.City + '%'
                        where tbl_Member_Model.status = 1  and CONVERT(VARCHAR, tbl_Member_Model.DateCreated, 23)='" + dategen + "'    and tbl_Area_Model.Area='" + category + "'  group by tbl_Member_Model.DateCreated,tbl_Area_Model.Area order by  tbl_Member_Model.DateCreated   ";
                                areas = category;
                            }
                            DataTable dt1 = db.SelectDb(sql1).Tables[0];
                            if (dt1.Rows.Count == 0)
                            {
                                datecreated = dategen;
                                total_count = 0;
                            }
                            else
                            {
                                foreach (DataRow dr in dt1.Rows)
                                {
                                    datecreated = dr["DateCreated"].ToString();
                                    total_count = int.Parse(dr["count"].ToString());
                                }
                            }


                            //    }




                            //}
                            string sql = $@"SELECT count(*) as count
                                         FROM  tbl_Member_Model
                                         WHERE DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
                            DataTable dt = db.SelectDb(sql).Tables[0];
                            var item = new ActiveMember();
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    count_ += total_count;
                                    item.count = total_count;
                                    item.AreaName = areas;

                                    item.Date = datecreated;
                                    result.Add(item);

                                }


                            }
                            else
                            {
                                return BadRequest("ERROR");
                            }

                        }
                    }
                }
                else
                {
                    return BadRequest("No Data Found");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> ActiveCollection()
        {
            var day_total = dbmet.dashdatecomputation();
            DataTable table = db.SelectDb_SP("sp_fieldareas").Tables[0];
            var result = new List<AreaActiveCollection>();
            int mem = 0;
            
            foreach (DataRow dr in table.Rows)
            {
                var no_payment = dbmet.GetActiveCollectionMemberDashboard(int.Parse(day_total.totaldays), dr["AreaID"].ToString()).FirstOrDefault();
                var no_payments = dbmet.GetActiveCollectionMemberDashboard(int.Parse(day_total.totaldays), dr["AreaID"].ToString()).Select(a => a.NoPayment).FirstOrDefault();
                var dailyCollectiblesSum = dbmet.getAreaLoanSummary_2(dr["AreaID"].ToString(), "").Select(a => double.Parse(a.DailyCollectibles)).Sum();
                var pastdue = dbmet.GetActiveCollectionDashboard(int.Parse(day_total.totaldays), dr["AreaID"].ToString()).Where(a => a.AreaId == dr["AreaID"].ToString()).Select(a => a.PastDueCollection).Sum();
                var count_mem = dbmet.GetActiveCollectionMemberDashboard(int.Parse(day_total.totaldays), dr["AreaID"].ToString()).GroupBy(x=>x).Select(group => new { group.Key, Counter = group.Count() });
                foreach (var item in count_mem)
                {
                    if (item.Counter == 1)
                    {
                        var mem_res = item.Key;
                        mem++;
                    }

                
                }
                double collectedamount_total = dailyCollectiblesSum * double.Parse(day_total.totaldays);
                var items = new AreaActiveCollection();
                items.AreaId = dr["AreaID"].ToString();
                items.Area = dr["AreaName"].ToString();
                items.NewAccount = mem;
                items.NoPayment = no_payment == null ? 0 : no_payment.NoPayment;
                items.PastDueCollection = no_payment == null ? 0 : no_payment.PastDueCollection;;
                items.ActiveCollection = collectedamount_total;
                result.Add(items);
                mem = 0;

            }

                return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> TopCollectibles()
        {
            var day_total = dbmet.dashdatecomputation();

            DataTable table = db.SelectDb_SP("sp_fieldareas").Tables[0];
            var result = new List<AreaVM>();
                var collect_res2 = new List<TopCollectiblesArea2>();

            foreach (DataRow dr in table.Rows)
            {
                var dailyCollectiblesSum = dbmet.getAreaLoanSummary_2(dr["AreaID"].ToString(), "").Select(a => double.Parse(a.DailyCollectibles)).Sum();
                if (dailyCollectiblesSum != 0)
                    {
                        double collectedamount_total = dailyCollectiblesSum * double.Parse(day_total.totaldays);
                        var items = new TopCollectiblesArea2();
                        items.Amount = collectedamount_total;
                        items.AreaName = dr["AreaName"].ToString();
                    items.DailyCollectibles = dailyCollectiblesSum;
                    items.SundayCount = int.Parse(day_total.sundaycount);
                    items.HolidayCount = int.Parse(day_total.ctr_holiday);
                        items.AreaID = dr["AreaID"].ToString();
                    collect_res2.Add(items);
                    }
                }

           
            return Ok(collect_res2);
        }

        [HttpGet]
        public async Task<IActionResult> TotalLapsesAreas()
        {
            var day_total = dbmet.dashdatecomputation();

            var list = dbmet.GetTotalLapses().ToList();
            var collect_res2 = new List<LapsesDaily>();
            for (int x= 0;x < list.Count;x++)
            {
                 if (list[x].TotalLapsesPayment != 0)
                {
                    double collectedamount_total = double.Parse(list[x].TotalLapsesPayment.ToString()) * double.Parse(day_total.totaldays);
                    var items = new LapsesDaily();
                    items.Amount = collectedamount_total;
                    items.AreaName = list[x].AreaName;
                    items.DailyLapses = list[x].TotalLapsesPayment;
                    items.SundayCount = int.Parse(day_total.sundaycount);
                    items.HolidayCount = int.Parse(day_total.ctr_holiday);
                    items.AreaID = list[x].AreaId;
                    collect_res2.Add(items);
                }
            }


            return Ok(collect_res2);
        }
        [HttpGet]
        public async Task<IActionResult> DashboaredView()
        {
          
            //var result = new List<CreditModel>();
            try
            {
                var result = new List<DashboardVM>();
                var memberlist = dbmet.GetAllMemberList().Where(a=>a.STATUS != "Inactive" && a.STATUS != "New Application").ToList();
                var memberlist_daily = dbmet.GetAllMemberList_daily().Where(a=>a.STATUS != "Inactive" && a.STATUS != "New Application").ToList();
            
                var getdailyreset = dbmet.GetSettingList().FirstOrDefault();
                int day = int.Parse(getdailyreset.DisplayReset) == 1 ? 30 : 334;
                string sql_loanbal = $@"select 
                                        sum(OutstandingBalance) as LoanBalance
                                        from 
                                        tbl_LoanHistory_Model where tbl_LoanHistory_Model.DateCreated >= DATEADD(day,-" + day + ", GETDATE())";
                DataTable tbl_loanbal = db.SelectDb(sql_loanbal).Tables[0];

                string sql_interest = $@"select 
                                        sum(ApproveedInterest) as TotalInterest
                                        from 
                                        tbl_LoanDetails_Model where DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
                DataTable tbl_interest = db.SelectDb(sql_interest).Tables[0];

                string sql_notary = $@"select 
                                        sum(ApprovedNotarialFee) as TotalNotarial
                                        from 
                                        tbl_LoanDetails_Model where DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
                DataTable tbl_notary = db.SelectDb(sql_notary).Tables[0];

     

                string sql_loancollection = $@"select 
                                        sum(ApprovedDailyAmountDue) as TotalCollection
                                        from 
                                        tbl_LoanDetails_Model where DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
                DataTable tbl_collection = db.SelectDb(sql_loancollection).Tables[0];

                //ApprovedAdvancePayment

                string sql_advancepayment = $@"select 
                                        sum(ApprovedAdvancePayment) as TotalAdvancePayment
                                        from 
                                        tbl_LoanDetails_Model where DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
                DataTable tbl_advancepayment = db.SelectDb(sql_advancepayment).Tables[0];

                string sql_savingsoutstanding = $@"select 
                                        sum(TotalSavingsAmount) as TotalSavings
                                        from 
                                        tbl_MemberSavings_Model ";
                DataTable tbl_savings = db.SelectDb(sql_savingsoutstanding).Tables[0];

                string sql_newacccounts = $@"SELECT tbl_Application_Model.NAID
                                        FROM tbl_Application_Model INNER JOIN
                                        tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                        where tbl_Application_Model.status = '7'";
                DataTable tbl_newaccount = db.SelectDb(sql_newacccounts).Tables[0];


                string sql_forapproval = $@"SELECT tbl_Application_Model.NAID
                                        FROM tbl_Application_Model INNER JOIN
                                        tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                        where tbl_Application_Model.NAID = '9'";
                DataTable tbl_approval = db.SelectDb(sql_forapproval).Tables[0];

                string sql_cr = $@"SELECT tbl_Application_Model.NAID
                                        FROM tbl_Application_Model INNER JOIN
                                        tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                        where tbl_Application_Model.NAID = '14'";
                DataTable tbl_cr = db.SelectDb(sql_cr).Tables[0];

                string sql_fullpayment = $@" SELECT tbl_LoanHistory_Model.OutstandingBalance,
                                         tbl_LoanHistory_Model.MemId
                                         FROM   tbl_LoanHistory_Model INNER JOIN
                                         tbl_Member_Model ON tbl_LoanHistory_Model.MemId = tbl_Member_Model.MemId
                                         where tbl_LoanHistory_Model.OutstandingBalance = 0";
                DataTable tbl_fullpayment = db.SelectDb(sql_fullpayment).Tables[0];


                string sql_collectedamount_daily = $@" select sum( CollectedAmount ) as totalcollected from
                                                tbl_Collection_AreaMember_Model 
                                                where  CONVERT(VARCHAR, tbl_Collection_AreaMember_Model.DateCollected, 23) <= CONVERT(VARCHAR, GETDATE(), 23) ";
                DataTable tbl_collectedamount_daily = db.SelectDb(sql_collectedamount_daily).Tables[0];


                var lifeinsurance = dbmet.getAreaLoanSummary().Select(a => double.Parse(a.LifeInsurance)).Sum();
                var loaninsurance = dbmet.getAreaLoanSummary().Select(a => double.Parse(a.LoanInsurance)).Sum();
                var totalincome = dbmet.GetSettingList().Select(a => a.MonthlyTarget).FirstOrDefault();
 
                var activemembercount = dbmet.GetActiveMember(day).ToList();
                var item = new DashboardVM();
                item.ActiveMemberCount = activemembercount.Count;
                item.TotalLoanBalance = Math.Ceiling(double.Parse(tbl_loanbal.Rows[0]["LoanBalance"].ToString() == "" ? "0" : tbl_loanbal.Rows[0]["LoanBalance"].ToString()));
                item.TotalInterest = Math.Ceiling(double.Parse(tbl_interest.Rows[0]["TotalInterest"].ToString() == "" ? "0" : tbl_interest.Rows[0]["TotalInterest"].ToString()));
                item.TotalLoanCollection = Math.Ceiling(double.Parse(tbl_collection.Rows[0]["TotalCollection"].ToString() == "" ? "0" : tbl_collection.Rows[0]["TotalCollection"].ToString()));
                item.TotaolAdvancePayment = Math.Ceiling(double.Parse(tbl_advancepayment.Rows[0]["TotalAdvancePayment"].ToString() == "" ? "0" : tbl_advancepayment.Rows[0]["TotalAdvancePayment"].ToString()));
                item.TotalSvaingsOutstanding = Math.Ceiling(double.Parse(tbl_savings.Rows[0]["TotalSavings"].ToString() == "" ? "0" : tbl_savings.Rows[0]["TotalSavings"].ToString()));
                item.TotalDailyOverallCollection = Math.Ceiling(double.Parse(tbl_collection.Rows[0]["TotalCollection"].ToString() == "" ? "0" : tbl_collection.Rows[0]["TotalCollection"].ToString()));
                item.TotalNewAccountsOverall = tbl_newaccount.Rows.Count;
                item.TotalApplicationforApproval = tbl_approval.Rows.Count;
                item.TotalActiveStanding = memberlist_daily.Count;
                item.TotalFullPayment = tbl_fullpayment.Rows.Count;
                item.TotalCR = tbl_cr.Rows.Count;
                item.TotalIncome = double.Parse(totalincome);
                item.ActiveMember = activemembercount.Count;
                item.TotalDailyCollection = double.Parse(tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString() == "" ? "0" : tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString());
                item.TotalIncomePercentage = double.Parse(tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString() == "" ? "0" : tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString()) /   double.Parse(totalincome) * 100;

           
                item.TotalOtherDeductions = Math.Ceiling(double.Parse(tbl_interest.Rows[0]["TotalInterest"].ToString() == "" ? "0" : tbl_interest.Rows[0]["TotalInterest"].ToString()) + double.Parse(loaninsurance.ToString()) + double.Parse(lifeinsurance.ToString()) + double.Parse(tbl_notary.Rows[0]["TotalNotarial"].ToString() == "" ?  "0" : tbl_notary.Rows[0]["TotalNotarial"].ToString()));
                //var areas = dbmet.CollectionGroupby().ToList();
                //var printed_area = dbmet.Collection_PrintedResult().ToList();
                //var raw_list = dbmet.Collection_NotPrintedResult().ToList();
                ////IEnumerable<AreaDetailsVM> notInListB = printed_area.Except(raw_list);
                //var list1 = raw_list.Where(p2 => !areas.Any(p1 => p1.AreaID == p2.AreaID)).ToList();
                //double collectedamount = 0;
                //// Concatenate the result with listB to get the desired list.
                ////List<AreaDetailsVM> resultList = 
                ////List<CollectionPrintedVM> resultList = dbmet.Collection_PrintedResult().ToList();
                //List<CollectionPrintedVM> resultList = areas.Concat(list1).ToList();
                //var collect_res = new List<TopCollectiblesArea>();
                //var collect_res2 = new List<TopCollectiblesArea2>();
                ////string t_colamount = "";
                ////for (int x = 0; x < resultList.Count; x++)
                ////{
                ////    var items = new TopCollectiblesArea();
                ////    t_colamount = resultList[x].Total_collectedAmount.ToString() == "" ? "0" : resultList[x].Total_collectedAmount.ToString();
                ////    items.Amount = Math.Ceiling(double.Parse(resultList[x].Total_collectedAmount.ToString()));
                ////    items.AreaName = resultList[x].AreaName;
                ////    items.AreaID = resultList[x].AreaID;
                ////    collect_res.Add(items);

                ////}
                //var lapses2 = new List<TotalLapsesArea2>();
                ////var lapses = new List<TotalLapsesArea>();
                ////for (int x = 0; x < resultList.Count; x++)
                ////{
                ////    var items = new TotalLapsesArea();

                ////    items.Amount = Math.Ceiling(double.Parse(resultList[x].Total_lapses.ToString()));
                ////    items.AreaName = resultList[x].AreaName;
                ////    items.AreaID = resultList[x].AreaID;


                ////    lapses.Add(items);

                ////}


                //var col_res_group = resultList.GroupBy(a => new { a.AreaID, a.AreaName }).ToList();
                //var col_res_grouplist = dbmet.Collection_PrintedResult().ToList();
                //foreach (var group in col_res_group)
                //{
                //    var Total_collectedAmount = resultList.Where(a => a.AreaName == group.Key.AreaName).Select(a => a.Total_collectedAmount).Sum();
                //    //var savings = list.Where(a => a.RefNo == group.Key.RefNo && a.Savings != "").Select(a => double.Parse(a.Savings)).Sum();
                //    if (Total_collectedAmount != 0)
                //    {
                //        var items = new TopCollectiblesArea2();
                //        //t_colamount =  //resultList[x].Total_collectedAmount.ToString() == "" ? "0" : resultList[x].Total_collectedAmount.ToString();
                //        items.Amount = Total_collectedAmount;//Math.Ceiling(double.Parse(resultList[x].Total_collectedAmount.ToString()));
                //        items.AreaName = group.Key.AreaName;
                //        items.AreaID = group.Key.AreaID;
                //        collect_res2.Add(items);
                //    }
                //}

                //foreach (var group in col_res_group)
                //{
                //    var Total_lapses = resultList.Where(a => a.AreaName == group.Key.AreaName).Select(a => a.Total_lapses).Sum();
                //    if (Total_lapses != 0)
                //    {

                //        var items = new TotalLapsesArea2();
                //        items.Amount = Total_lapses;
                //        items.AreaName = group.Key.AreaName;
                //        items.AreaID = group.Key.AreaID;


                //        lapses2.Add(items);
                //    }
                //}
                //var activecol = new List<AreaActiveCollection>();
                ////for (int x = 0; x < resultList.Count; x++)
                ////{
                //    //var _items = new AreaActiveCollection();

                //    //_items.Area = "";
                //    //_items.ActiveCollection = 0;
                //    //_items.NewAccount = 0;
                //    //_items.NoPayment = 0;
                //    //_items.PastDueCollection = 0;


                //    activecol.Add(_items);

                //}
                //AreaActiveCollection
                //item.AreaActiveCollection = activecol;
                //item.TotalLapsesArea = lapses2;
                //item.TopCollectiblesAreas = collect_res2;

                result.Add(item);

                

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}