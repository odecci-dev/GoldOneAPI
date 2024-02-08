using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoldOneAPI.Controllers.MemberController;
namespace AuthSystem.Models
{
    public class OfficerModel
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
}
