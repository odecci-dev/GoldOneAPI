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
            public List<TotalLapsesArea>? TotalLapsesArea { get; set; }
            public List<TopCollectiblesArea>? TopCollectiblesAreas { get; set; }
            public List<AreaActiveCollection>? AreaActiveCollection { get; set; }



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

        }
        public class ActiveMemberVM
        {
            public int? ActiveMemberCount { get; set; }
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
            public string? DateCreated { get; set; }

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
            public string? Area { get; set; }
            public double? ActiveCollection { get; set; }
            public int? NewAccount { get; set; }
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
                if (days == 0 && category == null)
                {

                    string query = "";
                    string areafilter = $@"SELECT  tbl_Area_Model.Id, tbl_Area_Model.Area,tbl_Area_Model.City from tbl_Area_Model where Status=1";
                    DataTable area_table = db.SelectDb(areafilter).Tables[0];
                    var item = new ActiveMember();
                    foreach (DataRow dr_area in area_table.Rows)
                    {
                       

                        var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                        for (int x = 0; x < area_city.Count; x++)
                        {
                            var spliter = area_city[x].Split(",");
                            string barangay = spliter[0];
                            string city = spliter[1];

                            string sql1 = $@"SELECT  DATENAME(month,DateCreated)  AS month , count(*) as count from tbl_Member_Model where status = 1 and DATENAME(month,DateCreated) = '" + list[0].label + "' and  tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "'    group by   DATENAME(month,DateCreated)   ";
                            DataTable dt1 = db.SelectDb(sql1).Tables[0];
                            query += sql1;

                            if (dt1.Rows.Count != 0)
                            {
                                datecreated = dt1.Rows[0]["month"].ToString();
                                total_count = int.Parse(dt1.Rows[0]["count"].ToString());
                            }


                        }

                        count_ += total_count;
                    


                    }
                 
                    item.AreaName = "ALL AREAS";
                    item.count = count_;
                    item.Date = datecreated;
                    result.Add(item);

                }
                else if (days == 0 && category != null)
                {
                    string query = "";
                    string areafilter = $@"SELECT  tbl_Area_Model.Id, tbl_Area_Model.Area,tbl_Area_Model.City from tbl_Area_Model where Status=1";
                    DataTable area_table = db.SelectDb(areafilter).Tables[0];
                    foreach (DataRow dr_area in area_table.Rows)
                    {
                        var item = new ActiveMember();
                        var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                        for (int x = 0; x < area_city.Count; x++)
                        {
                            var spliter = area_city[x].Split(",");
                            string barangay = spliter[0];
                            string city = spliter[1];
                        
                               string sql1 = $@"SELECT  DATENAME(month,DateCreated)  AS month , count(*) as count from tbl_Member_Model where status = 1 and DATENAME(month,DateCreated) = '" + list[0].label + "' and  tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "'    group by   DATENAME(month,DateCreated)   ";
                                DataTable dt1 = db.SelectDb(sql1).Tables[0];
                            query += sql1;

                                if (dt1.Rows.Count != 0)
                                {
                                datecreated = dt1.Rows[0]["month"].ToString();
                                total_count = int.Parse(dt1.Rows[0]["count"].ToString());
                                }

                  
                        }

                        count_ += total_count;
                        item.count = total_count;
                        item.AreaName = dr_area["Area"].ToString();
                     
                        item.Date = datecreated;
                        result.Add(item);


                    }

               }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
            return Ok(result);
        }
        //    [HttpGet]
        //public async Task<IActionResult> ActiveMemberFilterbyDays(int days,string? category)
        //{
        //    //var memberlist = dbmet.GetAllMemberList().Where(a => a.STATUS != "Inactive" && a.STATUS != "New Application").ToList();
        //    //var date = DateTime.Now.AddDays(days);
        //    //var activemembercount = memberlist.Where(a => Convert.ToDateTime(a.DateCreated) >= DateTime.Now.AddDays(-days)).ToList();
        //    //return Ok(activemembercount.Count);

        //    int total = 0;
        //    int daysLeft = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear - DateTime.Now.DayOfYear;
        //    int day = days == 1 ? 334 : days;
        //    string datecreated = "";
        //    int count_ = 0;
        //    var result = new List<ActiveMember>();
        //    try
        //    {
        //        string sqls = $@"select Count(*) as count from tbl_Member_Model where status=1";
        //        DataTable dts = db.SelectDb(sqls).Tables[0];

        //        foreach (DataRow dr in dts.Rows)
        //        {
        //            total = int.Parse(dr["count"].ToString());
        //        }

        //        DateTime startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(-day);

        //        DateTime endDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

        //        var months = MonthsBetween(startDate, endDate).ToList();
        //        var items = new List<monthsdate>();
        //        var mo = JsonConvert.SerializeObject(months);
        //        var list = JsonConvert.DeserializeObject<List<monthsdate>>(mo);
        //        if (days == 1)
        //        {
        //            for (int x = 0; x < list.Count; x++)
        //            {
        //                //var item = new monthsdate();
        //                //var month = list[x].label;
        //                //item.label = month;
        //                //items.Add(item);
        //                string sql1 = $@"SELECT  DATENAME(month,DateCreated)  AS month , count(*) as count from tbl_Member_Model where status = 1 and DATENAME(month,DateCreated) = '" + list[x].label + "'   group by   DATENAME(month,DateCreated)   ";
        //                DataTable dt1 = db.SelectDb(sql1).Tables[0];


        //                if (dt1.Rows.Count == 0)
        //                {
        //                    datecreated = list[x].label;
        //                    count_ = 0;
        //                }
        //                else
        //                {
        //                    foreach (DataRow dr in dt1.Rows)
        //                    {
        //                        datecreated = dr["month"].ToString();
        //                        count_ = int.Parse(dr["count"].ToString());
        //                    }
        //                }

        //                string sql = $@"SELECT count(*) as count
        //                 FROM  tbl_Member_Model
        //                 WHERE DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
        //                DataTable dt = db.SelectDb(sql).Tables[0];
        //                var item = new ActiveMember();
        //                if (dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow dr in dt.Rows)
        //                    {
        //                        double val1 = double.Parse(dr["count"].ToString());
        //                        double val2 = double.Parse(total.ToString());

        //                        double results = Math.Abs(val1 / val2 * 100);
        //                        item.count = int.Parse(dr["count"].ToString());
        //                        item.Date = datecreated;
        //                        result.Add(item);

        //                    }


        //                }
        //                else
        //                {
        //                    return BadRequest("ERROR");
        //                }

        //            }

        //        }
        //        else
        //        {
        //            string query = $@"select Count(*) as count from tbl_Member_Model where status=1";
        //            DataTable dtble = db.SelectDb(query).Tables[0];

        //            foreach (DataRow dr in dtble.Rows)
        //            {
        //                total = int.Parse(dr["count"].ToString());
        //            }
        //            List<DateTime> allDates = new List<DateTime>();

        //            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        //            {
        //                //allDates.Add(date.Date);
        //                var dategen = date.Date.ToString("yyyy-MM-dd");

        //                string sql1 = $@"select DateCreated,Count(*) as count from tbl_Member_Model where status = 1 and DateCreated='" + dategen + "' group by DateCreated order by  DateCreated ";
        //                DataTable dt1 = db.SelectDb(sql1).Tables[0];


        //                if (dt1.Rows.Count == 0)
        //                {
        //                    datecreated = dategen;
        //                    count_ = 0;
        //                }
        //                else
        //                {
        //                    foreach (DataRow dr in dt1.Rows)
        //                    {
        //                        datecreated = dr["DateCreated"].ToString();
        //                        count_ = int.Parse(dr["count"].ToString());
        //                    }
        //                }


        //                string sql = $@"SELECT count(*) as count
        //                 FROM  tbl_Member_Model
        //                 WHERE DateCreated >= DATEADD(day,-" + day + ", GETDATE()) and status= 1";
        //                DataTable dt = db.SelectDb(sql).Tables[0];

        //                double val1 = double.Parse(dt.Rows[0]["count"].ToString());
        //                var item = new ActiveMember();

        //                double val2 = double.Parse(total.ToString());

        //                double results = Math.Abs(val1 / val2 * 100);


        //                item.Date = DateTime.Parse(datecreated).ToString("dd");
        //                item.count = count_;
        //                item.AreaName = "ALL AREAS";
        //                result.Add(item);
        //            }

        //        }


        //        return Ok(result);
        //    }

        //    catch (Exception ex)
        //    {
        //        return BadRequest("ERROR");
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> DashboaredView()
        {
          
            //var result = new List<CreditModel>();
            try
            {
                var result = new List<DashboardVM>();
                var memberlist = dbmet.GetAllMemberList().Where(a=>a.STATUS != "Inactive" && a.STATUS != "New Application").ToList();
                var memberlist_daily = dbmet.GetAllMemberList_daily().Where(a=>a.STATUS != "Inactive" && a.STATUS != "New Application").ToList();

                string sql_loanbal = $@"select 
                                        sum(OutstandingBalance) as LoanBalance
                                        from 
                                        tbl_LoanHistory_Model
                                         ";
                DataTable tbl_loanbal = db.SelectDb(sql_loanbal).Tables[0];

                string sql_interest = $@"select 
                                        sum(ApproveedInterest) as TotalInterest
                                        from 
                                        tbl_LoanDetails_Model";
                DataTable tbl_interest = db.SelectDb(sql_interest).Tables[0];

                string sql_notary = $@"select 
                                        sum(ApprovedNotarialFee) as TotalNotarial
                                        from 
                                        tbl_LoanDetails_Model";
                DataTable tbl_notary = db.SelectDb(sql_notary).Tables[0];

     

                string sql_loancollection = $@"select 
                                        sum(ApprovedDailyAmountDue) as TotalCollection
                                        from 
                                        tbl_LoanDetails_Model";
                DataTable tbl_collection = db.SelectDb(sql_loancollection).Tables[0];

                //ApprovedAdvancePayment

                string sql_advancepayment = $@"select 
                                        sum(ApprovedAdvancePayment) as TotalAdvancePayment
                                        from 
                                        tbl_LoanDetails_Model";
                DataTable tbl_advancepayment = db.SelectDb(sql_advancepayment).Tables[0];

                string sql_savingsoutstanding = $@"select 
                                        sum(TotalSavingsAmount) as TotalSavings
                                        from 
                                        tbl_MemberSavings_Model";
                DataTable tbl_savings = db.SelectDb(sql_savingsoutstanding).Tables[0];

                string sql_newacccounts = $@"SELECT tbl_Application_Model.NAID
                                        FROM tbl_Application_Model INNER JOIN
                                        tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId
                                        where tbl_Application_Model.NAID = '7'";
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
                var activemembercount = memberlist.Where(a => Convert.ToDateTime(a.DateCreated) >= DateTime.Now.AddDays(-7)).ToList();
                var item = new DashboardVM();
                item.ActiveMemberCount = memberlist.Count;
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
                item.TotalDailyCollection = double.Parse(tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString() == null ? "0" : tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString());
                item.TotalIncomePercentage = double.Parse(tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString() == null ? "0" : tbl_collectedamount_daily.Rows[0]["totalcollected"].ToString()) /   double.Parse(totalincome) * 100;

           
                item.TotalOtherDeductions = Math.Ceiling(double.Parse(tbl_interest.Rows[0]["TotalInterest"].ToString() == "" ? "0" : tbl_interest.Rows[0]["TotalInterest"].ToString()) + double.Parse(loaninsurance.ToString()) + double.Parse(lifeinsurance.ToString()) + double.Parse(tbl_notary.Rows[0]["TotalNotarial"].ToString() == "" ?  "0" : tbl_notary.Rows[0]["TotalNotarial"].ToString()));
                var areas = dbmet.CollectionGroupby().ToList();
                var printed_area = dbmet.Collection_PrintedResult().ToList();
                var raw_list = dbmet.Collection_NotPrintedResult().ToList();
                //IEnumerable<AreaDetailsVM> notInListB = printed_area.Except(raw_list);
                var list1 = raw_list.Where(p2 => !areas.Any(p1 => p1.AreaID == p2.AreaID)).ToList();
                double collectedamount = 0;
                // Concatenate the result with listB to get the desired list.
                //List<AreaDetailsVM> resultList = 
                //List<CollectionPrintedVM> resultList = dbmet.Collection_PrintedResult().ToList();
                List<CollectionPrintedVM> resultList = areas.Concat(list1).ToList();
                var collect_res = new List<TopCollectiblesArea>();
                string t_colamount = "";
                for (int x = 0; x < resultList.Count; x++)
                {
                    var items = new TopCollectiblesArea();
                    t_colamount = resultList[x].Total_collectedAmount.ToString() == "" ? "0": resultList[x].Total_collectedAmount.ToString();
                    items.Amount =  Math.Ceiling(double.Parse(resultList[x].Total_collectedAmount.ToString()));
                    items.AreaName = resultList[x].AreaName;
                    items.AreaID = resultList[x].AreaID;


                    collect_res.Add(items);

                }
                var lapses = new List<TotalLapsesArea>();
                for (int x = 0; x < resultList.Count; x++)
                {
                    var items = new TotalLapsesArea();
 
                    items.Amount = Math.Ceiling(double.Parse(resultList[x].Total_lapses.ToString()));
                    items.AreaName = resultList[x].AreaName;
                    items.AreaID = resultList[x].AreaID;


                    lapses.Add(items);

                }
                var activecol = new List<AreaActiveCollection>();
                //for (int x = 0; x < resultList.Count; x++)
                //{
                    var _items = new AreaActiveCollection();

                    _items.Area = "";
                    _items.ActiveCollection = 0;
                    _items.NewAccount = 0;
                    _items.NoPayment = 0;
                    _items.PastDueCollection = 0;


                    activecol.Add(_items);

                //}
                //AreaActiveCollection
                item.AreaActiveCollection = activecol;
                item.TotalLapsesArea = lapses;
                item.TopCollectiblesAreas = collect_res;

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