using AuthSystem.Models;
using GoldOneAPI.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.CollectionController;
using static GoldOneAPI.Controllers.HolidayController;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationController : ControllerBase
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


        public class NotificationVM
        {
            public string? Id { get; set; }
            public string? Actions { get; set; }
            public string? DateCreated { get; set; }
            public string? Module { get; set; }
            public string? Name { get; set; }
            public string? UserId { get; set; }
            public string? isRead { get; set; }
            public string? Reference { get; set; }
        }
        public class NotifID
        {
            public string? Id { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> NotificationList()
        {
            try
            {

                var result = dbmet.GetNotificationList().ToList();
                return Ok(result);
               
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }



        }
        [HttpGet]
        public async Task<IActionResult> NotificationCount(string userid)
        {
            try
            {

                var result = dbmet.GetNotificationList().Where(a => a.UserId == userid).ToList().Count();
                return Ok(result);

            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }



        }


        [HttpPost]
        public async Task<IActionResult> UpdateReadNotification(NotifID data)
        {
            string result = "";
            try
            {
                string Update = "";


                sql = $@"SELECT [Id]
                          ,[Actions]
                          ,[DateCreated]
                          ,[Module]
                          ,[Name]
                          ,[isRead]
                          ,[UserId]
                      FROM [GoldOne].[dbo].[tbl_Notifications_Model] where Id = '"+data.Id+"'";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Update = $@"UPDATE [dbo].[tbl_Notifications_Model]
                               SET 
                                   [isRead] = '1'" +
                             "where Id = '" + data.Id + "'";


                    result = db.AUIDB_WithParam(Update) + " Updated";
                    return Ok(result);
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
        [HttpGet]
        public async Task<IActionResult> NotificationListFilterbyUserId(string userid)
        {
            try
            {

                var result = dbmet.GetNotificationList().Where(a=>a.UserId == userid).ToList();
                return Ok(result);

            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }



        }

    }
}