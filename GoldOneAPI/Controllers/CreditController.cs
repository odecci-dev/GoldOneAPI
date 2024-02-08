using AuthSystem.Models;
using GoldOneAPI.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        private readonly AppSettings _appSettings;
        //private ApplicationDbContext _context;
        private readonly JwtAuthenticationManager jwtAuthenticationManager;
        private readonly IWebHostEnvironment _environment;

        public CreditController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;
            //this.jwtAuthenticationManager = jwtAuthenticationManager;

        }
        public class CreditFilter
        {
            public string? Fullname { get; set; }
        }
        public class CIDateModel
        {
            public string? MemId { get; set; }
            public string? NAID { get; set; }
            public string? StatusId { get; set; }
            public string? CI_ApprovedBy { get; set; }
            public string? CI_ApprovedDate { get; set; }
            public string? Status { get; set; }
            public string? App_ApprovedBy_1 { get; set; }
            public string? App_ApprovalDate_1 { get; set; }
            public string? App_ApprovalDate_2 { get; set; }
            public string? App_ApprovedBy_2 { get; set; }
            public string? ProceedToCIDate { get; set; }
        }

        //[HttpGet]
        //public async Task<IActionResult> CreditList()
        //{
        //    var result = (dynamic)null;
        //    //var result = new List<CreditModel>();
        //    try
        //    {
        //        result = dbmet.GetCreditList().ToList();
        //        return Ok(result);
        //    }

        //    catch (Exception ex)
        //    {
        //        return BadRequest("ERROR");
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> CreditApplicationApprovalCount()
        {
            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetCreditList().ToList().Count();
      
                return Ok("You have total  " + result + " Application for Approval");
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }    
        [HttpGet]
        public async Task<IActionResult> GetCIDateList(string statusid)
        {
            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetDateCreditInvestigation().Where(a => a.StatusId == statusid).ToList();
      
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoanHistory(string memid)
        {
            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetLoanHistory().Where(a=>a.MemId == memid).ToList();

                return Ok(result );
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentHistory(string naid)
        {
            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.getLoanhistoryAll().Where(a => a.NAID == naid).ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public IActionResult DeclineCI(ApplicationModel data)
        {
            try
            {
                //var result = new ReturnValue();
                string filePath = @"C:\data\decline.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                string Update = "";
                sql = $@"select MemId from tbl_Application_Model where NAID ='" + data.NAID + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new JOModel();
                if (dt.Rows.Count != 0)
                {

                    Update = $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [Status] = '11' , 
                                   Remarks='" + data.Remarks + "'," +
                                   "DeclinedBy='" + data.UserId + "'," +
                                      "[DeclineDate] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "' " +
                                    " where NAID ='" + data.NAID + "'";
                    Update += $@"UPDATE [dbo].[tbl_Member_Model]
                               SET [Status] = '2'" +
                    " where MemId ='" + dt.Rows[0]["MemId"].ToString() + "'";

                    results = db.AUIDB_WithParam(Update) + " Updated";

                    string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                    DataTable username_tbl = db.SelectDb(username).Tables[0];
                    foreach (DataRow dr in username_tbl.Rows)
                    {
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        dbmet.InsertNotification("Declined Application  " + data.NAID + " ",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Credit Investigation Module", name, dr["UserId"].ToString(), "2",data.NAID);
                    }
                    return Ok(results);
                    //string username = $@"SELECT  Fname,Lname,Mname FROM [dbo].[tbl_User_Model] where UserId = '" + data.UserId + "'";
                    //DataTable username_tbl = db.SelectDb(username).Tables[0];
                    //string name = username_tbl.Rows[0]["Fname"].ToString() + " " + username_tbl.Rows[0]["Mname"].ToString() + " " + username_tbl.Rows[0]["Lname"].ToString();
                    //dbmet.InsertNotification("Declined Application  " + data.NAID + " ",
                    //      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Application Module", name, "2");

                  
                }
                else
                {
                    results = "Error!";
                    return BadRequest(results);
                }
            }
            catch (Exception ex)
            {
                results = ex.GetBaseException().ToString();
                return BadRequest(results);
            }
        }
        public class CIAPPROVE
        {
            public string? NAID { get; set; }
            public string? Remarks { get; set; }
            public double? LoanAmount { get; set; }
            public string? UserId { get; set; }
        }
        [HttpPost]
        public IActionResult CreditSubmitforApproval(CIAPPROVE data)
        {
            try
            {
                string Update = "";
                //var result = new ReturnValue();
                string filePath = @"C:\data\CI_Submit_for_approval.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                sql = $@"select * from tbl_Application_Model where NAID ='" + data.NAID + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new JOModel();
                if (dt.Rows.Count != 0)
                {

                    Update += $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [Status] = '9' , 
                                   Remarks='" + data.Remarks + "'," +
                                   "CI_ApprovedBy='" + data.UserId + "'," +
                                   
                                    "[CI_ApprovalDate] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                   " where NAID ='" + data.NAID + "'";

                  
                    Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [LoanAmount] = '" + data.LoanAmount + "' " +
                         "where NAID ='" + data.NAID + "'";


                    results = db.AUIDB_WithParam(Update) + " Updated";
                    string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                    DataTable username_tbl = db.SelectDb(username).Tables[0];
                    foreach (DataRow dr in username_tbl.Rows)
                    {
                        string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                        dbmet.InsertNotification("Submit for Approval Credit Investigation " + data.NAID + " ",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Credit Investigation Module", name, dr["UserId"].ToString(), "2",data.NAID);
                    }
                    return Ok(results);

                    //string username = $@"SELECT  Fname,Lname,Mname FROM [dbo].[tbl_User_Model] where UserId = '" + data.UserId + "'";
                    //DataTable username_tbl = db.SelectDb(username).Tables[0];
                    //string name = username_tbl.Rows[0]["Fname"].ToString() + " " + username_tbl.Rows[0]["Mname"].ToString() + " " + username_tbl.Rows[0]["Lname"].ToString();
                    //dbmet.InsertNotification("Approved Application For Credit Investigation  " + data.NAID + "",
                    //      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Credit Investigation Module", name, "2");

                  

                }
                else
                {
                    results = "Error!";
                    return BadRequest(results);
                }
            }
            catch (Exception ex)
            {
                results = ex.GetBaseException().ToString();
                return BadRequest(results);
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoanDetailsPagination(int page, int pageSize)
        {


            try
            {
                var result = dbmet.GetLoanHistory().ToList();


                int totalItems = dbmet.GetLoanHistory().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetLoanHistory().Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> CreditListPagination(int page , int pageSize )
        {

            
            try
            {
                var result = dbmet.GetCreditList().ToList();


                int totalItems = dbmet.GetCreditList().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetCreditList().Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> GetCIDate(int page, int pageSize)
        {


            try
            {
                var result = dbmet.GetCreditList().ToList();


                int totalItems = dbmet.GetCreditList().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetCreditList().Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> CreditListPaginationFilterbyFullname(int page, int pageSize,string? fullname, string loantype)
        {


            try
            {
                int totalItems = 0;
                int totalPages = 0;
                var result = (dynamic)null;
                var items = (dynamic)null;
                if (fullname == null)
                {
                    result = dbmet.GetCreditList().Where(a => a.LoanTypeName.ToUpper() == loantype.ToUpper()).ToList();


                    totalItems = dbmet.GetCreditList().Where(a =>  a.LoanTypeName.ToUpper() == loantype.ToUpper()).ToList().Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = dbmet.GetCreditList().Where(a =>  a.LoanTypeName.ToUpper() == loantype.ToUpper()).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    result = dbmet.GetCreditList().Where(a => a.Fullname.ToUpper().Contains(fullname.ToUpper()) && a.LoanTypeName.ToUpper() == loantype.ToUpper()).ToList();


                    totalItems = dbmet.GetCreditList().Where(a => a.Fullname.ToUpper().Contains(fullname.ToUpper()) && a.LoanTypeName.ToUpper() == loantype.ToUpper()).ToList().Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = dbmet.GetCreditList().Where(a => a.Fullname.ToUpper().Contains(fullname.ToUpper()) && a.LoanTypeName.ToUpper() == loantype.ToUpper()).Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        [HttpPost]
        public async Task<IActionResult> LoanDetailsFilterMemId(MemberIDFilter data)
        {

            try
            {
                var result = dbmet.GetLoanHistory().Where(a=>a.MemId == data.MemId).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }


        }
        [HttpPost]
        public async Task<IActionResult> CreditMemberFilterbyMemID(MemberIDFilter data)
        {

            try
            {
                var result = (dynamic)null;
                var counter = dbmet.GetMemberList().Where(a => a.MemId ==data.MemId).ToList();
                if (counter.Count != 0)
                {

                    return Ok(counter);

                }
                else
                {
                    return BadRequest("No Data");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }


        }


    }
}