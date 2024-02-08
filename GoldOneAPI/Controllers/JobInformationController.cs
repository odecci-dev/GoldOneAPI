using AuthSystem.Manager;
using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobInformationController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DbManager db = new DbManager();
        private readonly AppSettings _appSettings;
        //private ApplicationDbContext _context;
        private readonly JwtAuthenticationManager jwtAuthenticationManager;
        private readonly IWebHostEnvironment _environment;

        public JobInformationController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;
            //this.jwtAuthenticationManager = jwtAuthenticationManager;

        }

        public class JOModelVM
        {
            public int Id { get; set; }
            public string? JobDescription { get; set; }
            public string? YOS { get; set; }
            public string? MonthlySalary { get; set; }
            public string? OtherSOC { get; set; }
            public string? BO_Status { get; set; }
            public string? MemId { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyID { get; set; }
            public string? Status { get; set; }
            public string? Emp_Status { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }

        }
     
        [HttpGet]
        public async Task<IActionResult> GetJobInformationList()
        {

            string sql = $@"SELECT        tbl_JobInfo_Model.Id, tbl_JobInfo_Model.JobDescription, tbl_JobInfo_Model.YOS, tbl_JobInfo_Model.MonthlySalary, tbl_JobInfo_Model.OtherSOC, tbl_JobInfo_Model.DateCreated, tbl_JobInfo_Model.DateUpdated, 
                         tbl_JobInfo_Model.BO_Status, tbl_JobInfo_Model.MemId, tbl_CompanyModel.CompanyName, tbl_CompanyModel.CompanyID, tbl_Status_Model.Name AS Status, tbl_Status_Model_1.Name AS Emp_Status
FROM            tbl_JobInfo_Model INNER JOIN
                         tbl_CompanyModel ON tbl_JobInfo_Model.CompanyName = tbl_CompanyModel.CompanyID INNER JOIN
                         tbl_Status_Model ON tbl_JobInfo_Model.Status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_JobInfo_Model.Emp_Status = tbl_Status_Model_1.Id
WHERE        (tbl_JobInfo_Model.Status <> 2)";
            var result = new List<JOModelVM>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new JOModelVM();
                item.Id = int.Parse(dr["id"].ToString());
                item.JobDescription = dr["JobDescription"].ToString();
                item.YOS = dr["JobDescription"].ToString();
                item.MonthlySalary = dr["JobDescription"].ToString();
                item.OtherSOC = dr["JobDescription"].ToString();
                item.BO_Status = dr["JobDescription"].ToString();
                item.MemId = dr["JobDescription"].ToString();
                item.CompanyName = dr["JobDescription"].ToString();
                item.CompanyID = dr["JobDescription"].ToString();
                item.Status = dr["JobDescription"].ToString();
                item.Emp_Status = dr["JobDescription"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                result.Add(item);
            }

            return Ok(result);
        }
      
        [HttpPost]
        public IActionResult UpdateJobInfo(JOModel data)
        {
            try
            {
                sql = $@"select * from tbl_JobInfo_Model where id ='" + data.Id + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new JOModel();
                if (dt.Rows.Count != 0)
                {

                    string OTPInsert = $@"update tbl_JobInfo_Model set 
                                    JobDescription='" + data.JobDescription + "', " +
                                       "YOS='" + data.YOS + "', " +
                                       "CompanyName='" + data.CompanyID + "', " +
                                       "MonthlySalary='" + data.MonthlySalary + "', " +
                                       "OtherSOC='" + data.OtherSOC + "', " +
                                       "Status='" + data.Status + "', " +
                                       "Emp_Status='" + data.Emp_Status + "', " +
                                       "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                       "where Id='" + data.Id + "'";
                    
                    results = db.AUIDB_WithParam(OTPInsert) + " Updated";
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
        public IActionResult SavejobInfo(JOModel data)
        {

            try
            {
                string sql1 = $@"select * from tbl_JobInfo_Model where JobDescription ='" + data.JobDescription + "' ";
                DataTable dt1 = db.SelectDb(sql1).Tables[0];
                if (dt1.Rows.Count == 0)
                {


                    string Insert = $@"insert into   tbl_JobInfo_Model (
                                [JobDescription]
                               ,[YOS]
                               ,[CompanyName]
                               ,[MonthlySalary]
                               ,[OtherSOC]
                               ,[Status]
                               ,[BO_Status]
                               ,[Emp_Status]
                               ,[MemId]
                               ,[DateCreated]) 
                                values
                                ('" +data.JobDescription+"'," +
                               "'" + data.YOS + "'," +
                               "'" + data.CompanyID + "'," +
                               "'" + data.MonthlySalary + "'," +
                               "'" + data.OtherSOC + "'," +
                               "'" + data.Status + "'," +
                               "'" + data.BO_Status + "'," +
                               "'" + data.Emp_Status + "'," +
                               "'" + data.MemId + "'," +
                                "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                    results = db.AUIDB_WithParam(Insert) + " Added";
                    return Ok(results);
                }
                else
                {
                    results = "Duplicate Entry";
                    return Ok(results);
                }
            }
            catch (Exception ex)
            {
                results = ex.GetBaseException().ToString();
                return BadRequest(results);
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostJobSearching(FilterModel data)
        {


            try
            {
                sql = $@"SELECT        tbl_JobInfo_Model.Id, tbl_JobInfo_Model.JobDescription, tbl_JobInfo_Model.YOS, tbl_JobInfo_Model.MonthlySalary, tbl_JobInfo_Model.OtherSOC, tbl_JobInfo_Model.DateCreated, tbl_JobInfo_Model.DateUpdated, 
                         tbl_JobInfo_Model.BO_Status, tbl_JobInfo_Model.MemId, tbl_CompanyModel.CompanyName, tbl_CompanyModel.CompanyID, tbl_Status_Model.Name AS Status, tbl_Status_Model_1.Name AS Emp_Status
FROM            tbl_JobInfo_Model INNER JOIN
                         tbl_CompanyModel ON tbl_JobInfo_Model.CompanyName = tbl_CompanyModel.CompanyID INNER JOIN
                         tbl_Status_Model ON tbl_JobInfo_Model.Status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_JobInfo_Model.Emp_Status = tbl_Status_Model_1.Id
WHERE        (tbl_JobInfo_Model.Status <> 2) and " + data.Type + " like '%" + data.Values + "%'";

                var result = new List<JOModelVM>();
                DataTable table = db.SelectDb(sql).Tables[0];

                foreach (DataRow dr in table.Rows)
                {
               
                    var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var item = new JOModelVM();
                    item.Id = int.Parse(dr["id"].ToString());
                    item.JobDescription = dr["JobDescription"].ToString();
                    item.YOS = dr["JobDescription"].ToString();
                    item.MonthlySalary = dr["JobDescription"].ToString();
                    item.OtherSOC = dr["JobDescription"].ToString();
                    item.BO_Status = dr["JobDescription"].ToString();
                    item.MemId = dr["JobDescription"].ToString();
                    item.CompanyName = dr["JobDescription"].ToString();
                    item.CompanyID = dr["JobDescription"].ToString();
                    item.Status = dr["JobDescription"].ToString();
                    item.Emp_Status = dr["JobDescription"].ToString();
                    item.DateCreated = datec;
                    item.DateUpdated = dateu;
                    result.Add(item);
                }

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public IActionResult DeleteJobInformation(DeleteModel data)
        {
            try
            {
                sql = $@"select * from tbl_JobInfo_Model where id ='" + data.Id + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {

                    string OTPInsert = $@"update tbl_JobInfo_Model set Status='2' " +
                                    "where Id='" + data.Id + "'";
                   
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
    }
}
