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
using static GoldOneAPI.Controllers.FieldOfficerController;
using static GoldOneAPI.Controllers.FieldAreaController;
using static GoldOneAPI.Controllers.CollectionController;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaginationController : ControllerBase
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

        public class PaginationModel<T>
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<T> items { get; set; }


        }
        [HttpGet]
        public async Task<IActionResult> DisplayListPaginate(int page, int pageSize, string? FilterName, string? status, string module)
        {
  
            var model_result = (dynamic)null;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {

                switch (module)
                {
                    case "Member":
                        model_result = new PaginationModel<MemberDisplay>();
     
                        if (FilterName == null && status == null)
                        {

                            var Member = dbmet.GetMemberDisplay().ToList();
                            totalItems = Member.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Member.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else if (FilterName  != null && status == null)
                        {
                            var Member = dbmet.GetMemberDisplay().Where(a => a.Borrower.ToUpper().Contains(FilterName.ToUpper())).ToList();
                            totalItems = Member.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Member.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).OrderByDescending(a => a.DateCreated).Take(1).ToList();
                        }
                        else if (FilterName == null && status != null)
                        {
                            var Member = dbmet.GetMemberDisplay().Where(a => a.Status == status).ToList();
                            totalItems = Member.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Member.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {

                            var Member = dbmet.GetMemberDisplay().Where(a => a.Borrower.ToUpper().Contains(FilterName.ToUpper()) && a.Status == status).ToList();
                            totalItems = Member.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(int.Parse(page_size.ToString()).ToString()));

                            items = Member.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "Declined":
                        model_result = new PaginationModel<DeclineReports>();

                        if (FilterName == null && status == null)
                        {

                            var Member = dbmet.Report_Decline().ToList();
                            totalItems = Member.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Member.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {

                            var Member = dbmet.Report_Decline().Where(a => a.Borrower.ToUpper().Contains(FilterName.ToUpper()) ).ToList();
                            totalItems = Member.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(int.Parse(page_size.ToString()).ToString()));

                            items = Member.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "Holiday":
                        model_result = new PaginationModel<HolidayListVM>();
                        if (FilterName != null && status != null)
                        {
                            var Holiday = dbmet.GetHolidayList().Where(a => a.HolidayName.ToUpper().Contains(FilterName.ToUpper()) && a.Status == status).ToList();
                            totalItems = Holiday.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Holiday.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else if (FilterName != null)
                        {
                            var Holiday = dbmet.GetHolidayList().Where(a => a.HolidayName.ToUpper().Contains(FilterName.ToUpper()) && a.Status == "Active").ToList();
                            totalItems = Holiday.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Holiday.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {
                            var Holiday = dbmet.GetHolidayList().ToList();
                            totalItems = Holiday.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Holiday.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "FieldOfficer":
                        model_result = new PaginationModel<FoVM>();
                      
                        if (FilterName != null && status != null)
                        {
                            var FieldOfficer = dbmet.GetFieldOfficer().Where(a => a.Fullname.ToUpper().Contains(FilterName.ToUpper()) && a.Status == status).ToList();
                            totalItems = FieldOfficer.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = FieldOfficer.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else if (FilterName != null)
                        {
                            var FieldOfficer = dbmet.GetFieldOfficer().Where(a => a.Fullname.ToUpper().Contains(FilterName.ToUpper()) && a.Status == "Active").ToList();
                            totalItems = FieldOfficer.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = FieldOfficer.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {
                            var FieldOfficer = dbmet.GetFieldOfficer().ToList();
                            totalItems = FieldOfficer.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = FieldOfficer.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "Area":
                        model_result = new PaginationModel<AreaVM>();
                      
                        if (FilterName != null && status != null)
                        {
                            var Area = dbmet.GetFieldAreas().Where(a => a.AreaName.ToUpper().Contains(FilterName.ToUpper()) && a.Status == status).ToList();
                            totalItems = Area.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Area.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else if (FilterName != null)
                        {
                            var Area = dbmet.GetFieldAreas().Where(a => a.AreaName.ToUpper().Contains(FilterName.ToUpper()) && a.Status == "Active").ToList();
                            totalItems = Area.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Area.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {
                            var Area = dbmet.GetFieldAreas().ToList();
                            totalItems = Area.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = Area.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "UnAssignedArea":
                        model_result = new PaginationModel<AreaVM>();
                        
                        if (FilterName != null)
                        {
                            var UnAssignedArea = dbmet.GetUnAssignedLocationList().Where(a => a.AreaName.ToUpper().Contains(FilterName.ToUpper()) && a.Status == "Inactive").ToList();
                            totalItems = UnAssignedArea.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = UnAssignedArea.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {
                            var UnAssignedArea = dbmet.GetUnAssignedLocationList().ToList();
                            totalItems = UnAssignedArea.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = UnAssignedArea.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "LoanType":
                        model_result = new PaginationModel<LoanTypeDetailsVM>();
                       
                        if (FilterName != null && status != null)
                        {
                            var LoanType = dbmet.GetLoanTypeDetails().Where(a => a.LoanTypeName.ToUpper().Contains(FilterName.ToUpper())).ToList();

                            totalItems = LoanType.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = LoanType.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else if (FilterName != null)
                        {
                            var LoanType = dbmet.GetLoanTypeDetails().Where(a => a.LoanTypeName.ToUpper().Contains(FilterName.ToUpper()) && a.Status == "Active").ToList();
                            totalItems = LoanType.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = LoanType.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        else
                        {
                            var LoanType = dbmet.GetLoanTypeDetails().ToList();
                            totalItems = LoanType.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                            items = LoanType.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        }
                        break;
                    case "Collection":
                        model_result = new PaginationModel<AreaDetailsVM>();
                        var Collection = dbmet.ShowArea().ToList();
                        totalItems = Collection.Count;
                        totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                        items = Collection.Skip((page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                        break;
                    default:
                        items = null;
                        break;
                }
                var result = model_result;

                int pages = page == 0 ? 1 : page;
                result.CurrentPage = page == 0 ? "1" : page.ToString();
                int page_next = pages + 1;
                int page_prev = pages - 1;
                int t_record = int.Parse(items.Count.ToString()) / int.Parse(page_size);
                int t_records = totalItems / int.Parse(page_size);
                result.NextPage = items.Count % int.Parse(page_size) >= 0 ? page_next.ToString() : "0";
                result.PrevPage = pages == 1 ? "0" : page_prev.ToString();
                result.TotalPage = t_records.ToString();
                result.PageSize = page_size;
                result.TotalRecord = totalItems.ToString();
                result.items = items;
                return Ok(result);


            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
    }
}