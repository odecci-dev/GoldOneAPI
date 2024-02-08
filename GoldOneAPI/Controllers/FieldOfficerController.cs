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
using GoldOneAPI.Manager;
using System.Text.Json;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FieldOfficerController : ControllerBase
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

        public FieldOfficerController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;

        }
        #region MOdel
        public class FoVM
        {
            public string? Id { get; set; }
            public string? DateUpdated { get; set; }
            public string? DateCreated { get; set; }
          
            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? Country { get; set; }
            public string? Region { get; set; }
            public string? City { get; set; }
            public string? Barangay { get; set; }
            public string? HouseNo { get; set; }
            public string? EmailAddress { get; set; }
            public string? Cno { get; set; }
            public string? CivilStatus { get; set; }
            public string? POB { get; set; }
            public string? Age { get; set; }
            public string? DOB { get; set; }
            public string? Gender { get; set; }
            public string? Fullname { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? FOID { get; set; }
            public string? ProfilePath { get; set; }
            public string? FrontID_Path { get; set; }
            public string? BackID_Path { get; set; }
            public List<FileModel>? Files { get; set; }
            //requirements
            public string? SSS { get; set; }
            public string? PagIbig { get; set; }
            public string? PhilHealth { get; set; }
            public string? IdNum { get; set; }
            public string? TypeID { get; set; }
            public string? IDType_Name{ get; set; }
            public string? arealists { get; set; }
        }
        public class arealist
        {
            public string? AreaName { get; set; }
            public string? AreaID { get; set; }
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> IDTypeList()
        {

            try
            {
                var result =  dbmet.GetIDTypeList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
       [HttpGet]
        public async Task<IActionResult> FieldOfficerList()
        {

            try
            {
                var result =  dbmet.GetFieldOfficer().Where(a=>a.StatusId == "1").ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> FieldOfficerFilterPaginate(string? fullname, int page, int pageSize)
        {
            var result = (dynamic)null;
            try
            {



                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (fullname == null)
                {
                    var list = dbmet.GetFieldOfficer().Where(a => a.StatusId == "1").ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = dbmet.GetFieldOfficer().Where(a => a.StatusId == "1" && a.Fullname.ToUpper().Contains(fullname.ToUpper())).ToList();


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



                result = items == null ? "Null data" : items;
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> FieldOfficer_Trash_FilterPaginate(string? fullname, int page, int pageSize)
        {
            var result = (dynamic)null;
            try
            {



                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (fullname == null)
                {
                    var list = dbmet.GetFieldOfficer().Where(a => a.StatusId == "2").ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = dbmet.GetFieldOfficer().Where(a => a.StatusId == "2" && a.Fullname.ToUpper().Contains(fullname.ToUpper())).ToList();


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



                result = items == null ? "Null data" : items;
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public IActionResult FieldOfficerFilterbyFOID(FitlerFOID data)
        {
            try
            {
                var result = dbmet.GetFieldOfficer().Where(a=>a.FOID == data.FOID).ToList();
                if (result.Count != 0)
                {
                    return Ok(result);
                }
                else
                {
                    var result1 = dbmet.GetFieldOfficer().ToList();
                    return Ok(result1);
                }

            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public IActionResult FieldOfficerFilterbyFullname(FilterFullname data)
        {
            try
            {
                var result = dbmet.GetFieldOfficer().Where(a=>a.Fullname.ToUpper().Contains(data.Fullname.ToUpper())).ToList();
                if (result.Count != 0)
                {
                    return Ok(result);
                }
                else
                {
                    if (data.Fullname == "")
                    {
                        var result1 = dbmet.GetFieldOfficer().ToList();
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
      
        [HttpGet]
        public async Task<IActionResult> GetLastOfficerList()
        {

            try
            {
                var result = dbmet.GetFieldOfficer().ToList().LastOrDefault();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveFieldOfficer(OfficerModel data )
        {
            string result = "";
            try
            {
                string Insert = "";
                string filePath = @"C:\data\savefield_officer.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"select * from tbl_FieldOfficer_Model where Fname ='" + data.Fname + "' and Mname= '" + data.Mname + "'  and Lname= '" + data.Lname + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count == 0)
                {
                   string Insert_FO = $@"INSERT INTO [dbo].[tbl_FieldOfficer_Model]
                               ([Fname]
                               ,[Mname]
                               ,[Lname]
                               ,[Suffix]
                               ,[Gender]
                               ,[DOB]
                               ,[Age]
                               ,[POB]
                               ,[CivilStatus]
                               ,[Cno]
                               ,[EmailAddress]
                               ,[HouseNo]
                               ,[Barangay]
                               ,[City]
                               ,[Region]
                               ,[Country]
                               ,[Status]

                               ,[ProfilePath]
                               ,[FrontID_Path]
                               ,[BackID_Path]
                               ,[ID_Number]
                               ,[SSS]
                               ,[PagIbig]
                               ,[PhilHealth]
                               ,[IDType]

                               ,[DateCreated])
                         VALUES
                               ('" + data.Fname + "'," +
                                "'"+data.Mname+"'," +
                               "'"+data.Lname+"'," +
                              "'"+data.Suffix+"'," +
                               "'"+data.Gender+"'," +
                               "'"+ Convert.ToDateTime(data.DOB).ToString("yyyy-MM-dd") + "'," +
                               "'"+data.Age+"'," +
                               "'"+data.POB+"'," +
                              "'"+data.CivilStatus+"'," +
                               "'"+data.Cno+"'," +
                                "'"+data.EmailAddress+"'," +
                              "'"+data.HouseNo+"'," +
                              "'"+data.Barangay+"'," +
                                "'"+data.City+"'," +
                              "'"+data.Region+"'," +
                             "'"+data.Country+"'," +
                                "'1'," +
                                     "'" + data.ProfilePath + "'," +
                                          "'" + data.FrontID_Path + "'," +
                                               "'" + data.BackID_Path + "'," +
                                                    "'" + data.IdNum + "'," +
                                                         "'" + data.SSS + "'," +
                                                              "'" + data.PagIbig + "'," +
                                                                   "'" + data.PhilHealth + "'," +
                                                                        "'" + data.TypeID + "'," +
                                "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                   db.AUIDB_WithParam(Insert_FO) ;


                     sql = $@"select Top(1) FOID from tbl_FieldOfficer_Model order by id desc";

                    DataTable table = db.SelectDb(sql).Tables[0];
                    var Foid = table.Rows[0]["FOID"].ToString();
                    string fo_uploadfile = "";
                    if (data.uploadFiles.Count != 0)
                    {
                        for (int x = 0; x < data.uploadFiles.Count; x++)
                        {
                            fo_uploadfile = $@"INSERT INTO [dbo].[tbl_FOFile_Model]
                                           ([FOID]
                                           ,[FilePath]
                                           ,[FileType]
                                           ,[DateCreated])
                                             VALUES
                                           ('" + Foid + "'," +
                                           "'" + data.uploadFiles[x].FilePath + "'," +
                                          "'2'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            result = db.AUIDB_WithParam(fo_uploadfile) + " Added";
                        }
                   
                    }

                    return Ok(result);
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
        public class UpdateOfficerModel
        {
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Gender { get; set; }
            public DateTime? DOB { get; set; }
            public int? Age { get; set; }
            public string? POB { get; set; }
            public string? CivilStatus { get; set; }
            public string? Cno { get; set; }
            public string? EmailAddress { get; set; }
            public string? HouseNo { get; set; }
            public string? Barangay { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? Country { get; set; }
            public string? FOID { get; set; }

            //requirements
            public string? SSS { get; set; }
            public string? PagIbig { get; set; }
            public string? PhilHealth { get; set; }
            public string? IdNum { get; set; }
            public string? TypeID { get; set; }
            public string? ProfilePath { get; set; }


            public string? FrontID_Path { get; set; }
            public string? BackID_Path { get; set; }


            public List<FO_UploadFile> uploadFiles { get; set; }


        }
        [HttpPost]
        public async Task<IActionResult> UpdateFieldOfficer(UpdateOfficerModel data)
        {
            string result = "";
            try
            {
                string Update = "";
                string filePath = @"C:\data\update_field_officer.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"SELECT        tbl_FieldOfficer_Model.Id, tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix, tbl_FieldOfficer_Model.Gender, tbl_FieldOfficer_Model.DOB, 
                         tbl_FieldOfficer_Model.Age, tbl_FieldOfficer_Model.POB, tbl_FieldOfficer_Model.CivilStatus, tbl_FieldOfficer_Model.Cno, tbl_FieldOfficer_Model.EmailAddress, tbl_FieldOfficer_Model.HouseNo, tbl_FieldOfficer_Model.Barangay, 
                         tbl_FieldOfficer_Model.City, tbl_FieldOfficer_Model.Region, tbl_FieldOfficer_Model.Country, tbl_FieldOfficer_Model.DateCreated, tbl_FieldOfficer_Model.DateUpdated, tbl_FieldOfficer_Model.FOID, 
                         tbl_FieldOfficer_Model.ProfilePath, tbl_FieldOfficer_Model.FrontID_Path, tbl_FieldOfficer_Model.BackID_Path, tbl_FieldOfficer_Model.ID_Number, tbl_FieldOfficer_Model.SSS, tbl_FieldOfficer_Model.TIN, 
                         tbl_FieldOfficer_Model.PagIbig, tbl_FieldOfficer_Model.PhilHealth, tbl_IDType_Model.Type AS IDType_Name, tbl_Status_Model.Name AS Status, tbl_IDType_Model.TypeID
FROM            tbl_FieldOfficer_Model left JOIN
                         tbl_IDType_Model ON tbl_FieldOfficer_Model.IDType = tbl_IDType_Model.TypeID left JOIN
                         tbl_Status_Model ON tbl_FieldOfficer_Model.Status = tbl_Status_Model.Id
WHERE        (tbl_FieldOfficer_Model.Status = 1) and     tbl_FieldOfficer_Model.FOID = '" +data.FOID+"'";
                DataTable table1 = db.SelectDb(sql).Tables[0];

                if (table1.Rows.Count != 0)
                {
                    Update = $@"UPDATE [dbo].[tbl_FieldOfficer_Model]
                           SET [Fname] = '" + data.Fname + "', " +
                              "[Mname] = '" + data.Mname + "', " +
                              "[Lname] = '" + data.Lname + "', " +
                              "[Suffix] ='" + data.Suffix + "', " +
                              "[Gender] ='" + data.Gender + "', " +
                              "DOB='" + Convert.ToDateTime(data.DOB).ToString("yyyy-MM-dd") + "', " +
                              "[Age] = '" + data.Age + "', " +
                              "[POB] = '" + data.POB + "', " +
                              "[CivilStatus] = '" + data.CivilStatus + "', " +
                              "[Cno] ='" + data.Cno + "', " +
                              "[EmailAddress] = '" + data.EmailAddress + "', " +
                              "[HouseNo] = '" + data.HouseNo + "', " +
                              "[Barangay] = '" + data.Barangay + "', " +
                              "[City] = '" + data.City + "', " +
                              "[Region] = '" + data.Region + "', " +
                              "[Country] = '" + data.Country + "', " +
                               "[ProfilePath]= '" + data.ProfilePath + "', " +
                              "[FrontID_Path] =  '" + data.FrontID_Path + "', " +
                              "[BackID_Path] = '" + data.BackID_Path + "', " +
                              "[ID_Number] = '" + data.IdNum + "', " +
                              "[SSS] =  '" + data.SSS + "', " +
                              "[PagIbig] =  '" + data.PagIbig + "', " +
                              "[PhilHealth] =  '" + data.PhilHealth + "', " +
                              "[IDType] = '" + data.TypeID + "', " +
                               "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                       "WHERE        tbl_FieldOfficer_Model.FOID = '" + data.FOID + "'";
                    result = db.AUIDB_WithParam(Update) + " Updated";




                    ////



                  
                    string fo_uploadfile = "";
                    if (data.uploadFiles.Count != 0)
                    {
                        string delete = $@"DELETE FROM [dbo].[tbl_FOFile_Model] WHERE   FOID = '" + data.FOID + "'";
                        db.AUIDB_WithParam(delete);
                        for (int x = 0; x < data.uploadFiles.Count; x++)
                        {
                            fo_uploadfile = $@"INSERT INTO [dbo].[tbl_FOFile_Model]
                                           ([FOID]
                                           ,[FilePath]
                                           ,[FileType]
                                           ,[DateCreated])
                                             VALUES
                                           ('" + data.FOID + "'," +
                                           "'" + data.uploadFiles[x].FilePath + "'," +
                                          "'2'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            result = db.AUIDB_WithParam(fo_uploadfile) + " Added";
                        }

                    }
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Invalid Update");
                }


            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }

        }
        [HttpPost]
        public IActionResult RestoreFO(FOIDModel data)
        {
            try
            {
                string filePath = @"C:\data\restore_FO.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));

                sql = $@"select * from tbl_FieldOfficer_Model where FOID ='" + data.FOID + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {
                    
                        string OTPInsert = $@"update tbl_FieldOfficer_Model set Status='1' " +
                                        "where FOID='" + data.FOID + "'";
                        db.AUIDB_WithParam(OTPInsert);
                        results = "Successfully Restored";
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
        public IActionResult DeleteFO(FOIDModel data)
        {
            try
            {
                string filePath = @"C:\data\deleteFO.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                sql = $@"select * from tbl_FieldOfficer_Model where FOID ='" + data.FOID + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {
                    string sql2 = $@"SELECT  *
                              FROM tbl_Area_Model where FOID ='" + data.FOID + "'";
                    DataTable dt2 = db.SelectDb(sql2).Tables[0];
                    if (dt2.Rows.Count == 0)
                    {
                        string OTPInsert = $@"update tbl_FieldOfficer_Model set Status='2' " +
                                        "where FOID='" + data.FOID + "'";
                        db.AUIDB_WithParam(OTPInsert);
                        results = "Successfully Deleted";
                        return Ok(results);
                    }
                    else
                    {
                        return Ok("Field Officer is already Assigned and cannot be Deleted!");
                    }


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