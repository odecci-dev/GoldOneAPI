using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoldOneAPI.Controllers.MemberController;

namespace AuthSystem.Models
{
    public class SaveMemberModel
    {

        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Mname { get; set; }
        public string? Suffix { get; set; }
        public int? Age { get; set; }
        public string? Barangay { get; set; }
        public string? City { get; set; }
        public string? Civil_Status { get; set; }
        public string? Cno { get; set; }
        public string? Country { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmailAddress { get; set; }
        public string? Gender { get; set; }
        public string? HouseNo { get; set; }
        public int? House_Stats { get; set; }
        public string? POB { get; set; }
        public string? Province { get; set; }
        public int? YearsStay { get; set; }
        public string? ZipCode { get; set; }
        public int? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        //-----------MonthlyBills

        public string? MemId { get; set; }
        public decimal? ElectricBill { get; set; }
        public decimal? WaterBill { get; set; }
        public decimal? OtherBills { get; set; }
        public decimal? DailyExpenses { get; set; }

        //-----------JobInfo
        public string? JobDescription { get; set; }
        public int? YOS { get; set; }
        public decimal? MonthlySalary { get; set; }
        public string? OtherSOC { get; set; }
        public int? BO_Status { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public int? Emp_Status { get; set; }

        //----Family
        public string? F_Fname { get; set; }
        public string? F_Lname { get; set; }
        public string? F_Mname { get; set; }
        public string? F_Suffix { get; set; }
        public DateTime? F_DOB { get; set; }
        public int? F_Age { get; set; }
        public int? F_NOD { get; set; }
        public int? F_YOS{ get; set; }
        public int? F_Emp_Status { get; set; }
        public string? F_Job { get; set; }
        public string? F_CompanyName { get; set; }
        public string? F_RTTB { get; set; }
        public string? FamId { get; set; }
        //---business
        public List<BusinessModel>? Business { get; set; }
        //-- loan
        public decimal? LoanAmount { get; set; }
        public string? LoanTypeId { get; set; }
        public string? TermsOfPayment { get; set; }
        public string? Purpose { get; set; }
        //child
        public List<ChildModel>? Child { get; set; }
        //Appliances
        public List<ApplianceModel>? Appliances { get; set; }
        //assets
        public List<AssetsModel>? Assets { get; set; }
        public List<PropertyDetailsModel>? Property { get; set; }
        //bank
        public List<BankModel>? Bank { get; set; }
        //comaker
        public string? Co_Fname { get; set; }
        public string? Co_Lname { get; set; }
        public string? Co_Mname { get; set; }
        public string? Co_Suffix { get; set; }
        public int? Co_Age { get; set; }
        public string? Co_Barangay { get; set; }
        public string? Co_City { get; set; }
        public string? Co_Civil_Status { get; set; }
        public string? Co_Cno { get; set; }
        public string? Co_Country { get; set; }
        public DateTime? Co_DOB { get; set; }
        public string? Co_EmailAddress { get; set; }
        public string? Co_Gender { get; set; }
        public string? Co_HouseNo { get; set; }
        public int? Co_House_Stats { get; set; }
        public string? Co_POB { get; set; }
        public string? Co_Province { get; set; }
        public int? Co_YearsStay { get; set; }
        public string? Co_ZipCode { get; set; }
        public string? Co_RTTB { get; set; }
        public int? Co_Status { get; set; }


        //comaker

        public string? Co_JobDescription { get; set; }
        public int? Co_YOS { get; set; }
        public decimal? Co_MonthlySalary { get; set; }
        public string? Co_OtherSOC { get; set; }
        public int? Co_BO_Status { get; set; }
        public string? Co_CompanyName { get; set; }
        public string? Co_CompanyAddress { get; set; }
        public string? Co_CompanyID { get; set; }
        public int? Co_Emp_Status { get; set; }

        //new application
        public string? Remarks { get; set; }
        public int? ApplicationStatus { get; set; }

        //profile image

        public string? ProfileName { get; set; }
        public string? ProfileFilePath { get; set; }

        public string? Co_ProfileName { get; set; }
        public string? Co_ProfileFilePath { get; set; }

        public List<UploadFile>? RequirementsFile { get; set; }

        public List<Signature_Upload>? SignatureUpload { get; set; }

        public List<C0_UploadFile>? Co_RequirementsFile { get; set; }
        public List<C0_UploadFile>? Co_SignatureUpload { get; set; }
        public string? UserId { get; set; }
        public string? NAID { get; set; }
    }


    }


