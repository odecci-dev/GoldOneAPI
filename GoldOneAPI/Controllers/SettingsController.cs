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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();

        public class SettingsModel
        {
            public string? Id { get; set; }
            public string? MonthlyTarget { get; set; }
            public string? DisplayReset { get; set; }
            public string? CompanyCno { get; set; }
            public string? CompanyAddress { get; set; }
            public string? CompanyEmail { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> SettingList()
        {
            try
            {
                var result = dbmet.GetSettingList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
     
        [HttpPost]
        public IActionResult UpdateSettings(SettingsModel data)
        {
            string results = "";
            try
            {
             
                sql = $@"select * from tbl_settings_model where Id ='" + data.Id + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {
                    string update_app = $@"UPDATE [dbo].[tbl_settings_model]
                                       SET [MonthlyTarget] = '" + data.MonthlyTarget + "'," +
                                       "[DisplayReset] = '" + data.DisplayReset + "'," +
                                       "[CompanyAddress] = '" + data.CompanyAddress + "'," +
                                       "[CompanyEmail] = '" + data.CompanyEmail + "'," +
                                       "[CompanyCno] ='" + data.CompanyCno + "' " +
                                       "where Id ='" + data.Id + "'";
                    db.AUIDB_WithParam(update_app);
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