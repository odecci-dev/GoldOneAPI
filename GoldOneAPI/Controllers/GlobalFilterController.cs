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
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GlobalFilterController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();

        public class StatusVM
        {
            public int? Status { get; set; }
        }
            public class FilterParam
        {
            public string? LoanType { get; set; }
            public string? Fullname { get; set; }
            public string? From { get; set; }
            public string? To { get; set; }
            public List<StatusVM>? statusid { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> FilterSearch(FilterParam data)
        {

            var list = (dynamic)null;
            var items = (dynamic)null;
            var from = data.From == "" ? "0" : data.From;
            var to = data.To == "" ? "0" : data.To;
            int totalItems = 0;
            int totalPages = 0;
            try
            {
                string status_id = "";
                var stats = new List<StatusVM>();
                for (int x = 0; x < data.statusid.Count; x++)
                {
                    var item = new StatusVM();
                    item.Status = data.statusid[x].Status;
                    stats.Add(item);
                }

                var filter = stats.ToList();
                if (data.LoanType == "")
                {
                    //var result = dbmet.GetApplicationList().ToList();
                    //totalItems = result.Count;
                    //totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);

                    //list = result.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();


                    if (filter[0].Status != 0)
                    {

                        var no_amount = dbmet.GetApplicationList().Where(a => a.LoanAmount != "").ToList();

                        var result = no_amount.Where(a => a.Borrower.ToUpper().Contains(data.Fullname.ToUpper()) || Convert.ToDecimal(a.LoanAmount) < Convert.ToDecimal(from) || Convert.ToDecimal(a.LoanAmount) < Convert.ToDecimal(to)).ToList();
                        var filteredFirstList = result.Where(a => filter.Any(b => b.Status.ToString() == a.StatusId.ToString())).ToList();

                        totalItems = filteredFirstList.Count;
                        totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);
                        list = filteredFirstList.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();



                    }
                    else
                    {
                        var result = dbmet.GetApplicationList().ToList();
                        totalItems = result.Count;
                        totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);

                        list = result.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();

                    }
                    var paginationHeader = new
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        CurrentPage = data.Page,
                        PageSize = data.PageSize
                    };
                    items = list == null ? "Null data" : list;
                    return Ok(items);
                }
                else
                {
                    if (data.LoanType == "LT-02")
                    {
                        

                        if (filter[0].Status != 0)
                        {

                            var no_amount = dbmet.GetApplicationList().Where(a => a.LoanAmount != "" && a.LoanTypeID == data.LoanType).ToList();

                            var result = no_amount.Where(a => a.Borrower.ToUpper().Contains(data.Fullname.ToUpper()) || Convert.ToDecimal(a.LoanAmount) < Convert.ToDecimal(from) || Convert.ToDecimal(a.LoanAmount) < Convert.ToDecimal(to)).ToList();
                            var filteredFirstList = result.Where(a => filter.Any(b => b.Status.ToString() == a.StatusId.ToString())).ToList();

                            totalItems = filteredFirstList.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);
                            list = filteredFirstList.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();



                        }
                        else
                        {
                            if (data.Fullname == "" && data.From == "" && data.To == "" && stats[0].Status == 0)
                            {
                                var result = dbmet.GetApplicationList().Where(a => a.LoanTypeID == data.LoanType).ToList();


                                totalItems = result.Count;
                                totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);

                                list = result.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();

                            }
                            else
                            {
                                var no_amount = dbmet.GetApplicationList().Where(a => a.LoanAmount != "" && a.LoanTypeID == data.LoanType).ToList();
                                var result = no_amount.Where(a => a.Borrower.ToUpper().Contains(data.Fullname.ToUpper()) && Convert.ToDecimal(a.LoanAmount) >= Convert.ToDecimal(data.From) && Convert.ToDecimal(a.LoanAmount) >= Convert.ToDecimal(data.To)).ToList();
                                var filteredFirstList = result.Where(a => filter.Any(b => b.Status.ToString() == a.StatusId.ToString())).ToList();

                                totalItems = filteredFirstList.ToList().Count;
                                totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);

                                list = filteredFirstList.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();

                            }
                        }

                        items = list == null ? "Null data" : list;
                    }
                    else
                    {

                        if (filter[0].Status != 0)
                        {

                            var no_amount = dbmet.GetApplicationList().Where(a => a.LoanAmount != "" && a.LoanTypeID == data.LoanType).ToList();

                            var result = no_amount.Where(a => a.Borrower.ToUpper().Contains(data.Fullname.ToUpper()) || Convert.ToDecimal(a.LoanAmount) < Convert.ToDecimal(from) || Convert.ToDecimal(a.LoanAmount) < Convert.ToDecimal(to)).ToList();
                            var filteredFirstList = result.Where(a => filter.Any(b => b.Status.ToString() == a.StatusId.ToString())).ToList();

                            totalItems = filteredFirstList.Count;
                            totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);
                            list = filteredFirstList.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();



                        }
                        else
                        {
                            if (data.Fullname == "" && data.From == "" && data.To == "" && stats[0].Status == 0)
                            {
                                var result = dbmet.GetApplicationList().Where(a => a.LoanTypeID == data.LoanType).ToList();


                                totalItems = result.Count;
                                totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);

                                list = result.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();

                            }
                            else
                            {
                                var no_amount = dbmet.GetApplicationList().Where(a => a.LoanAmount != "" && a.LoanTypeID == data.LoanType).ToList();
                                var result = no_amount.Where(a => a.Borrower.ToUpper().Contains(data.Fullname.ToUpper()) && Convert.ToDecimal(a.LoanAmount) >= Convert.ToDecimal(data.From) && Convert.ToDecimal(a.LoanAmount) >= Convert.ToDecimal(data.To)).ToList();
                                var filteredFirstList = result.Where(a => filter.Any(b => b.Status.ToString() == a.StatusId.ToString())).ToList();

                                totalItems = filteredFirstList.ToList().Count;
                                totalPages = (int)Math.Ceiling((double)totalItems / data.PageSize);

                                list = filteredFirstList.Skip((data.Page - 1) * data.PageSize).Take(data.PageSize).ToList();

                            }
                        }

                        var paginationHeader = new
                        {
                            TotalItems = totalItems,
                            TotalPages = totalPages,
                            CurrentPage = data.Page,
                            PageSize = data.PageSize
                        };
                        items = list == null ? "Null data" : list;
                    }

                    return Ok(items);
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }

    }
}