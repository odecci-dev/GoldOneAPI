using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Text;
using static GoldOneAPI.Controllers.MemberController;
using static GoldOneAPI.Controllers.HolidayController;
using static GoldOneAPI.Controllers.GroupController;
using static GoldOneAPI.Controllers.LoanTypeController;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using GoldOneAPI.Manager;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FieldAreaController : ControllerBase
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

        public FieldAreaController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;
            //this.jwtAuthenticationManager = jwtAuthenticationManager;

        }
        #region model

        public class LocVM
        {
            public string? Location { get; set; }
        }  
        public class AreaFilterVM
        {
            public string? AreaName { get; set; }
        }
        public class AreaVM
        {
            public string? AreaName { get; set; }

            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? DateUpdated { get; set; }
            public string? DateCreated { get; set; }
            public string? Location { get; set; }
            public string? FOID { get; set; }
            public string? Fullname { get; set; }
            public string? AreaID { get; set; }

        }
        public class AreaVM2
        {
            public string? AreaName { get; set; }

            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? DateUpdated { get; set; }
            public string? DateCreated { get; set; }
            public List<LocVM>? Location { get; set; }
            public string? FOID { get; set; }
            public string? Fullname { get; set; }
            public string? AreaID { get; set; }

        }
        public class AreaFO
        {
            public string? Location { get; set; }
            public string? FOID { get; set; }
            public string? Fullname { get; set; }
        }
            public class SaveArea
        {
            public string? AreaID { get; set; }
            public string? AreaName { get; set; }
            public List<String>? Location { get; set; }
            public string? FOID { get; set; }

        }
        public class AssignArea
        {

            public string? AreaName { get; set; }
            public List<String>? Location { get; set; }
            public string? FOID { get; set; }

        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> AreasList()
        {


            var result = (dynamic)null;
            try
            {
                result = dbmet.GetFieldAreas().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAreaDetails( string AreaID)
        {


            var result = (dynamic)null;
            try
            {
                result = dbmet.GetAreaViewing(AreaID).ToList();
                if (result.Count != 0)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("No Date Exist!");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> AreasListPaginate(int page, int pageSize, string? Areaname)
        {


            try
            {
                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (Areaname == null || Areaname == "")
                {
                    var list = dbmet.GetFieldAreas().ToList();
                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    var list = dbmet.GetFieldAreas().Where(a => a.AreaName.ToUpper().Contains(Areaname.ToUpper())).ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
        public async Task<IActionResult> UnAssignedLocationList()
        {

            var result = (dynamic)null;
            try
            {
                result = dbmet.GetUnAssignedLocationList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UnAssignedLocationListPaginate(int page, int pageSize,string? Areaname)
        {

            try
            {
                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (Areaname == null)
                {
                    var list = dbmet.GetUnAssignedLocationList().ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    var list = dbmet.GetUnAssignedLocationList().Where(a => a.AreaName.ToUpper().Contains(Areaname.ToUpper())).ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
        public async Task<IActionResult> AreaFilter(AreaFilterVM data)
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetFieldAreas().Where(a=>a.AreaName.ToUpper().Contains(data.AreaName.ToUpper())).ToList();
                if(result.Count != 0)
                {
                    return Ok(result);
                }
                 else
                {
                    if (data.AreaName == "")
                    {
                        var results = dbmet.GetFieldAreas().ToList();
                        return Ok(results);
                    }
                    else
                    {
                        return BadRequest("No Data");
                    }
                 
                }

            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }

        }

        [HttpPost]
        public async Task<IActionResult> UnAssignedFilter(LocVM data)
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetUnAssignedLocationList().Where(a=>a.Location.ToUpper().Contains(data.Location.ToUpper())).ToList();
                if(result.Count != 0)
                {
                    return Ok(result);
                }
                else
                {
                    if (data.Location == "")
                    {
                        var result1 = dbmet.GetUnAssignedLocationList().ToList();
                        return Ok(result1);
                    }
                    else
                    {
                        return BadRequest("0 Matches");
                    }
                }
            
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        public class citymodel
        {
            public string? Location { get; set; }

        }
        public class citymodel_2
        {
            public string? Location { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateArea(SaveArea data)
        {
            string result = "";
            try
            {

                string filePath = @"C:\data\update_area.json"; // Replace with your desired file path


                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                string update = "";

                    string validate = $@"select * from tbl_Area_Model where AreaID= '" + data.AreaID + "' ";
                    DataTable table1 = db.SelectDb(validate).Tables[0];
                    if (table1.Rows.Count != 0)
                    {
                        //string Delete = $@"DELETE FROM [dbo].[tbl_Area_Model] WHERE  AreaID= '" + data.AreaID + "' ";
                        //db.AUIDB_WithParam(Delete);

                        string city = "";
                 
                    var city_res = new List<String>();
                    var city_res_2 = new List<String>();
                    for (int x = 0; x < data.Location.Count; x++)
                    {
                        city += data.Location[x] + "|";
                        string Location = data.Location[x];
                        city_res_2.Add(Location);
                        string select_sql = $@"select * from tbl_Area_Model where City = '" + data.Location[x] + "' ";
                        DataTable city_exist = db.SelectDb(select_sql).Tables[0];
                        if (city_exist.Rows.Count != 0)
                        {
                            string Delete = $@"DELETE FROM [dbo].[tbl_Area_Model] WHERE  City= '" + data.Location[x] + "' and FOID IS NULL ";
                            db.AUIDB_WithParam(Delete);

                        }


                    }
                    sql = $@"select * from tbl_Area_Model where AreaID = '" + data.AreaID + "' ";
                        DataTable area_exist = db.SelectDb(sql).Tables[0];

                        // Split the string based on the comma (',') delimiter
                        string[] citiesArrays = area_exist.Rows[0]["City"].ToString().Split('|');
                        string string_re = "";
                        //string cities = "";
                        foreach (string cities in citiesArrays)
                            {
                            
                                string locations = cities;
                                city_res.Add(locations);
                            }
                    string remove_item = "";
                    IEnumerable<string> notEqualValues = city_res.Except(city_res).Concat(city_res.Except(city_res_2));

                    foreach (string value in notEqualValues)
                    {
                        remove_item = value;

                   
                        //if (city_exist.Rows.Count != 0)
                        //{
                        //    string Delete = $@"DELETE FROM [dbo].[tbl_Area_Model] WHERE  City= '" + remove_item + "' ";
                        //    db.AUIDB_WithParam(Delete);

                        //}
                        //else
                        //{
                            string Insert = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                    ([City]
                                    , [Status]
                                    ,[DateCreated])
                             VALUES
                                ('" + remove_item + "'," +
                              "'1'," +
                                    "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            result = db.AUIDB_WithParam(Insert) + " Added";

                        //}
                      

                    }
                    string[] city_ = remove_item.Split(',');
                  

                    if (remove_item != "")
                    {
                        
                        string sql = $@"select * 
                                    from
                                    tbl_Application_Model inner join
                                    tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID inner join
                                    tbl_Member_Model on tbl_Member_Model.MemId = tbl_Application_Model.MemId where tbl_Member_Model.City like '%"+city_+"%'";
                        DataTable area_exist_mem = db.SelectDb(sql).Tables[0];
                        if (area_exist_mem.Rows.Count == 0)
                        {
                        }
                        else
                        {

                        }
                        
                    }
                        string Update = $@"
                                     UPDATE [dbo].[tbl_Area_Model]
                                     SET [Area] = '" + data.AreaName + "' ," +
                                        "[City] = '" + city.Substring(0, city.Length - 1) + "' ," +
                                        "[FOID] = '" + data.FOID + "' ," +
                                        "[Status] = '1' ," +
                                        "[DateUpdated] = '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "' " +
                                   "WHERE  AreaID= '" + data.AreaID + "' ";
                        result = db.AUIDB_WithParam(Update) + " Updated";
                    
                    //string Insert = $@"INSERT INTO [dbo].[tbl_Area_Model]
                    //                ([Area]
                    //                ,[City]
                    //                ,[FOID]
                    //                ,[Status]
                    //                ,[DateCreated])
                    //          VALUES
                    //               ('" + data.AreaName + "'," +
                    //            "'" + city.Substring(0, city.Length - 1) + "'," +
                    //              "'" + data.FOID + "'," +
                    //               "'1'," +
                    //                  "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                    // result = db.AUIDB_WithParam(Insert) + " Added";

                }


            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
            return Ok(result);
        }
        public class AreaIDVM
        {
            public string? AreaID { get; set; }
            public string? AreaRefno { get; set; }
            public string? Remarks { get; set; }
            public string? FOID { get; set; }
        }
        public class AreaCollectionUpdate
        {
            public string? AreaID { get; set; }
            public string? Denomination { get; set; }
            public string? AreaRefno { get; set; }
        }
        public class Remit
        {
            public string? MemId { get; set; }
            public double? Savings { get; set; }
            public string? ModeOfPayment { get; set; }
            public string? AreaRefno { get; set; }
            public string? AreaID { get; set; }
            public double? AmountCollected { get; set; }
            public double? AdvancePayment { get; set; }
            public double? Lapses { get; set; }
            public string? UserId { get; set; }
            public string? FOID { get; set; }
            //public List<Fieldexpenses> FieldExpenses{ get; set;}
       
        }

        public class RemitCalcu
        {
            public string? MemId { get; set; }
            public string? AreaRefno { get; set; }
            public double? AmountCollected { get; set; }

        }
        public class Fieldexpenses
        {
            public string? ExpensesDescription { get; set; }
            public string? FieldExpenses { get; set; }
            public string? AreaRefno { get; set; }
            public string? AreaId { get; set; }
        }
        public class RemitAmount
        {
            public string? MemId { get; set; }
            public double? AmountCollected { get; set; }
        }
        public class RemitAmountResult
        {
            public string? lapses { get; set; }
            public string? advance { get; set; }
        }
        public class RemitCalculation
        {
            public string? lapses { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAreas(AreaIDVM data)
        {
    
            try
            {
                string filePath = @"C:\data\deleteareas.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                sql = $@"select * from tbl_Area_Model where AreaID = '" +data.AreaID + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    //Delete = $@"UPDATE tbl_Area_Model Set FOID = NULL , Area= NULL ,City = NULL  where AreaID = '" + data.AreaID + "' ";

                
                        string citiesString =table1.Rows[0]["City"].ToString();

                        // Split the string based on the comma (',') delimiter
                        string[] citiesArray = citiesString.Split('|');
                        string string_re = "";
                        // Print each city name
                        var location = new List<LocVM>();
                        foreach (string city in citiesArray)
                        {
                            string Insert = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                       ([City]
                                       ,[Status]
                                       ,[DateCreated])
                                 VALUES
                                      ('" + city + "'," +
                                      "'1'," +
                                         "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            results = db.AUIDB_WithParam(Insert) + " Deleted";
                        }

                    string delete_ = $@"DELETE FROM  [dbo].[tbl_Area_Model]  where AreaID = '" + data.AreaID + "' ";

                    db.AUIDB_WithParam(delete_);

                   

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
        public async Task<IActionResult> AssigningFieldArea(AssignArea data)
        {
            string result = "";
            try
            {
                string update = "";
                string filePath = @"C:\data\assigned_field_area.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                string validate_areaname = $@"select * from tbl_Area_Model where Area= '" + data.AreaName + "' ";
                DataTable table2 = db.SelectDb(validate_areaname).Tables[0];
                if (table2.Rows.Count != 0)
                {
                    return Ok("AreaName is already Exist!");
                }
                else
                {

                    string validate_fo = $@"select * from tbl_Area_Model where FOID= '" + data.FOID + "' ";
                    DataTable fo_table = db.SelectDb(validate_fo).Tables[0];
                    if (fo_table.Rows.Count != 0)
                    {
                    
                        return Ok( "ERROR: "+ "Field Officer is already Assigned!");
                    }
                    else
                    {
                        string city = "";
                        for (int x = 0; x < data.Location.Count; x++)
                        {

                            string Delete = $@"DELETE FROM [dbo].[tbl_Area_Model] WHERE  City= '" + data.Location[x] + "' AND FOID IS NULL";
                            db.AUIDB_WithParam(Delete);


                            string validate = $@"select * from tbl_Area_Model where City like '%" + data.Location[x] + "%' AND FOID IS NOT NULL ";
                            DataTable table1 = db.SelectDb(validate).Tables[0];
                            if (table1.Rows.Count == 0)
                            {
                                city += data.Location[x] + "|";
                            }


                        }

                        string Insert = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                       ([Area]
                                       ,[City]
                                       ,[FOID]
                                       ,[Status]
                                       ,[DateCreated])
                                 VALUES
                                      ('" + data.AreaName + "'," +
                                   "'" + city.Substring(0, city.Length - 1) + "'," +
                                     "'" + data.FOID + "'," +
                                      "'1'," +
                                         "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                        result = db.AUIDB_WithParam(Insert) + " Added";


                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
            return Ok(result);
        }
    }
}