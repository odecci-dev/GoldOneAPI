using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Text;
using static GoldOneAPI.Controllers.MemberController;
using static GoldOneAPI.Controllers.GroupController;
using System.Data.SqlClient;
using static GoldOneAPI.Controllers.FieldAreaController;
using GoldOneAPI.Manager;
using System.Text.Json;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HolidayController : ControllerBase
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
        public class HolidayListVM
        {
            public string? HolidayName { get; set; }
            public string? Date { get; set; }
            public string? Location { get; set; }
            public string? RepeatYearly { get; set; }
            public string? DateCreatd { get; set; }
            public string? DateUpdated { get; set; }
            public string? Status { get; set; }
            public string? HolidayID { get; set; }


        }
        public class HolidayModelUpdate
        {
            public string? HolidayName { get; set; }
            public DateTime? Date { get; set; }
            public string? Location { get; set; }
            public int? RepeatYearly { get; set; }
            public string? HolidayID { get; set; }

        }
        public HolidayController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;

        }

        [HttpGet]
        public async Task<IActionResult> HolidayList()
        {

            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetHolidayList().Where(a => a.Status == "Active").ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> HolidayTrash(int page, int pageSize, string? holidayname)
        {
            try
            {
                if (holidayname != null)
                {
                    var result = dbmet.GetHolidayList().Where(a => a.Status == "InActive" && a.HolidayName.ToUpper().Contains(holidayname.ToUpper())).ToList();


                    int totalItems = dbmet.GetHolidayList().Where(a => a.Status == "InActive" &&  a.HolidayName.ToUpper().Contains(holidayname.ToUpper())).ToList().Count;
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    var items = dbmet.GetHolidayList().Where(a => a.Status == "InActive" &&  a.HolidayName.ToUpper().Contains(holidayname.ToUpper())).Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                    var result = dbmet.GetHolidayList().Where(a => a.Status == "InActive" ).ToList();


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
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> HolidaySearching(int page, int pageSize, string? holidayname)
        {
            try
            {
                if (holidayname != null)
                {
                    var result = dbmet.GetHolidayList().Where(a => a.Status == "Active" &&  a.HolidayName.ToUpper().Contains(holidayname.ToUpper())).ToList();


                    int totalItems = dbmet.GetHolidayList().Where(a => a.Status == "Active" &&  a.HolidayName.ToUpper().Contains(holidayname.ToUpper())).ToList().Count;
                    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    var items = dbmet.GetHolidayList().Where(a => a.Status == "Active" &&  a.HolidayName.ToUpper().Contains(holidayname.ToUpper())).Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
                    var result = dbmet.GetHolidayList().Where(a => a.Status == "Active").ToList();


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
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLastHolidayList()
        {


            try
            {
                var result = (dynamic)null;
                //var result = new List<CreditModel>();
                try
                {
                    result = dbmet.GetHolidayList().OrderByDescending(a=>a.HolidayID).FirstOrDefault();

                    return Ok(result);
                }

                catch (Exception ex)
                {
                    return BadRequest("ERROR");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        public class HolidayFilter
        {
            public string? HolidayID { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> HolidayViewFilter(HolidayFilter data)
        {
            try
            {
                var result = (dynamic)null;
                //var result = new List<CreditModel>();
                try
                {
                    var counter = dbmet.GetHolidayList().Where(a => a.HolidayID == data.HolidayID && a.Status == "Active").Select(a => a.HolidayID).ToList();
                    if( counter.Count != 0)
                    {
                        result = dbmet.GetHolidayList().Where(a => a.HolidayID == data.HolidayID && a.Status == "Active").ToList();
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest("No data available");
                    }
            
                }

                catch (Exception ex)
                {
                    return BadRequest("ERROR");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNewHoliday(HolidayModel data)
        {
            string result = "";
            try
            {
                string Insert = "";
                //string filePath = @"C:\data\Add_new_holiday.json"; // Replace with your desired file path

                //dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                string areafilter = $@"SELECT [Id]
                                  ,[Area]
                                  ,[City]
                                   FROM [dbo].[tbl_Area_Model]
                                    Where  City='" + data.Location + "'";
                DataTable area_table = db.SelectDb(areafilter).Tables[0];
                if (area_table.Rows.Count == 0)
                {
                    string insert_area = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                       ([Area],[City])
                                        VALUES
                                        ('" + data.Location + "', " +
                                        "'" + data.Location + "')";
                    results = db.AUIDB_WithParam(insert_area) + " Added";
                }
                sql = $@"select * from tbl_Holiday_Model where HolidayName ='" + data.HolidayName + "' and Status  = 1";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count == 0)
                {
                    Insert = $@"INSERT INTO [dbo].[tbl_Holiday_Model]
                               ([HolidayName]
                               ,[Date]
                               ,[Location]
                               ,[RepeatYearly]
                               ,[Status]
                               ,[DateCreated])
                         VALUES
                               ('" + data.HolidayName + "'," +
                                "'" + Convert.ToDateTime(data.Date).ToString("yyyy-MM-dd") + "', " +
                                "'" + data.Location + "'," +
                                "'" + data.RepeatYearly + "'," +
                                "'1'," +
                                "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";


                    results = db.AUIDB_WithParam(Insert) + " Added";
                    return Ok(results);
                }
                else
                {
                    return BadRequest("Duplicate Entry");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateHolidayDetails(HolidayModelUpdate data)
        {
            string result = "";
            try
            {
                string Update = "";

                string filePath = @"C:\data\update_holiday.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                sql = $@"select * from tbl_Holiday_Model where HolidayID ='" + data.HolidayID + "'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Update = $@"UPDATE [dbo].[tbl_Holiday_Model]
                               SET 
                                   [HolidayName] = '" + data.HolidayName + "', " +
                                  "[Date] ='" + Convert.ToDateTime(data.Date).ToString("yyyy-MM-dd") + "', " +
                                  "[Location] ='" + data.Location + "', " +
                                  "[RepeatYearly] = '" + data.RepeatYearly + "', " +
                                  "[DateUpdated] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                             "WHERE  HolidayID ='" + data.HolidayID + "'";


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
        public async Task<IActionResult> DeleteHoliday(HolidayIDModel data)
        {
            string result = "";
            try
            {
                string Delete = "";
                string filePath = @"C:\data\delete_holiday.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"select * from tbl_Holiday_Model where HolidayID ='" + data.HolidayID + "'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Delete = $@"UPDATE [dbo].[tbl_Holiday_Model] Set Status = 2 Where HolidayID ='" + data.HolidayID + "' ";


                    results = db.AUIDB_WithParam(Delete) + " Deleted";
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
        public async Task<IActionResult> RestoreHoliday(HolidayIDModel data)
        {
            string result = "";
            try
            {
                string Delete = "";
                string filePath = @"C:\data\restore_holiday.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"select * from tbl_Holiday_Model where HolidayID ='" + data.HolidayID + "'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Delete = $@"UPDATE [dbo].[tbl_Holiday_Model] Set Status = 1 Where HolidayID ='" + data.HolidayID + "' ";


                    results = db.AUIDB_WithParam(Delete) + " Restored";
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

    }
}