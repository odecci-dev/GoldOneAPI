using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Text;
using GoldOneAPI.Manager;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";

        DbManager db = new DbManager();

        DBMethods dbmet = new DBMethods();
        private readonly AppSettings _appSettings;
        //private ApplicationDbContext _context;
        private readonly JwtAuthenticationManager jwtAuthenticationManager;
        private readonly IWebHostEnvironment _environment;

        public UserRegistrationController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;
            //this.jwtAuthenticationManager = jwtAuthenticationManager;

        }
        public class UserModel
        {
            public int Id { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Cno { get; set; }
            public string? Address { get; set; }
            public string? ProfilePath { get; set; }
            public string? FOID { get; set; }
            public int? UserTypeID { get; set; }
            public int? Status { get; set; }
            public List<UserModule> usermodule {get; set; }
        }

        public class UserModel2
        {
            public int Id { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Cno { get; set; }
            public string? Address { get; set; }
            public int? Status { get; set; }
            public string? ProfilePath { get; set; }
        }
        public class Changepassword
        {
            public string? UserId { get; set; }
            public string? Password { get; set; }

        }
        public class UserModule
        {
            public int Id { get; set; }
            public string? user_id { get; set; }
            public string? module_code { get; set; }
            public string? created_by { get; set; }
            public string? module_category { get; set; }

        }
        public class ModuleVM
        {
            public int Id { get; set; }
            public string? module_code { get; set; }
            public string? module_name { get; set; }
            public string? created_by { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }
            public string? module_category { get; set; }

        }
        public class Module
        {
            public int Id { get; set; }
            public string? module_name { get; set; }
            public string? created_by { get; set; }
            public string? module_category { get; set; }
        }
        public class UserModuleVM
        {
            public int Id { get; set; }
            public string? module_name { get; set; }
            public string? module_code { get; set; }
            public string? Fullname { get; set; }
            public string? CreatedBy { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }
            public string? UserId { get; set; }
            public string? module_category { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetUserModuleByUserID(FilerUserID data)
        {


            try
            {
                var result = dbmet.GetUserModuleList().Where(a => a.UserId == data.UserID).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> FilterUserInfoByUID(FilerUserID data)
        {


            try
            {

                var result = dbmet.GetUserList().Where(a => a.UserId == data.UserID).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUserModuleList()
        {
            try
            {
                var result = dbmet.GetUserModuleList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }

        }
        [HttpGet]
        public async Task<IActionResult> UserList()
        {

            try
            {
                var result = dbmet.GetUserList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserListFilter(int page, int pageSize, string? fullname)
        {
            var result = (dynamic)null;
            try
            {



                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (fullname == null)
                {
                    var list = dbmet.GetUserList().ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = dbmet.GetUserList().Where(a => a.Fullname.ToUpper().Contains(fullname.ToUpper())).ToList();
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
        public async Task<IActionResult> GetLastUserList()
        {

            try
            {
                var result = dbmet.GetUserList().OrderByDescending(a => a.Id).FirstOrDefault();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetModuleList()
        {

            try
            {
                var result = dbmet.GetModuleList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostUserSearching(List<FilterModel> data)
        {


            try
            {
                string w_column = "";
                for (int x = 0; x < data.Count; x++)
                {
                    if (data.Count == 1)
                    {
                        w_column += " and " + data[x].Column + " like '%" + data[x].Values + "%' ";
                    }
                    else
                    {
                        w_column += " and " + data[x].Column + " like '%" + data[x].Values + "%'";
                    }
                }
                sql = $@"SELECT        tbl_User_Model.Id, tbl_User_Model.Username, tbl_User_Model.Password, tbl_User_Model.Fname, tbl_User_Model.Mname, tbl_User_Model.Lname, tbl_User_Model.Cno, tbl_User_Model.Address, 
                         tbl_Status_Model.Name AS Status, tbl_User_Model.DateCreated, tbl_User_Model.DateUpdated, tbl_User_Model.UTID, tbl_User_Model.UserId, tbl_UserType_Model.UserType,
                         tbl_User_Model.ProfilePath,
                         tbl_FieldOfficer_Model.FOID,
                         tbl_FieldOfficer_Model.Fname as FO_Fname,
                         tbl_FieldOfficer_Model.Mname as FO_Mname,
                         tbl_FieldOfficer_Model.Lname as FO_Lname

FROM            tbl_User_Model INNER JOIN
                         tbl_Status_Model ON tbl_User_Model.Status = tbl_Status_Model.Id INNER JOIN
                         tbl_UserType_Model ON tbl_User_Model.UTID = tbl_UserType_Model.Id left JOIN
                         tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_User_Model.FOID where tbl_User_Model.Status =1  " + w_column + " ";
                var result = new List<UserVM>();
                DataTable table = db.SelectDb(sql).Tables[0];

                foreach (DataRow dr in table.Rows)
                {
                    var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var item = new UserVM();
                    item.Id = int.Parse(dr["id"].ToString());
                    item.Username = dr["Username"].ToString();
                    item.Fname = dr["Fname"].ToString();
                    item.Lname = dr["Lname"].ToString();
                    item.Mname = dr["Mname"].ToString();
                    item.FOID = dr["FOID"].ToString();
                    //item.Suffix = dr["Suffix"].ToString();
                    item.Cno = dr["Cno"].ToString();
                    item.Address = dr["Address"].ToString();
                    item.StatusName = dr["Status"].ToString();
                    item.DateCreated = datec;
                    item.DateUpdated = dateu;
                    item.UserId = dr["UserId"].ToString();
                    item.UserTypeId = dr["UTID"].ToString();
                    item.UserType = dr["UserType"].ToString();
                    item.ProfilePath = dr["ProfilePath"].ToString();
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
        public IActionResult ChangePassword(Changepassword data)
        {
            string result = "";
            try
            {
                var usercount = dbmet.GetUserList().Where(a => a.UserId == data.UserId).ToList();
                if(usercount.Count != 0)
                {
                    string update = $@"update tbl_User_Model set 
                                    Password='" +Cryptography.Encrypt(data.Password)+ "', " +
                              "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                              "where UserId='" + data.UserId + "'";
                    result = db.AUIDB_WithParam(update) + " Updated";
                }
                else
                {
                    result = "No data found";
                }
          

                return Ok(result);
            }
            catch (Exception ex)
            {
                results = ex.GetBaseException().ToString();
                return BadRequest(result);
            }
        }

        [HttpPost]
        public IActionResult UpdateUserInfo(UserModel data)
        {
            try
            {
                string update = "";
                sql = $@"select * from tbl_User_Model where id ='" + data.Id + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new UserModel();
                if (dt.Rows.Count != 0)
                {

                     update += $@"update tbl_User_Model set 
                                    Fname='" + data.Fname + "', " +
                                    "Lname='" + data.Lname + "', " +
                                    "Mname='" + data.Mname + "', " +
                                    "Username='" + data.Username + "', " +
                                    "Address='" + data.Address + "', " +
                                    "Cno='" + data.Cno + "', " +
                                    "Status='" + data.Status + "', " +
                                    "ProfilePath='" + data.ProfilePath + "', " +
                                    "FOID='" + data.FOID + "', " +
                                    "UTID='" + data.UserTypeID + "', " +
                                    "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                    "where Id='" + data.Id + "'";
                    results = db.AUIDB_WithParam(update) + " Updated";
             
                    if (data.usermodule.Count != 0)
                    {
                        string delete = $@"DELETE FROM[dbo].[users_module] WHERE user_id='" + dt.Rows[0]["UserId"].ToString() + "'";
                        db.AUIDB_WithParam(delete);
                        for (int x = 0; x < data.usermodule.Count; x++)
                        {

                            string Insert_module = $@"insert into   [dbo].[users_module]
                                   ([user_id]
                                   ,[module_code]
                                   ,[created_by]
                                   ,[DateCreated])
                                    values
                                    ('" + dt.Rows[0]["UserId"].ToString() + "', " +
                                               "'" + data.usermodule[x].module_code + "'," +
                                               "'" + data.usermodule[x].created_by + "', " +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                            results = db.AUIDB_WithParam(Insert_module) + " Added";
                        }
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
        public IActionResult UpdateUserInformation(UserModel2 data)
        {
            try
            {
                string update = "";
                sql = $@"select * from tbl_User_Model where id ='" + data.Id + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new UserModel();
                if (dt.Rows.Count != 0)
                {

                    update += $@"update tbl_User_Model set 
                                    Fname='" + data.Fname + "', " +
                                   "Lname='" + data.Lname + "', " +
                                   "Mname='" + data.Mname + "', " +
                                   "Username='" + data.Username + "', " +
                                   "Address='" + data.Address + "', " +
                                   "Cno='" + data.Cno + "', " +
                                   "Status='" + data.Status + "', " +

                                    "ProfilePath='" + data.ProfilePath + "', " +
                                   "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                   "where Id='" + data.Id + "'";
                    results = db.AUIDB_WithParam(update) + " Updated";

                    

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
        public IActionResult SaveUser(UserModel data)
        {
            try
            {
                string sql1 = $@"select * from tbl_User_Model where Username ='" + data.Username + "'";
                DataTable dt1 = db.SelectDb(sql1).Tables[0];
                if (dt1.Rows.Count == 0)
                {


                    string Insert = $@"insert into   tbl_User_Model (Fname, Lname, Mname,Username,Password,Cno,Address,UTID,ProfilePath,FOID,DateCreated,Status) 
                                values
                                ('" + data.Fname + "', " +
                                "'" + data.Lname + "'," +
                                "'" + data.Mname + "', " +
                                "'" + data.Username + "', " +
                                "'" + Cryptography.Encrypt(data.Password) + "', " +
                                "'" + data.Cno + "', " +
                                "'" + data.Address + "', " +
                                "'" + data.UserTypeID + "', " +
                                "'" + data.ProfilePath + "', " +
                                "'" + data.FOID + "', " +
                                "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                "'" + data.Status + "') ";
                    results = db.AUIDB_WithParam(Insert) + " Added";

                    var userid = dbmet.GetUserList().LastOrDefault();

                    if(userid != null )
                    {
                        for (int x = 0; x < data.usermodule.Count; x++)
                        {
                            string Insert_module = $@"insert into   [dbo].[users_module]
                                   ([user_id]
                                   ,[module_code]
                                   ,[created_by]
                                   ,[DateCreated])
                                    values
                                    ('" + userid.UserId + "', " +
                                            "'" + data.usermodule[x].module_code + "'," +
                                            "'" + data.usermodule[x].created_by + "', " +
                                            "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                            results = db.AUIDB_WithParam(Insert_module) + " Added";

                        }
                    }
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
        //[HttpPost]
        //public IActionResult SaveUserModule(List<UserModule> data)
        //{
        //    try
        //    {
        //        string sql1 = $@"select * from users_module where user_id ='" + data[0].user_id + "'";
        //        DataTable dt1 = db.SelectDb(sql1).Tables[0];
        //        if (dt1.Rows.Count == 0)
        //        {

        //            for (int x = 0; x < data.Count; x++)
        //            {
        //                string Insert = $@"insert into   [dbo].[users_module]
        //                           ([user_id]
        //                           ,[module_code]
        //                           ,[created_by]
        //                           ,[DateCreated])
        //                            values
        //                            ('" + data[x].user_id + "', " +
        //                                "'" + data[x].module_code + "'," +
        //                                "'" + data[x].created_by + "', " +
        //                                "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

        //                results = db.AUIDB_WithParam(Insert) + " Added";

        //            }
        //            return Ok(results);
        //        }
        //        else
        //        {
        //            results = "Duplicate Entry";
        //            return Ok(results);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        results = ex.GetBaseException().ToString();
        //        return BadRequest(results);
        //    }

        //}
        [HttpPost]
        public IActionResult SaveModule(Module data)
        {
            try
            {
                string sql1 = $@"select * from modules where module_name ='" + data.module_name + "'";
                DataTable dt1 = db.SelectDb(sql1).Tables[0];
                if (dt1.Rows.Count == 0)
                {


                    string Insert = $@"insert into  [dbo].[modules]
                                   ([module_name]
                                   ,[created_by]
                                   ,[module_category]
                                   ,[DateCreated])
                                    values
                                    ('" + data.module_name + "'," +
                                    "'" + data.created_by + "', " +
                                    "'" + data.module_category + "', " +
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
        public class DeleteUserModel
        {

            public string Id { get; set; }

        }
        [HttpPost]
        public IActionResult DeleteUser(DeleteUserModel data)
        {
            try
            {
                sql = $@"select * from tbl_User_Model where Id ='" + data.Id + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {

                    string OTPInsert = $@"update tbl_User_Model set Status='2'" +
                                    "where Id='" + data.Id + "'";
                    db.AUIDB_WithParam(OTPInsert);
                    results = db.AUIDB_WithParam(OTPInsert) +" Deleted";
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
        public class LoginModel
        {
            public string? Username { get; set; }
            public string? Password { get; set; }

        }
        [HttpPost]
        public IActionResult LogIn(LoginModel data)
        {
            string results = "";
            bool compr_user = false;
            string sql = $@"SELECT        tbl_User_Model.Id, tbl_User_Model.Username, tbl_User_Model.Password, tbl_User_Model.Fname, tbl_User_Model.Mname, tbl_User_Model.Lname, tbl_User_Model.Cno, tbl_User_Model.Address, tbl_User_Model.DateCreated, 
                         tbl_User_Model.DateUpdated, tbl_User_Model.UserId, tbl_Status_Model.Name AS Status, tbl_UserType_Model.UserType
FROM            tbl_User_Model INNER JOIN
                         tbl_Status_Model ON tbl_User_Model.Status = tbl_Status_Model.Id INNER JOIN
                         tbl_UserType_Model ON tbl_User_Model.UTID = tbl_UserType_Model.Id  
              WHERE        (tbl_User_Model.Username = '" + data.Username + "' COLLATE Latin1_General_CS_AS) AND (tbl_User_Model.Password = '" + Cryptography.Encrypt(data.Password) + "' COLLATE Latin1_General_CS_AS) AND (tbl_User_Model.Status = 1)";


            DataTable dt = db.SelectDb(sql).Tables[0];
            if (data.Username.Length != 0 || data.Password.Length != 0)
            {
                if (dt.Rows.Count != 0)
                {
                    compr_user = String.Equals(dt.Rows[0]["Username"].ToString().Trim(), data.Username, StringComparison.Ordinal);
                }

                if (compr_user)
                {
                    string pass = Cryptography.Decrypt(dt.Rows[0]["password"].ToString().Trim());
                    if ((pass).Equals(data.Password))
                    {
                        results = "Successfully Log In";
                        return Ok(results);
                    }
                    else
                    {

                        results = "Invalid Log In";
                        return BadRequest(results);

                    }

                }
                else
                {
                    return BadRequest("Invalid Log In");
                }
            }
            else
            {
                return BadRequest("Invalid Log In");
            }


        }
    }
}
