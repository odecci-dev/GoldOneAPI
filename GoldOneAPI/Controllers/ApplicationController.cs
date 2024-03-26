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
    public class ApplicationController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();


        public class ApplicationVM
        {
            public string? MemId { get; set; }
            public string? GroupId { get; set; }
            public string? GroupName { get; set; }
            public string? DateCreated { get; set; }
            public string? DateApproval { get; set; }
            public string? Remarks { get; set; }
            public string? NAID { get; set; }
            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? Borrower { get; set; }
            public string? Mem_status { get; set; }
            public string? LoanAmount { get; set; }
            public string? LoanType { get; set; }
            public string? LoanTypeID { get; set; }
            public string? LDID { get; set; }
            public string? BorrowerCno { get; set; }
            public string? Cno { get; set; }
            public string? CoBorrower { get; set; }
            public string? Co_Cno { get; set; }
            public string? RefNo { get; set; }
            public string? AreaName { get; set; }
            public string? TermsOfPayment { get; set; }
            public string? NameOfTerms { get; set; }
            public string? Interest { get; set; }

            public string? CI_ApprovedBy { get; set; }
            public string? CI_ApprovalDate { get; set; }
            public string? ReleasingDate { get; set; }
            public string? DeclineDate { get; set; }
            public string? DeclinedBy { get; set; }
            public string? App_ApprovedBy_1 { get; set; }
            public string? App_ApprovalDate_1 { get; set; }
            public string? App_ApprovedBy_2 { get; set; }
            public string? App_ApprovalDate_2 { get; set; }
            public string? App_Note { get; set; }
            public string? App_Notedby { get; set; }
            public string? App_NotedDate { get; set; }
            public string? CreatedBy { get; set; }
            public string? SubmittedBy { get; set; }
            public string? ReleasedBy { get; set; }
            public string? DateSubmitted { get; set; }

            public string? ModeOfRelease { get; set; }
            public string? ModeOfReleaseReference { get; set; }
            public string? Courerier { get; set; }
            public string? CourierCNo { get; set; }
            public string? CourerierName { get; set; }
            public string? Denomination { get; set; }
            public string? ApprovedLoanAmount { get; set; }
            public string? ApprovedTermsOfPayment { get; set; }
            public string? Days { get; set; }
            public string? ProfilePath { get; set; }



        }
        public class NewApplicationVM
        {
            public string? MemId { get; set; }
            public string? DateCreated { get; set; }
            public string? DateApproval { get; set; }
            public string? Remarks { get; set; }
            public string? NAID { get; set; }
            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? Borrower { get; set; }
            public string? LoanAmount { get; set; }
            public string? LoanType { get; set; }
            public string? BorrowerCno { get; set; }
            public string? CoBorrower { get; set; }
            public string? Co_Cno { get; set; }
            public string? RefNo { get; set; }
            public string? AreaName { get; set; }
            public string? TermsOfPayment { get; set; }
            public string? Interest { get; set; }
            public List<ApplicationVM2>? IndividualLoan { get; set; }
            public List<GroupApplicationVM2>? GroupLoan { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> IndividualList()
        {
            try
            {
                var result = dbmet.GetIndividualList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> IndividualListPaginate(int page, int pageSize)
        {
            try
            {
                var result = dbmet.GetIndividualList().ToList();


                int totalItems = dbmet.GetIndividualList().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetIndividualList().Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> IndividualListPaginateFilter(int page, int pageSize, string borrower)
        {
            try
            {
                var result = dbmet.GetIndividualList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper())).ToList();


                int totalItems = dbmet.GetIndividualList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper())).ToList().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetIndividualList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper())).Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> NewApplicationList()
        {
            try
            {
                var result = dbmet.GetNewApplicationList().Where(a=>a.StatusId == "7").ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        [HttpGet]
        public async Task<IActionResult> NewApplicationListPaginate(int page, int pageSize)
        {
            try
            {
                var result = dbmet.GetNewApplicationList().Where(a => a.StatusId == "7").ToList();
                int totalItems = result.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> NewApplicationListPaginateFilterbyFullname(int page, int pageSize, string fullname, string loantype)
        {
            try
            {
                var result = dbmet.GetNewApplicationList().Where(a => a.Borrower.ToUpper().Contains(fullname.ToUpper()) && a.LoanType.ToUpper() == loantype.ToUpper() && a.StatusId == "7").ToList();
                int totalItems = dbmet.GetNewApplicationList().Where(a => a.Borrower.ToUpper().Contains(fullname.ToUpper()) && a.LoanType.ToUpper() == loantype.ToUpper() && a.StatusId == "7").ToList().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetNewApplicationList().Where(a => a.Borrower.ToUpper().Contains(fullname.ToUpper()) && a.LoanType.ToUpper() == loantype.ToUpper() && a.StatusId == "7").Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> ApplicationListPaginateFilter(int page, int pageSize, string borrower)
        {
            try
            {
                var result = dbmet.GetNewApplicationList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper()) && a.StatusId == "7" ).ToList();
                int totalItems = dbmet.GetNewApplicationList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper()) && a.StatusId == "7").ToList().Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var items = dbmet.GetNewApplicationList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper()) && a.StatusId == "7").Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
        public async Task<IActionResult> Application_TrashListPaginateFilter(int page, int pageSize, string? borrower)
        {
            try
            {
                if ( borrower != null)
                {
                    var result = dbmet.GetApplicationList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper()) && a.StatusId == "").ToList();
                    int totalItems = dbmet.GetApplicationList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper()) && a.StatusId == "").ToList().Count;
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    var items = dbmet.GetApplicationList().Where(a => a.Borrower.ToUpper().Contains(borrower.ToUpper()) && a.StatusId == "").Skip((page - 1) * pageSize).Take(pageSize).ToList();

                    var paginationHeader = new
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        CurrentPage = page,
                        PageSize = pageSize
                    };
                    return Ok(items);
                }
                else
                {
                    var result = dbmet.GetApplicationList().Where(a =>  a.StatusId == "").ToList();
                    int totalItems = dbmet.GetApplicationList().Where(a =>  a.StatusId == "").ToList().Count;
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    var items = dbmet.GetApplicationList().Where(a => a.StatusId == "").Skip((page - 1) * pageSize).Take(pageSize).ToList();

                    var paginationHeader = new
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        CurrentPage = page,
                        PageSize = pageSize
                    };
                    return Ok(items);
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLastApplication()
        {
            var res = (dynamic)null;
            try
            {
                var result = dbmet.GetNewApplicationList().Where(a => a.StatusId == "7" || a.StatusId == "8").LastOrDefault();
                if (result == null)
                {
                    res = "no data";
                }
                else
                {
                    res = result;
                }
                return Ok(res);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        public class Application
        {

            public string NAID { get; set; }

        }
        [HttpPost]
        public IActionResult RestoreApplication(Application data)
        {
            try
            {
                sql = $@"select tbl_Member_Model.MemId from tbl_Application_Model inner join 
                tbl_Member_Model on tbl_Member_Model.MemId = tbl_Application_Model.MemId where NAID ='" + data.NAID + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                string results = "";
                if (dt.Rows.Count != 0)
                {
                    string sql1 = $@"select * from tbl_Member_Model where MemId ='" + dt.Rows[0]["MemId"].ToString() + "' ";
                    DataTable dt2 = db.SelectDb(sql).Tables[0];
                    if (dt2.Rows.Count != 0)
                    {
                        //string OTPInsert = $@"delete table tbl_Application_Model " +
                        //                "where NAID='" + data.NAID + "'";
                        string OTPInsert = $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [Status] = '7' " +
                                "where NAID='" + data.NAID + "' and MemId='"+ dt.Rows[0]["MemId"].ToString() + "' and Status = 0";
                        db.AUIDB_WithParam(OTPInsert);
                        string member = $@"UPDATE [dbo].[tbl_Member_Model]
                               SET [Status] = '2' " +
                              "where MemId='" + dt.Rows[0]["MemId"].ToString() + "'";
                        db.AUIDB_WithParam(member);
                        results = "Successfully Deleted";
                       
                    }


                    return Ok(results);
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

        [HttpPost]
        public IActionResult DeleteApplication(Application data)
        {
            try
            {
                sql = $@"select * from tbl_Application_Model where NAID ='" + data.NAID + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {

                    //string OTPInsert = $@"delete table tbl_Application_Model " +
                    //                "where NAID='" + data.NAID + "'";
                    string OTPInsert = $@"UPDATE [dbo].[tbl_Application_Model]
                               SET [Status] = '0' " +
                            "where NAID='" + data.NAID + "'";
                   
                    results = db.AUIDB_WithParam(OTPInsert)+" Deleted";
                    return Ok(results);


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


        [HttpPost]
        public IActionResult ApplicationFilterbyFullname(FilterFullname data)
        {
            try
            {
                var result = (dynamic)null;
                var counter = dbmet.GetApplicationList().Where(a => a.Borrower.ToUpper().Contains(data.Fullname.ToUpper())).ToList();
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