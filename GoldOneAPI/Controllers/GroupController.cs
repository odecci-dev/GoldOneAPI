using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Text;
using static GoldOneAPI.Controllers.MemberController;
using GoldOneAPI.Manager;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
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

        public GroupController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;
            //this.jwtAuthenticationManager = jwtAuthenticationManager;

        }
        public class Loans
        {

            public string? LoanAmount { get; set; }
            public string? LDID { get; set; }
            public string? TermsOfPayment { get; set; }
            public string? TopId { get; set; }
            public string? Purpose { get; set; }
            public string? Status { get; set; }
            public string? StatusId { get; set; }
        }
        public class GroupVM
        {
            public string? GroupName { get; set; }
            public string? GroupID { get; set; }
            public string? DateCreated { get; set; }
            public string? DateUpdated { get; set; }
            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? ApplicationStatusId { get; set; }
            public string? ApplicationStatus { get; set; }
            public List<GroupModelVM> MemberList { get; set; }
        }
        public class GroupApplicationVM
        {
            public string? GroupName { get; set; }
            public string? MemId { get; set; }
            public string? DateCreated { get; set; }
            public string? DateApproval { get; set; }
            public string? Remarks { get; set; }
            public string? NAID { get; set; }
            public string? Status { get; set; }
            public string? GroupId { get; set; }
            public string? Borrower { get; set; }
            public string? Borrower_Cno { get; set; }
            public string? Cno { get; set; }
            public string? Co_Borrower { get; set; }
            public string? coBorrower { get; set; }
            public string? Co_Borrower_Cno { get; set; }
            public string? Co_Cno { get; set; }
            public string? StatusId { get; set; }
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
            public string? loanamount { get; set; }


            public List<GroupApplicationVM2>? Loandetails { get; set; }
        }
        public class GroupApp
        {
            public string? MemId { get; set; }
            public string? GroupName { get; set; }
            public string? GroupId { get; set; }
            public string? DateCreated { get; set; }
            public string? DateApproval { get; set; }
            public string? Remarks { get; set; }
            public string? NAID { get; set; }
            public string? Status { get; set; }
            public string? StatusId { get; set; }
            public string? Borrower { get; set; }
            public string? Mem_status { get; set; }
            public string? LoanAmount { get; set; }
            public string? LoanType { get; set; }
            public string? LoanTypeID { get; set; }
            public string? LDID { get; set; }
            public string? BorrowerCno { get; set; }
            public string? Cno { get; set; }
            public string? CoBorrower { get; set; }
            public string? Co_Cno { get; set; }
            public string? RefNo { get; set; }
            public string? AreaName { get; set; }
            public string? TermsOfPayment { get; set; }
            public string? NameOfTerms { get; set; }
            public string? Interest { get; set; }

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
            public string? ReleasedBy { get; set; }
            public string? DateSubmitted { get; set; }

            public string? ModeOfRelease { get; set; }
            public string? ModeOfReleaseReference { get; set; }
            public string? Courerier { get; set; }
            public string? CourierCNo { get; set; }
            public string? CourerierName { get; set; }
            public string? Denomination { get; set; }
            public string? ApprovedLoanAmount { get; set; }
            public string? ApprovedTermsOfPayment { get; set; }
            public string? Days { get; set; }
        }
        public class GroupDetails
        {
            public List<SaveMemberModel>? Member { get; set; }
            public string? GroupName { get; set; }
            public string? GroupId { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GroupListMember()
        {


            var result = (dynamic)null;
            try
            {
                result = dbmet.GetGroupListMember().ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLastGroupListMember()
        {


            try
            {
                var result = (dynamic)null;
                try
                {
                    result = dbmet.GetGroupListMember().OrderByDescending(a => a.GroupID).FirstOrDefault();

                    return Ok(result);
                }

                catch (Exception ex)
                {
                    return BadRequest("ERROR");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GroupApplicationList(string NAID)
        {

            var result = (dynamic)null;
            try
            {
                result = dbmet.GetGroupListMember().Where(a => a.ApplicationStatus == NAID).ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> GroupListFilterByGroupName(GroupModel data)
        {
            try
            {
                var result = (dynamic)null;
                //var result = new List<CreditModel>();
                try
                {
                    var counter = dbmet.GetGroupListMember().Where(a => a.GroupName == data.GroupName).ToList();
                    if (counter.Count != 0)
                    {
                        result = counter;
                        return Ok(result);
                    }
                    else
                    {
                        if (data.GroupName == "")
                        {
                            result = dbmet.GetGroupListMember().ToList();
                            return Ok(result);
                        }
                        else
                        {
                            return BadRequest("No Data Found");

                        }
                    }

                }

                catch (Exception ex)
                {
                    return BadRequest("ERROR");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> GroupApplicationFilter(List<FilterModel> data)
        {
            var result = new List<GroupApplicationVM>();
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
                sql = $@"SELECT        
                        tbl_Group_Model.GroupName, 
                        tbl_GroupDetails_Model.MemId, 
                        tbl_Application_Model.DateCreated, 
                        tbl_Application_Model.DateApproval, 
                        tbl_Application_Model.Remarks, 
                        tbl_Application_Model.NAID, 
                        tbl_Status_Model.Name as Status, 
                        tbl_GroupDetails_Model.GroupId, 
                        Concat(
                        tbl_Member_Model.Fname, ' ',
                        tbl_Member_Model.Mname, ' ',
                        tbl_Member_Model.Lname, ' ',
                        tbl_Member_Model.Suffix) As Borrower, 
                        tbl_Member_Model.Cno as Borrower_Cno , 
                        concat(
                        tbl_CoMaker_Model.Fname , ' ',
                        tbl_CoMaker_Model.Mname , ' ',
                        tbl_CoMaker_Model.Lnam, ' ',
                        tbl_CoMaker_Model.Suffi) as Co_Borrower, 
                        tbl_CoMaker_Model.Cno AS Co_Borrower_Cno
                        FROM            tbl_GroupDetails_Model INNER JOIN
                         tbl_Group_Model ON tbl_GroupDetails_Model.GroupId = tbl_Group_Model.GroupID INNER JOIN
                         tbl_Application_Model ON tbl_GroupDetails_Model.MemId = tbl_Application_Model.MemId INNER JOIN
                         tbl_Status_Model ON tbl_Application_Model.Status = tbl_Status_Model.Id INNER JOIN
                         tbl_Member_Model ON tbl_GroupDetails_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_CoMaker_Model ON tbl_GroupDetails_Model.MemId = tbl_CoMaker_Model.MemId  where tbl_Member_Model.Status =1 " + w_column + " ";

                DataTable table = db.SelectDb(sql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var dateu = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var item = new GroupApplicationVM();
                        item.GroupName = dr["GroupName"].ToString();
                        item.MemId = dr["MemId"].ToString();
                        item.DateCreated = datec;
                        item.DateApproval = dateu;
                        item.Remarks = dr["Remarks"].ToString();
                        item.NAID = dr["NAID"].ToString();
                        item.Status = dr["Status"].ToString();
                        item.GroupId = dr["GroupId"].ToString();
                        item.Borrower = dr["Borrower"].ToString();
                        item.Borrower_Cno = dr["Borrower_Cno"].ToString();
                        item.Co_Borrower = dr["Co_Borrower"].ToString();
                        item.Co_Borrower_Cno = dr["Co_Borrower_Cno"].ToString();
                        //grouploan
                        string loan_sql = $@"SELECT       tbl_LoanDetails_Model.Id, tbl_LoanDetails_Model.LoanAmount, tbl_LoanDetails_Model.TermsOfPayment, tbl_LoanDetails_Model.Purpose, tbl_LoanDetails_Model.MemId, tbl_LoanDetails_Model.DateCreated, 
                         tbl_LoanDetails_Model.DateUpdated, tbl_LoanDetails_Model.GroupId, tbl_LoanType_Model.LoanTypeName
                            FROM            tbl_LoanDetails_Model INNER JOIN
                                                     tbl_LoanType_Model ON tbl_LoanDetails_Model.Status = tbl_LoanType_Model.Id
                            WHERE        (tbl_LoanDetails_Model.Status = '2') AND (tbl_LoanDetails_Model.GroupId = '" + dr["GroupId"].ToString() + "') ";
                        DataTable tbl_loan = db.SelectDb(loan_sql).Tables[0];
                        if (tbl_loan.Rows.Count != 0)
                        {
                            var tbl_loan_res = new List<GroupApplicationVM2>();
                            foreach (DataRow b_dr in tbl_loan.Rows)
                            {
                                var loan_item = new GroupApplicationVM2();
                                loan_item.LoanAmount = b_dr["LoanAmount"].ToString();
                                loan_item.LoanType = b_dr["LoanTypeName"].ToString();
                                tbl_loan_res.Add(loan_item);
                            }
                            item.Loandetails = tbl_loan_res;
                        }
                        result.Add(item);
                    }

                }
                else
                {
                    return BadRequest("No data available");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }


            return Ok(result);
        }
        public class GroupIDModel
        {
            public string? GroupID { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> FilterByGroupID(GroupIDModel data)
        {
            try
            {
                var result = (dynamic)null;
                //var result = new List<CreditModel>();
                try
                {
                    var counter = dbmet.GetGroupListMember().Where(a => a.GroupID == data.GroupID).ToList();
                    if (counter.Count != 0)
                    {
                        result = counter;
                        return Ok(result);
                    }
                    else
                    {
                        result = dbmet.GetGroupListMember().ToList();
                        return Ok(result);
                    }

                }

                catch (Exception ex)
                {
                    return BadRequest("ERROR");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }


        }
        [HttpPost]
        public async Task<IActionResult> AddNewGroupName(GroupModel data)
        {
            string result = "";
            try
            {
                string Insert = "";


                sql = $@"select * from tbl_Group_Model where GroupName ='" + data.GroupName + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count == 0)
                {
                    Insert = $@"INSERT INTO [dbo].[tbl_Group_Model]
                               ([GroupName]
                               ,[Status] 
                               ,[DateCreated])
                         VALUES
                               ('" + data.GroupName + "'," +
                                "'1'," +
                              "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";


                    results = db.AUIDB_WithParam(Insert) + " Added";
                    return Ok(results);
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
        [HttpPost]
        public async Task<IActionResult> DeleteGroupDetails(GroupDetails data)
        {
            string result = "";
            try
            {
                string Delete = "";


                sql = $@"select * from tbl_GroupDetails_Model where GroupId ='" + data.GroupId + "' and MemId= '" + data.Member[0].MemId + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Delete = $@"DELETE FROM [dbo].[tbl_GroupDetails_Model]
                             where GroupId ='" + data.GroupId + "' and MemId= '" + data.Member[0].MemId + "' ";


                    results = db.AUIDB_WithParam(Delete) + " Deleted";
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
        public async Task<IActionResult> SaveGroupList(GroupDetails data)
        {
            string result = "";
            string GroupId = "";
            try
            {
                string Insert = "";
                //check if groupname exist
                string group_exist = $@"select * from tbl_Group_Model where GroupName ='" + data.GroupName + "'  ";
                DataTable tbl_group_exist = db.SelectDb(group_exist).Tables[0];
                if (tbl_group_exist.Rows.Count == 0) // if not existing add new group name
                {

                    string save_group = $@"INSERT INTO [dbo].[tbl_Group_Model]
                                               ([GroupName]
                                               ,[Status]
                                               ,[DateCreated])
                                         VALUES
                                               ('" + data.GroupName + "'," +
                                        "'1'," +
                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    result = db.AUIDB_WithParam(save_group) + " Added";
                    sql = $@"select Top(1) GroupId from tbl_Group_Model order by id desc";
                    DataTable table = db.SelectDb(sql).Tables[0];
                    GroupId = table.Rows[0]["GroupId"].ToString();

                   

             
                    
                }
                else
                {
                    sql = $@"select Top(1) GroupId from tbl_Group_Model where GroupName ='" + data.GroupName + "' ";
                    DataTable table = db.SelectDb(sql).Tables[0];
                    GroupId = table.Rows[0]["GroupId"].ToString();
                }
                for (int x = 0; x < data.Member.Count; x++)
                {

                    //check if memid is exist
                    string mem_exist = $@"select *  from tbl_Member_Model where 
                                        Lower(Fname) ='" + data.Member[x].Fname.ToLower() + "' " +
                                        "and Lower(Lname) ='" + data.Member[x].Lname.ToLower() + "' " +
                                        "and Lower(Mname) ='" + data.Member[x].Mname.ToLower() + "'";
                    DataTable mem_exist_tables = db.SelectDb(mem_exist).Tables[0];
                    if (mem_exist_tables.Rows.Count == 0)
                    {
                        //not exist in membertable
                        //insert new
                        //get memid inserted
                        //insert in groupdetails
                        //insert loan details groupid
                        string areafilter = $@"SELECT [Id]
                                  ,[Area]
                                  ,[City]
                                   FROM [dbo].[tbl_Area_Model]
                                    Where City like'%" + data.Member[0].Barangay + ", " + data.Member[0].City + "%'";
                        DataTable area_table = db.SelectDb(areafilter).Tables[0];
                        if (area_table.Rows.Count == 0)
                        {
                            string insert_area = $@"INSERT INTO [dbo].[tbl_Area_Model]
                                       ([Status],[City])
                                        VALUES
                                        ('1', " +
                                                "'" + data.Member[0].Barangay + ", " + data.Member[0].City + "')";
                            results = db.AUIDB_WithParam(insert_area) + " Added";
                        }

                        string Insert_MEM = $@"insert into   tbl_Member_Model (Fname, Lname, Mname, Suffix, Age, Barangay, City, Civil_Status, Cno, Country, DOB, EmailAddress, Gender, HouseNo, House_Stats, POB, Province, YearsStay, ZipCode,DateCreated, Status) 
                                values
                                ('" + data.Member[0].Fname + "', " +
                               "'" + data.Member[0].Lname + "'," +
                               "'" + data.Member[0].Mname + "', " +
                               "'" + data.Member[0].Suffix + "', " +
                               "'" + data.Member[0].Age + "', " +
                               "'" + data.Member[0].Barangay + "', " +
                               "'" + data.Member[0].City + "', " +
                               "'" + data.Member[0].Civil_Status + "', " +
                               "'" + data.Member[0].Cno + "', " +
                               "'" + data.Member[0].Country + "', " +
                               "'" + Convert.ToDateTime(data.Member[0].DOB).ToString("yyyy-MM-dd") + "', " +
                               "'" + data.Member[0].EmailAddress + "', " +
                               "'" + data.Member[0].Gender + "', " +
                               "'" + data.Member[0].HouseNo + "', " +
                               "'" + data.Member[0].House_Stats + "', " +
                               "'" + data.Member[0].POB + "', " +
                               "'" + data.Member[0].Province + "', " +
                               "'" + data.Member[0].YearsStay + "', " +
                               "'" + data.Member[0].ZipCode + "', " +
                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                               "'" + data.Member[0].Status + "') ";
                        results = db.AUIDB_WithParam(Insert_MEM) + " Added";
                        sql = $@"select Top(1) MemId from tbl_Member_Model order by id desc";

                        DataTable table = db.SelectDb(sql).Tables[0];
                        var memid = table.Rows[0]["MemId"].ToString();

                        //group details

                        string save_group_details = $@"INSERT INTO [dbo].[tbl_GroupDetails_Model]
                                               ([MemId]
                                               ,[GroupId]
                                               ,[DateCreated])
                                         VALUES
                                               ('" + memid + "'," +
                                     "'" + GroupId + "'," +
                                    "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        db.AUIDB_WithParam(save_group_details);
                        //monthlybills
                        string username = $@"SELECT  Fname,Lname,Mname,UserId FROM [dbo].[tbl_User_Model] where Status=1";
                        DataTable username_tbl = db.SelectDb(username).Tables[0];
                        foreach (DataRow dr in username_tbl.Rows)
                        {
                            string name = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                            dbmet.InsertNotification("Inserted New Borrower  " + memid + " from Application Creation",
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Application Module", name, dr["UserId"].ToString(), "2",memid);
                        }
                        string _Insert = $@"INSERT INTO 
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
                                    "'" + data.Member[0].ElectricBill + "'," +
                                    "'" + data.Member[0].WaterBill + "'," +
                                    "'" + data.Member[0].OtherBills + "'," +
                                    "'" + data.Member[0].DailyExpenses + "'," +
                                    "'1'," +
                                    "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        db.AUIDB_WithParam(_Insert);

                        //jobinfo
                        Insert += $@"insert into   tbl_JobInfo_Model (
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
                                ('" + data.Member[0].JobDescription + "'," +
                                     "'" + data.Member[0].YOS + "'," +
                                     "'" + data.Member[0].CompanyName + "'," +
                                     "'" + data.Member[0].CompanyAddress + "'," +
                                     "'" + data.Member[0].MonthlySalary + "'," +
                                     "'" + data.Member[0].OtherSOC + "'," +
                                     "'" + data.Member[0].Status + "'," +
                                     "'" + data.Member[0].BO_Status + "'," +
                                     "'1'," +
                                     "'" + memid + "'," +
                                      "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                        if (data.Member[0].Business.Count != 0)
                        {
                            string b_files = "";
                            for (int i = 0; i < data.Member[0].Business.Count; i++)
                            {


                                if (data.Member[0].Business[0].BusinessFiles.Count != 0)
                                {

                                    for (int a = 0; a < data.Member[0].Business[0].BusinessFiles.Count; a++)
                                    {
                                        b_files += data.Member[0].Business[0].BusinessFiles[a].FilePath + "|";
                                    }

                                }
                                string results = b_files.Substring(0, b_files.Length - 1);
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
                                ('" + data.Member[0].Business[i].BusinessName + "'," +
                                          "'" + data.Member[0].Business[i].BusinessType + "'," +
                                          "'" + data.Member[0].Business[i].BusinessAddress + "'," +
                                          "'" + data.Member[0].Business[i].B_status + "'," +
                                          "'" + data.Member[0].Business[i].YOB + "'," +
                                          "'" + data.Member[0].Business[i].NOE + "'," +
                                          "'" + data.Member[0].Business[i].Salary + "'," +
                                          "'" + data.Member[0].Business[i].VOS + "'," +
                                          "'" + data.Member[0].Business[i].AOS + "'," +
                                          "'1'," +
                                          "'" + memid + "'," +
                                          "'" + results + "'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                                db.AUIDB_WithParam(b_insert);
                            }
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
                                ('" + data.Member[0].F_Fname + "'," +
                                      "'" + data.Member[0].F_Mname + "'," +
                                      "'" + data.Member[0].F_Lname + "'," +
                                      "'" + data.Member[0].F_Suffix + "'," +
                                      "'" + Convert.ToDateTime(data.Member[0].F_DOB).ToString("yyyy-MM-dd") + "'," +
                                      "'" + data.Member[0].F_Age + "'," +
                                      "'" + data.Member[0].F_Emp_Status + "'," +
                                      "'" + data.Member[0].F_Job + "'," +
                                      "'" + data.Member[0].F_YOS + "'," +
                                      "'" + data.Member[0].F_CompanyName + "'," +
                                      "'" + data.Member[0].F_NOD + "'," +
                                      "'" + data.Member[0].F_RTTB + "'," +
                                      "'" + memid + "'," +
                                      "'1'," +
                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        results = db.AUIDB_WithParam(Insert_Fam) + " Added";

                        if (data.Member[0].Child.Count != 0)
                        {


                            for (int i = 0; i < data.Member[0].Child.Count; i++)
                            {
                                sql = $@"select Top(1) FamId from tbl_FamBackground_Model order by id desc";

                                DataTable table1 = db.SelectDb(sql).Tables[0];
                                var famid = table1.Rows[0]["FamId"].ToString();
                                Insert += $@"INSERT INTO [dbo].[tbl_ChildInfo_Model]
                                   ([Fname]
                                   ,[Mname]
                                   ,[Lname]
                                   ,[Age]
                                   ,[NOS]
                                   ,[FamId]
                                   ,[Status]
                                   ,[DateCreated])
                             VALUES
                                   ('" + data.Member[0].Child[i].Fname + "'," +
                                           "'" + data.Member[0].Child[i].Mname + "'," +
                                           "'" + data.Member[0].Child[i].Lname + "'," +
                                           "'" + data.Member[0].Child[i].Age + "'," +
                                          "'" + data.Member[0].Child[i].NOS + "'," +
                                          "'" + famid + "'," +
                                         "'1'," +
                                             "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            }
                        }
                        //motor
                        if (data.Member[0].Assets.Count != 0)
                        {


                            for (int i = 0; i < data.Member[0].Assets.Count; i++)
                            {


                                Insert += $@"INSERT INTO [dbo].[tbl_AssetsProperties_Model]
                       ([MotorVehicles]
                       ,[Status]
                       ,[MemId]   
                       ,[DateCreated])
                         VALUES
                               ('" + data.Member[0].Assets[i].MotorVehicles + "'," +
                                           "'1'," +
                                           "'" + memid + "'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";



                            }

                        }

                        //property
                        if (data.Member[0].Property.Count != 0)
                        {


                            for (int i = 0; i < data.Member[0].Property.Count; i++)
                            {

                                Insert += $@"INSERT INTO [dbo].[tbl_Property_Model]
                                   ([Property]
                                   ,[Status]
                                   ,[MemId]
                                   ,[DateCreated])
                           VALUES
                               ('" + data.Member[0].Property[i].Property + "'," +
                                           "'1'," +
                                           "'" + memid + "'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            }

                        }

                        if (data.Member[0].Bank.Count != 0)
                        {


                            for (int i = 0; i < data.Member[0].Bank.Count; i++)
                            {

                                Insert += $@"INSERT INTO [dbo].[tbl_BankAccounts_Model]
                        ([BankName]
                       ,[Address]
                       ,[Status]
                       ,[MemId]
                       ,[DateCreated])
                         VALUES
                               ('" + data.Member[0].Bank[i].BankName + "'," +
                                              "'" + data.Member[0].Bank[i].Address + "'," +
                                              "'1'," +
                                              "'" + memid + "'," +
                                              "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            }
                        }
                        string createdby = data.Member[0].UserId;
                        string submittedby = "";
                        string datesubmitted = "";
                        if (data.Member[0].ApplicationStatus == 7)
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

                            submittedby = data.Member[0].UserId;
                            datesubmitted = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (data.Member[0].NAID == string.Empty || data.Member[0].NAID == null)
                        {
                            Insert += $@"insert into   tbl_Application_Model
                               ([MemId]
                               ,[Remarks]
                               ,[Status]
                               ,[CreatedBy]
                               ,[SubmittedBy]
                               ,[DateSubmitted]
                               ,[DateCreated])
                                values
                                ('" + memid + "'," +
                                    "'" + data.Member[0].Remarks + "'," +
                                    "'" + data.Member[0].ApplicationStatus + "'," +
                                    "'" + createdby + "'," +
                                    "'" + submittedby + "'," +
                                    "'" + datesubmitted + "'," +
                                    "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";

                            results = db.AUIDB_WithParam(Insert) + " Added";

                        }
                        else
                        {
                            string update_app = $@"UPDATE [dbo].[tbl_Application_Model]
                                       SET [Status] = '" + data.Member[0].ApplicationStatus + "'," +
                                              "[CI_ApprovedBy] = '" + data.Member[0].UserId + "'," +
                                              "[CI_ApprovalDate] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                              "where NAID='" + data.Member[0].NAID + "'";
                            db.AUIDB_WithParam(update_app);
                        }

                        sql = $@"select Top(1) NAID from tbl_Application_Model order by id desc";

                        DataTable tbl = db.SelectDb(sql).Tables[0];
                        var NAID = tbl.Rows[0]["NAID"].ToString();
                        //loan details

                        string __Insert = $@"insert into   tbl_LoanDetails_Model
                                ([LoanAmount]
                               ,[TermsOfPayment]
                               ,[Purpose]
                               ,[GroupId]
                               ,[Status]
                               ,[LoanTypeID]
                               ,[NAID]
                               ,[DateCreated])
                                values
                                ('" + data.Member[0].LoanAmount + "'," +
                                      "'" + data.Member[0].TermsOfPayment + "'," +
                                      "'" + data.Member[0].Purpose + "'," +
                                      "'" + GroupId + "'," +
                                      "'1'," +
                                      "'" + data.Member[0].LoanTypeId + "'," +
                                      "'" + NAID + "'," +
                                       "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        db.AUIDB_WithParam(__Insert);

                        sql = $@"select Top(1) LDID from tbl_LoanDetails_Model order by id desc";

                        DataTable tbl_ldid = db.SelectDb(sql).Tables[0];
                        var ldid = tbl_ldid.Rows[0]["LDID"].ToString();

                        //GroupId
                        string update_group = $@"UPDATE [dbo].[tbl_Group_Model]
                                               SET [LDID] = '"+ ldid + "'"+
                                              "where GroupId='" + GroupId + "'";
                        db.AUIDB_WithParam(update_group);

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
                                ('" + data.Member[0].Co_Fname + "', " +
                                     "'" + data.Member[0].Co_Lname + "'," +
                                     "'" + data.Member[0].Co_Mname + "', " +
                                     "'" + data.Member[0].Co_Suffix + "', " +
                                     "'" + data.Member[0].Co_Gender + "', " +
                                     "'" + Convert.ToDateTime(data.Member[0].Co_DOB).ToString("yyyy-MM-dd") + "', " +
                                     "'" + data.Member[0].Co_Age + "', " +
                                     "'" + data.Member[0].Co_POB + "', " +
                                     "'" + data.Member[0].Co_Civil_Status + "', " +
                                     "'" + data.Member[0].Co_Cno + "', " +
                                     "'" + data.Member[0].Co_EmailAddress + "', " +
                                     "'" + data.Member[0].Co_House_Stats + "', " +
                                     "'" + data.Member[0].Co_HouseNo + "', " +
                                     "'" + data.Member[0].Co_Barangay + "', " +
                                     "'" + data.Member[0].Co_City + "', " +
                                     "'" + data.Member[0].Co_Province + "', " +
                                     "'" + data.Member[0].Co_Country + "', " +
                                     "'" + data.Member[0].Co_ZipCode + "', " +
                                     "'" + data.Member[0].Co_YearsStay + "', " +
                                     "'" + data.Member[0].Co_RTTB + "', " +
                                     "'1', " +
                                     "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                     "'" + memid + "') ";

                        results = db.AUIDB_WithParam(Insert_comaker) + " Added";

                        sql = $@"select Top(1) CMID from tbl_CoMaker_Model order by id desc";

                        DataTable tables = db.SelectDb(sql).Tables[0];
                        var cmid = tables.Rows[0]["CMID"].ToString();
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
                                ('" + data.Member[0].Co_JobDescription + "'," +
                                        "'" + data.Member[0].Co_YOS + "'," +
                                        "'" + data.Member[0].Co_CompanyName + "'," +
                                        "'" + data.Member[0].Co_MonthlySalary + "'," +
                                        "'" + data.Member[0].Co_OtherSOC + "'," +
                                        "'1'," +
                                        "'" + data.Member[0].Co_BO_Status + "'," +
                                        "'" + data.Member[0].Co_Emp_Status + "'," +
                                        "'" + cmid + "'," +
                                        "'" + data.Member[0].Co_CompanyAddress + "'," +
                                         "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "') ";



                        results = db.AUIDB_WithParam(insert_jobcomaker) + " Added";

                        if (data.Member[0].Appliances.Count != 0)
                        {
                            sql = $@"select Top(1) NAID from tbl_Application_Model order by id desc";

                            DataTable na_table = db.SelectDb(sql).Tables[0];
                            var naid = na_table.Rows[0]["NAID"].ToString();

                            string na_insert = "";
                            for (int i = 0; i < data.Member[0].Appliances.Count; i++)
                            {

                                na_insert += $@"INSERT INTO[dbo].[tbl_Appliance_Model]
                               ([Brand]
                               ,[Description]
                               ,[NAID]
                               ,[Status]
                               ,[DateCreated])
                                VALUES
                               ('" + data.Member[0].Appliances[i].Brand + "'," +
                                              "'" + data.Member[0].Appliances[i].Appliances + "'," +
                                              "'" + naid + "'," +
                                              "'1'," +
                                              "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            }

                            results = db.AUIDB_WithParam(na_insert) + " Added";


                        }
                        string file_upload = $@"INSERT INTO [dbo].[tbl_fileupload_Model]
                                           ([MemId]
                                           ,[FileName]
                                           ,[FilePath]
                                           ,[Status]
                                           ,[Type]
                                           ,[DateCreated])
                                     VALUES
                                           ('" + memid + "'," +
                                                  "'" + data.Member[0].ProfileName + "'," +
                                                   "'" + data.Member[0].ProfileFilePath + "'," +
                                                  "'1'," +
                                                  "'1'," +
                                                   "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        results = db.AUIDB_WithParam(file_upload) + " Added";


                        string co_profile = $@"INSERT INTO  [dbo].[tbl_CoMakerFileUpload_Model]
                                       ([CMID]
                                       ,[FileName]
                                       ,[FilePath]
                                       ,[Status]
                                       ,[DateCreated])
                                     VALUES
                                           ('" + cmid + "'," +
                                                 "'" + data.Member[0].Co_ProfileName + "'," +
                                                  "'" + data.Member[0].Co_ProfileFilePath + "'," +
                                                 "'1'," +
                                                  "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        results = db.AUIDB_WithParam(co_profile) + " Added";
                        string co_signature_upload = "";
                        if (data.Member[0].Co_SignatureUpload.Count != 0)
                        {
                            for (int w = 0; w < data.Member[0].Co_SignatureUpload.Count; w++)
                            {
                                co_signature_upload = $@"INSERT INTO  [dbo].[tbl_CoMakerFileUpload_Model]
                                       ([CMID]
                                       ,[FileName]
                                       ,[FilePath]
                                       ,[Status]
                                       ,[DateCreated])
                                     VALUES
                                           ('" + cmid + "'," +
                                          "'" + data.Member[0].Co_SignatureUpload[w].FileName + "'," +
                                           "'" + data.Member[0].Co_SignatureUpload[w].FilePath + "'," +
                                          "'3'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                results = db.AUIDB_WithParam(co_signature_upload) + " Added";

                            }

                        }

                        string co_uploadfile = "";
                        if (data.Member[0].Co_RequirementsFile.Count != 0)
                        {
                            for (int e = 0; e < data.Member[0].Co_RequirementsFile.Count; e++)
                            {
                                co_uploadfile = $@"INSERT INTO  [dbo].[tbl_CoMakerFileUpload_Model]
                                       ([CMID]
                                       ,[FileName]
                                       ,[FilePath]
                                       ,[Status]
                                       ,[DateCreated])
                                     VALUES
                                           ('" + cmid + "'," +
                                              "'" + data.Member[0].Co_RequirementsFile[e].FileName + "'," +
                                               "'" + data.Member[0].Co_RequirementsFile[e].FilePath + "'," +
                                              "'2'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                results = db.AUIDB_WithParam(co_uploadfile) + " Added";
                            }

                        }

                        string uploadfile_req = "";
                        if (data.Member[0].RequirementsFile.Count != 0)
                        {
                            for (int a = 0; a < data.Member[0].RequirementsFile.Count; a++)
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
                                              "'" + data.Member[0].RequirementsFile[a].FileName + "'," +
                                               "'" + data.Member[0].RequirementsFile[a].FilePath + "'," +
                                              "'1'," +
                                              "'2'," +
                                               "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                results = db.AUIDB_WithParam(uploadfile_req) + " Added";
                            }


                        }

                        string signature_upload = "";
                        if (data.Member[0].SignatureUpload.Count != 0)
                        {
                            for (int c = 0; c < data.Member[0].SignatureUpload.Count; c++)
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
                                          "'" + data.Member[0].SignatureUpload[c].FileName + "'," +
                                           "'" + data.Member[0].SignatureUpload[c].FilePath + "'," +
                                          "'1'," +
                                          "'3'," +
                                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                results = db.AUIDB_WithParam(signature_upload) + " Added";
                            }


                        }


                    }
                    else
                    {
                        return BadRequest("Member Already Exist!");
                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
            return Ok(results);
        }
    }

}
