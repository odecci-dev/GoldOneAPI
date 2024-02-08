using GoldOneAPI.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();

        public class Reports_Release
        {
            public string? NAID { get; set; }
            public string? Borrower { get; set; }
            public string? Co_Borrower { get; set; }
            public string? Area { get; set; }
            public string? Status { get; set; }
            public string? LoanType { get; set; }
            public string? LoanAmount { get; set; }
            public string? AdvancePayment { get; set; }
            public string? ReleasingDate { get; set; }
            public string? MemId { get; set; }
            public string? DueDate { get; set; }
            public string? TermofPayment { get; set; }

        }
        public class Reports_Collection
        {
            public string? AreaName { get; set; }
            public string? FieldOfficer { get; set; }
            public string? TotalCollection { get; set; }
            public string? TotalSavings { get; set; }
            public string? TotalLapses { get; set; }
            public string? TotalAdvance { get; set; }
            public string? CashRemit { get; set; }
            public string? TotalNP { get; set; }
            public string? DateCollected { get; set; }

        }
        public class Reports_Savings
        {
            public string? Borrower { get; set; }
            public string? AreaName { get; set; }
            public string? TotalSavings { get; set; }
            public string? TotalCollection { get; set; }

        }
        public class Reports_PastDue
        {
            public string? Borrower { get; set; }
            public string? LoanAmount { get; set; }
            public string? DateReleased { get; set; }
            public string? DueDate { get; set; }
            public string? TotalNP { get; set; }
            public string? TotalPastDueDay { get; set; }
            public string? TotalCollection { get; set; }

        }
        public class Reports_Outstanding
        {
            public string? OverAllActiveMemberCount { get; set; }
            public List<Reports_OverAllOutstanding>? OverAllOutstanding { get; set; }
            public List<Reports_ActiveOutstanding>? ActiveOutstanding { get; set; }
            public List<Reports_PDOutstanding>? PDOutstanding { get; set; }
        }
        public class Reports_PDOutstanding
        {
            public string? DueDate { get; set; }
            public string? DateCollected { get; set; }
            public string? Penalty { get; set; }
            public string? LoanBalance { get; set; }
            public string? LoanCollection { get; set; }
            public string? LoanRelease { get; set; }

        }
        public class Reports_ActiveOutstanding
        {
            public string? DueDate { get; set; }
            public string? DateCollected { get; set; }
            public string? Penalty { get; set; }
            public string? Savings { get; set; }
            public string? LoanBalance { get; set; }
            public string? LoanCollection { get; set; }
            public string? LoanRelease { get; set; }
            public string? Interest { get; set; }
            public string? AdvancePayment { get; set; }
            public string? LoanInsurance { get; set; }
            public string? AmountPayable { get; set; }
            public string? OtherDeductions { get; set; }
            public string? LoanOutstanding { get; set; }
            public string? ReleasingDate { get; set; }

        }
        public class Reports_OverAllOutstanding
        {
            public string? DueDate { get; set; }
            public string? DateCollected { get; set; }
            public string? Penalty { get; set; }
            public string? Savings { get; set; }
            public string? LoanBalance { get; set; }
            public string? LoanCollection { get; set; }
            public string? LoanRelease { get; set; }
            public string? Interest { get; set; }
            public string? AdvancePayment { get; set; }
            public string? LoanInsurance { get; set; }
            public string? AmountPayable { get; set; }
            public string? OtherDeductions { get; set; }
            public string? LoanOutstanding { get; set; }
            public string? ReleasingDate { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> Reports_ReleasingList(int page, int pageSize, DateTime datefrom , DateTime dateto)
        {

            var res_list = dbmet.GetReport_ReleaseList().ToList();

            //return Ok();

            try
            {
                //var filteredList = yourList.Where(item => item.DateProperty >= dateFrom && item.DateProperty <= dateTo).ToList();
                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
             
                    var list = res_list.Where(a => Convert.ToDateTime(a.ReleasingDate) >= datefrom && Convert.ToDateTime(a.ReleasingDate) <=dateto ).ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                

                var paginationHeader = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };


                return Ok(items);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Reports_CollectionList(int page, int pageSize, DateTime datefrom, DateTime dateto)
        {

            var res_list = dbmet.GetReport_CollectionList().ToList();

            //return Ok();

            try
            {
                //var filteredList = yourList.Where(item => item.DateProperty >= dateFrom && item.DateProperty <= dateTo).ToList();
                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;

                var list = res_list.Where(a => Convert.ToDateTime(a.DateCollected) >= datefrom && Convert.ToDateTime(a.DateCollected) <= dateto).ToList();


                totalItems = res_list.Count;
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                items = res_list.Skip((page - 1) * pageSize).Take(pageSize).ToList();


                var paginationHeader = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };


                return Ok(items);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> Reports_SavingsList(int page, int pageSize,string? borrower)
        {


            var res_list = dbmet.GetReport_MemberSavingsList().ToList();

            //return Ok();

            try
            {
                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if(borrower == null)
                {
                    var list = res_list.ToList();
                    totalItems = res_list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = res_list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = res_list.Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper())).ToList();
                    totalItems = res_list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = res_list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }


                var paginationHeader = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };


                return Ok(items);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Reports_PastDueList(int page, int pageSize, string? borrower)
        {


            var res_list = dbmet.GetReport_MemberPastDueList().ToList();

            //return Ok();

            try
            {
                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (borrower == null)
                {
                    var list = res_list.ToList();
                    totalItems = res_list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = res_list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = res_list.Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper())).ToList();
                    totalItems = res_list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = res_list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }


                var paginationHeader = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };


                return Ok(items);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Reports_OutstandingList(DateTime datefrom, DateTime dateto)
        {

            try
            {
               
                var items = dbmet.Collection_PrintedResult().ToList();


                return Ok(items);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
    }
}