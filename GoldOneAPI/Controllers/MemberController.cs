using AuthSystem.Models;
using GoldOneAPI.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using static GoldOneAPI.Controllers.ApplicationController;
using static GoldOneAPI.Controllers.GroupController;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Linq;
using static GoldOneAPI.Controllers.FieldAreaController;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections;
using static GoldOneAPI.Controllers.HolidayController;
using System.Text;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DbManager db = new DbManager();
        private readonly AppSettings _appSettings;
        DBMethods dbmet = new DBMethods();
        private readonly JwtAuthenticationManager jwtAuthenticationManager;
        private readonly IWebHostEnvironment _environment;

        public MemberController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;
            //this.jwtAuthenticationManager = jwtAuthenticationManager;

        }

        #region membermode vm

        public class ApplicationVM2
        {
            public string? LoanAmount { get; set; }
            public string? Terms { get; set; }
            public string? NameOfTerms { get; set; }
            public string? LoanType { get; set; }
            public string? LoanTypeID { get; set; }
            public string? InterestRate { get; set; }
            public string? LDID { get; set; }

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
            public string? DateSubmitted { get; set; }
            public string? ReleasedBy { get; set; }

            public string? ModeOfRelease { get; set; }
            public string? ModeOfReleaseReference { get; set; }
            public string? Courerier { get; set; }
            public string? CourierCNo { get; set; }
            public string? CourerierName { get; set; }
            public string? Denomination { get; set; }
            public string? AreaName { get; set; }
            public string? Remarks { get; set; }
            public string? ApprovedLoanAmount { get; set; }
            public string? ApprovedTermsOfPayment { get; set; }
            public string? Days { get; set; }

        }
        public class GroupApplicationVM2
        {
            public string? LoanAmount { get; set; }
            public string? GroupId { get; set; }
            public string? Terms { get; set; }
            public string? LoanType { get; set; }
            public string? InterestRate { get; set; }
            public string? LDID { get; set; }

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
            public string? DateSubmitted { get; set; }
            public string? ReleasedBy { get; set; }
            public string? Days { get; set; }
            public string? ApprovedLoanAmount { get; set; }
            public string? ApprovedTermsOfPayment { get; set; }
            public string? ModeOfRelease { get; set; }
            public string? ModeOfReleaseReference { get; set; }
            public string? Courerier { get; set; }
            public string? CourierCNo { get; set; }
            public string? CourerierName { get; set; }
            public string? Denomination { get; set; }
            public string? AreaName { get; set; }
            public string? Remarks { get; set; }

        }
        public class UploadFile
        {

            public string? FileName { get; set; }
            public string? FilePath { get; set; }
        }
        public class FO_UploadFile
        {

            public string? FilePath { get; set; }
        }
        public class C0_UploadFile
        {

            public string? FileName { get; set; }
            public string? FilePath { get; set; }
        }
        public class Signature_Upload
        {

            public string? FileName { get; set; }
            public string? FilePath { get; set; }
        }
        public class BusinessModelVM
        {
            public string? BusinessName { get; set; }
            public string? BusinessType { get; set; }
            public string? BusinessAddress { get; set; }
            public string? B_status { get; set; }
            public string? B_statusID { get; set; }
            public int? YOB { get; set; }
            public int? NOE { get; set; }
            public decimal? Salary { get; set; }
            public decimal? VOS { get; set; }
            public decimal? AOS { get; set; }
            public List<FileModel>? BusinessFiles { get; set; }

        }
        public class MemberModelVM
        {

            public string? Fullname { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Age { get; set; }
            public string? Barangay { get; set; }
            public string? City { get; set; }
            public string? Civil_Status { get; set; }
            public string? Cno { get; set; }
            public string? Country { get; set; }
            public string? DOB { get; set; }
            public string? EmailAddress { get; set; }
            public string? Gender { get; set; }
            public string? HouseNo { get; set; }
            public string? House_Stats { get; set; }
            public string? POB { get; set; }
            public string? Province { get; set; }
            public string? YearsStay { get; set; }
            public string? ZipCode { get; set; }
            public string? Status { get; set; }
            public string? DateCreated { get; set; }
            public string? ApplicationStatus { get; set; }
            public string? HouseStatusId { get; set; }

            //-----------MonthlyBills

            public string? MemId { get; set; }
            public string? ElectricBill { get; set; }
            public string? WaterBill { get; set; }
            public string? OtherBills { get; set; }
            public string? DailyExpenses { get; set; }

            //-----------JobInfo
            public string? JobDescription { get; set; }
            public string? YOS { get; set; }
            public string? MonthlySalary { get; set; }
            public string? OtherSOC { get; set; }
            public string? BO_Status { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyAddress { get; set; }
            public string? Coj_YOS { get; set; }
            public string? Emp_Status { get; set; }

            //----Family
            public string? F_Fname { get; set; }
            public string? F_Lname { get; set; }
            public string? F_Mname { get; set; }
            public string? F_Suffix { get; set; }
            public string? F_DOB { get; set; }
            public string? F_Age { get; set; }
            public string? F_NOD { get; set; }
            public string? F_YOS { get; set; }
            public string? F_Emp_Status { get; set; }
            public string? F_Job { get; set; }
            public string? F_CompanyName { get; set; }
            public string? F_RTTB { get; set; }
            public string? FamId { get; set; }
            //---business
            public List<BusinessModelVM>? Business { get; set; }
            //-- loan
            //public decimal? LoanAmount { get; set; }
            public List<ApplicationVM2>? IndividualLoan { get; set; }
            public List<GroupApplicationVM2>? GroupLoan { get; set; }

            public string? TermsOfPayment { get; set; }
            public string? Purpose { get; set; }
            //child
            public List<ChildModel>? Child { get; set; }
            //Appliances
            public List<ApplianceModel>? Appliances { get; set; }
            //assets
            public List<AssetsModel>? Assets { get; set; }
            //Property
            public List<PropertyDetailsModel>? Property { get; set; }
            //bank
            public List<BankModel>? Bank { get; set; }
            //comaker
            public List<FileModel>? Files { get; set; }
            //comaker
            public string? Co_Fname { get; set; }
            public string? Co_Lname { get; set; }
            public string? Co_Mname { get; set; }
            public string? Co_Suffix { get; set; }
            public string? Co_Age { get; set; }
            public string? Co_Barangay { get; set; }
            public string? Co_City { get; set; }
            public string? Co_Civil_Status { get; set; }
            public string? Co_Cno { get; set; }
            public string? Co_Country { get; set; }
            public string? Co_DOB { get; set; }
            public string? Co_EmailAddress { get; set; }
            public string? Co_Gender { get; set; }
            public string? Co_HouseNo { get; set; }
            public string? Co_House_Stats { get; set; }
            public string? Co_HouseStatusId { get; set; }
            public string? Co_POB { get; set; }
            public string? Co_Province { get; set; }
            public string? Co_YearsStay { get; set; }
            public string? Co_ZipCode { get; set; }
            public string? Co_RTTB { get; set; }


            //comaker

            public string? Co_JobDescription { get; set; }
            public string? Co_MonthlySalary { get; set; }
            public string? Co_OtherSOC { get; set; }
            public string? Co_BO_Status { get; set; }
            public string? Co_CompanyName { get; set; }
            public string? Co_CompanyAddress { get; set; }
            public string? Co_Emp_Status { get; set; }
            public List<FileModel>? Co_Files { get; set; }
            //new application
            public string? CMID { get; set; }
        }
        public class GroupModelVM
        {

            public string? Fullname { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Age { get; set; }
            public string? Barangay { get; set; }
            public string? City { get; set; }
            public string? Civil_Status { get; set; }
            public string? Cno { get; set; }
            public string? Country { get; set; }
            public string? DOB { get; set; }
            public string? EmailAddress { get; set; }
            public string? Gender { get; set; }
            public string? HouseNo { get; set; }
            public string? House_Stats { get; set; }
            public string? POB { get; set; }
            public string? Province { get; set; }
            public string? YearsStay { get; set; }
            public string? ZipCode { get; set; }
            public string? Status { get; set; }
            public string? DateCreated { get; set; }
            public string? HouseStatusId { get; set; }

            //-----------MonthlyBills

            public string? MemId { get; set; }
            public string? ElectricBill { get; set; }
            public string? WaterBill { get; set; }
            public string? OtherBills { get; set; }
            public string? DailyExpenses { get; set; }

            //-----------JobInfo
            public string? JobDescription { get; set; }
            public string? YOS { get; set; }
            public string? MonthlySalary { get; set; }
            public string? OtherSOC { get; set; }
            public string? BO_Status { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyAddress { get; set; }
            public string? Coj_YOS { get; set; }
            public string? Emp_Status { get; set; }

            //----Family
            public string? F_Fname { get; set; }
            public string? F_Lname { get; set; }
            public string? F_Mname { get; set; }
            public string? F_Suffix { get; set; }
            public string? F_DOB { get; set; }
            public string? F_Age { get; set; }
            public string? F_NOD { get; set; }
            public string? F_YOS { get; set; }
            public string? F_Emp_Status { get; set; }
            public string? F_Job { get; set; }
            public string? F_CompanyName { get; set; }
            public string? F_RTTB { get; set; }
            public string? FamId { get; set; }
            public string? GroupName { get; set; }
            public string? GroupID { get; set; }
            //---business
            public List<BusinessModelVM>? Business { get; set; }
            //-- loan
            //public decimal? LoanAmount { get; set; }
            public List<ApplicationVM2>? GroupLoan { get; set; }


            //child
            public List<ChildModel>? Child { get; set; }
            //Appliances
            public List<ApplianceModel>? Appliances { get; set; }
            //assets
            public List<AssetsModel>? Assets { get; set; }
            //Property
            public List<PropertyDetailsModel>? Property { get; set; }
            //bank
            public List<BankModel>? Bank { get; set; }
            //comaker
            public List<FileModel>? Files { get; set; }
            //comaker
            public string? Co_Fname { get; set; }
            public string? Co_Lname { get; set; }
            public string? Co_Mname { get; set; }
            public string? Co_Suffix { get; set; }
            public string? Co_Age { get; set; }
            public string? Co_Barangay { get; set; }
            public string? Co_City { get; set; }
            public string? Co_Civil_Status { get; set; }
            public string? Co_Cno { get; set; }
            public string? Co_Country { get; set; }
            public string? Co_DOB { get; set; }
            public string? Co_EmailAddress { get; set; }
            public string? Co_Gender { get; set; }
            public string? Co_HouseNo { get; set; }
            public string? Co_House_Stats { get; set; }
            public string? Co_HouseStatusId { get; set; }
            public string? Co_POB { get; set; }
            public string? Co_Province { get; set; }
            public string? Co_YearsStay { get; set; }
            public string? Co_ZipCode { get; set; }
            public string? Co_RTTB { get; set; }


            //comaker

            public string? Co_JobDescription { get; set; }
            public string? Co_MonthlySalary { get; set; }
            public string? Co_OtherSOC { get; set; }
            public string? Co_BO_Status { get; set; }
            public string? Co_CompanyName { get; set; }
            public string? Co_CompanyAddress { get; set; }
            public string? Co_Emp_Status { get; set; }
            public List<FileModel>? Co_Files { get; set; }
            //new application
            public string? CMID { get; set; }
        }
        #endregion
        #region models
        public class MemberModel
        {
            public int Id { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Age { get; set; }
            public string? Barangay { get; set; }
            public string? City { get; set; }
            public string? Civil_Status { get; set; }
            public string? Cno { get; set; }
            public string? Country { get; set; }
            public string? DOB { get; set; }
            public string? EmailAddress { get; set; }
            public string? Gender { get; set; }
            public string? HouseNo { get; set; }
            public string? House_Stats { get; set; }
            public string? POB { get; set; }
            public string? Province { get; set; }
            public string? YearsStay { get; set; }
            public string? ZipCode { get; set; }
            public string? Status { get; set; }

        }
        public class FilterFullname
        {
            public string? Fullname { get; set; }

        }
        public class FitlerFOID
        {
            public string? FOID { get; set; }

        }
        public class AppID
        {
            public string? ApplicationID { get; set; }

        }
        public class HouseStats
        {
            public int Id { get; set; }
            public string? HouseStatus { get; set; }
            public string? Status { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }

        }
        public class PaymentHistory
        {
            public int Id { get; set; }
            public string? LoanAmount { get; set; }
            public string? OutStandingBalance { get; set; }
            public string? PaidAmount { get; set; }
            public string? Collector { get; set; }
            public string? PaymentDate { get; set; }
            public string? PaymentType { get; set; }
            public string? Penalty { get; set; }
            public string? MemId { get; set; }
            public string? DateCreated { get; set; }

        }
        public class LoanHistory
        {
            public int Id { get; set; }
            public string? LoanAmount { get; set; }
            public string? Savings { get; set; }
            public string? Penalty { get; set; }
            public string? OutstandingBalance { get; set; }
            public string? DateReleased { get; set; }
            public string? DueDate { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }
            public string? MemId { get; set; }

        }

        #endregion
        [HttpGet]
        public async Task<IActionResult> MemberList()
        {
            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetMemberList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Member_FilterByActive(string status, int applicationStatus)
        {
            var result = (dynamic)null;
            //var result = new List<CreditModel>();
            try
            {
                result = dbmet.GetMemberList().Where(a => a.Status == status && a.ApplicationStatus == applicationStatus.ToString()).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLastMemberList()
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetMemberList().ToList().LastOrDefault();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public IActionResult MembershipFilterByFullname(FilterFullname data)
        {
            try
            {

                var result = dbmet.GetMembershipFilterByFullname(data.Fullname).ToList();
                if (result.Count != 0)
                {
                    return Ok(result);
                }
                else
                {
                    var res = dbmet.GetMemberList().Where(a => a.Fullname.ToUpper().Contains(data.Fullname.ToUpper())).ToList();
                    if (data.Fullname == "")
                    {
                        var res_ = dbmet.GetMemberList().ToList();
                        return Ok(res_);
                    }
                    else
                    {
                        if (res.Count != 0)
                        {
                            return Ok(res);
                        }
                        else
                        {
                            return BadRequest("No Data");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                results = ex.GetBaseException().ToString();
                return BadRequest(results);
            }
        }
        public class MemberDisplay
        {
            public string? Borrower { get; set; }
            public string? Status { get; set; }
            public string? CurrentLoan { get; set; }
            public string? OutstandingBalance { get; set; }
            public string? LastUpdated { get; set; }
            public string? MemId { get; set; }
            public string? Naid { get; set; }
            public string? ProfilePath { get; set; }

        }
        public class DeclineReports
        {
            public string? Borrower { get; set; }
            public string? NAID { get; set; }
            public string? LoanAmount { get; set; }
            public string? Remarks { get; set; }

        }
        public class Savingmodule
        {
            public string? tbl_Member_Model { get; set; }
            public string? tbl_JobInfo_Model { get; set; }
            public string? tbl_FamBackground_Model { get; set; }
            public string? tbl_ChildInfo_Model { get; set; }
            public string? tbl_BusinessInformation_Model { get; set; }
            public string? tbl_CoMaker_Model { get; set; }
            public string? tbl_AssetsProperties_Model { get; set; }
            public string? tbl_Property_Model { get; set; }
            public string? tbl_BankAccounts_Model { get; set; }
            public string? tbl_Application_Model { get; set; }
            public string? tbl_Appliance_Model { get; set; }
            public string? tbl_Area_Model { get; set; }
            public string? tbl_CoMaker_JobInfo_Model { get; set; }
            public string? tbl_LoanDetails_Model { get; set; }
            public string? tbl_fileupload_Model { get; set; }
            public string? tbl_MonthlyBills_Model { get; set; }
            public string? tbl_CoMakerFileUpload_Model { get; set; }
            public string? NAID { get; set; }
            public string? promtresult { get; set; }
            public string? promtresult_status { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> Member_DisplayList(int page, int pageSize, string? MemberName, string? status)
        {
            var result = (dynamic)null;
            try
            {



                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (MemberName == null && status == null)
                {
                    var list = dbmet.GetMemberDisplay().ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = dbmet.GetMemberDisplay().Where(a => a.Status == status && a.Borrower.ToUpper().Contains(MemberName.ToUpper())).ToList();
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
        public class MemberOnChange
        {
            public string? Fname { get; set; }
            public string? Mname { get; set; }
            public string? Lname { get; set; }
            public string? POB { get; set; }
            public string? Barangay { get; set; }
            public string? DOB { get; set; }
            public string? Age { get; set; }

        }

        [HttpPost]
        public IActionResult Member_ValidationOnChange(MemberOnChange data)
        {

            string applicationfilter = $@"select tbl_Application_Model.NAID
                                from
                                tbl_Application_Model inner join
                                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID inner join
                                tbl_Member_Model on tbl_Member_Model.MemId = tbl_Application_Model.MemId inner join
                                tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Member_Model.MemId inner join
                                tbl_FamBackground_Model on tbl_FamBackground_Model.MemId = tbl_Member_Model.MemId inner join
                                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId
                                where  tbl_Member_Model.Fname ='" + data.Fname + "' and tbl_Member_Model.Mname ='" + data.Mname + "'" +
                            "and  tbl_Member_Model.Lname ='" + data.Lname + "' and tbl_Member_Model.POB ='" + data.POB + "'" +
                            "and tbl_Member_Model.Barangay = '" + data.Barangay + "' and tbl_Member_Model.Age='" + data.Age + "' and tbl_Member_Model.DOB='" + data.DOB + "'";
            DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];
            if (tbl_applicationfilter.Rows.Count != 0)
            {
                var result = dbmet.GetApplicationMemberFilterList(tbl_applicationfilter.Rows[0]["NAID"].ToString()).ToList();
                return Ok(result);
            }
            else
            {
                return BadRequest("No data");
            }


        }
        [HttpPost]
        public IActionResult Member_PromptExistingLoan(MemberOnChange data)
        {
            string result = "";
            string applicationfilter = $@"select tbl_Application_Model.NAID
                                from
                                tbl_Application_Model inner join
                                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID inner join
                                tbl_Member_Model on tbl_Member_Model.MemId = tbl_Application_Model.MemId inner join
                                tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Member_Model.MemId inner join
                                tbl_FamBackground_Model on tbl_FamBackground_Model.MemId = tbl_Member_Model.MemId inner join
                                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId
                                where  tbl_Application_Model.Status in (8,9,10)
                                and tbl_Member_Model.Fname ='" + data.Fname + "' and tbl_Member_Model.Mname ='" + data.Mname + "'" +
                            "and  tbl_Member_Model.Lname ='" + data.Lname + "' and tbl_Member_Model.POB ='" + data.POB + "'" +
                            "and tbl_Member_Model.Barangay = '" + data.Barangay + "' and tbl_Member_Model.Age='" + data.Age + "' and tbl_Member_Model.DOB='" + data.DOB + "'";
            DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];
            if (tbl_applicationfilter.Rows.Count != 0)
            {
                result = "Member has a Existing Loans";

            }
            else
            {

            }

            return Ok(result);

        }
        [HttpPost]
        public IActionResult ApplicationMemberDetails(AppID data)
        {
            try
            {
                var result = dbmet.GetApplicationMemberFilterList(data.ApplicationID).ToList();
                if (result.Count != 0)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("No Data");
                }
            }
            catch (Exception ex)
            {
                results = ex.GetBaseException().ToString();
                return BadRequest(results);
            }
        }
        [HttpGet]
        public async Task<IActionResult> MemberGetBusinessFileView(string memid)
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetBusinessMemberFiles(memid).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        //        public async Task<IActionResult> GetPaymentHistory()
        //        {

        //            string sql = $@"SELECT      tbl_PaymentHistory_Model.MemId, tbl_PaymentHistory_Model.DateCreated, tbl_PaymentHistory_Model.Penalty, tbl_PaymentHistory_Model.PaymentType, tbl_PaymentHistory_Model.PaymentDate, 
        //                         tbl_PaymentHistory_Model.Collector, tbl_PaymentHistory_Model.PaidAmount, tbl_PaymentHistory_Model.OutStandingBalance, tbl_PaymentHistory_Model.LoanAmount
        //FROM            tbl_PaymentHistory_Model INNER JOIN
        //                         tbl_Member_Model ON tbl_PaymentHistory_Model.MemId = tbl_Member_Model.MemId ";
        //            var result = new List<PaymentHistory>();
        //            DataTable table = db.SelectDb(sql).Tables[0];

        //            foreach (DataRow dr in table.Rows)
        //            {
        //                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
        //                var item = new PaymentHistory(); item.LoanAmount = dr["LoanAmount"].ToString();
        //                item.MemId = dr["MemId"].ToString();
        //                item.OutStandingBalance = dr["OutStandingBalance"].ToString();
        //                item.PaidAmount = dr["PaidAmount"].ToString();
        //                item.Collector = dr["Collector"].ToString();
        //                item.PaymentDate = dr["PaymentDate"].ToString();
        //                item.PaymentType = dr["PaymentType"].ToString();
        //                item.Penalty = dr["Penalty"].ToString();
        //                item.DateCreated = datec;
        //                result.Add(item);
        //            }

        //            return Ok(result);
        //        }
        [HttpPost]
        public IActionResult UpdateMemberInfo(List<SaveMemberModel> data)
        {
            try
            {
                string filePath = @"C:\data\updatememberinfo.json"; // Replace with your desired file path



                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data[0]));
                string Update = "";
                sql = $@"SELECT        tbl_Member_Model.Id, tbl_Member_Model.Fname, tbl_Member_Model.Lname, tbl_Member_Model.Mname, tbl_Member_Model.Suffix, tbl_Member_Model.Age, tbl_Member_Model.Barangay, tbl_Member_Model.City, 
                         tbl_Member_Model.Civil_Status, tbl_Member_Model.Cno, tbl_Member_Model.Country, tbl_Member_Model.DOB, tbl_Member_Model.EmailAddress, tbl_Member_Model.Gender, tbl_Member_Model.HouseNo, 
                         tbl_Member_Model.House_Stats, tbl_Member_Model.POB, tbl_Member_Model.Province, tbl_Member_Model.YearsStay, tbl_Member_Model.ZipCode, tbl_Member_Model.Status, tbl_Member_Model.DateCreated, 
                         tbl_Member_Model.DateUpdated, tbl_Member_Model.MemId, tbl_Member_Model.OwnProperty, tbl_Member_Model.OwnVehicles, tbl_FamBackground_Model.FamId, tbl_CoMaker_Model.CMID, tbl_Application_Model.NAID
FROM            tbl_Member_Model INNER JOIN
                         tbl_FamBackground_Model ON tbl_Member_Model.MemId = tbl_FamBackground_Model.MemId INNER JOIN
                         tbl_CoMaker_Model ON tbl_Member_Model.MemId = tbl_CoMaker_Model.MemId INNER JOIN
                         tbl_Application_Model ON tbl_Member_Model.MemId = tbl_Application_Model.MemId where tbl_Member_Model.MemId ='" + data[0].MemId + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new UserModel();
                string city_ = Regex.Replace(data[0].Barangay.ToUpper() + data[0].City.ToUpper(), "[^a-zA-Z]", "");
                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data[0]));
                string areafilter = $@"SELECT [Id]
                                  ,[Area]
                                  ,[City]
                                   FROM [dbo].[tbl_Area_Model]
                                    Where City like '%" + data[0].Barangay + ", " + data[0].City + "%'";
                DataTable area_table = db.SelectDb(areafilter).Tables[0];
                var val_city = data[0].Barangay.ToLower() + ", " + data[0].City.ToLower();
                //var val = area_table.Rows[0]["City"].ToString().ToLower().Split("|")[0];
                bool area_city = true;
                if (area_table.Rows.Count != 0)
                {
                    area_city = area_table.Rows[0]["City"].ToString().ToLower().Split("|")[0].Equals(val_city);
                }
                else
                {
                    area_city = false;
                }

                if (area_table.Rows.Count == 0 && !area_city)
                {
                    string insert_area = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                               ([Status],[City])
                                                VALUES
                                                ('1', " +
                                        "'" + data[0].Barangay + ", " + data[0].City + "')";
                    results = db.AUIDB_WithParam(insert_area) + " Added";
                }

                if (dt.Rows.Count != 0)
                {


                    Update += $@"UPDATE [dbo].[tbl_Application_Model]
                              SET 
                              [Status] ='" + data[0].ApplicationStatus + "' " +
                              "where NAID ='" + dt.Rows[0]["NAID"].ToString() + "'";

                    //Member
                    Update += $@"update tbl_Member_Model set 
                                       Fname='" + data[0].Fname + "', " +
                                       "Lname='" + data[0].Lname + "', " +
                                       "Mname='" + data[0].Mname + "', " +
                                       "Suffix='" + data[0].Suffix + "', " +
                                       "Age='" + data[0].Age + "', " +
                                       "Barangay='" + data[0].Barangay + "', " +
                                       "City='" + data[0].City + "', " +
                                       "Civil_Status='" + data[0].Civil_Status + "', " +
                                       "Cno='" + data[0].Cno + "', " +
                                       "Country='" + data[0].Country + "', " +
                                       "DOB='" + Convert.ToDateTime(data[0].DOB).ToString("yyyy-MM-dd") + "', " +
                                       "EmailAddress='" + data[0].EmailAddress + "', " +
                                       "Gender='" + data[0].Gender + "', " +
                                       "HouseNo='" + data[0].HouseNo + "', " +
                                       "House_Stats='" + data[0].House_Stats + "', " +
                                       "POB='" + data[0].POB + "', " +
                                       "Province='" + data[0].Province + "', " +
                                       "YearsStay='" + data[0].YearsStay + "', " +
                                       "ZipCode='" + data[0].ZipCode + "', " +
                                       "Status='" + data[0].Status + "', " +
                                       "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                       "where MemId='" + data[0].MemId + "'";

                    //Job
                    Update += $@"UPDATE [dbo].[tbl_JobInfo_Model]
                               SET 
                                  [JobDescription] = '" + data[0].JobDescription + "', " +
                                  "[YOS] = '" + data[0].YOS + "', " +
                                  "[CompanyName] = '" + data[0].CompanyName + "', " +
                                  "[MonthlySalary] ='" + data[0].MonthlySalary + "', " +
                                  "[CompanyAddress] ='" + data[0].CompanyAddress + "', " +
                                  "[OtherSOC] ='" + data[0].OtherSOC + "', " +
                                  "[Status] = '" + data[0].Status + "', " +
                                  "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                  "[BO_Status] ='" + data[0].BO_Status + "', " +
                                  "[Emp_Status] ='" + data[0].Emp_Status + "' " +
                                    "where MemId='" + data[0].MemId + "'";
                    //FAM

                    sql = $@"select CMID from tbl_FamBackground_Model where MemId='" + data[0].MemId + "'";
                    DataTable famtbl = db.SelectDb(sql).Tables[0];
                    if (famtbl.Rows.Count != 0)
                    {
                        string Insert_Fam = $@"
                                UPDATE [dbo].[tbl_FamBackground_Model]
                                   SET 
                                       [Fname] = '" + data[0].F_Fname + "', " +
                                      "[Mname] ='" + data[0].F_Mname + "', " +
                                      "[Lname] = '" + data[0].F_Lname + "', " +
                                      "[Suffix] = '" + data[0].F_Suffix + "', " +
                                      "[DOB] ='" + Convert.ToDateTime(data[0].F_DOB).ToString("yyyy-MM-dd") + "', " +
                                      "[Age] ='" + data[0].F_Age + "', " +
                                      "[Emp_Status] = '" + data[0].F_Emp_Status + "', " +
                                      "[Position] = '" + data[0].F_Job + "', " +
                                      "[YOS] ='" + data[0].F_YOS + "', " +
                                      "[CmpId] = '" + data[0].F_CompanyName + "', " +
                                      "[NOD] = '" + data[0].F_NOD + "', " +
                                      "[RTTB] ='" + data[0].F_RTTB + "', " +
                                      "[MemId] ='" + data[0].MemId + "', " +
                                      "[Status] = '1', " +
                                      "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                      "where MemId='" + data[0].MemId + "'";
                        db.AUIDB_WithParam(Insert_Fam);

                    }
                    else
                    {
                        string Insert_Fam = $@"insert into   tbl_FamBackground_Model
                               ([Fname]
                                ,[Mname]
                               ,[Lname]
                               ,[Suffix]
                               ,[DOB]
                               ,[Age]
                               ,[Emp_Status]
                               ,[Position]
                                ,[YOS]
                               ,[CmpId]
                               ,[NOD]
                               ,[RTTB]
                               ,[MemId]
                               ,[Status]
                               ,[DateCreated])
                                values
                                ('" + data[0].F_Fname + "'," +
                                         "'" + data[0].F_Mname + "'," +
                                         "'" + data[0].F_Lname + "'," +
                                         "'" + data[0].F_Suffix + "'," +
                                         "'" + Convert.ToDateTime(data[0].F_DOB).ToString("yyyy-MM-dd") + "'," +
                                         "'" + data[0].F_Age + "'," +
                                         "'" + data[0].F_Emp_Status + "'," +
                                         "'" + data[0].F_Job + "'," +
                                         "'" + data[0].F_YOS + "'," +
                                         "'" + data[0].F_CompanyName + "'," +
                                         "'" + data[0].F_NOD + "'," +
                                         "'" + data[0].F_RTTB + "'," +
                                         "'" + data[0].MemId + "'," +
                                         "'1'," +
                                          "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        db.AUIDB_WithParam(Insert_Fam);
                    }

                    //child
                    if (data[0].Child.Count != 0)
                    {
                        string child_val = $@"SELECT * FROM [dbo].[tbl_ChildInfo_Model]  where [FamId] = '" + dt.Rows[0]["FamId"].ToString() + "'";
                        DataTable child_val_tbl = db.SelectDb(child_val).Tables[0];
                        if (child_val_tbl.Rows.Count != 0)
                        {
                            string Delete = $@"DELETE FROM [dbo].[tbl_ChildInfo_Model]  where [FamId] = '" + dt.Rows[0]["FamId"].ToString() + "'";
                            db.AUIDB_WithParam(Delete);
                        }

                        for (int i = 0; i < data[0].Child.Count; i++)
                        {

                            Update += $@"INSERT INTO [dbo].[tbl_ChildInfo_Model]
                                   ([Fname]
                                   ,[Mname]
                                   ,[Lname]
                                   ,[Age]
                                   ,[NOS]
                                   ,[FamId]
                                   ,[Status]
                                   ,[DateCreated])
                             VALUES
                                   ('" + data[0].Child[i].Fname + "'," +
                                       "'" + data[0].Child[i].Mname + "'," +
                                       "'" + data[0].Child[i].Lname + "'," +
                                       "'" + data[0].Child[i].Age + "'," +
                                      "'" + data[0].Child[i].NOS + "'," +
                                      "'" + dt.Rows[0]["FamId"].ToString() + "'," +
                                     "'1'," +
                                         "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                        }

                    }
                    //business
                    if (data[0].Business.Count != 0)
                    {

                        string businessval = $@"SELECT * FROM [dbo].[tbl_BusinessInformation_Model]  where MemId='" + data[0].MemId + "'";
                        DataTable businessval_tbl = db.SelectDb(businessval).Tables[0];
                        if (businessval_tbl.Rows.Count != 0)
                        {

                            string Delete = $@"DELETE FROM [dbo].[tbl_BusinessInformation_Model]  where MemId='" + data[0].MemId + "'";
                            db.AUIDB_WithParam(Delete);
                        }
                        string b_files = "";
                        for (int i = 0; i < data[0].Business.Count; i++)
                        {


                            if (data[0].Business[0].BusinessFiles.Count != 0)
                            {

                                for (int x = 0; x < data[0].Business[0].BusinessFiles.Count; x++)
                                {
                                    b_files += data[0].Business[0].BusinessFiles[i].FilePath + "|";
                                }

                            }
                            string result_ = b_files.Substring(0, b_files.Length - 1);
                            string b_insert = $@"insert into   tbl_BusinessInformation_Model
                            ([BusinessName]
                           ,[BusinessType]
                           ,[BusinessAddress]
                           ,[B_status]
                           ,[YOB]
                           ,[NOE]
                           ,[Salary]
                           ,[VOS]
                           ,[AOS]
                           ,[Status]
                           ,[MemId]
                           ,[FilesUploaded]
                           ,[DateCreated])
                                values
                                ('" + data[0].Business[i].BusinessName.Replace("'", "''") + "'," +
                                      "'" + data[0].Business[i].BusinessType + "'," +
                                      "'" + data[0].Business[i].BusinessAddress + "'," +
                                      "'" + data[0].Business[i].B_status + "'," +
                                      "'" + data[0].Business[i].YOB + "'," +
                                      "'" + data[0].Business[i].NOE + "'," +
                                      "'" + data[0].Business[i].Salary + "'," +
                                      "'" + data[0].Business[i].VOS + "'," +
                                      "'" + data[0].Business[i].AOS + "'," +
                                      "'1'," +
                                      "'" + data[0].MemId + "'," +
                                      "'" + data[0].Business[0].BusinessFiles[i].FilePath + "'," +
                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            db.AUIDB_WithParam(b_insert);
                        }
                    }
                    //monthlybills
                    Update += $@"UPDATE [dbo].[tbl_MonthlyBills_Model]
                               SET [MemId] =  '" + data[0].MemId + "', " +
                                  "[ElectricBill] = '" + data[0].ElectricBill + "', " +
                                  "[WaterBill] ='" + data[0].WaterBill + "', " +
                                  "[OtherBills] = '" + data[0].OtherBills + "', " +
                                  "[DailyExpenses] ='" + data[0].DailyExpenses + "', " +
                                  "[Status] = '1', " +
                               "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                              "where MemId = '" + data[0].MemId + "'";
                    //loan
                    Update += $@"UPDATE [dbo].[tbl_LoanDetails_Model]
                               SET [LoanAmount] = '" + data[0].LoanAmount + "', " +
                                  "[TermsOfPayment] = '" + data[0].TermsOfPayment + "', " +
                                  "[Purpose] ='" + data[0].Purpose + "', " +
                                  "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                  "[Status] = '1' " +
                                  "where MemId='" + data[0].MemId + "'";
                    //comaker
                    Update += $@"UPDATE [dbo].[tbl_CoMaker_Model]
                           SET [Fname] ='" + data[0].Co_Fname + "', " +
                              "[Mname] = '" + data[0].Co_Mname + "', " +
                              "[Lnam] = '" + data[0].Co_Lname + "', " +
                              "[Suffi] = '" + data[0].Co_Suffix + "', " +
                              "[Gender] = '" + data[0].Co_Gender + "', " +
                              "[DOB] = '" + Convert.ToDateTime(data[0].Co_DOB).ToString("yyyy-MM-dd") + "', " +
                              "[Age] = '" + data[0].Co_Age + "', " +
                              "[POB] = '" + data[0].Co_POB + "', " +
                              "[CivilStatus] = '" + data[0].Co_Civil_Status + "', " +
                              "[Cno] = '" + data[0].Co_Cno + "', " +
                              "[EmailAddress] = '" + data[0].Co_EmailAddress + "', " +
                              "[House_Stats] = '" + data[0].Co_House_Stats + "', " +
                              "[HouseNo] = '" + data[0].Co_HouseNo + "', " +
                              "[Barangay] = '" + data[0].Co_Barangay + "', " +
                              "[City] = '" + data[0].Co_City + "', " +
                              "[Region] = '" + data[0].Co_Province + "', " +
                              "[Country] = '" + data[0].Co_Country + "', " +
                              "[ZipCode] = '" + data[0].Co_ZipCode + "', " +
                              "[YearsStay] = '" + data[0].Co_YearsStay + "', " +
                              "[RTTB] = '" + data[0].Co_RTTB + "', " +
                              "[Status] = '1', " +
                              "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                              "where MemId='" + data[0].MemId + "'";
                    //co_job

                    sql = $@"select CMID from tbl_CoMaker_JobInfo_Model where CMID='" + dt.Rows[0]["CMID"].ToString() + "'";
                    DataTable table1 = db.SelectDb(sql).Tables[0];
                    if (table1.Rows.Count != 0)
                    {

                        string insert_jobcomaker = $@"UPDATE [dbo].[tbl_CoMaker_JobInfo_Model]
                               SET [JobDescription] = '" + data[0].Co_JobDescription + "', " +
                                      "[YOS] = '" + data[0].Co_YOS + "', " +
                                      "[CompanyName] = '" + data[0].Co_CompanyName.Replace("'", "''") + "', " +
                                      "[CompanyAddress] = '" + data[0].Co_CompanyAddress + "', " +
                                      "[MonthlySalary] = '" + data[0].Co_MonthlySalary + "', " +
                                      "[OtherSOC] = '" + data[0].Co_OtherSOC + "', " +
                                      "[Status] ='1', " +
                                      "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                      "[BO_Status] = '" + data[0].Co_BO_Status + "', " +
                                      "[Emp_Status] = '" + data[0].Co_Emp_Status + "'" +
                                      "where CMID='" + dt.Rows[0]["CMID"].ToString() + "'";
                        db.AUIDB_WithParam(insert_jobcomaker);
                    }
                    else
                    {
                        string insert_jobcomaker = $@"INSERT INTO [dbo].[tbl_CoMaker_JobInfo_Model]
                               ([JobDescription]
                               ,[YOS]
                               ,[CompanyName]
                               ,[MonthlySalary]
                               ,[OtherSOC]
                               ,[Status]
                               ,[BO_Status]
                               ,[Emp_Status]
                               ,[CMID]
                               ,[CompanyAddress]
                               ,[DateCreated])
                                values
                                ('" + data[0].Co_JobDescription + "'," +
                                "'" + data[0].Co_YOS + "'," +
                                "'" + data[0].Co_CompanyName + "'," +
                                "'" + data[0].Co_MonthlySalary + "'," +
                                "'" + data[0].Co_OtherSOC + "'," +
                                "'1'," +
                                "'" + data[0].Co_BO_Status + "'," +
                                "'" + data[0].Co_Emp_Status + "'," +
                                "'" + dt.Rows[0]["CMID"].ToString() + "'," +
                                "'" + data[0].Co_CompanyAddress + "'," +
                                "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";




                        db.AUIDB_WithParam(insert_jobcomaker);
                    }




                    if (data[0].Assets.Count != 0)
                    {


                        for (int i = 0; i < data[0].Assets.Count; i++)
                        {
                            Update += $@"UPDATE [dbo].[tbl_AssetsProperties_Model]
                                       SET [MotorVehicles] =  '" + data[0].Assets[i].MotorVehicles + "', " +
                                         "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                         "[Status] = '1' " +
                                         "where MemId = '" + data[0].MemId + "'";
                        }

                    }
                    if (data[0].Property.Count != 0)
                    {


                        for (int i = 0; i < data[0].Property.Count; i++)
                        {

                            Update += $@"UPDATE [dbo].[tbl_Property_Model]
                               SET [Property] = '" + data[0].Property[i].Property + "'," +
                                  "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                    "[Status] = '1' " +
                                  "where MemId = '" + data[0].MemId + "'";

                        }

                    }
                    if (data[0].Bank.Count != 0)
                    {


                        for (int i = 0; i < data[0].Bank.Count; i++)
                        {

                            Update += $@"UPDATE [dbo].[tbl_BankAccounts_Model]
                                       SET [BankName] = '" + data[0].Bank[i].BankName + "', " +
                                           "[Address] = '" + data[0].Bank[i].Address + "', " +
                                           "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                           "where MemId = '" + data[0].MemId + "'";
                        }
                    }
                    if (data[0].Appliances.Count != 0)
                    {
                        for (int i = 0; i < data[0].Appliances.Count; i++)
                        {

                            Update += $@"UPDATE [dbo].[tbl_Appliance_Model]
                                       SET [Brand] = '" + data[0].Appliances[i].Brand + "', " +
                                          "[Description] = '" + data[0].Appliances[i].Appliances + "', " +
                                          "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                          "where NAID = '" + dt.Rows[0]["NAID"].ToString() + "'";
                        }
                    }
                    results = db.AUIDB_WithParam(Update) + " Updated";


                    string file_upload = $@"
                                            UPDATE [dbo].[tbl_fileupload_Model]
                                               SET [FileName] = '" + data[0].ProfileName + "', " +
                                                  "[FilePath] = '" + data[0].ProfileFilePath + "', " +
                                                  "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ," +
                                                  "[Type] = '1'" +
                                              "where MemId = '" + data[0].MemId + "' and Type = '1' ";
                    db.AUIDB_WithParam(file_upload);


                    string co_profile = $@"
                                            UPDATE [dbo].[tbl_CoMakerFileUpload_Model]
                                               SET [FileName] = '" + data[0].Co_ProfileName + "', " +
                                               "[FilePath] = '" + data[0].Co_ProfileFilePath + "', " +
                                               "[DateCreated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                            "where CMID='" + dt.Rows[0]["CMID"].ToString() + "' and Status = '1'";
                    db.AUIDB_WithParam(co_profile);

                    string co_signature_upload = "";
                    if (data[0].Co_SignatureUpload.Count != 0)
                    {
                        for (int x = 0; x < data[0].Co_SignatureUpload.Count; x++)
                        {
                            co_signature_upload = $@"
                                            UPDATE [dbo].[tbl_CoMakerFileUpload_Model]
                                               SET [FileName] = '" + data[0].Co_SignatureUpload[x].FileName + "', " +
                                               "[FilePath] = '" + data[0].Co_SignatureUpload[x].FilePath + "', " +
                                               "[DateCreated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                           "where CMID='" + dt.Rows[0]["CMID"].ToString() + "' and Status='3'";
                            db.AUIDB_WithParam(co_signature_upload);

                        }

                    }

                    string co_uploadfile = "";
                    if (data[0].Co_RequirementsFile.Count != 0)
                    {
                        for (int x = 0; x < data[0].Co_RequirementsFile.Count; x++)
                        {

                            co_uploadfile = $@"
                                            UPDATE [dbo].[tbl_CoMakerFileUpload_Model]
                                               SET [FileName] = '" + data[0].Co_RequirementsFile[x].FileName + "', " +
                                             "[FilePath] = '" + data[0].Co_RequirementsFile[x].FilePath + "', " +
                                             "[DateCreated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                        "where CMID='" + dt.Rows[0]["CMID"].ToString() + "' and Status='2'";
                            db.AUIDB_WithParam(co_uploadfile);
                        }

                    }

                    string uploadfile_req = "";
                    if (data[0].RequirementsFile.Count != 0)
                    {
                        for (int x = 0; x < data[0].RequirementsFile.Count; x++)
                        {

                            uploadfile_req = $@"
                                            UPDATE [dbo].[tbl_fileupload_Model]
                                               SET [FileName] = '" + data[0].RequirementsFile[x].FileName + "', " +
                                                "[FilePath] = '" + data[0].RequirementsFile[x].FilePath + "', " +
                                                "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                            "where MemId = '" + data[0].MemId + "' and Type = '2' ";
                            db.AUIDB_WithParam(uploadfile_req);

                        }


                    }

                    string signature_upload = "";
                    if (data[0].SignatureUpload.Count != 0)
                    {
                        for (int x = 0; x < data[0].SignatureUpload.Count; x++)
                        {

                            uploadfile_req = $@"
                                            UPDATE [dbo].[tbl_fileupload_Model]
                                               SET [FileName] = '" + data[0].SignatureUpload[x].FileName + "', " +
                                           "[FilePath] = '" + data[0].SignatureUpload[x].FilePath + "', " +
                                           "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                       "where MemId = '" + data[0].MemId + "' and Type = '3' ";
                            db.AUIDB_WithParam(uploadfile_req);
                        }


                    }
                    //////////--------------------------------

                    return Ok(results);

                }
                else
                {

                    results = "Error! 0 Records Found Please Check Member ID if existing!--------->";
                    string filePath_ = @"C:\data\Update_MemberInfo_ErrorLogs.json"; // Replace with your desired file path

                    System.IO.File.WriteAllText(filePath_, JsonSerializer.Serialize(results));
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
        public async Task<IActionResult> PostMemberSearching(List<FilterModel> data)
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
                sql = $@"SELECT        tbl_Member_Model.Id, tbl_Member_Model.Fname, tbl_Member_Model.Lname, tbl_Member_Model.Mname, tbl_Member_Model.Suffix, tbl_Member_Model.Age, tbl_Member_Model.Barangay, tbl_Member_Model.City, 
                         tbl_Member_Model.Civil_Status, tbl_Member_Model.Cno, tbl_Member_Model.Country, tbl_Member_Model.DOB, tbl_Member_Model.EmailAddress, tbl_Member_Model.Gender, tbl_Member_Model.HouseNo, 
                         tbl_Member_Model.POB, tbl_Member_Model.Province, tbl_Member_Model.YearsStay, tbl_Member_Model.ZipCode, tbl_HouseStatus_Model.Id AS HouseStatus_Id, tbl_HouseStatus_Model.Description AS HouseStatus, 
                         tbl_Status_Model.Name AS status, tbl_Member_Model.DateCreated, tbl_Member_Model.DateUpdated, tbl_JobInfo_Model.CompanyAddress, tbl_JobInfo_Model.CompanyName, tbl_JobInfo_Model.JobDescription,file_.FilePath as ProfilePath
                        FROM            tbl_Member_Model INNER JOIN
                                                 tbl_HouseStatus_Model ON tbl_Member_Model.House_Stats = tbl_HouseStatus_Model.Id INNER JOIN
                                                 tbl_Status_Model ON tbl_Member_Model.Status = tbl_Status_Model.Id INNER JOIN
                                                 tbl_JobInfo_Model ON tbl_Member_Model.MemId = tbl_JobInfo_Model.MemId left join
						                        (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId 
                        WHERE        (tbl_Member_Model.Status = 1) " + w_column + "";

                var result = new List<MemberVM>();
                DataTable table = db.SelectDb(sql).Tables[0];

                foreach (DataRow dr in table.Rows)
                {
                    var item = new MemberVM();
                    var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                    item.Id = int.Parse(dr["id"].ToString());
                    item.Fname = dr["Fname"].ToString();
                    item.Lname = dr["Lname"].ToString();
                    item.Mname = dr["Mname"].ToString();
                    item.Suffix = dr["Suffix"].ToString();
                    item.Age = dr["Age"].ToString();
                    item.Barangay = dr["Barangay"].ToString();
                    item.City = dr["City"].ToString();
                    item.Civil_Status = dr["Civil_Status"].ToString();
                    item.Country = dr["Country"].ToString();
                    item.DOB = dr["DOB"].ToString();
                    item.EmailAddress = dr["EmailAddress"].ToString();
                    item.Gender = dr["Gender"].ToString();
                    item.HouseNo = dr["HouseNo"].ToString();
                    item.House_Stats = dr["HouseStatus"].ToString();
                    item.HouseStatus_Id = dr["HouseStatus_Id"].ToString();
                    item.CompanyAddress = dr["CompanyAddress"].ToString();
                    item.CompanyName = dr["CompanyName"].ToString();
                    item.JobDescription = dr["JobDescription"].ToString();
                    item.YearsStay = dr["YearsStay"].ToString();
                    item.Province = dr["Province"].ToString();
                    item.ZipCode = dr["ZipCode"].ToString();
                    item.Status = dr["status"].ToString();
                    item.POB = dr["POB"].ToString();
                    item.Cno = dr["Cno"].ToString();
                    item.DateCreated = datec;
                    item.DateUpdated = dateu;
                    item.ProfilePath = dr["ProfilePath"].ToString();
                    result.Add(item);
                }

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("Invalid Column Assign Please assign Table name + Column name");
            }
        }
        [HttpPost]
        public IActionResult DeleteMember(DeleteModel data)
        {
            try
            {
                string filePath = @"C:\data\deletemember.json"; // Replace with your desired file path



                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
                sql = $@"select * from tbl_Member_Model where MemId ='" + data.MemId + "'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var result = new DeleteModel();
                if (dt.Rows.Count != 0)
                {

                    string deletemember = $@"update tbl_Member_Model set Status='2' " +
                                    "where MemId='" + data.MemId + "'";
                    db.AUIDB_WithParam(deletemember);

                    string deletecomaker = $@"update tbl_CoMaker_Model set Status='2' " +
                                   "where MemId='" + data.MemId + "'";
                    db.AUIDB_WithParam(deletecomaker);
                    results = "Successfully Deleted";
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
        #region MonthlyBills
        public class MonthlyBillsVM
        {
            public int Id { get; set; }
            public string? MemId { get; set; }
            public decimal? ElectricBill { get; set; }
            public decimal? WaterBill { get; set; }
            public decimal? OtherBills { get; set; }
            public decimal? DailyExpenses { get; set; }
            public string? Status { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }

        }
        public class ReturnValue
        {
            public string? NAID { get; set; }
            public string? Status { get; set; }
            public string? Result { get; set; }

        }
        public class FileUploaded
        {
            public int Id { get; set; }
            public string? MemId { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public string? Status { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> GetMonthlyBillsList()
        {

            string sql = $@"SELECT        tbl_Status_Model.Name AS Status, tbl_MonthlyBills_Model.DateCreated, tbl_MonthlyBills_Model.DateUpdated, tbl_MonthlyBills_Model.DailyExpenses, tbl_MonthlyBills_Model.OtherBills, tbl_MonthlyBills_Model.WaterBill, 
                         tbl_MonthlyBills_Model.ElectricBill, tbl_MonthlyBills_Model.MemId , tbl_MonthlyBills_Model.Id
                         FROM            tbl_Status_Model INNER JOIN
                         tbl_MonthlyBills_Model ON tbl_Status_Model.Id = tbl_MonthlyBills_Model.Status INNER JOIN
                         tbl_Member_Model ON tbl_MonthlyBills_Model.MemId = tbl_Member_Model.MemId
                         WHERE     (tbl_Member_Model.Status <> 2)";
            var result = new List<MonthlyBillsVM>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new MonthlyBillsVM();
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item.Id = int.Parse(dr["Id"].ToString());
                item.MemId = dr["MemId"].ToString();
                item.ElectricBill = Convert.ToDecimal(dr["ElectricBill"].ToString());
                item.WaterBill = Convert.ToDecimal(dr["WaterBill"].ToString());
                item.OtherBills = Convert.ToDecimal(dr["OtherBills"].ToString());
                item.DailyExpenses = Convert.ToDecimal(dr["DailyExpenses"].ToString());

                item.Status = dr["Status"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                result.Add(item);
            }

            return Ok(result);
        }


        #endregion

        #region UploadFiles
        [HttpGet]
        public async Task<IActionResult> GetPostUploadedFiles()
        {


            try
            {
                sql = $@"SELECT        tbl_Member_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_fileupload_Model.DateCreated, tbl_fileupload_Model.DateUpdated, tbl_Status_Model.Name AS Status
                        FROM            tbl_Member_Model INNER JOIN
                         tbl_fileupload_Model ON tbl_Member_Model.MemId = tbl_fileupload_Model.MemId INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                         WHERE     (tbl_Member_Model.Status <> 2) ";
                var result = new List<FileUploaded>();
                DataTable table = db.SelectDb(sql).Tables[0];

                foreach (DataRow dr in table.Rows)
                {
                    var item = new FileUploaded();
                    item.MemId = dr["MemId"].ToString();
                    item.FileName = dr["FileName"].ToString();
                    item.FilePath = dr["FilePath"].ToString();
                    item.Status = dr["Status"].ToString();
                    item.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    item.DateUpdated = Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
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
        public async Task<IActionResult> PostUploadedFiles(MemberIdModel data)
        {

            try
            {
                sql = $@"SELECT        tbl_Member_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_fileupload_Model.DateCreated, tbl_fileupload_Model.DateUpdated, tbl_Status_Model.Name AS Status
                        FROM            tbl_Member_Model INNER JOIN
                         tbl_fileupload_Model ON tbl_Member_Model.MemId = tbl_fileupload_Model.MemId INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                         WHERE     (tbl_Member_Model.Status <> 2) and tbl_fileupload_Model.MemId ='" + data.MemId + "'";
                var result = new List<FileUploaded>();
                DataTable table = db.SelectDb(sql).Tables[0];

                foreach (DataRow dr in table.Rows)
                {
                    var item = new FileUploaded();
                    item.MemId = dr["MemId"].ToString();
                    item.FileName = dr["FileName"].ToString();
                    item.FilePath = dr["FilePath"].ToString();
                    item.Status = dr["MemId"].ToString();
                    item.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    item.DateUpdated = Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
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
        public string UploadProfileImage([FromForm] IFormFile file, [FromForm] string memid)
        {
            try
            {
                var filePath = DBConn.Path;
                string FileName = file.FileName;
                string getextension = Path.GetExtension(FileName);
                string files = Path.Combine(filePath, FileName);
                var imagePath = Path.Combine(filePath, files);
                using (FileStream streams = new FileStream(Path.Combine(filePath, files), FileMode.Create))
                {
                    file.CopyTo(streams);
                }

                //string query = $@"update  UsersModel set FilePath='" + filePath + "' ";

                // return db.AUIDB_WithParam(query)+ " Added";

                string Insert = $@"INSERT INTO [dbo].[tbl_fileupload_Model]
                               ([MemId]
                               ,[FileName]
                               ,[FilePath]
                               ,[Status]
                               ,[DateCreated]
                               ,[Type])
                                VALUES
                               ('" + memid + "', " +
                              "'" + FileName + "'," +
                              "'" + imagePath + "'," +
                              "'1'," +
                              "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                              "'1') ";

                return db.AUIDB_WithParam(Insert) + " Added";


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion


        [HttpPost]
        public IActionResult SaveAll(List<SaveMemberModel> data)
        {
            string memid = "";
            string results = "";
            string Insert = "";
            string Update = "";
            string naid = "";
            string promtresult = "";
            string promtresult_status = "";
            try
            {
                var result = new ReturnValue();
                var saving = new Savingmodule();
                string filePath = @"C:\data\data.json"; // Replace with your desired file path

                dbmet.insertlgos(filePath, JsonSerializer.Serialize(data[0]));

                string areafilter = $@"SELECT [Id]
                                  ,[Area]
                                  ,[City]
                                   FROM [dbo].[tbl_Area_Model]
                                    Where City like '%" + data[0].Barangay + ", " + data[0].City + "%'";
                DataTable area_table = db.SelectDb(areafilter).Tables[0];
                var val_city = data[0].Barangay.ToLower() + ", " + data[0].City.ToLower();
                //var val = area_table.Rows[0]["City"].ToString().ToLower().Split("|")[0];
                bool area_city = true;
                if (area_table.Rows.Count != 0)
                {
                    area_city = area_table.Rows[0]["City"].ToString().ToLower().Split("|")[0].Equals(val_city);
                }
                else
                {
                    area_city = false;
                }

                if (area_table.Rows.Count == 0 && !area_city)
                {
                    string insert_area = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                               ([Status],[City])
                                                VALUES
                                                ('1', " +
                                        "'" + data[0].Barangay + ", " + data[0].City + "')";
                    saving.tbl_Area_Model = db.AUIDB_WithParam(insert_area);
                }

                string applicationfilter = $@"select *
                                from
                                tbl_Application_Model inner join
                                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID inner join
                                tbl_Member_Model on tbl_Member_Model.MemId = tbl_Application_Model.MemId inner join
                                tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Member_Model.MemId inner join
                                tbl_FamBackground_Model on tbl_FamBackground_Model.MemId = tbl_Member_Model.MemId inner join
                                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId
                                where  tbl_Application_Model.Status in (8,9,10)
                                and tbl_Member_Model.Fname ='" + data[0].Fname + "' and tbl_Member_Model.Mname ='" + data[0].Mname + "'" +
                                "and  tbl_Member_Model.Lname ='" + data[0].Lname + "' and tbl_Member_Model.POB ='" + data[0].POB + "'" +
                                "and tbl_Member_Model.Barangay = '" + data[0].Barangay + "' and tbl_Member_Model.Status = 1 ";
                DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];
                if (tbl_applicationfilter.Rows.Count != 0)
                {

                    //saving.NAID = naid;
                    saving.promtresult = "Member Already Exist or Has a Pending Loans";
                    saving.promtresult_status = "ERROR";
                    //result.Result = "Member Already Exist or Has a Pending Loans";
                    //result.Status = "ERROR";
                    return Ok(saving);
                }
                else
                {



                    string comaker_filter = $@"select * from
                                            tbl_CoMaker_Model
                                            where Fname='" + data[0].Co_Fname + "'" +
                                            "and Mname='" + data[0].Co_Mname + "' " +
                                            "and Lnam='" + data[0].Co_Lname + "' and Status =1 ";
                    DataTable tbl_comaker = db.SelectDb(comaker_filter).Tables[0];
                    if (tbl_comaker.Rows.Count != 0)
                    {


                        //saving.NAID = naid;
                        saving.promtresult = "Co-Maker is Already Used!";
                        saving.promtresult_status = "ERROR";
                        //result.Result = "Member Already Exist or Has a Pending Loans";
                        //result.Status = "ERROR";
                        return Ok(saving);
                    }
                    else
                    {
                        var validate_exist = dbmet.GetMemberList().Where(a => a.Fname == data[0].Fname && a.Lname ==
                        data[0].Lname && a.POB == data[0].POB && Convert.ToDateTime(a.DOB).ToString("yyyy-MM-dd") ==
                        Convert.ToDateTime(data[0].DOB).ToString("yyyy-MM-dd") && a.Barangay == data[0].Barangay).FirstOrDefault();
                        if (validate_exist != null)
                        {
                            string loan_sql = $@"select * from
                                         tbl_LoanHistory_Model
                                        where tbl_LoanHistory_Model.OutstandingBalance <> 0 and  tbl_LoanHistory_Model.MemId = ' " + validate_exist.MemId + "'";
                            DataTable tbl_loan_result = db.SelectDb(loan_sql).Tables[0];
                            if (tbl_loan_result.Rows.Count != 0)
                            {
                                promtresult = "This Member has exisitng loan";
                                promtresult_status = "WARNING";
                            }
                            else
                            {
                                promtresult = "Successfully Added";
                                promtresult_status = "SUCCESS";
                            }

                            if (area_table.Rows.Count == 0)
                            {
                                string insert_area = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                       ([Status],[City])
                                        VALUES
                                        ('1', " +
                                                    "'" + data[0].Barangay + ", " + data[0].City + "')";
                                saving.tbl_Area_Model = db.AUIDB_WithParam(insert_area);
                            }

                            string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                            DataTable username_tbl = db.SelectDb(username).Tables[0];
                            foreach (DataRow dr in username_tbl.Rows)
                            {
                                string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                                dbmet.InsertNotification("Update Borrower  " + validate_exist.MemId + " from Application Creation",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Application Module", name, dr["UserId"].ToString(), "2", validate_exist.MemId);
                            }
                            string member = $@"update tbl_Member_Model set 
                                       Fname='" + data[0].Fname + "', " +
                                               "Lname='" + data[0].Lname + "', " +
                                               "Mname='" + data[0].Mname + "', " +
                                               "Suffix='" + data[0].Suffix + "', " +
                                               "Age='" + data[0].Age + "', " +
                                               "Barangay='" + data[0].Barangay + "', " +
                                               "City='" + data[0].City + "', " +
                                               "Civil_Status='" + data[0].Civil_Status + "', " +
                                               "Cno='" + data[0].Cno + "', " +
                                               "Country='" + data[0].Country + "', " +
                                               "DOB='" + Convert.ToDateTime(data[0].DOB).ToString("yyyy-MM-dd") + "', " +
                                               "EmailAddress='" + data[0].EmailAddress + "', " +
                                               "Gender='" + data[0].Gender + "', " +
                                               "HouseNo='" + data[0].HouseNo + "', " +
                                               "House_Stats='" + data[0].House_Stats + "', " +
                                               "POB='" + data[0].POB + "', " +
                                               "Province='" + data[0].Province + "', " +
                                               "YearsStay='" + data[0].YearsStay + "', " +
                                               "ZipCode='" + data[0].ZipCode + "', " +
                                               "Status='" + data[0].Status + "', " +
                                               "DateUpdated='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                               "where MemId='" + validate_exist.MemId + "'";
                            saving.tbl_Member_Model = db.AUIDB_WithParam(member);
                            //Job
                            string jobinfo = $@"UPDATE [dbo].[tbl_JobInfo_Model]
                               SET 
                                  [JobDescription] = '" + data[0].JobDescription + "', " +
                                          "[YOS] = '" + data[0].YOS + "', " +
                                          "[CompanyName] = '" + data[0].CompanyName + "', " +
                                          "[MonthlySalary] ='" + data[0].MonthlySalary + "', " +
                                          "[OtherSOC] ='" + data[0].OtherSOC + "', " +
                                          "[Status] = '" + data[0].Status + "', " +
                                          "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                          "[BO_Status] ='" + data[0].BO_Status + "', " +
                                          "[CompanyAddress] ='" + data[0].CompanyAddress + "', " +
                                          "[Emp_Status] ='" + data[0].Emp_Status + "' " +
                                       "where MemId='" + validate_exist.MemId + "'";
                            saving.tbl_JobInfo_Model = db.AUIDB_WithParam(jobinfo);
                            //FAM
                            string famback = $@"
                                UPDATE [dbo].[tbl_FamBackground_Model]
                                   SET 
                                       [Fname] = '" + data[0].F_Fname + "', " +
                                              "[Mname] ='" + data[0].F_Mname + "', " +
                                              "[Lname] = '" + data[0].F_Lname + "', " +
                                              "[Suffix] = '" + data[0].F_Suffix + "', " +
                                              "[DOB] ='" + Convert.ToDateTime(data[0].F_DOB).ToString("yyyy-MM-dd") + "', " +
                                              "[Age] ='" + data[0].F_Age + "', " +
                                              "[Emp_Status] = '" + data[0].F_Emp_Status + "', " +
                                              "[Position] = '" + data[0].F_Job + "', " +
                                              "[YOS] ='" + data[0].F_YOS + "', " +
                                              "[CmpId] = '" + data[0].F_CompanyName + "', " +
                                              "[NOD] = '" + data[0].F_NOD + "', " +
                                              "[RTTB] ='" + data[0].F_RTTB + "', " +
                                              "[MemId] ='" + validate_exist.MemId + "', " +
                                              "[Status] = '1', " +
                                              "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                               "where MemId='" + validate_exist.MemId + "'";

                            saving.tbl_FamBackground_Model = db.AUIDB_WithParam(famback);

                            //child
                            if (data[0].Child.Count != 0)
                            {

                                string child_val = $@"SELECT * FROM [dbo].[tbl_ChildInfo_Model]  where [FamId] = '" + validate_exist.FamId + "'";
                                DataTable child_val_tbl = db.SelectDb(child_val).Tables[0];
                                if (child_val_tbl.Rows.Count != 0)
                                {
                                    string Delete = $@"DELETE FROM [dbo].[tbl_ChildInfo_Model]  where [FamId] = '" + validate_exist.FamId + "'";
                                    db.AUIDB_WithParam(Delete);
                                }


                                for (int i = 0; i < data[0].Child.Count; i++)
                                {

                                    string childinfo = $@"INSERT INTO [dbo].[tbl_ChildInfo_Model]
                                   ([Fname]
                                   ,[Mname]
                                   ,[Lname]
                                   ,[Age]
                                   ,[NOS]
                                   ,[FamId]
                                   ,[Status]
                                   ,[DateCreated])
                             VALUES
                                   ('" + data[0].Child[i].Fname + "'," +
                                               "'" + data[0].Child[i].Mname + "'," +
                                               "'" + data[0].Child[i].Lname + "'," +
                                               "'" + data[0].Child[i].Age + "'," +
                                              "'" + data[0].Child[i].NOS + "'," +
                                              "'" + validate_exist.FamId + "'," +
                                             "'1'," +
                                                 "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                                    saving.tbl_ChildInfo_Model = db.AUIDB_WithParam(childinfo);
                                }

                            }
                            //business
                            if (data[0].Business.Count != 0)
                            {

                                string businessval = $@"SELECT * FROM [dbo].[tbl_BusinessInformation_Model]  where MemId='" + validate_exist.MemId + "'";
                                DataTable businessval_tbl = db.SelectDb(businessval).Tables[0];
                                if (businessval_tbl.Rows.Count != 0)
                                {

                                    string Delete = $@"DELETE FROM [dbo].[tbl_BusinessInformation_Model]  where MemId='" + validate_exist.MemId + "'";
                                    db.AUIDB_WithParam(Delete);
                                }

                                string b_files = "";
                                for (int i = 0; i < data[0].Business.Count; i++)
                                {


                                    if (data[0].Business[0].BusinessFiles.Count != 0)
                                    {

                                        for (int x = 0; x < data[0].Business[0].BusinessFiles.Count; x++)
                                        {
                                            b_files += data[0].Business[0].BusinessFiles[x].FilePath + "|";
                                        }

                                    }
                                    string result_ = b_files.Substring(0, b_files.Length - 1);
                                    string b_insert = $@"insert into   tbl_BusinessInformation_Model
                                        ([BusinessName]
                                       ,[BusinessType]
                                       ,[BusinessAddress]
                                       ,[B_status]
                                       ,[YOB]
                                       ,[NOE]
                                       ,[Salary]
                                       ,[VOS]
                                       ,[AOS]
                                       ,[Status]
                                       ,[MemId]
                                       ,[FilesUploaded]
                                       ,[DateCreated])
                                            values
                                            ('" + data[0].Business[i].BusinessName + "'," +
                                                          "'" + data[0].Business[i].BusinessType + "'," +
                                                          "'" + data[0].Business[i].BusinessAddress + "'," +
                                                          "'" + data[0].Business[i].B_status + "'," +
                                                          "'" + data[0].Business[i].YOB + "'," +
                                                          "'" + data[0].Business[i].NOE + "'," +
                                                          "'" + data[0].Business[i].Salary + "'," +
                                                          "'" + data[0].Business[i].VOS + "'," +
                                                          "'" + data[0].Business[i].AOS + "'," +
                                                          "'1'," +
                                                          "'" + validate_exist.MemId + "'," +
                                                          "'" + data[0].Business[0].BusinessFiles[i].FilePath + "'," +
                                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                                    saving.tbl_BusinessInformation_Model = db.AUIDB_WithParam(b_insert);
                                }
                            }

                            //comaker
                            string comaker = $@"UPDATE [dbo].[tbl_CoMaker_Model]
                           SET [Fname] ='" + data[0].Co_Fname + "', " +
                                      "[Mname] = '" + data[0].Co_Mname + "', " +
                                      "[Lnam] = '" + data[0].Co_Lname + "', " +
                                      "[Suffi] = '" + data[0].Co_Suffix + "', " +
                                      "[Gender] = '" + data[0].Co_Gender + "', " +
                                      "[DOB] = '" + Convert.ToDateTime(data[0].Co_DOB).ToString("yyyy-MM-dd") + "', " +
                                      "[Age] = '" + data[0].Co_Age + "', " +
                                      "[POB] = '" + data[0].Co_POB + "', " +
                                      "[CivilStatus] = '" + data[0].Co_Civil_Status + "', " +
                                      "[Cno] = '" + data[0].Co_Cno + "', " +
                                      "[EmailAddress] = '" + data[0].Co_EmailAddress + "', " +
                                      "[House_Stats] = '" + data[0].Co_House_Stats + "', " +
                                      "[HouseNo] = '" + data[0].Co_HouseNo + "', " +
                                      "[Barangay] = '" + data[0].Co_Barangay + "', " +
                                      "[City] = '" + data[0].Co_City + "', " +
                                      "[Region] = '" + data[0].Co_Province + "', " +
                                      "[Country] = '" + data[0].Co_Country + "', " +
                                      "[ZipCode] = '" + data[0].Co_ZipCode + "', " +
                                      "[YearsStay] = '" + data[0].Co_YearsStay + "', " +
                                      "[RTTB] = '" + data[0].Co_RTTB + "', " +
                                      "[Status] = '1', " +
                                      "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                        "where MemId='" + validate_exist.MemId + "'";
                            saving.tbl_CoMaker_Model = db.AUIDB_WithParam(comaker);
                            //co_job
                            string comaker_jobinfo = $@"UPDATE [dbo].[tbl_CoMaker_JobInfo_Model]
                               SET [JobDescription] = '" + data[0].Co_JobDescription + "', " +
                                          "[YOS] = '" + data[0].Co_YOS + "', " +
                                          "[CompanyName] = '" + data[0].Co_CompanyName + "', " +
                                          "[CompanyAddress] = '" + data[0].Co_CompanyAddress + "', " +
                                          "[MonthlySalary] = '" + data[0].Co_MonthlySalary + "', " +
                                          "[OtherSOC] = '" + data[0].Co_OtherSOC + "', " +
                                          "[Status] ='1', " +
                                          "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                          "[BO_Status] = '" + data[0].Co_BO_Status + "', " +
                                          "[Emp_Status] = '" + data[0].Co_Emp_Status + "'" +
                                          "where CMID='" + validate_exist.CMID + "'";
                            saving.tbl_CoMaker_JobInfo_Model = db.AUIDB_WithParam(comaker_jobinfo);


                            if (data[0].Assets.Count != 0)
                            {


                                for (int i = 0; i < data[0].Assets.Count; i++)
                                {


                                    string assets = $@"UPDATE [dbo].[tbl_AssetsProperties_Model]
                                       SET [MotorVehicles] =  '" + data[0].Assets[i].MotorVehicles + "', " +
                                                 "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                                 "[Status] = '1' " +
                                             "where MemId='" + validate_exist.MemId + "'";
                                    saving.tbl_AssetsProperties_Model = db.AUIDB_WithParam(assets);
                                }

                            }
                            if (data[0].Property.Count != 0)
                            {


                                for (int i = 0; i < data[0].Property.Count; i++)
                                {

                                    string property = $@"UPDATE [dbo].[tbl_Property_Model]
                               SET [Property] = '" + data[0].Property[i].Property + "'," +
                                          "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                            "[Status] = '1' " +
                                           "where MemId='" + validate_exist.MemId + "'";
                                    saving.tbl_Property_Model = db.AUIDB_WithParam(property);
                                }

                            }
                            if (data[0].Bank.Count != 0)
                            {


                                for (int i = 0; i < data[0].Bank.Count; i++)
                                {

                                    string bank = $@"UPDATE [dbo].[tbl_BankAccounts_Model]
                                       SET [BankName] = '" + data[0].Bank[i].BankName + "', " +
                                                   "[Address] = '" + data[0].Bank[i].Address + "', " +
                                                   "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                                "where MemId='" + validate_exist.MemId + "'";
                                    saving.tbl_BankAccounts_Model = db.AUIDB_WithParam(bank);
                                }
                            }

                            //results = db.AUIDB_WithParam(Update) + " Updated";



                            string createdby = data[0].UserId;
                            string submittedby = "";
                            string datesubmitted = "";
                            if (data[0].ApplicationStatus == 7)
                            {

                                submittedby = "";
                                datesubmitted = "";

                                string Update_memstats = $@"update tbl_Member_Model set 
                                  Status='2'" +
                                          "where MemId='" + validate_exist.MemId + "'";
                                db.AUIDB_WithParam(Update_memstats);
                            }
                            else
                            {

                                submittedby = data[0].UserId;
                                datesubmitted = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                            }


                            string application = $@"insert into   tbl_Application_Model
                               ([MemId]
                               ,[Remarks]
                               ,[Status]
                               ,[CreatedBy]
                               ,[SubmittedBy]
                               ,[DateSubmitted]
                               ,[DateCreated])
                                values
                                ('" + validate_exist.MemId + "'," +
                                            "'" + data[0].Remarks + "'," +
                                            "'" + data[0].ApplicationStatus + "'," +
                                            "'" + createdby + "'," +
                                            "'" + submittedby + "'," +
                                            "'" + datesubmitted + "'," +
                                            "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                            saving.tbl_Application_Model = db.AUIDB_WithParam(application);
                            sql = $@"select   NAID from tbl_Application_Model order by id desc";

                            DataTable table = db.SelectDb(sql).Tables[0];
                            string NAID = table.Rows[0]["NAID"].ToString();
                            string loandetails = $@"insert into   tbl_LoanDetails_Model
                                ([LoanAmount]
                               ,[TermsOfPayment]
                               ,[Purpose]
                               ,[MemId]
                               ,[Status]
                               ,[LoanTypeID]
                               ,[NAID]
                               ,[DateCreated])
                                values
                                ('" + data[0].LoanAmount + "'," +
                                     "'" + data[0].TermsOfPayment + "'," +
                                     "'" + data[0].Purpose + "'," +
                                     "'" + validate_exist.MemId + "'," +
                                     "'1'," +
                                     "'" + data[0].LoanTypeId + "'," +
                                     "'" + NAID + "'," +
                                      "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            saving.tbl_LoanDetails_Model = db.AUIDB_WithParam(loandetails);


                            if (data[0].Appliances.Count != 0)
                            {
                                sql = $@"select   NAID from tbl_Application_Model order by id desc";

                                DataTable na_table = db.SelectDb(sql).Tables[0];
                                naid = na_table.Rows[0]["NAID"].ToString();

                                string na_insert = "";
                                for (int i = 0; i < data[0].Appliances.Count; i++)
                                {

                                    string appliance = $@"INSERT INTO[dbo].[tbl_Appliance_Model]
                               ([Brand]
                               ,[Description]
                               ,[NAID]
                               ,[Status]
                               ,[DateCreated])
                                VALUES
                               ('" + data[0].Appliances[i].Brand + "'," +
                                                  "'" + data[0].Appliances[i].Appliances + "'," +
                                                  "'" + naid + "'," +
                                                  "'1'," +
                                                  "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_Appliance_Model = db.AUIDB_WithParam(appliance);
                                }

                            }


                            saving.NAID = naid;
                            saving.promtresult = promtresult;
                            saving.promtresult_status = promtresult_status;
                            return Ok(saving);
                        }
                        else
                        {




                            //member
                            string Insert_MEM = $@"insert into   tbl_Member_Model (Fname, Lname, Mname, Suffix, Age, Barangay, City, Civil_Status, Cno, Country, DOB, EmailAddress, Gender, HouseNo, House_Stats, POB, Province, YearsStay, ZipCode,DateCreated, Status) 
                                values
                                ('" + data[0].Fname + "', " +
                                       "'" + data[0].Lname + "'," +
                                       "'" + data[0].Mname + "', " +
                                       "'" + data[0].Suffix + "', " +
                                       "'" + data[0].Age + "', " +
                                       "'" + data[0].Barangay + "', " +
                                       "'" + data[0].City + "', " +
                                       "'" + data[0].Civil_Status + "', " +
                                       "'" + data[0].Cno + "', " +
                                       "'" + data[0].Country + "', " +
                                       "'" + Convert.ToDateTime(data[0].DOB).ToString("yyyy-MM-dd") + "', " +
                                       "'" + data[0].EmailAddress + "', " +
                                       "'" + data[0].Gender + "', " +
                                       "'" + data[0].HouseNo + "', " +
                                       "'" + data[0].House_Stats + "', " +
                                       "'" + data[0].POB + "', " +
                                       "'" + data[0].Province + "', " +
                                       "'" + data[0].YearsStay + "', " +
                                       "'" + data[0].ZipCode + "', " +
                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                       "'" + data[0].Status + "') ";
                            saving.tbl_Member_Model = db.AUIDB_WithParam(Insert_MEM);
                            sql = $@"select   MemId from tbl_Member_Model order by id desc";

                            DataTable table = db.SelectDb(sql).Tables[0];
                            memid = table.Rows[0]["MemId"].ToString();
                            //monthlybills
                            string loan_sql = $@"select * from
                                         tbl_LoanHistory_Model
                                        where tbl_LoanHistory_Model.OutstandingBalance <> 0 and  tbl_LoanHistory_Model.MemId = ' " + memid + "'";
                            DataTable tbl_loan_result = db.SelectDb(loan_sql).Tables[0];
                            if (tbl_loan_result.Rows.Count != 0)
                            {
                                promtresult = "This Member has exisitng loan";
                                promtresult_status = "WARNING";
                            }
                            else
                            {
                                promtresult = "Successfully Added";
                                promtresult_status = "SUCCESS";
                            }
                            string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                            DataTable username_tbl = db.SelectDb(username).Tables[0];
                            foreach (DataRow dr in username_tbl.Rows)
                            {
                                string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                                dbmet.InsertNotification("Inserted New Borrower  " + memid + " from Application Creation",
                                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Application Module", name, dr["UserId"].ToString(), "2", memid);
                            }
                            string monthly = $@"INSERT INTO 
                                        [dbo].[tbl_MonthlyBills_Model]
                                       ([MemId]
                                       ,[ElectricBill]
                                       ,[WaterBill]
                                       ,[OtherBills]
                                       ,[DailyExpenses]
                                       ,[Status]
                                       ,[DateCreated])
                                VALUES
                               ('" + memid + "', " +
                                        "'" + data[0].ElectricBill + "'," +
                                        "'" + data[0].WaterBill + "'," +
                                        "'" + data[0].OtherBills + "'," +
                                        "'" + data[0].DailyExpenses + "'," +
                                        "'1'," +
                                        "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            saving.tbl_MonthlyBills_Model = db.AUIDB_WithParam(monthly);

                            //jobinfo
                            string jobinfo = $@"insert into   tbl_JobInfo_Model (
                                [JobDescription]
                               ,[YOS]
                               ,[CompanyName]
                               ,[CompanyAddress]
                               ,[MonthlySalary]
                               ,[OtherSOC]
                               ,[Status]
                               ,[BO_Status]
                               ,[Emp_Status]
                               ,[MemId]
                               ,[DateCreated]) 
                                values
                                ('" + data[0].JobDescription + "'," +
                                         "'" + data[0].YOS + "'," +
                                         "'" + data[0].CompanyName + "'," +
                                         "'" + data[0].CompanyAddress + "'," +
                                         "'" + data[0].MonthlySalary + "'," +
                                         "'" + data[0].OtherSOC + "'," +
                                         "'" + data[0].Status + "'," +
                                         "'" + data[0].BO_Status + "'," +
                                         "'" + data[0].Emp_Status + "'," +
                                         "'" + memid + "'," +
                                          "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            saving.tbl_JobInfo_Model = db.AUIDB_WithParam(jobinfo);

                            if (data[0].Business.Count != 0)
                            {
                                string b_files = "";
                                for (int i = 0; i < data[0].Business.Count; i++)
                                {


                                    if (data[0].Business[0].BusinessFiles.Count != 0)
                                    {

                                        for (int x = 0; x < data[0].Business[0].BusinessFiles.Count; x++)
                                        {
                                            b_files += data[0].Business[0].BusinessFiles[x].FilePath + "|";
                                        }

                                    }
                                    string _result = b_files.Substring(0, b_files.Length - 1);
                                    string b_insert = $@"insert into   tbl_BusinessInformation_Model
                            ([BusinessName]
                           ,[BusinessType]
                           ,[BusinessAddress]
                           ,[B_status]
                           ,[YOB]
                           ,[NOE]
                           ,[Salary]
                           ,[VOS]
                           ,[AOS]
                           ,[Status]
                           ,[MemId]
                           ,[FilesUploaded]
                           ,[DateCreated])
                                values
                                ('" + data[0].Business[i].BusinessName + "'," +
                                              "'" + data[0].Business[i].BusinessType + "'," +
                                              "'" + data[0].Business[i].BusinessAddress + "'," +
                                              "'" + data[0].Business[i].B_status + "'," +
                                              "'" + data[0].Business[i].YOB + "'," +
                                              "'" + data[0].Business[i].NOE + "'," +
                                              "'" + data[0].Business[i].Salary + "'," +
                                              "'" + data[0].Business[i].VOS + "'," +
                                              "'" + data[0].Business[i].AOS + "'," +
                                              "'1'," +
                                              "'" + memid + "'," +
                                              "'" + data[0].Business[0].BusinessFiles[i].FilePath + "'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                                    saving.tbl_BusinessInformation_Model = db.AUIDB_WithParam(b_insert);
                                }
                            }

                            //motor
                            if (data[0].Assets.Count != 0)
                            {


                                for (int i = 0; i < data[0].Assets.Count; i++)
                                {


                                    string assets = $@"INSERT INTO [dbo].[tbl_AssetsProperties_Model]
                                   ([MotorVehicles]
                                   ,[Status]
                                   ,[MemId]   
                                   ,[DateCreated])
                                     VALUES
                               ('" + data[0].Assets[i].MotorVehicles + "'," +
                                               "'1'," +
                                               "'" + memid + "'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                                    saving.tbl_AssetsProperties_Model = db.AUIDB_WithParam(assets);

                                }

                            }

                            //property
                            if (data[0].Property.Count != 0)
                            {


                                for (int i = 0; i < data[0].Property.Count; i++)
                                {

                                    string property = $@"INSERT INTO [dbo].[tbl_Property_Model]
                                   ([Property]
                                   ,[Status]
                                   ,[MemId]
                                   ,[DateCreated])
                           VALUES
                               ('" + data[0].Property[i].Property + "'," +
                                               "'1'," +
                                               "'" + memid + "'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                                    saving.tbl_Property_Model = db.AUIDB_WithParam(property);
                                }

                            }

                            if (data[0].Bank.Count != 0)
                            {


                                for (int i = 0; i < data[0].Bank.Count; i++)
                                {

                                    string bank = $@"INSERT INTO [dbo].[tbl_BankAccounts_Model]
                        ([BankName]
                       ,[Address]
                       ,[Status]
                       ,[MemId]
                       ,[DateCreated])
                         VALUES
                               ('" + data[0].Bank[i].BankName + "'," +
                                                   "'" + data[0].Bank[i].Address + "'," +
                                                   "'1'," +
                                                   "'" + memid + "'," +
                                                   "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_BankAccounts_Model = db.AUIDB_WithParam(bank);
                                }
                            }
                            string createdby = data[0].UserId;
                            string submittedby = "";
                            string datesubmitted = "";
                            if (data[0].ApplicationStatus == 7)
                            {

                                submittedby = "";
                                datesubmitted = "";

                                string Update_memstats = $@"update tbl_Member_Model set 
                                  Status='2'" +
                                         "where MemId='" + memid + "'";
                                db.AUIDB_WithParam(Update_memstats);

                            }
                            else
                            {

                                submittedby = data[0].UserId;
                                datesubmitted = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (data[0].NAID == string.Empty || data[0].NAID == null)
                            {
                                string application = $@"insert into   tbl_Application_Model
                               ([MemId]
                               ,[Remarks]
                               ,[Status]
                               ,[CreatedBy]
                               ,[SubmittedBy]
                               ,[DateSubmitted]
                               ,[DateCreated])
                                values
                                ('" + memid + "'," +
                                        "'" + data[0].Remarks + "'," +
                                        "'" + data[0].ApplicationStatus + "'," +
                                        "'" + createdby + "'," +
                                        "'" + submittedby + "'," +
                                        "'" + datesubmitted + "'," +
                                        "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                                saving.tbl_Application_Model = db.AUIDB_WithParam(application);

                            }
                            else
                            {
                                string application = $@"UPDATE [dbo].[tbl_Application_Model]
                                       SET [Status] = '" + data[0].ApplicationStatus + "'," +
                                                  "[CI_ApprovedBy] = '" + data[0].UserId + "'," +
                                                  "[CI_ApprovalDate] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                                  "where NAID='" + data[0].NAID + "'";
                                saving.tbl_Application_Model = db.AUIDB_WithParam(application);
                            }
                            string Insert_Fam = $@"insert into   tbl_FamBackground_Model
                               ([Fname]
                                ,[Mname]
                               ,[Lname]
                               ,[Suffix]
                               ,[DOB]
                               ,[Age]
                               ,[Emp_Status]
                               ,[Position]
                                ,[YOS]
                               ,[CmpId]
                               ,[NOD]
                               ,[RTTB]
                               ,[MemId]
                               ,[Status]
                               ,[DateCreated])
                                values
                                ('" + data[0].F_Fname + "'," +
                                          "'" + data[0].F_Mname + "'," +
                                          "'" + data[0].F_Lname + "'," +
                                          "'" + data[0].F_Suffix + "'," +
                                          "'" + Convert.ToDateTime(data[0].F_DOB).ToString("yyyy-MM-dd") + "'," +
                                          "'" + data[0].F_Age + "'," +
                                          "'" + data[0].F_Emp_Status + "'," +
                                          "'" + data[0].F_Job + "'," +
                                          "'" + data[0].F_YOS + "'," +
                                          "'" + data[0].F_CompanyName + "'," +
                                          "'" + data[0].F_NOD + "'," +
                                          "'" + data[0].F_RTTB + "'," +
                                          "'" + memid + "'," +
                                          "'1'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            saving.tbl_FamBackground_Model = db.AUIDB_WithParam(Insert_Fam);
                            sql = $@"select FamId from tbl_FamBackground_Model order by id desc";

                            DataTable fam_tbl = db.SelectDb(sql).Tables[0];
                            string famid = fam_tbl.Rows[0]["FamId"].ToString();
                            if (data[0].Child.Count != 0)
                            {


                                for (int i = 0; i < data[0].Child.Count; i++)
                                {


                                    string childinfo = $@"INSERT INTO [dbo].[tbl_ChildInfo_Model]
                                   ([Fname]
                                   ,[Mname]
                                   ,[Lname]
                                   ,[Age]
                                   ,[NOS]
                                   ,[FamId]
                                   ,[Status]
                                   ,[DateCreated])
                             VALUES
                                   ('" + data[0].Child[i].Fname + "'," +
                                               "'" + data[0].Child[i].Mname + "'," +
                                               "'" + data[0].Child[i].Lname + "'," +
                                               "'" + data[0].Child[i].Age + "'," +
                                              "'" + data[0].Child[i].NOS + "'," +
                                              "'" + famid + "'," +
                                             "'1'," +
                                                 "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_ChildInfo_Model = db.AUIDB_WithParam(childinfo);
                                }
                            }
                            sql = $@"select   NAID from tbl_Application_Model order by id desc";

                            DataTable tbl = db.SelectDb(sql).Tables[0];

                            naid = tbl.Rows.Count == 0 ? "NA-01" : tbl.Rows[0]["NAID"].ToString();
                            //loan details


                            string Insert_comaker = $@"insert into   [dbo].[tbl_CoMaker_Model]
                                ([Fname]
                               ,[Mname]
                               ,[Lnam]
                               ,[Suffi]
                               ,[Gender]
                               ,[DOB]
                               ,[Age]
                               ,[POB]
                               ,[CivilStatus]
                               ,[Cno]
                               ,[EmailAddress]
                               ,[House_Stats]
                               ,[HouseNo]
                               ,[Barangay]
                               ,[City]
                               ,[Region]
                               ,[Country]
                               ,[ZipCode]
                               ,[YearsStay]
                               ,[RTTB]
                               ,[Status]
                               ,[DateCreated]
                               ,[MemId])
                                values
                                ('" + data[0].Co_Fname + "', " +
                                         "'" + data[0].Co_Lname + "'," +
                                         "'" + data[0].Co_Mname + "', " +
                                         "'" + data[0].Co_Suffix + "', " +
                                         "'" + data[0].Co_Gender + "', " +
                                         "'" + Convert.ToDateTime(data[0].Co_DOB).ToString("yyyy-MM-dd") + "', " +
                                         "'" + data[0].Co_Age + "', " +
                                         "'" + data[0].Co_POB + "', " +
                                         "'" + data[0].Co_Civil_Status + "', " +
                                         "'" + data[0].Co_Cno + "', " +
                                         "'" + data[0].Co_EmailAddress + "', " +
                                         "'" + data[0].Co_House_Stats + "', " +
                                         "'" + data[0].Co_HouseNo + "', " +
                                         "'" + data[0].Co_Barangay + "', " +
                                         "'" + data[0].Co_City + "', " +
                                         "'" + data[0].Co_Province + "', " +
                                         "'" + data[0].Co_Country + "', " +
                                         "'" + data[0].Co_ZipCode + "', " +
                                         "'" + data[0].Co_YearsStay + "', " +
                                         "'" + data[0].Co_RTTB + "', " +
                                         "'1', " +
                                         "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                         "'" + memid + "') ";

                            saving.tbl_CoMaker_Model = db.AUIDB_WithParam(Insert_comaker);

                            sql = $@"select   CMID from tbl_CoMaker_Model order by id desc";

                            DataTable tables = db.SelectDb(sql).Tables[0];
                            string cmid = tables.Rows[0]["CMID"].ToString();

                            string insert_jobcomaker = $@"INSERT INTO [dbo].[tbl_CoMaker_JobInfo_Model]
                               ([JobDescription]
                               ,[YOS]
                               ,[CompanyName]
                               ,[MonthlySalary]
                               ,[OtherSOC]
                               ,[Status]
                               ,[BO_Status]
                               ,[Emp_Status]
                               ,[CMID]
                               ,[CompanyAddress]
                               ,[DateCreated])
                                values
                                ('" + data[0].Co_JobDescription + "'," +
                                            "'" + data[0].Co_YOS + "'," +
                                            "'" + data[0].Co_CompanyName + "'," +
                                            "'" + data[0].Co_MonthlySalary + "'," +
                                            "'" + data[0].Co_OtherSOC + "'," +
                                            "'1'," +
                                            "'" + data[0].Co_BO_Status + "'," +
                                            "'" + data[0].Co_Emp_Status + "'," +
                                            "'" + cmid + "'," +
                                            "'" + data[0].Co_CompanyAddress + "'," +
                                             "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";




                            saving.tbl_CoMaker_JobInfo_Model = db.AUIDB_WithParam(insert_jobcomaker);

                            if (data[0].Appliances.Count != 0)
                            {
                                sql = $@"select   NAID from tbl_Application_Model order by id desc";

                                DataTable na_table = db.SelectDb(sql).Tables[0];
                                naid = tbl.Rows.Count == 0 ? "NA-01" : tbl.Rows[0]["NAID"].ToString();

                                string na_insert = "";
                                for (int i = 0; i < data[0].Appliances.Count; i++)
                                {

                                    na_insert += $@"INSERT INTO[dbo].[tbl_Appliance_Model]
                               ([Brand]
                               ,[Description]
                               ,[NAID]
                               ,[Status]
                               ,[DateCreated])
                                VALUES
                               ('" + data[0].Appliances[i].Brand + "'," +
                                                  "'" + data[0].Appliances[i].Appliances + "'," +
                                                  "'" + naid + "'," +
                                                  "'1'," +
                                                  "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";


                                }

                                saving.tbl_Appliance_Model = db.AUIDB_WithParam(na_insert);


                            }
                            string loandetails = $@"insert into   tbl_LoanDetails_Model
                                ([LoanAmount]
                               ,[TermsOfPayment]
                               ,[Purpose]
                               ,[MemId]
                               ,[Status]
                               ,[LoanTypeID]
                               ,[NAID]
                               ,[DateCreated])
                                values
                                ('" + data[0].LoanAmount + "'," +
                                         "'" + data[0].TermsOfPayment + "'," +
                                         "'" + data[0].Purpose + "'," +
                                         "'" + memid + "'," +
                                         "'1'," +
                                         "'" + data[0].LoanTypeId + "'," +
                                         "'" + naid + "'," +
                                          "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            saving.tbl_LoanDetails_Model = db.AUIDB_WithParam(loandetails);

                            string file_upload = $@"INSERT INTO [dbo].[tbl_fileupload_Model]
                                           ([MemId]
                                           ,[FileName]
                                           ,[FilePath]
                                           ,[Status]
                                           ,[Type]
                                           ,[DateCreated])
                                     VALUES
                                           ('" + memid + "'," +
                                                      "'" + data[0].ProfileName + "'," +
                                                       "'" + data[0].ProfileFilePath + "'," +
                                                      "'1'," +
                                                      "'1'," +
                                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            saving.tbl_fileupload_Model = db.AUIDB_WithParam(file_upload);


                            string co_profile = $@"INSERT INTO  [dbo].[tbl_CoMakerFileUpload_Model]
                                       ([CMID]
                                       ,[FileName]
                                       ,[FilePath]
                                       ,[Status]
                                       ,[DateCreated])
                                     VALUES
                                           ('" + cmid + "'," +
                                                     "'" + data[0].Co_ProfileName + "'," +
                                                      "'" + data[0].Co_ProfileFilePath + "'," +
                                                     "'1'," +
                                                      "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            saving.tbl_CoMakerFileUpload_Model = db.AUIDB_WithParam(co_profile);
                            string co_signature_upload = "";
                            if (data[0].Co_SignatureUpload.Count != 0)
                            {
                                for (int x = 0; x < data[0].Co_SignatureUpload.Count; x++)
                                {
                                    co_signature_upload = $@"INSERT INTO  [dbo].[tbl_CoMakerFileUpload_Model]
                                       ([CMID]
                                       ,[FileName]
                                       ,[FilePath]
                                       ,[Status]
                                       ,[DateCreated])
                                     VALUES
                                           ('" + cmid + "'," +
                                              "'" + data[0].Co_SignatureUpload[x].FileName + "'," +
                                               "'" + data[0].Co_SignatureUpload[x].FilePath + "'," +
                                              "'3'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_CoMakerFileUpload_Model = db.AUIDB_WithParam(co_signature_upload);

                                }

                            }

                            string co_uploadfile = "";
                            if (data[0].Co_RequirementsFile.Count != 0)
                            {
                                for (int x = 0; x < data[0].Co_RequirementsFile.Count; x++)
                                {
                                    co_uploadfile = $@"INSERT INTO  [dbo].[tbl_CoMakerFileUpload_Model]
                                       ([CMID]
                                       ,[FileName]
                                       ,[FilePath]
                                       ,[Status]
                                       ,[DateCreated])
                                     VALUES
                                           ('" + cmid + "'," +
                                                  "'" + data[0].Co_RequirementsFile[x].FileName + "'," +
                                                   "'" + data[0].Co_RequirementsFile[x].FilePath + "'," +
                                                  "'2'," +
                                                   "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_CoMakerFileUpload_Model = db.AUIDB_WithParam(co_uploadfile);
                                }

                            }

                            string uploadfile_req = "";
                            if (data[0].RequirementsFile.Count != 0)
                            {
                                for (int x = 0; x < data[0].RequirementsFile.Count; x++)
                                {
                                    uploadfile_req = $@"INSERT INTO [dbo].[tbl_fileupload_Model]
                                           ([MemId]
                                           ,[FileName]
                                           ,[FilePath]
                                           ,[Status]
                                           ,[Type]
                                           ,[DateCreated])
                                     VALUES
                                           ('" + memid + "'," +
                                                  "'" + data[0].RequirementsFile[x].FileName + "'," +
                                                   "'" + data[0].RequirementsFile[x].FilePath + "'," +
                                                  "'1'," +
                                                  "'2'," +
                                                   "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_fileupload_Model = db.AUIDB_WithParam(uploadfile_req);
                                }


                            }

                            string signature_upload = "";
                            if (data[0].SignatureUpload.Count != 0)
                            {
                                for (int x = 0; x < data[0].SignatureUpload.Count; x++)
                                {
                                    signature_upload = $@"INSERT INTO [dbo].[tbl_fileupload_Model]
                                           ([MemId]
                                           ,[FileName]
                                           ,[FilePath]
                                           ,[Status]
                                           ,[Type]
                                           ,[DateCreated])
                                     VALUES
                                           ('" + memid + "'," +
                                              "'" + data[0].SignatureUpload[x].FileName + "'," +
                                               "'" + data[0].SignatureUpload[x].FilePath + "'," +
                                              "'1'," +
                                              "'3'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                    saving.tbl_fileupload_Model = db.AUIDB_WithParam(signature_upload);
                                }


                            }

                            saving.NAID = naid;
                            saving.promtresult = promtresult;
                            saving.promtresult_status = promtresult_status;
                            return Ok(saving);
                        }

                    }

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
