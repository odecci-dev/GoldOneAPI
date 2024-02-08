using AuthSystem.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using static GoldOneAPI.Controllers.ApplicationController;
using static GoldOneAPI.Controllers.ApprovalController;
using static GoldOneAPI.Controllers.CollectionController;
using static GoldOneAPI.Controllers.CreditController;
using static GoldOneAPI.Controllers.FieldAreaController;
using static GoldOneAPI.Controllers.FieldOfficerController;
using static GoldOneAPI.Controllers.GroupController;
using static GoldOneAPI.Controllers.HolidayController;
using static GoldOneAPI.Controllers.LoanSummaryController;
using static GoldOneAPI.Controllers.LoanTypeController;
using static GoldOneAPI.Controllers.MemberController;
using static GoldOneAPI.Controllers.NotificationController;
using static GoldOneAPI.Controllers.ReportsController;
using static GoldOneAPI.Controllers.SettingsController;
using static GoldOneAPI.Controllers.UserRegistrationController;

namespace GoldOneAPI.Manager
{
    public class DBMethods
    {
        string sql = "";
        DbManager db = new DbManager();
        public int CountSundaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            int sundayCount = 0;
            DateTime currentDate = startDate;

            // Iterate through each day from start date to end date
            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    sundayCount++;
                }

                // Move to the next day
                currentDate = currentDate.AddDays(1);
            }

            return sundayCount;
        }
        public string insertlgos(string filepath, string logs)
        {
            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));


            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data[0]));
            //



            // Read the existing content of the file
            string existingContent = System.IO.File.ReadAllText(filepath);

            // Create a StringBuilder to manipulate the content
            StringBuilder sb = new StringBuilder(existingContent);

            // Insert the new text at a specific position (e.g., at the beginning of the file)
            sb.Insert(0, logs + " \n------------------" + DateTime.Now + "--------------- \n");

            // Write the modified content back to the file
            System.IO.File.WriteAllText(filepath, sb.ToString());


            return "";


        }

        public string InsertStringsAtIndexes(string originalString, int index1, string stringToInsert1, int index2, string stringToInsert2, int index3, string stringToInsert3)
        {
            StringBuilder stringBuilder = new StringBuilder(originalString);
            stringBuilder.Insert(index1, stringToInsert1);
            stringBuilder.Insert(index2 + stringToInsert1.Length, stringToInsert2);
            stringBuilder.Insert(index3 + stringToInsert1.Length + stringToInsert2.Length, stringToInsert3);
            return stringBuilder.ToString();
        }
        public string InsertNotification(string actions, string datecreated, string module, string name, string userid, string read, string reference)
        {
            string Insert = $@"INSERT INTO [dbo].[tbl_Notifications_Model]
                               ([Actions]
                               ,[DateCreated]
                               ,[Module]
                               ,[Name]
                               ,[UserId]
                               ,[isRead]
                               ,[Reference])
                         VALUES
                               ('" + actions + "'," +
                              "'" + datecreated + "'," +
                             "'" + module + "'," +
                             "'" + name + "'," +
                             "'" + userid + "'," +
                             "'" + read + "'," +
                              "'" + reference + "') ";

            return db.AUIDB_WithParam(Insert);
        }
        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomStringBuilder = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                randomStringBuilder.Append(chars[index]);
            }

            return randomStringBuilder.ToString();
        }
        public List<UserVM> GetUserList()
        {

            var result = new List<UserVM>();

            DataTable dt = db.SelectDb_SP("sp_userlist").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new UserVM();
                item.Id = int.Parse(dr["id"].ToString());
                item.Username = dr["Username"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Fullname = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                //item.Suffix = dr["Suffix"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.Address = dr["Address"].ToString();
                item.StatusName = dr["Status"].ToString();
                item.UserType = dr["UserType"].ToString();
                item.UserTypeId = dr["UserTypeId"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                item.UserId = dr["UserId"].ToString();
                item.ProfilePath = dr["ProfilePath"].ToString();
                item.FOID = dr["FOID"].ToString();
                item.FieldOfficer = dr["FO_Fname"].ToString() + ", " + dr["FO_Lname"].ToString();
                result.Add(item);
            }

            return result;
        }
        public List<MemberDisplay> GetMemberDisplay()
        {
            string applicationfilter = $@"SELECT  tbl_Member_Model.Fname, tbl_Member_Model.Lname, tbl_Member_Model.Mname, tbl_Member_Model.Suffix, tbl_Status_Model.Name AS Status, tbl_Status_Model.Id AS StausId, tbl_Member_Model.MemId, 
                         tbl_LoanDetails_Model.LoanAmount, tbl_LoanHistory_Model.OutstandingBalance, tbl_Member_Model.DateUpdated,tbl_Application_Model.NAID,tbl_fileupload_Model.FilePath
						 ,profiles.TypeName
                         FROM            tbl_Application_Model INNER JOIN
						 tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
						 tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId inner join 
                         tbl_Status_Model ON tbl_Member_Model.Status = tbl_Status_Model.Id left JOIN
                         tbl_LoanHistory_Model ON tbl_Member_Model.MemId = tbl_LoanHistory_Model.MemId inner join
						 tbl_fileupload_Model on tbl_Member_Model.MemId = tbl_fileupload_Model.MemId inner join 
						 tbl_TypesModel as profiles  on tbl_fileupload_Model.Type = profiles.Id  where tbl_fileupload_Model.Type = 1";
            DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];
            var result = new List<MemberDisplay>();
            foreach (DataRow dr in tbl_applicationfilter.Rows)
            {
                var item = new MemberDisplay();
                item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Suffix"].ToString();
                item.Status = dr["Status"].ToString();
                item.CurrentLoan = dr["LoanAmount"].ToString();
                item.OutstandingBalance = dr["OutstandingBalance"].ToString() == "" ? "0": dr["OutstandingBalance"].ToString();
                item.LastUpdated = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd");
                item.MemId = dr["MemId"].ToString();
                item.Naid = dr["NAID"].ToString();
                item.ProfilePath = dr["FilePath"].ToString();

                result.Add(item);
            }

            return result;
        }
        public List<DeclineReports> Report_Decline()
        {
            string applicationfilter = $@"select tbl_Member_Model.Fname,  tbl_Member_Model.Lname, tbl_Member_Model.Mname , tbl_Member_Model.Suffix, tbl_Application_Model.NAID, tbl_Application_Model.Remarks, tbl_LoanDetails_Model.LoanAmount from tbl_Application_Model
                        inner join 
                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                        tbl_Member_Model on tbl_Member_Model.MemId = tbl_LoanDetails_Model.MemId where tbl_Application_Model.Status =11";
            DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];
            var result = new List<DeclineReports>();
            foreach (DataRow dr in tbl_applicationfilter.Rows)
            {
                var item = new DeclineReports();
                item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Suffix"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Remarks = dr["Remarks"].ToString();
                item.LoanAmount = dr["LoanAmount"].ToString();
                result.Add(item);
            }

            return result;
        }
        public List<NotificationVM> GetNotificationList()
        {

            var result = new List<NotificationVM>();

            DataTable dt = db.SelectDb_SP("sp_NotificationList").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                var item = new NotificationVM();
                item.Id = dr["Id"].ToString();
                item.Actions = dr["Actions"].ToString();
                item.DateCreated = datec;
                item.Module = dr["Module"].ToString();
                item.UserId = dr["UserId"].ToString();
                item.Reference = dr["Reference"].ToString();
                item.Name = dr["Name"].ToString();
                item.isRead = dr["isRead"].ToString() == "2" ? "Unread" : "Read";

                result.Add(item);
            }

            return result;
        }

        public List<CollectionTotals> GetCollectionDetailList(string areaid)
        {
            string pastamount = "0";


            var areas = getAreaLoanSummary().GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.FOID, a.Area_RefNo }).ToList();
            var list = getAreaLoanSummary().Where(a => a.AreaID == areaid).ToList();
            var res = new List<CollectionTotals>();

            var dailyCollectiblesSum = getAreaLoanSummary().Where(a => a.AreaID == areaid).Select(a => double.Parse(a.DailyCollectibles)).Sum();
            var savings = getAreaLoanSummary().Where(a => a.AreaID == areaid).Select(a => double.Parse(a.TotalSavingsAmount)).Sum();
            var balance = getAreaLoanSummary().Where(a => a.AreaID == areaid).Select(a => double.Parse(a.AmountDue)).Sum();
            var advance = getAreaLoanSummary().Where(a => a.AreaID == areaid).Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
            var lapses = getAreaLoanSummary().Where(a => a.AreaID == areaid).Select(a => double.Parse(a.LapsePayment)).Sum();
            var collectedamount = getAreaLoanSummary().Where(a => a.AreaID == areaid).Select(a => double.Parse(a.CollectedAmount)).Sum();

            var items = new CollectionTotals();
            items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
            items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
            items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
            items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
            items.Total_lapses = Math.Ceiling(lapses);
            items.Total_collectedAmount = Math.Ceiling(collectedamount);
            var collection_list = new List<CollectionVM>();
            DateTime dueDate = Convert.ToDateTime(Convert.ToDateTime(list[0].DateCollected).ToString("yyyy-MM-dd"));// Get the due date from your database
            string payment_status = "";
            // Get the current date and time
            DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            if (list.Count != 0)
            {
                if (dueDate < currentDate)
                {
                    payment_status = "No Payment";
                }
                else
                {
                    payment_status = "Paid";
                }

                for (int x = 0; x < list.Count; x++)
                {
                    var item = new CollectionVM();
                    item.Borrower = list[x].Borrower;
                    item.Cno = list[x].Cno;
                    item.Co_Borrower = list[x].Co_Fname + " " + list[x].Co_Mname + " " + list[x].Co_Lname;
                    item.Co_Cno = list[x].Co_Cno;
                    item.LoanPrincipal = list[x].LoanPrincipal;
                    item.ReleasingDate = list[x].ReleasingDate;
                    item.DueDate = list[x].DueDate;
                    item.TypeOfCollection = list[x].TypeOfCollection;
                    item.TotalSavingsAmount = list[x].TotalSavingsAmount;
                    item.DailyCollectibles = list[x].DailyCollectibles;
                    item.LapsePayment = list[x].LapsePayment;
                    item.AdvancePayment = list[x].AdvancePayment;
                    item.Payment_Status = list[x].Payment_Status;
                    item.Collection_Status = list[x].Collection_Status;
                    item.MemId = list[x].MemId;
                    item.FilePath = list[x].FilePath;
                    item.NAID = list[x].NAID;
                    item.AmountDue = list[x].AmountDue;
                    item.DateOfFullPayment = list[x].FilePath == null ? "Ongoing Collection" : list[x].DateOfFullPayment;
                    item.CollectedAmount = list[x].CollectedAmount;
                    item.AreaID = list[x].AreaID;
                    item.AreaName = list[x].AreaName;
                    item.Area_RefNo = list[x].Area_RefNo;
                    item.Collection_RefNo = list[x].Collection_RefNo;
                    item.FOID = list[x].FOID;
                    item.FieldOfficer = list[x].FieldOfficer;
                    item.Payment_Method = payment_status;
                    item.ApprovedAdvancePayment = list[x].ApprovedAdvancePayment;
                    item.DateCreated = list[x].DateCreated;
                    if (DateTime.TryParseExact(list[x].DueDate.ToString(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime expirationDate))
                    {


                        // Compare the parsed expiration date with the current date.
                        if (expirationDate < currentDate)
                        {
                            pastamount = list[x].AmountDue;
                        }
                        else
                        {
                            pastamount = "0.00";
                        }

                    }
                    item.PastDue = pastamount;
                    collection_list.Add(item);

                }

            }
            items.Collection = collection_list;
            res.Add(items);

            return res;
        }
        public List<AreaDetailsVM> GetCollectionDetailListFilterbyRefno(string refno)
        {

            var areas = getAreaLoanSummary().GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.FOID, a.Area_RefNo }).ToList();
            var list = getAreaLoanSummary().ToList();
            bool containsNow = list.Any(dt => dt.ToString() == DateTime.Parse(Convert.ToDateTime(list[0].DateCreated).ToString("yyyy-MM-dd")).ToString());
            var res = new List<AreaDetailsVM>();
            foreach (var group in areas)
            {
                DateTime dueDate = Convert.ToDateTime(Convert.ToDateTime(list[0].DateCreated).ToString("yyyy-MM-dd"));
                DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));


                var advance_payment = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID && a.AdvancePayment != null).Select(a => double.Parse(a.AdvancePayment)).Sum();

                var dailyCollectiblesSum = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.DailyCollectibles)).Sum();
                var savings = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.TotalSavingsAmount)).Sum();
                var balance = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.AmountDue)).Sum();
                var advance = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                var lapses = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.LapsePayment)).Sum();
                var collectedamount = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.CollectedAmount)).Sum();

                var items = new AreaDetailsVM();
                items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
                items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
                items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
                items.Total_lapses = Math.Ceiling(lapses);
                items.Total_collectedAmount = Math.Ceiling(collectedamount);
                items.AreaName = group.Key.AreaName;
                items.AreaID = group.Key.AreaID;
                items.FieldOfficer = group.Key.FieldOfficer;
                items.FOID = group.Key.FOID;
                items.Area_RefNo = group.Key.Area_RefNo;
                //items.Collection_RefNo = group.Key.Collection_RefNo;
                //items.DateCreated = group.Key.DateCreated;
                items.TotalItems = list.Where(a => a.Area_RefNo == group.Key.Area_RefNo).ToList().Count.ToString();
                items.ExpectedCollection = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.AdvancePayment = Math.Ceiling(advance_payment);
                res.Add(items);
                //}

                //var dailyCollectiblesSum = dbmet.getAreaLoanSummary().Where(a=>a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.DailyCollectibles)).Sum();
                ///
            }
            return res;
        }


        public List<CollectionTotals> GetCollectionDetailListFilterbyAreaRefno(string areaid, string area_refno)
        {

            var res = new List<CollectionTotals>();
            var collection_list = new List<CollectionVM>();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string datec = "";
            string Collection_Status_Id = "";
            string FieldExpenses = "0";
            string Area_RefNo = "PENDING";
            string Collection_RefNo = "PENDING";
            string Collection_Status = "NO PAYMENT";
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
                        where tbl_Area_Model.AreaID ='" + areaid + "'";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {
                string reference = $@"select
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status ,
                CASE 
                WHEN tbl_CollectionArea_Model.Collection_Status IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.Collection_Status END as Collection_Status_Id,
                CASE 
                WHEN tbl_CollectionArea_Model.Area_RefNo IS NULL THEN 'PENDING'
                ELSE tbl_CollectionArea_Model.Area_RefNo END AS Area_RefNo,
                CASE
                WHEN tbl_CollectionModel.RefNo IS NULL THEN 'PENDING' 
                ELSE  tbl_CollectionModel.RefNo  END as Collection_RefNo,
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status,tbl_CollectionModel.DateCreated,
				 CASE 
                WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.FieldExpenses END as FieldExpenses
                from 
                tbl_CollectionArea_Model left join
                tbl_CollectionModel on tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo left join
                tbl_CollectionStatus_Model as col_stats on col_stats.Id = tbl_CollectionArea_Model.Collection_Status 
                where tbl_CollectionArea_Model.AreaID = '" + dr_area["AreaID"].ToString() + "' and tbl_CollectionArea_Model.Area_RefNo ='" + area_refno + "'";
                DataTable tbl_reference = db.SelectDb(reference).Tables[0];
                if (tbl_reference.Rows.Count != 0)
                {
                    datec = tbl_reference.Rows[0]["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(tbl_reference.Rows[0]["DateCreated"].ToString()).ToString("yyyy-MM-dd");//
                    Collection_Status_Id = "0";
                    Area_RefNo = tbl_reference.Rows[0]["Area_RefNo"].ToString();
                    Collection_RefNo = tbl_reference.Rows[0]["Collection_RefNo"].ToString();
                    Collection_Status = tbl_reference.Rows[0]["Collection_Status"].ToString();
                    FieldExpenses = tbl_reference.Rows[0]["FieldExpenses"].ToString();
                }

                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    string barangay = spliter[0];
                    string city = spliter[1];

                    string sql_applicationdetails = $@"
                
                    select 
                    tbl_Member_Model.Fname,
                    tbl_Member_Model.Mname,
                    tbl_Member_Model.Lname,
                    tbl_Member_Model.Suffix,
                    tbl_Member_Model.Cno,
                    tbl_Member_Model.MemId,
                    tbl_Application_Model.NAID,
                    tbl_CoMaker_Model.Fname as Co_Fname,
                    tbl_CoMaker_Model.Mname as Co_Mname,
                    tbl_CoMaker_Model.lnam  as Co_Lname,
                    tbl_CoMaker_Model.Suffi as Co_Suffix,
                    tbl_CoMaker_Model.Cno as Co_Cno,
                    tbl_Application_Model.Remarks ,

                    tbl_LoanHistory_Model.Penalty,
                    Case when tbl_LoanDetails_Model.ApprovedDailyAmountDue  IS NULL then 0 
                    else tbl_LoanDetails_Model.ApprovedDailyAmountDue end
                    as DailyCollectibles,
                    case when tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0
                    ELSE tbl_LoanHistory_Model.OutstandingBalance end as AmountDue ,
                    CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DueDate END AS DueDate,
                    case when tbl_LoanHistory_Model.DateOfFullPayment  IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS  DateOfFullPayment,
                    CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL THEN 0 
                    ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount,
                    CASE WHEN tbl_LoanDetails_Model.ApprovedAdvancePayment IS NULL THEN 0 
                    ELSE tbl_LoanDetails_Model.ApprovedAdvancePayment  END AS ApprovedAdvancePayment,
                    tbl_LoanDetails_Model.LoanAmount as LoanPrincipal,
                    tbl_Application_Model.ReleasingDate , 
                    tbl_TermsTypeOfCollection_Model.TypeOfCollection,
                    CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount,
                    CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment,
                    CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment,
                    tbl_Collection_AreaMember_Model.Payment_Status as Payment_Status_Id,
                        CASE
                            WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING'
                            ELSE tbl_CollectionStatus_Model.[Status]
                        END as Payment_Status,


                CASE 
                WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT'
                WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT'
                ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method,




                CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                ELSE tbl_Collection_AreaMember_Model.DateCollected  END AS DateCollected,

                CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND'
                ELSE file_.FilePath END AS FilePath,

                tbl_LoanDetails_Model.ModeOfRelease,
                tbl_LoanDetails_Model.ModeOfReleaseReference,
                tbl_TermsOfPayment_Model.LoanInsuranceAmount,
                tbl_TermsOfPayment_Model.LifeInsuranceAmount, tbl_TermsTypeOfCollection_Model.Value
                from
                tbl_Application_Model left join 
                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_Member_Model on tbl_LoanDetails_Model.MemId  = tbl_Member_Model.MemId left join 
                tbl_LoanHistory_Model on tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId left JOIN 
                tbl_Collection_AreaMember_Model on tbl_Collection_AreaMember_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId left JOIN
                tbl_MemberSavings_Model on tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId left JOIN
                tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId left JOIN
                tbl_TermsTypeOfCollection_Model on tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId left JOIN
                tbl_CollectionStatus_Model on tbl_CollectionStatus_Model.Id = tbl_Collection_AreaMember_Model.Payment_Status left join 
                (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId
                where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "' and tbl_Application_Model.Status = 14 " +
                "and tbl_Collection_AreaMember_Model.Payment_Status  ='2' and tbl_Collection_AreaMember_Model.Payment_Method ='No Payment' and tbl_Collection_AreaMember_Model.Area_RefNo = '" + area_refno + "' ";
                    DataTable tbl_application_details = db.SelectDb(sql_applicationdetails).Tables[0];
                    var items = new CollectionTotals();
                    if (tbl_application_details.Rows.Count != 0)
                    {
                        if (tbl_application_details.Rows[0]["TypeOfCollection"].ToString() == "Daily")
                        {
                            foreach (DataRow dr in tbl_application_details.Rows)
                            {
                                var item = new CollectionVM();
                                item.Borrower = dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Lname"].ToString() + ", " + dr["Suffix"].ToString();
                                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                item.LapsePayment = dr["LapsePayment"].ToString();
                                item.AdvancePayment = dr["AdvancePayment"].ToString();
                                item.Payment_Method = dr["Payment_Method"].ToString();
                                item.Payment_Status = dr["Payment_Status"].ToString();
                                item.MemId = dr["MemId"].ToString();
                                item.NAID = dr["NAID"].ToString();
                                item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                item.AreaID = dr_area["AreaID"].ToString();
                                item.Area_RefNo = Area_RefNo;
                                item.CollectedAmount = dr["CollectedAmount"].ToString();


                                collection_list.Add(item);
                            }
                            items.Collection = collection_list;
                            res.Add(items);

                        }
                        else
                        {

                            int day_val = int.Parse(tbl_application_details.Rows[0]["Value"].ToString());
                            DateTime date1 = Convert.ToDateTime(currentDate.ToString());
                            DateTime date2 = Convert.ToDateTime(tbl_application_details.Rows[0]["ReleasingDate"].ToString()).AddDays(day_val);
                            TimeSpan difference = date2 - date1;
                            int dayDifference = difference.Days;
                            if (dayDifference == 0)
                            {
                                foreach (DataRow dr in tbl_application_details.Rows)
                                {
                                    var item = new CollectionVM();
                                    item.Borrower = dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Lname"].ToString() + ", " + dr["Suffix"].ToString();
                                    item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                    item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                    item.LapsePayment = dr["LapsePayment"].ToString();
                                    item.AdvancePayment = dr["AdvancePayment"].ToString();
                                    item.Payment_Method = dr["Payment_Method"].ToString();
                                    item.Payment_Status = dr["Payment_Status"].ToString();
                                    item.MemId = dr["MemId"].ToString();
                                    item.NAID = dr["NAID"].ToString();
                                    item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                    item.AreaID = dr_area["AreaID"].ToString();
                                    item.Area_RefNo = Area_RefNo;
                                    item.CollectedAmount = dr["CollectedAmount"].ToString();


                                    collection_list.Add(item);
                                }
                                items.Collection = collection_list;
                                res.Add(items);

                            }
                        }
                    }
                }


            }

            return res;
        }
        public List<CIDateModel> GetDateCreditInvestigation()
        {

            var result = new List<CIDateModel>();

            DataTable dt = db.SelectDb_SP("getDateCI").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var ProceedToCIDate = dr["ProceedToCIDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ProceedToCIDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new CIDateModel();
                item.MemId = dr["MemId"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.CI_ApprovedDate = CI_ApprovalDate;
                item.Status = dr["Status"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovalDate_1 = App_ApprovalDate_1;
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovalDate_2 = App_ApprovalDate_2;
                item.ProceedToCIDate = ProceedToCIDate;
                result.Add(item);
            }

            return result;
        }
        public List<AllMemberModel> GetAllMemberList()
        {

            var result = new List<AllMemberModel>();

            DataTable dt = db.SelectDb_SP("sp_AllMemberList").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new AllMemberModel();
                item.MemId = dr["MemId"].ToString();
                item.Member = dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.Age = dr["Age"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.City = dr["City"].ToString();
                item.Civil_Status = dr["Civil_Status"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.Country = dr["Country"].ToString();
                item.DOB = dr["DOB"].ToString();
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.HouseStatus = dr["HouseStatus"].ToString();
                item.HouseStatusID = dr["HouseStatusID"].ToString();
                item.POB = dr["POB"].ToString();
                item.Province = dr["Province"].ToString();
                item.YearsStay = dr["YearsStay"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.STATUS = dr["STATUS"].ToString();
                item.StatusID = dr["StatusID"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;

                result.Add(item);
            }

            return result;
        }

        public List<AllMemberModel> GetAllMemberList_daily()
        {

            var result = new List<AllMemberModel>();

            DataTable dt = db.SelectDb_SP("sp_AllMemberList_daily").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new AllMemberModel();
                item.MemId = dr["MemId"].ToString();
                item.Member = dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.Age = dr["Age"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.City = dr["City"].ToString();
                item.Civil_Status = dr["Civil_Status"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.Country = dr["Country"].ToString();
                item.DOB = dr["DOB"].ToString();
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.HouseStatus = dr["HouseStatus"].ToString();
                item.HouseStatusID = dr["HouseStatusID"].ToString();
                item.POB = dr["POB"].ToString();
                item.Province = dr["Province"].ToString();
                item.YearsStay = dr["YearsStay"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.STATUS = dr["STATUS"].ToString();
                item.StatusID = dr["StatusID"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;

                result.Add(item);
            }

            return result;
        }
        public List<CollectionTypeVM> GetTypeOfCollection()
        {

            var result = new List<CollectionTypeVM>();

            DataTable dt = db.SelectDb_SP("sp_CollectionType").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var item = new CollectionTypeVM();
                item.Id = dr["Id"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();

                result.Add(item);
            }

            return result;
        }
        public List<UserModuleVM> GetUserModuleList()
        {

            var result = new List<UserModuleVM>();

            DataTable dt = db.SelectDb_SP("sp_userModuleList").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new UserModuleVM();
                item.module_name = dr["module_name"].ToString();
                item.module_code = dr["module_code"].ToString();
                item.module_category = dr["module_category"].ToString();
                item.module_code = dr["module_code"].ToString();
                item.Fullname = dr["Fullname"].ToString();
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.UserId = dr["user_id"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;

                result.Add(item);
            }

            return result;
        }
        public List<ModuleVM> GetModuleList()
        {

            var result = new List<ModuleVM>();

            DataTable dt = db.SelectDb_SP("sp_modulelist").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new ModuleVM();
                item.module_name = dr["module_name"].ToString();
                item.module_code = dr["module_code"].ToString();
                item.module_category = dr["module_category"].ToString();
                item.created_by = dr["created_by"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;

                result.Add(item);
            }

            return result;
        }
        public List<CreditModel> GetCreditList()
        {
            var result = new List<CreditModel>();
            try
            {
                DataTable table = db.SelectDb_SP("sp_creditList").Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var datea = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var ci_approvaldate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var releasingdate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    var item = new CreditModel();
                    item.DateCreated = datec;
                    item.DateApproval = datea;
                    item.Remarks = dr["Remarks"].ToString();
                    item.NAID = dr["NAID"].ToString();
                    item.CI_Status = dr["CI_Status"].ToString();
                    item.Fname = dr["Fname"].ToString();
                    item.Mname = dr["Mname"].ToString();
                    item.Lname = dr["Lname"].ToString();
                    item.Suffix = dr["Suffix"].ToString();
                    item.Fullname = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                    item.Cno = dr["Cno"].ToString();
                    item.LoanAmount = dr["LoanAmount"].ToString();
                    item.LoanTypeID = dr["LoanTypeID"].ToString();
                    item.NameOfTerms = dr["NameOfTerms"].ToString();
                    item.Days = dr["Days"].ToString();
                    item.LAL_Type = dr["LAL_Type"].ToString();
                    item.Loan_amount_Lessthan = dr["Loan_amount_Lessthan"].ToString();
                    item.LAG_Type = dr["LAG_Type"].ToString();
                    item.Loan_amount_Greater = dr["Loan_amount_GreaterEqual"].ToString();
                    item.LoanInsurance = dr["LoanInsurance"].ToString();
                    item.LoanI_Type = dr["LoanI_Type"].ToString();
                    item.LifeInsurance = dr["LifeInsurance"].ToString();
                    item.LifeI_Type = dr["LifeI_Type"].ToString();
                    item.CI_ApprovalDate = ci_approvaldate;
                    item.ReleasingDate = releasingdate;

                    double LoanInterest = 0;
                    double amount = Convert.ToDouble(dr["LoanAmount"].ToString());
                    double percentage = Convert.ToDouble(dr["InterestRate"].ToString());
                    LoanInterest = amount * percentage;
                    item.InterestAmount = LoanInterest.ToString("0.00");
                    item.InterestRate = dr["InterestRate"].ToString();
                    item.IR_Type = dr["IR_Type"].ToString();
                    item.MemId = dr["MemId"].ToString();
                    item.LoanTypeName = dr["LoanTypeName"].ToString();
                    string Formula = dr["Formula"].ToString();
                    int index1 = 0;
                    string LoanAmount = "Loan Amount";
                    int index2 = 0;
                    string Interest = "Interest";
                    int index3 = 0;
                    string Days = "Days";
                    int countlength = dr["Formula"].ToString().Length;
                    if (countlength == 4)
                    {
                        index3 = 4;

                    }
                    else
                    {
                        index1 = 2;
                        index2 = 3;
                        index3 = 5;
                    }
                    item.Formula = InsertStringsAtIndexes(Formula, index1, LoanAmount, index2, Interest, index3, Days);
                    result.Add(item);

                }

                return result;
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public List<CreditModel> GetCreditListFilter(string borrower)
        {
            var list = GetCreditList().Where(a => a.Fullname.Contains(borrower)).ToList();
            return list;
        }

        public List<SettingsModel> GetSettingList()
        {

            var result = new List<SettingsModel>();
            DataTable dt = db.SelectDb_SP("sp_settingslist").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var item = new SettingsModel();
                item.Id = dr["Id"].ToString();
                item.MonthlyTarget = dr["MonthlyTarget"].ToString();
                item.DisplayReset = dr["DisplayReset"].ToString();
                item.CompanyCno = dr["CompanyCno"].ToString();
                item.CompanyAddress = dr["CompanyAddress"].ToString();
                item.CompanyEmail = dr["CompanyEmail"].ToString();
                result.Add(item);
            }

            return result;


        }
        public List<ApplicationVM> GetIndividualList()
        {

            var result = new List<ApplicationVM>();
            DataTable dt = db.SelectDb_SP("sp_GetIndividualList").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datea = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new ApplicationVM();
                item.MemId = dr["MemId"].ToString();
                item.DateCreated = datec;
                item.DateApproval = datea;
                item.Remarks = dr["Remarks"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Status = dr["Status"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                item.Borrower = dr["Borrower"].ToString();
                item.LoanAmount = dr["LoanAmount"].ToString();
                item.LoanType = dr["LoanTypeName"].ToString();
                item.BorrowerCno = dr["BorrowerCno"].ToString();
                item.CoBorrower = dr["CoBorrower"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                result.Add(item);
            }

            return result;


        }
        public List<MonthlyReferenceModel> GetMonthlyReferenceNo()
        {

            var result = new List<MonthlyReferenceModel>();
            DataTable dt = db.SelectDb_SP("GetMonthlyProcedure").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new MonthlyReferenceModel();
                item.Id = dr["Id"].ToString();
                item.ReferenceNo = dr["ReferenceNo"].ToString();
                item.DateCreated = datec;
                result.Add(item);
            }

            return result;


        }
        public List<HolidayListVM> GetHolidayList()
        {

            var result = new List<HolidayListVM>();
            DataTable dt = db.SelectDb_SP("GetHolidayList").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new HolidayListVM();
                item.HolidayName = dr["HolidayName"].ToString();
                item.Date = Convert.ToDateTime(dr["Date"].ToString()).ToString("yyyy-MM-dd");
                item.Location = dr["Location"].ToString();
                item.RepeatYearly = dr["RepeatYearly"].ToString();
                item.DateCreatd = datec;
                item.DateUpdated = dateu;
                item.Status = dr["Status"].ToString();
                item.HolidayID = dr["HolidayID"].ToString();
                result.Add(item);
            }

            return result;


        }

        //public List<CollectionModel> GetCollectiblesList()
        //{

        //    var result = new List<CollectionModel>();
        //    DataTable dt = db.SelectDb_SP("GetCollectionList").Tables[0];

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
        //        var item = new CollectionModel();
        //        item.Id = dr["Id"].ToString();
        //        item.MonthlyRefNo = dr["MonthlyRefNo"].ToString();
        //        item.RefNo = dr["RefNo"].ToString();
        //        item.DateCreated = datec;
        //        result.Add(item);
        //    }

        //    return result;


        //}
        public List<LoanDetailsVM2> GetLoanDetailsApproval()
        {

            var result = new List<LoanDetailsVM2>();
            DataTable dt = db.SelectDb_SP("GetLoanDetailsIndividual").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new LoanDetailsVM2();
                item.MemId = dr["MemId"].ToString();
                item.DateCreated = datec;
                item.DateApproval = dateu;
                item.Remarks = dr["Remarks"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Status = dr["Status"].ToString();
                item.LoanType = dr["LoanTypeName"].ToString();
                item.TermsOfPayment = dr["TermsOfPayment"].ToString();
                item.LoanAmount = dr["LoanAmount"].ToString();
                item.Purpose = dr["Purpose"].ToString();
                item.LDID = dr["LDID"].ToString();
                item.RefNo = dr["RefNo"].ToString();
                var loancount = GetLoanHistory().Where(a => a.MemId == dr["MemId"].ToString()).ToList();
                var paymentcount = GetPaymentHistory().Where(a => a.MemId == dr["MemId"].ToString()).ToList();
                item.No_of_Loans = loancount.Count.ToString() == "0" ? "0" : loancount.Count.ToString();
                item.No_of_Payment = paymentcount.Count.ToString() == "0" ? "0" : paymentcount.Count.ToString();

                result.Add(item);
            }

            return result;


        }
        public List<PaymentModel> GetPaymentHistory()
        {

            var result = new List<PaymentModel>();
            DataTable dt = db.SelectDb_SP("GetPaymentHistory").Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var paymentdate = dr["PaymentDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["PaymentDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datecreated = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new PaymentModel();
                item.MemId = dr["MemId"].ToString();
                item.DateCreated = datecreated;
                item.LoanAmount = dr["LoanAmount"].ToString();
                item.OutStandingBalance = dr["OutStandingBalance"].ToString();
                item.PaidAmount = dr["PaidAmount"].ToString();
                item.Collector = dr["Collector"].ToString();
                item.PaymentDate = paymentdate;
                item.PaymentType = dr["PaymentType"].ToString();
                item.Penalty = dr["Penalty"].ToString();

                result.Add(item);
            }

            return result;


        }
        public List<GroupVM> GetGroupListMember()
        {

            var result = new List<GroupVM>();
            DataTable dt = db.SelectDb_SP("sp_group").Tables[0];

            foreach (DataRow _dr in dt.Rows)
            {
                var DateUpdated = _dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(_dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datecreated = _dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(_dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var _item = new GroupVM();
                _item.DateCreated = datecreated;
                _item.DateUpdated = DateUpdated;
                _item.GroupID = _dr["GroupID"].ToString();
                _item.GroupName = _dr["GroupName"].ToString();
                _item.StatusId = _dr["StatusId"].ToString();
                _item.Status = _dr["Status"].ToString();
                _item.ApplicationStatusId = _dr["ApplicationStatusId"].ToString();
                _item.ApplicationStatus = _dr["ApplicationStatus"].ToString();

                //grouploan
                string sql_group = $@"SELECT [Id]
                                  ,[MemId]
                                  ,[GroupId]
                                  ,[DateCreated]
                                  ,[DateUpdated]
                              FROM [dbo].[tbl_GroupDetails_Model]
                            WHERE        (GroupId = '" + _dr["GroupID"].ToString() + "')";

                DataTable mem_table = db.SelectDb(sql_group).Tables[0];

                var memid = mem_table.Rows[0]["MemId"].ToString();
                var param = new IDataParameter[]
              {
                    new SqlParameter("@MemId",memid)
              };
                DataTable table = db.SelectDb_SP("[sp_groupdetaillist]", param).Tables[0];

                var mem_res = new List<GroupModelVM>();
                foreach (DataRow dr in table.Rows)
                {

                    var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    string BOstatus = dr["BO_Status"].ToString() == "true" ? "1" : "0";
                    var item = new GroupModelVM();
                    item.Fullname = dr["Fullname"].ToString();
                    item.Fname = dr["Fname"].ToString();
                    item.Lname = dr["Lname"].ToString();
                    item.Mname = dr["Mname"].ToString();
                    item.Suffix = dr["Suffix"].ToString();
                    item.Age = dr["Age"].ToString();
                    item.Barangay = dr["Barangay"].ToString();
                    item.City = dr["City"].ToString();
                    item.Civil_Status = dr["Civil_Status"].ToString();
                    item.Cno = dr["Cno"].ToString();
                    item.House_Stats = dr["House_Stats"].ToString();
                    item.Country = dr["Country"].ToString();
                    item.DOB = dr["DOB"].ToString();
                    item.EmailAddress = dr["EmailAddress"].ToString();
                    item.Gender = dr["Gender"].ToString();
                    item.HouseNo = dr["HouseNo"].ToString();
                    item.HouseStatusId = dr["HouseStatusId"].ToString();
                    item.Co_HouseStatusId = dr["Co_HouseStatusId"].ToString();
                    item.POB = dr["POB"].ToString();
                    item.Province = dr["Province"].ToString();
                    item.MemId = dr["MemId"].ToString();
                    item.Status = dr["MemberStatus"].ToString();
                    item.DateCreated = datec;
                    item.YearsStay = dr["YearsStay"].ToString();
                    item.ZipCode = dr["ZipCode"].ToString();
                    //monthlybills
                    item.ElectricBill = dr["ElectricBill"].ToString();
                    item.WaterBill = dr["WaterBill"].ToString();
                    item.ElectricBill = dr["ElectricBill"].ToString();
                    item.OtherBills = dr["OtherBills"].ToString();
                    item.DailyExpenses = dr["DailyExpenses"].ToString();
                    //job
                    item.Emp_Status = dr["Emp_Status"].ToString();
                    item.BO_Status = BOstatus;
                    item.OtherSOC = dr["OtherSOC"].ToString();
                    item.MonthlySalary = dr["MonthlySalary"].ToString();
                    item.CompanyName = dr["CompanyName"].ToString();
                    item.CompanyAddress = dr["CompanyAddress"].ToString();
                    item.YOS = dr["YOS"].ToString();
                    item.JobDescription = dr["JobDescription"].ToString();
                    item.CompanyAddress = dr["CompanyAddress"].ToString();

                    //famv
                    var famnod = dr["Fam_NOD"].ToString() == "" ? "0" : dr["Fam_NOD"].ToString();
                    var F_YOS = dr["Fam_YOS"].ToString() == "" ? "0" : dr["Fam_YOS"].ToString();
                    var F_Age = dr["Fam_Age"].ToString() == "" ? "0" : dr["Fam_Age"].ToString();
                    var F_Emp_Status = dr["Fam_EmpStatus"].ToString() == "" ? "0" : dr["Fam_EmpStatus"].ToString();
                    var F_DOB = dr["Fam_DOB"].ToString() == "" ? "" : Convert.ToDateTime(dr["Fam_DOB"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    item.F_Fname = dr["Fam_Fname"].ToString();
                    item.F_Lname = dr["Fam_Lname"].ToString();
                    item.F_Mname = dr["Fam_Mname"].ToString();
                    item.F_Suffix = dr["Fam_Suffix"].ToString();
                    item.F_RTTB = dr["Fam_RTTB"].ToString();
                    item.F_NOD = famnod.ToString();
                    item.F_CompanyName = dr["Fam_CompanyName"].ToString();
                    item.F_YOS = F_YOS;
                    item.F_Job = dr["Position"].ToString();
                    item.F_Emp_Status = F_Emp_Status;
                    item.F_Age = F_Age;
                    item.F_DOB = F_DOB;
                    item.FamId = dr["FamId"].ToString();

                    string sql_child = $@"SELECT        Id, Fname, Mname, Lname, Age, NOS, FamId, Status, DateCreated, DateUpdated
                            FROM            tbl_ChildInfo_Model
                            WHERE        (FamId = '" + dr["FamId"].ToString() + "')";

                    DataTable child_table = db.SelectDb(sql_child).Tables[0];
                    var child_res = new List<ChildModel>();
                    foreach (DataRow c_dr in child_table.Rows)
                    {
                        var items = new ChildModel();
                        items.Fname = c_dr["Fname"].ToString();
                        items.Lname = c_dr["Mname"].ToString();
                        items.Mname = c_dr["Lname"].ToString();
                        items.Age = int.Parse(c_dr["Age"].ToString());
                        items.NOS = c_dr["NOS"].ToString();
                        items.FamId = c_dr["FamId"].ToString();
                        child_res.Add(items);

                    }
                    item.Child = child_res;
                    //business
                    string sql_business = $@"SELECT        tbl_BusinessInformation_Model.Id, tbl_BusinessInformation_Model.BusinessName, tbl_BusinessInformation_Model.BusinessAddress, tbl_BusinessInformation_Model.YOB, tbl_BusinessInformation_Model.NOE, 
                         tbl_BusinessInformation_Model.Salary, tbl_BusinessInformation_Model.VOS, tbl_BusinessInformation_Model.AOS, tbl_BusinessInformation_Model.DateCreated, tbl_BusinessInformation_Model.DateUpdated, 
                         tbl_BusinessInformation_Model.BIID, tbl_Status_Model.Name AS Business_Status, tbl_Status_Model_1.Name AS Status, tbl_BusinessInformation_Model.BusinessType,tbl_BusinessInformation_Model.B_status AS B_statusID,tbl_BusinessInformation_Model.FilesUploaded

                        FROM            tbl_BusinessInformation_Model INNER JOIN
                                                 tbl_Status_Model ON tbl_BusinessInformation_Model.B_status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_BusinessInformation_Model.Status = tbl_Status_Model_1.Id
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                    DataTable b_table = db.SelectDb(sql_business).Tables[0];
                    var b_res = new List<BusinessModelVM>();
                    foreach (DataRow b_dr in b_table.Rows)
                    {
                        var b_item = new BusinessModelVM();
                        b_item.BusinessName = b_dr["BusinessName"].ToString();
                        b_item.BusinessType = b_dr["BusinessType"].ToString();
                        b_item.BusinessAddress = b_dr["BusinessAddress"].ToString();
                        b_item.B_statusID = b_dr["B_statusID"].ToString();
                        b_item.YOB = int.Parse(b_dr["YOB"].ToString());
                        b_item.NOE = int.Parse(b_dr["NOE"].ToString());
                        b_item.Salary = decimal.Parse(b_dr["Salary"].ToString());
                        b_item.VOS = decimal.Parse(b_dr["VOS"].ToString());
                        b_item.AOS = decimal.Parse(b_dr["AOS"].ToString());
                        var b_files = new List<FileModel>();
                        b_item.B_status = b_dr["Business_Status"].ToString();
                        if (b_dr["FilesUploaded"].ToString() != null)
                        {
                            var files = b_dr["FilesUploaded"].ToString().Split('^');

                            for (int x = 0; x < files.ToList().Count; x++)
                            {
                                var items = new FileModel();
                                items.FilePath = files[x];
                                b_files.Add(items);
                            }

                        }
                        b_item.BusinessFiles = b_files;
                        b_res.Add(b_item);
                    }

                    //Motor
                    string sql_assets = $@"SELECT        MotorVehicles FROM            tbl_AssetsProperties_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                    DataTable assets_table = db.SelectDb(sql_assets).Tables[0];
                    var assest_res = new List<AssetsModel>();
                    foreach (DataRow b_dr in assets_table.Rows)
                    {
                        var assets_item = new AssetsModel();
                        assets_item.MotorVehicles = b_dr["MotorVehicles"].ToString();
                        assest_res.Add(assets_item);
                    }

                    //Property
                    string sql_property = $@"SELECT     Property  FROM   tbl_Property_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                    DataTable property_table = db.SelectDb(sql_property).Tables[0];
                    var property_res = new List<PropertyDetailsModel>();
                    foreach (DataRow b_dr in property_table.Rows)
                    {
                        var property_item = new PropertyDetailsModel();
                        property_item.Property = b_dr["Property"].ToString();
                        property_res.Add(property_item);
                    }

                    //Bank
                    string sql_bank = $@"SELECT        BankName, Address, DateCreated, DateUpdated, BankID, Status, MemId
                                        FROM            tbl_BankAccounts_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                    DataTable bank_table = db.SelectDb(sql_bank).Tables[0];
                    var bank_res = new List<BankModel>();
                    foreach (DataRow b_dr in bank_table.Rows)
                    {
                        var bank_item = new BankModel();
                        bank_item.BankName = b_dr["BankName"].ToString();
                        bank_item.Address = b_dr["Address"].ToString();
                        bank_res.Add(bank_item);
                    }
                    string sql_appliances = $@"SELECT        tbl_Appliance_Model.Brand, tbl_Appliance_Model.Description, tbl_Appliance_Model.NAID
                         FROM            tbl_Application_Model INNER JOIN
                         tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_Appliance_Model ON tbl_Application_Model.NAID = tbl_Appliance_Model.NAID
                                        WHERE        (tbl_Member_Model.MemId = '" + dr["MemId"].ToString() + "')";

                    DataTable appliances_table = db.SelectDb(sql_appliances).Tables[0];
                    var appliances_res = new List<ApplianceModel>();
                    foreach (DataRow b_dr in appliances_table.Rows)
                    {
                        var appliances_item = new ApplianceModel();
                        appliances_item.Appliances = b_dr["Description"].ToString();
                        appliances_item.Brand = b_dr["Brand"].ToString();
                        appliances_item.NAID = b_dr["NAID"].ToString();
                        appliances_res.Add(appliances_item);
                    }
                    //files

                    string sql_files = $@"SELECT        tbl_fileupload_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_TypesModel.TypeName, tbl_Status_Model.Name AS Status
                         FROM            tbl_fileupload_Model INNER JOIN
                         tbl_TypesModel ON tbl_fileupload_Model.Type = tbl_TypesModel.Id INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                                        WHERE        (tbl_fileupload_Model.MemId = '" + dr["MemId"].ToString() + "')";

                    DataTable file_table = db.SelectDb(sql_files).Tables[0];
                    var file_res = new List<FileModel>();
                    if (file_table.Rows.Count != 0)
                    {
                        foreach (DataRow b_dr in file_table.Rows)
                        {
                            var file_item = new FileModel();
                            file_item.FileName = b_dr["FileName"].ToString();
                            file_item.FilePath = b_dr["FilePath"].ToString();
                            file_item.FileType = b_dr["TypeName"].ToString();
                            file_res.Add(file_item);
                        }
                    }
                    item.Files = file_res;
                    item.Property = property_res;
                    item.Appliances = appliances_res;
                    item.Bank = bank_res;
                    item.Assets = assest_res;
                    item.Business = b_res;
                    //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                    //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                    // item.LoanAmount = decimal.Parse(amount);
                    var individualloan = GetApplicationListFilter(_dr["ApplicationStatusId"].ToString()).ToList();
                    var group = new List<ApplicationVM2>();
                    var individual_ = new List<ApplicationVM2>();


                    //if (grouploan.Count != 0)
                    //{
                    //    for (int x = 0; x < grouploan.Count; x++)
                    //    {
                    //        var g_item = new GroupApplicationVM2();
                    //        g_item.LoanAmount = grouploan[x].Loandetails[0].LoanAmount;
                    //        g_item.Terms = grouploan[x].Loandetails[0].Terms;
                    //        g_item.LoanType = grouploan[x].Loandetails[0].LoanType;
                    //        g_item.InterestRate = grouploan[x].Loandetails[0].InterestRate;
                    //        g_item.GroupId = grouploan[x].GroupId;
                    //        g_item.LDID = grouploan[x].Loandetails[0].LDID;

                    //        g_item.CI_ApprovedBy = grouploan[x].Loandetails[0].CI_ApprovedBy;
                    //        g_item.CI_ApprovalDate = grouploan[x].Loandetails[0].CI_ApprovalDate;
                    //        g_item.ReleasingDate = grouploan[x].Loandetails[0].ReleasingDate;
                    //        g_item.DeclineDate = grouploan[x].Loandetails[0].DeclineDate;
                    //        g_item.App_ApprovedBy_1 = grouploan[x].Loandetails[0].App_ApprovedBy_1;
                    //        g_item.App_ApprovalDate_1 = grouploan[x].Loandetails[0].App_ApprovalDate_1;
                    //        g_item.App_ApprovedBy_2 = grouploan[x].Loandetails[0].App_ApprovedBy_2;
                    //        g_item.App_ApprovalDate_2 = grouploan[x].Loandetails[0].App_ApprovalDate_2;
                    //        g_item.App_Note = grouploan[x].Loandetails[0].App_Note;
                    //        g_item.App_Notedby = grouploan[x].Loandetails[0].App_Notedby;
                    //        g_item.App_NotedDate = grouploan[x].Loandetails[0].App_NotedDate;
                    //        g_item.CreatedBy = grouploan[x].Loandetails[0].CreatedBy;
                    //        g_item.SubmittedBy = grouploan[x].Loandetails[0].SubmittedBy;
                    //        g_item.DateSubmitted = grouploan[x].Loandetails[0].DateSubmitted;
                    //        g_item.ReleasedBy = grouploan[x].Loandetails[0].ReleasedBy;

                    //        g_item.ModeOfRelease = grouploan[x].Loandetails[0].ModeOfRelease;
                    //        g_item.ModeOfReleaseReference = grouploan[x].Loandetails[0].ModeOfReleaseReference;
                    //        g_item.Courerier = grouploan[x].Loandetails[0].Courerier;
                    //        g_item.CourierCNo = grouploan[x].Loandetails[0].CourierCNo;
                    //        g_item.CourerierName = grouploan[x].Loandetails[0].CourerierName;
                    //        g_item.Denomination = grouploan[x].Loandetails[0].Denomination;
                    //        g_item.AreaName = grouploan[x].Loandetails[0].AreaName;
                    //        g_item.Remarks = grouploan[x].Loandetails[0].Remarks;
                    //        g_item.ApprovedLoanAmount = grouploan[x].Loandetails[0].ApprovedLoanAmount;
                    //        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                    //        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                    //        g_item.Days = grouploan[x].Loandetails[0].Days;
                    //        group.Add(g_item);
                    //    }

                    //}
                    //else
                    //{
                    for (int x = 0; x < individualloan.Count; x++)
                    {
                        var i_item = new ApplicationVM2();
                        i_item.LoanAmount = individualloan[x].LoanAmount;
                        i_item.Terms = individualloan[x].TermsOfPayment;
                        i_item.InterestRate = individualloan[x].Interest;
                        i_item.LoanType = individualloan[x].LoanType;
                        i_item.LoanTypeID = individualloan[x].LoanTypeID;
                        i_item.LDID = individualloan[x].LDID;
                        i_item.NameOfTerms = individualloan[x].NameOfTerms;

                        i_item.CI_ApprovedBy = individualloan[x].CI_ApprovedBy;
                        i_item.CI_ApprovalDate = individualloan[x].CI_ApprovalDate;
                        i_item.ReleasingDate = individualloan[x].ReleasingDate;
                        i_item.DeclineDate = individualloan[x].DeclineDate;
                        i_item.DeclinedBy = individualloan[x].DeclinedBy;
                        i_item.App_ApprovedBy_1 = individualloan[x].App_ApprovedBy_1;
                        i_item.App_ApprovalDate_1 = individualloan[x].App_ApprovalDate_1;
                        i_item.App_ApprovedBy_2 = individualloan[x].App_ApprovedBy_2;
                        i_item.App_ApprovalDate_2 = individualloan[x].App_ApprovalDate_2;
                        i_item.App_Note = individualloan[x].App_Note;
                        i_item.App_Notedby = individualloan[x].App_Notedby;
                        i_item.App_NotedDate = individualloan[x].App_NotedDate;
                        i_item.CreatedBy = individualloan[x].CreatedBy;
                        i_item.SubmittedBy = individualloan[x].SubmittedBy;
                        i_item.DateSubmitted = individualloan[x].DateSubmitted;
                        i_item.ReleasedBy = individualloan[x].ReleasedBy;

                        i_item.ModeOfRelease = individualloan[x].ModeOfRelease;
                        i_item.ModeOfReleaseReference = individualloan[x].ModeOfReleaseReference;
                        i_item.Courerier = individualloan[x].Courerier;
                        i_item.CourierCNo = individualloan[x].CourierCNo;
                        i_item.CourerierName = individualloan[x].CourerierName;
                        i_item.Denomination = individualloan[x].Denomination;
                        i_item.AreaName = individualloan[x].AreaName;
                        i_item.Remarks = individualloan[x].Remarks;
                        i_item.ApprovedLoanAmount = individualloan[x].ApprovedLoanAmount;
                        i_item.ApprovedTermsOfPayment = individualloan[x].ApprovedTermsOfPayment;
                        i_item.Days = individualloan[x].Days;

                        individual_.Add(i_item);
                    }
                    //}
                    //item.GroupLoan = group;
                    item.GroupLoan = individual_;
                    //co maker
                    item.Co_Fname = dr["Co_Fname"].ToString();
                    item.Co_Mname = dr["Co_Mname"].ToString();
                    item.Co_Lname = dr["Lnam"].ToString();
                    item.Co_Suffix = dr["Co_Suffix"].ToString();
                    item.Co_Gender = dr["Co_Gender"].ToString();
                    var co_dob = dr["Co_DOB"].ToString() == "" ? "0.00" : Convert.ToDateTime(dr["Co_DOB"].ToString()).ToString("yyyy-MM-dd");
                    item.Co_DOB = co_dob;
                    item.Co_POB = dr["Co_POB"].ToString();
                    item.Co_Age = dr["Co_Age"].ToString();
                    item.Co_Cno = dr["Co_Cno"].ToString();
                    item.Co_Civil_Status = dr["CivilStatus"].ToString();
                    item.Co_EmailAddress = dr["Co_EmailAddress"].ToString();
                    item.Co_HouseNo = dr["Co_HouseNo"].ToString();
                    item.Co_Barangay = dr["Co_Barangay"].ToString();
                    item.Co_City = dr["Co_City"].ToString();
                    item.Co_Province = dr["Region"].ToString();
                    item.Co_Country = dr["Co_Country"].ToString();
                    item.Co_ZipCode = dr["Co_ZipCode"].ToString();
                    item.Co_YearsStay = dr["Co_YOS"].ToString();
                    item.Co_RTTB = dr["Co_RTTB"].ToString();
                    item.CMID = dr["CMID"].ToString();
                    //Co_job
                    item.Co_JobDescription = dr["Co_JobDescription"].ToString();
                    item.Coj_YOS = dr["Coj_YOS"].ToString();
                    item.Co_CompanyName = dr["Co_CompanyName"].ToString();
                    item.Co_CompanyAddress = dr["Co_CompanyAddress"].ToString();
                    item.Co_MonthlySalary = dr["Co_MonthlySalary"].ToString();
                    item.Co_OtherSOC = dr["Co_OtherSOC"].ToString();
                    item.Co_Emp_Status = dr["Co_Emp_Status"].ToString();
                    item.Co_BO_Status = dr["Co_BO_Status"].ToString();
                    item.Co_House_Stats = dr["Co_House_Stats"].ToString();


                    string co_sql_files = $@"SELECT        tbl_CoMakerFileUpload_Model.Id, tbl_CoMakerFileUpload_Model.CMID, tbl_CoMakerFileUpload_Model.FileName, tbl_CoMakerFileUpload_Model.FilePath, tbl_CoMakerFileUpload_Model.DateCreated, 
                         tbl_TypesModel.TypeName, tbl_CoMakerFileUpload_Model.Status
                            FROM            tbl_CoMakerFileUpload_Model INNER JOIN
                         tbl_TypesModel ON tbl_CoMakerFileUpload_Model.Status = tbl_TypesModel.Id
                                        WHERE        (tbl_CoMakerFileUpload_Model.CMID = '" + dr["CMID"].ToString() + "')";

                    DataTable co_file_table = db.SelectDb(co_sql_files).Tables[0];
                    var co_file_res = new List<FileModel>();
                    if (co_file_table.Rows.Count != 0)
                    {
                        foreach (DataRow b_dr in co_file_table.Rows)
                        {
                            var file_item = new FileModel();
                            file_item.FileName = b_dr["FileName"].ToString();
                            file_item.FilePath = b_dr["FilePath"].ToString();
                            file_item.FileType = b_dr["TypeName"].ToString();
                            co_file_res.Add(file_item);
                        }
                    }
                    item.Co_Files = co_file_res;



                    mem_res.Add(item);
                }

                _item.MemberList = mem_res;
                result.Add(_item);
            }


            return result;

        }
        public List<GroupApp> GetGroupList()
        {
            var result = new List<GroupApp>();
            DataTable dt = db.SelectDb_SP("sp_GroupApps").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datea = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");


                var CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                //var type = dr["LoanTypeName"].ToString() == "" ? "Group" : dr["LoanTypeName"].ToString();
                var item = new GroupApp();
                item.GroupName = dr["GroupName"].ToString();
                item.GroupId = dr["GroupId"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.DateCreated = datec;
                item.DateApproval = datea;
                item.Remarks = dr["Remarks"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Status = dr["Status"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                item.Mem_status = dr["Mem_status"].ToString();
                item.Borrower = dr["Borrower"].ToString();

                item.RefNo = dr["RefNo"].ToString();
                item.AreaName = dr["Area"].ToString();
                item.Cno = dr["BorrowerCno"].ToString();
                item.CoBorrower = dr["CoBorrower"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.CI_ApprovalDate = CI_ApprovalDate;
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.ReleasingDate = ReleasingDate;
                item.DeclineDate = DeclineDate;
                item.DeclinedBy = dr["DeclinedBy"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovalDate_1 = App_ApprovalDate_1;
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovalDate_2 = App_ApprovalDate_2;
                item.App_Note = dr["App_Note"].ToString();
                item.App_Notedby = dr["App_Notedby"].ToString();
                item.App_NotedDate = App_NotedDate;
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.SubmittedBy = dr["SubmittedBy"].ToString();
                item.DateSubmitted = DateSubmitted;
                item.ReleasedBy = dr["ReleasedBy"].ToString();


                item.LDID = dr["LDID"].ToString();
                item.LoanAmount = dr["LoanAmount"].ToString();
                item.LoanType = dr["LoanTypeName"].ToString();
                item.LoanTypeID = dr["LoanTypeID"].ToString();
                var type = dr["InterestType"].ToString() == "Percentage" ? "Percentage" : dr["InterestType"].ToString();
                item.Interest = dr["InterestRate"].ToString() + " Percentage";
                item.TermsOfPayment = dr["TermsOfPayment"].ToString() + " " + dr["Days"].ToString() + " Days";
                item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                item.NameOfTerms = dr["NameOfTerms"].ToString();
                item.ModeOfReleaseReference = dr["ModeOfReleaseReference"].ToString();
                item.Courerier = dr["Courerier"].ToString();
                item.CourierCNo = dr["CourierCNo"].ToString();
                item.CourerierName = dr["CourerierName"].ToString();
                item.Denomination = dr["Denomination"].ToString();
                item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString();
                item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString();
                item.Days = dr["Days"].ToString();

                result.Add(item);
            }

            return result;
        }
        public List<GroupApplicationVM> GetGroupApplicationList()
        {

            var result = new List<GroupApplicationVM>();
            DataTable dt = db.SelectDb_SP("GetGroupApplicationList").Tables[0];

            string CI_ApprovalDate = "";
            string App_ApprovalDate_1 = "";
            string App_ApprovalDate_2 = "";
            string App_NotedDate = "";
            string DateSubmitted = "";
            string DeclineDate = "";
            string ReleasingDate = "";

            string CI_ApprovedBy = "";
            string DeclinedBy = "";
            string App_ApprovedBy_1 = "";
            string App_ApprovedBy_2 = "";
            string App_Note = "";
            string App_Notedby = "";
            string CreatedBy = "";
            string SubmittedBy = "";
            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

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
                item.Cno = dr["Borrower_Cno"].ToString();
                item.coBorrower = dr["Co_Borrower"].ToString();
                item.Co_Cno = dr["Co_Borrower_Cno"].ToString();
                item.StatusId = dr["StatusId"].ToString();


                item.CI_ApprovalDate = CI_ApprovalDate;
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.ReleasingDate = ReleasingDate;
                item.DeclineDate = DeclineDate;
                item.DeclinedBy = dr["DeclinedBy"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovalDate_1 = App_ApprovalDate_1;
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovalDate_2 = App_ApprovalDate_2;
                item.App_Note = dr["App_Note"].ToString();
                item.App_Notedby = dr["App_Notedby"].ToString();
                item.App_NotedDate = App_NotedDate;
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.SubmittedBy = dr["SubmittedBy"].ToString();
                item.DateSubmitted = DateSubmitted;

                //grouploan
                string loan_sql = $@"SELECT        tbl_LoanDetails_Model.Id, tbl_LoanDetails_Model.LoanAmount,  tbl_LoanDetails_Model.Purpose, tbl_LoanDetails_Model.MemId, tbl_LoanDetails_Model.DateCreated, 
                         tbl_LoanDetails_Model.DateUpdated, tbl_LoanDetails_Model.GroupId, tbl_LoanType_Model.LoanTypeName, tbl_TermsOfPayment_Model.NameOfTerms as TermsOfPayment,
                         tbl_TermsOfPayment_Model.[Days], tbl_TermsOfPayment_Model.InterestRate ,tbl_TermsOfPayment_Model.InterestType,tbl_LoanDetails_Model.LDID,
                              tbl_LoanDetails_Model.ModeOfRelease,
                        tbl_LoanDetails_Model.ModeOfReleaseReference,
                        tbl_LoanDetails_Model.Courerier,
                        tbl_LoanDetails_Model.CourierCNo,
                        tbl_LoanDetails_Model.CourerierName,
                        tbl_LoanDetails_Model.Denomination,tbl_LoanDetails_Model.ApprovedLoanAmount,
						tbl_LoanDetails_Model.ApprovedTermsOfPayment,
						tbl_TermsOfPayment_Model.Days

                        FROM            tbl_LoanDetails_Model left JOIN
                                                 tbl_LoanType_Model ON tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID left join
                                                 tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_TermsOfPayment_Model.LoanTypeId
                        WHERE        (tbl_LoanType_Model.LoanTypeID = 'LT-02') 
                        AND (tbl_LoanDetails_Model.GroupId = '" + dr["GroupId"].ToString() + "') ";
                DataTable tbl_loan = db.SelectDb(loan_sql).Tables[0];
                var tbl_loan_res = new List<GroupApplicationVM2>();
                string type = "";
                string terms = "";
                string interest = "";
                string groupid = "";
                string ldid = "";

                string ModeOfRelease = "";
                string Courerier = "";
                string CourierCNo = "";
                string CourerierName = "";
                string ModeOfReleaseReference = "";
                string Denomination = "";
                string area = "";
                string ApprovedLoanAmount = "";
                string ApprovedTermsOfPayment = "";
                string Days = "";


                if (tbl_loan.Rows.Count != 0)
                {

                    foreach (DataRow b_dr in tbl_loan.Rows)
                    {


                        var loan_item = new GroupApplicationVM2();
                        terms = b_dr["TermsOfPayment"].ToString() + " " + b_dr["Days"].ToString() + " Days";
                        groupid = b_dr["GroupId"].ToString();
                        ldid = b_dr["LDID"].ToString();
                        ApprovedLoanAmount = b_dr["ApprovedLoanAmount"].ToString();
                        ApprovedTermsOfPayment = b_dr["ApprovedTermsOfPayment"].ToString();
                        Days = b_dr["Days"].ToString();

                        ModeOfRelease = b_dr["ModeOfRelease"].ToString();
                        Courerier = b_dr["Courerier"].ToString();
                        CourierCNo = b_dr["CourierCNo"].ToString();
                        CourerierName = b_dr["CourerierName"].ToString();
                        ModeOfReleaseReference = b_dr["ModeOfReleaseReference"].ToString();
                        Denomination = b_dr["Denomination"].ToString();
                        //area = b_dr["Area"].ToString();

                        type = b_dr["InterestType"].ToString() == "Percentage" ? "Percentage" : b_dr["InterestType"].ToString();
                        interest = b_dr["InterestRate"].ToString() + " Percentage";
                        loan_item.LoanAmount = b_dr["LoanAmount"].ToString();
                        loan_item.GroupId = groupid;
                        loan_item.LoanType = b_dr["LoanTypeName"].ToString();
                        loan_item.Terms = terms;
                        loan_item.LDID = ldid;

                        loan_item.ModeOfRelease = ModeOfRelease;
                        loan_item.Courerier = Courerier;
                        loan_item.CourierCNo = CourierCNo;
                        loan_item.CourerierName = CourerierName;
                        loan_item.ModeOfReleaseReference = ModeOfReleaseReference;
                        loan_item.Denomination = Denomination;

                        loan_item.InterestRate = type;
                        loan_item.AreaName = area;
                        loan_item.Days = Days;


                        tbl_loan_res.Add(loan_item);
                    }

                }
                else
                {
                    var loan_item = new GroupApplicationVM2();
                    loan_item.LoanAmount = "0";
                    loan_item.LoanType = "0";
                    loan_item.GroupId = "";
                    loan_item.Terms = terms;
                    loan_item.LDID = ldid;
                    loan_item.InterestRate = type;
                    loan_item.ModeOfRelease = ModeOfRelease;
                    loan_item.Courerier = Courerier;
                    loan_item.CourierCNo = CourierCNo;
                    loan_item.CourerierName = CourerierName;
                    loan_item.ModeOfReleaseReference = ModeOfReleaseReference;
                    loan_item.Denomination = Denomination;
                    loan_item.AreaName = area;
                    loan_item.ApprovedLoanAmount = ApprovedLoanAmount;
                    loan_item.ApprovedTermsOfPayment = ApprovedTermsOfPayment;
                    loan_item.Days = Days;
                    tbl_loan_res.Add(loan_item);
                }
                item.Loandetails = tbl_loan_res;
                result.Add(item);
            }


            return result;


        }
        public List<NewApplicationVM> GetNewApplicationList()
        {
            var result = new List<NewApplicationVM>();

            DataTable dt = db.SelectDb_SP("sp_newapplication").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datea = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                //var type = dr["LoanTypeName"].ToString() == "" ? "Group" : dr["LoanTypeName"].ToString();
                var item = new NewApplicationVM();
                item.MemId = dr["MemId"].ToString();
                item.DateCreated = datec;
                item.DateApproval = datea;
                item.Remarks = dr["Remarks"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Status = dr["Status"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                item.Borrower = dr["Borrower"].ToString();
                item.RefNo = dr["RefNo"].ToString();
                item.AreaName = dr["Area"].ToString();
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                var grouploan = GetGroupApplicationList().Where(a => a.NAID == dr["NAID"].ToString()).ToList();
                var individualloan = GetApplicationList().Where(a => a.NAID == dr["NAID"].ToString()).ToList();
                var group = new List<GroupApplicationVM2>();
                var individual_ = new List<ApplicationVM2>();


                if (grouploan.Count != 0)
                {
                    for (int x = 0; x < grouploan.Count; x++)
                    {
                        var g_item = new GroupApplicationVM2();
                        g_item.LoanAmount = grouploan[x].Loandetails[0].LoanAmount;
                        g_item.Terms = grouploan[x].Loandetails[0].Terms;
                        g_item.LoanType = grouploan[x].Loandetails[0].LoanType;
                        g_item.InterestRate = grouploan[x].Loandetails[0].InterestRate;
                        g_item.GroupId = grouploan[x].GroupId;
                        g_item.LDID = grouploan[x].Loandetails[0].LDID;

                        g_item.CI_ApprovedBy = grouploan[x].Loandetails[0].CI_ApprovedBy;
                        g_item.CI_ApprovalDate = grouploan[x].Loandetails[0].CI_ApprovalDate;
                        g_item.ReleasingDate = grouploan[x].Loandetails[0].ReleasingDate;
                        g_item.DeclineDate = grouploan[x].Loandetails[0].DeclineDate;
                        g_item.App_ApprovedBy_1 = grouploan[x].Loandetails[0].App_ApprovedBy_1;
                        g_item.App_ApprovalDate_1 = grouploan[x].Loandetails[0].App_ApprovalDate_1;
                        g_item.App_ApprovedBy_2 = grouploan[x].Loandetails[0].App_ApprovedBy_2;
                        g_item.App_ApprovalDate_2 = grouploan[x].Loandetails[0].App_ApprovalDate_2;
                        g_item.App_Note = grouploan[x].Loandetails[0].App_Note;
                        g_item.App_Notedby = grouploan[x].Loandetails[0].App_Notedby;
                        g_item.App_NotedDate = grouploan[x].Loandetails[0].App_NotedDate;
                        g_item.CreatedBy = grouploan[x].Loandetails[0].CreatedBy;
                        g_item.SubmittedBy = grouploan[x].Loandetails[0].SubmittedBy;
                        g_item.DateSubmitted = grouploan[x].Loandetails[0].DateSubmitted;
                        g_item.ReleasedBy = grouploan[x].Loandetails[0].ReleasedBy;

                        g_item.ModeOfRelease = grouploan[x].Loandetails[0].ModeOfRelease;
                        g_item.ModeOfReleaseReference = grouploan[x].Loandetails[0].ModeOfReleaseReference;
                        g_item.Courerier = grouploan[x].Loandetails[0].Courerier;
                        g_item.CourierCNo = grouploan[x].Loandetails[0].CourierCNo;
                        g_item.CourerierName = grouploan[x].Loandetails[0].CourerierName;
                        g_item.Denomination = grouploan[x].Loandetails[0].Denomination;
                        g_item.AreaName = grouploan[x].Loandetails[0].AreaName;
                        g_item.Remarks = grouploan[x].Loandetails[0].Remarks;
                        g_item.ApprovedLoanAmount = grouploan[x].Loandetails[0].ApprovedLoanAmount;
                        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                        g_item.Days = grouploan[x].Loandetails[0].Days;
                        group.Add(g_item);
                    }

                }
                else
                {
                    for (int x = 0; x < individualloan.Count; x++)
                    {
                        var i_item = new ApplicationVM2();
                        i_item.LoanAmount = individualloan[x].LoanAmount;
                        i_item.Terms = individualloan[x].TermsOfPayment;
                        i_item.InterestRate = individualloan[x].Interest;
                        i_item.LoanType = individualloan[x].LoanType;
                        i_item.LoanTypeID = individualloan[x].LoanTypeID;
                        i_item.LDID = individualloan[x].LDID;

                        i_item.CI_ApprovedBy = individualloan[x].CI_ApprovedBy;
                        i_item.CI_ApprovalDate = individualloan[x].CI_ApprovalDate;
                        i_item.ReleasingDate = individualloan[x].ReleasingDate;
                        i_item.DeclineDate = individualloan[x].DeclineDate;
                        i_item.DeclinedBy = individualloan[x].DeclinedBy;
                        i_item.App_ApprovedBy_1 = individualloan[x].App_ApprovedBy_1;
                        i_item.App_ApprovalDate_1 = individualloan[x].App_ApprovalDate_1;
                        i_item.App_ApprovedBy_2 = individualloan[x].App_ApprovedBy_2;
                        i_item.App_ApprovalDate_2 = individualloan[x].App_ApprovalDate_2;
                        i_item.App_Note = individualloan[x].App_Note;
                        i_item.App_Notedby = individualloan[x].App_Notedby;
                        i_item.App_NotedDate = individualloan[x].App_NotedDate;
                        i_item.CreatedBy = individualloan[x].CreatedBy;
                        i_item.SubmittedBy = individualloan[x].SubmittedBy;
                        i_item.DateSubmitted = individualloan[x].DateSubmitted;
                        i_item.ReleasedBy = individualloan[x].ReleasedBy;

                        i_item.ModeOfRelease = individualloan[x].ModeOfRelease;
                        i_item.ModeOfReleaseReference = individualloan[x].ModeOfReleaseReference;
                        i_item.Courerier = individualloan[x].Courerier;
                        i_item.CourierCNo = individualloan[x].CourierCNo;
                        i_item.CourerierName = individualloan[x].CourerierName;
                        i_item.Denomination = individualloan[x].Denomination;
                        i_item.AreaName = individualloan[x].AreaName;
                        i_item.Remarks = individualloan[x].Remarks;
                        i_item.ApprovedLoanAmount = individualloan[x].ApprovedLoanAmount;
                        i_item.ApprovedTermsOfPayment = individualloan[x].ApprovedTermsOfPayment;
                        i_item.Days = individualloan[x].Days;

                        individual_.Add(i_item);
                    }
                }
                item.GroupLoan = group;
                item.IndividualLoan = individual_;
                item.BorrowerCno = dr["BorrowerCno"].ToString();
                item.CoBorrower = dr["CoBorrower"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                result.Add(item);
            }



            return result;
        }
        public List<ApplicationVM> GetApplicationList()
        {
            var result = new List<ApplicationVM>();
            string areafilter = $@"SELECT [Id]
                              ,[Area]
                              ,[City]
                              ,[FOID]
                              ,[Status]
                              ,[DateCreated]
                              ,[DateUpdated]
                              ,[AreaID]
                          FROM [dbo].[tbl_Area_Model]";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {
                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    string barangay = spliter[0];
                    var ct = spliter.Count().ToString();
                    string city = ct == "1" ? "" : spliter[1];


                    string sql = $@" SELECT        tbl_Application_Model.Id, tbl_Application_Model.MemId, tbl_Application_Model.DateCreated, tbl_Application_Model.DateApproval, tbl_Application_Model.Remarks, tbl_Application_Model.NAID, tbl_Status_Model.Name AS Status, 
                         tbl_LoanDetails_Model.LoanAmount, tbl_Member_Model.Cno AS BorrowerCno, tbl_CoMaker_Model.Cno AS Co_Cno, tbl_LoanType_Model.LoanTypeName, tbl_Member_Model.Cno AS BorrowerCno, 
                         tbl_CoMaker_Model.Cno AS Co_Cno, tbl_Status_Model.Id AS StatusId, tbl_Application_Model.RefNo,  tbl_TermsOfPayment_Model.NameOfTerms AS TermsOfPayment, tbl_TermsOfPayment_Model.Days, 
                         tbl_TermsOfPayment_Model.InterestRate, tbl_TermsOfPayment_Model.InterestType, tbl_LoanDetails_Model.LDID, tbl_Application_Model.CI_ApprovedBy, tbl_Application_Model.CI_ApprovalDate, 
                         tbl_Application_Model.ReleasingDate, tbl_Application_Model.DeclineDate, tbl_Application_Model.DeclinedBy, tbl_Application_Model.App_ApprovedBy_1, tbl_Application_Model.App_ApprovalDate_1, 
                         tbl_Application_Model.App_ApprovedBy_2, tbl_Application_Model.App_ApprovalDate_2, tbl_Application_Model.App_Note, tbl_Application_Model.App_Notedby, tbl_Application_Model.App_NotedDate, 
                         tbl_Application_Model.CreatedBy, tbl_Application_Model.SubmittedBy, tbl_Application_Model.DateSubmitted, CI_ApprovedBy.Username as CI_ApprovedBy,
						 CI_ApprovedBy.Username as CI_ApprovedBy,
						 App_ApprovedBy_1.Username as App_ApprovedBy_1,
						 App_ApprovedBy_2.Username as App_ApprovedBy_2,
						 App_Notedby.Username as App_Notedby,
						 CreatedBy.Username as CreatedBy,
						 SubmittedBy.Username as SubmittedBy,
                          ReleasedBy.Username as ReleasedBy,
                          Concat(
                        tbl_CoMaker_Model.Lnam  ,', ', 
                        tbl_CoMaker_Model.Fname ,', ', 
                        tbl_CoMaker_Model.Mname) CoBorrower,
                        Concat(
                        tbl_Member_Model.Lname,', ', 
                        tbl_Member_Model.Fname,', ', 
                        tbl_Member_Model.Mname) as Borrower,
                        tbl_LoanDetails_Model.ModeOfRelease,
                        tbl_LoanDetails_Model.ModeOfReleaseReference,
                        tbl_LoanDetails_Model.Courerier,
                        tbl_LoanDetails_Model.CourierCNo,
                        tbl_LoanDetails_Model.CourerierName,
                        tbl_LoanDetails_Model.Denomination,
						tbl_LoanDetails_Model.LoanTypeID,
						tbl_LoanType_Model.LoanTypeName,
						tbl_LoanDetails_Model.ApprovedLoanAmount,
						tbl_LoanDetails_Model.ApprovedTermsOfPayment,
						tbl_TermsOfPayment_Model.Days,
                        tbl_TermsOfPayment_Model.NameOfTerms,
						Mem_Status.Name as Mem_status,
                        file_.FilePath as ProfilePath,
						tbl_LoanDetails_Model.GroupId,
						tbl_Group_Model.GroupName
from  tbl_Application_Model left join
tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID left join
tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join 
tbl_Status_Model on tbl_Application_Model.Status = tbl_Status_Model.Id left join
tbl_Status_Model as Mem_Status on Mem_Status.Id = tbl_Member_Model.Status left join
tbl_LoanType_Model on tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID left join
tbl_CoMaker_Model on tbl_Member_Model.MemId = tbl_CoMaker_Model.MemId left join
tbl_TermsOfPayment_Model on tbl_TermsOfPayment_Model.TopId = tbl_LoanDetails_Model.TermsOfPayment left join
 tbl_User_Model as CI_ApprovedBy ON tbl_Application_Model.CI_ApprovedBy = CI_ApprovedBy.UserId left JOIN
                          tbl_User_Model as App_ApprovedBy_1 ON tbl_Application_Model.App_ApprovedBy_1 = App_ApprovedBy_1.UserId left JOIN
                           tbl_User_Model as App_ApprovedBy_2 ON tbl_Application_Model.App_ApprovedBy_2 = App_ApprovedBy_2.UserId left JOIN
                            tbl_User_Model as App_Notedby ON tbl_Application_Model.App_Notedby = App_Notedby.UserId left JOIN
                             tbl_User_Model as CreatedBy ON tbl_Application_Model.CreatedBy = CreatedBy.UserId left JOIN
                                    tbl_User_Model as ReleasedBy ON tbl_Application_Model.ReleasedBy = ReleasedBy.UserId left JOIN
                              tbl_User_Model as SubmittedBy ON tbl_Application_Model.SubmittedBy = SubmittedBy.UserId left JOIN
							  tbl_Group_Model on tbl_Group_Model.GroupID = tbl_LoanDetails_Model.GroupId left join
                              (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId  
where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "'";
                    DataTable dt = db.SelectDb(sql).Tables[0];
                    //DataTable dt = db.SelectDb_SP("sp_GetApplicationList").Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var datea = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");


                        var CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        var DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        //var type = dr["LoanTypeName"].ToString() == "" ? "Group" : dr["LoanTypeName"].ToString();
                        var item = new ApplicationVM();
                        item.MemId = dr["MemId"].ToString();
                        item.DateCreated = datec;
                        item.DateApproval = datea;
                        item.Remarks = dr["Remarks"].ToString();
                        item.NAID = dr["NAID"].ToString();
                        item.Status = dr["Status"].ToString();
                        item.StatusId = dr["StatusId"].ToString();
                        item.Mem_status = dr["Mem_status"].ToString();
                        item.Borrower = dr["Borrower"].ToString();
                        item.GroupId = dr["GroupId"].ToString();
                        item.GroupName = dr["GroupName"].ToString();

                        item.RefNo = dr["RefNo"].ToString();
                        item.AreaName = dr_area["Area"].ToString();
                        item.Cno = dr["BorrowerCno"].ToString();
                        item.CoBorrower = dr["CoBorrower"].ToString();
                        item.Co_Cno = dr["Co_Cno"].ToString();
                        item.CI_ApprovalDate = CI_ApprovalDate;
                        item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                        item.ReleasingDate = ReleasingDate;
                        item.DeclineDate = DeclineDate;
                        item.DeclinedBy = dr["DeclinedBy"].ToString();
                        item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                        item.App_ApprovalDate_1 = App_ApprovalDate_1;
                        item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                        item.App_ApprovalDate_2 = App_ApprovalDate_2;
                        item.App_Note = dr["App_Note"].ToString();
                        item.App_Notedby = dr["App_Notedby"].ToString();
                        item.App_NotedDate = App_NotedDate;
                        item.CreatedBy = dr["CreatedBy"].ToString();
                        item.SubmittedBy = dr["SubmittedBy"].ToString();
                        item.DateSubmitted = DateSubmitted;
                        item.ReleasedBy = dr["ReleasedBy"].ToString();


                        item.LDID = dr["LDID"].ToString();
                        item.LoanAmount = dr["LoanAmount"].ToString();
                        item.LoanType = dr["LoanTypeName"].ToString();
                        item.LoanTypeID = dr["LoanTypeID"].ToString();
                        var type = dr["InterestType"].ToString() == "Percentage" ? "Percentage" : dr["InterestType"].ToString();
                        item.Interest = dr["InterestRate"].ToString() + " Percentage";
                        item.TermsOfPayment = dr["TermsOfPayment"].ToString() + " " + dr["Days"].ToString() + " Days";
                        item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                        item.NameOfTerms = dr["NameOfTerms"].ToString();
                        item.ModeOfReleaseReference = dr["ModeOfReleaseReference"].ToString();
                        item.Courerier = dr["Courerier"].ToString();
                        item.CourierCNo = dr["CourierCNo"].ToString();
                        item.CourerierName = dr["CourerierName"].ToString();
                        item.Denomination = dr["Denomination"].ToString();
                        item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString();
                        item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString();
                        item.Days = dr["Days"].ToString();

                        result.Add(item);
                    }

                }
            }

            return result;
        }
        public List<ColAreaVM> GetAreaListCollection()
        {
            var result = new List<ColAreaVM>();
            string sql_areas = $@"
            SELECT tbl_Area_Model.[AreaId],
                    tbl_Area_Model.Area as AreaName
                                      ,[Area_RefNo]
                                      ,tbl_CollectionStatus_Model.[Status] as Printed_Status
                                      ,Collection_Status.[Status] as Collection_Status
                                      ,[Denomination]
                                      ,[FieldExpenses]
                                      ,[CollectionRefNo]
                                      ,[Remarks]
                                  FROM [GoldOne].[dbo].[tbl_CollectionArea_Model] left join 
                                  tbl_Area_Model on tbl_Area_Model.AreaID = tbl_CollectionArea_Model.AreaId left join
                                  tbl_CollectionStatus_Model on tbl_CollectionStatus_Model.Id = tbl_CollectionArea_Model.Printed_Status left join 
                                  tbl_CollectionStatus_Model as Collection_Status on tbl_CollectionArea_Model.Collection_Status = Collection_Status.Id 
                                 ";
            DataTable tbl_sql_areas = db.SelectDb(sql_areas).Tables[0];
            foreach (DataRow dr_area in tbl_sql_areas.Rows)
            {
                var a_item = new ColAreaVM();
                a_item.AreaId = dr_area["AreaId"].ToString();
                a_item.Area_RefNo = dr_area["Area_RefNo"].ToString();
                a_item.AreaName = dr_area["AreaName"].ToString();
                a_item.Printed_Status = dr_area["AreaId"].ToString();
                a_item.Collection_Status = dr_area["Collection_Status"].ToString();
                a_item.Denomination = dr_area["Denomination"].ToString();
                a_item.FieldExpenses = dr_area["FieldExpenses"].ToString();
                a_item.CollectionRefNo = dr_area["CollectionRefNo"].ToString();
                a_item.Remarks = dr_area["Remarks"].ToString();
                result.Add(a_item);
            }

            return result;

        }
        public List<ColVM> GetCollectionsList()
        {
            var res = new List<ColVM>();

            string sql = $@"SELECT RefNo,DateCreated
                          FROM [GoldOne].[dbo].[tbl_CollectionModel]";
            DataTable table = db.SelectDb(sql).Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                var items = new ColVM();
                items.CollectionRef = dr["RefNo"].ToString();
                items.DateCreated = dr["DateCreated"].ToString();

                string sql_areas = $@"SELECT [AreaId]
                                      ,[Area_RefNo]
                                      ,tbl_CollectionStatus_Model.[Status] as Printed_Status
                                      ,Collection_Status.[Status] as Collection_Status
                                      ,[Denomination]
                                      ,[FieldExpenses]
                                      ,[CollectionRefNo]
                                      ,[Remarks]
                                  FROM [GoldOne].[dbo].[tbl_CollectionArea_Model] left join 
                                  tbl_CollectionStatus_Model on tbl_CollectionStatus_Model.Id = tbl_CollectionArea_Model.Printed_Status left join 
                                  tbl_CollectionStatus_Model as Collection_Status on tbl_CollectionArea_Model.Collection_Status = Collection_Status.Id
                                  where CollectionRefNo ='" + dr["RefNo"].ToString() + "'";
                DataTable tbl_sql_areas = db.SelectDb(sql_areas).Tables[0];



                var area_res = new List<ColAreaVM>();
                foreach (DataRow dr_area in tbl_sql_areas.Rows)
                {
                    var a_item = new ColAreaVM();
                    a_item.AreaId = dr_area["AreaId"].ToString();
                    a_item.Area_RefNo = dr_area["Area_RefNo"].ToString();
                    a_item.Printed_Status = dr_area["AreaId"].ToString();
                    a_item.Collection_Status = dr_area["Collection_Status"].ToString();
                    a_item.Denomination = dr_area["Denomination"].ToString();
                    a_item.FieldExpenses = dr_area["FieldExpenses"].ToString();
                    a_item.CollectionRefNo = dr_area["CollectionRefNo"].ToString();
                    a_item.Remarks = dr_area["Remarks"].ToString();

                    var param = new IDataParameter[]
                     {
                         new SqlParameter("@AreaId",dr_area["AreaId"].ToString())
                     };
                    DataTable tbl_sql_mem_info = db.SelectDb_SP("sp_colelction_memberinfo", param).Tables[0];
                    var mem_info = new List<CollectionVM2>();
                    if (tbl_sql_mem_info.Rows.Count != 0)
                    {
                        foreach (DataRow dr_meminfo in tbl_sql_mem_info.Rows)
                        {
                            var m_item = new CollectionVM2();
                            m_item.Borrower = dr_meminfo["Fname"].ToString() + ", " + dr_meminfo["Mname"].ToString() + ", " + dr_meminfo["Lname"].ToString();
                            m_item.Cno = dr_meminfo["Cno"].ToString();
                            m_item.MemId = dr_meminfo["MemId"].ToString();
                            m_item.FilePath = dr_meminfo["ProfilePath"].ToString();
                            m_item.NAID = dr_meminfo["NAID"].ToString();
                            m_item.Co_Borrower = dr_meminfo["Co_Fname"].ToString() + ", " + dr_meminfo["Co_Mname"].ToString() + ", " + dr_meminfo["Co_Lname"].ToString();
                            m_item.Co_Cno = dr_meminfo["Co_Cno"].ToString();
                            m_item.DailyCollectibles = dr_meminfo["DailyCollectibles"].ToString();
                            m_item.AmountDue = dr_meminfo["AmountDue"].ToString();
                            m_item.PastDue = dr_meminfo["PastDue"].ToString();
                            m_item.DueDate = dr_meminfo["DueDate"].ToString();
                            m_item.DateOfFullPayment = dr_meminfo["DateOfFullPayment"].ToString();
                            m_item.TotalSavingsAmount = dr_meminfo["TotalSavingsAmount"].ToString();
                            m_item.ApprovedAdvancePayment = dr_meminfo["ApprovedAdvancePayment"].ToString();
                            double loan = double.Parse(dr_meminfo["LoanAmount"].ToString()) * double.Parse(dr_meminfo["InterestRate"].ToString());

                            m_item.LoanPrincipal = dr_meminfo["DeductInterest"].ToString() == "1" ? Math.Ceiling(loan).ToString() : dr_meminfo["LoanAmount"].ToString();
                            m_item.ReleasingDate = dr_meminfo["ReleasingDate"].ToString();
                            m_item.AreaName = dr_meminfo["AreaName"].ToString();
                            m_item.AreaID = dr_meminfo["AreaID"].ToString();
                            m_item.FOID = dr_meminfo["FOID"].ToString();
                            m_item.FieldOfficer = dr_meminfo["FO_Fname"].ToString() + ", " + dr_meminfo["FO_Mname"].ToString() + ", " + dr_meminfo["FO_Lname"].ToString();
                            m_item.DateCreated = dr_meminfo["DateCreated"].ToString();
                            m_item.TypeOfCollection = dr_meminfo["TypeOfCollection"].ToString();

                            //create a query for conditions in payment status by date
                            double CollectedAmount = 0;
                            double LapsePayment = 0;
                            double AdvancePayment = 0;
                            string Payment_Status_Id = "";
                            string Payment_Status = "";
                            string Payment_Method = "";
                            string DateCollected = "";
                            string sql_collection_status = $@"SELECT [NAID]
                                      ,[AdvancePayment]
                                      ,[LapsePayment]
                                      ,[CollectedAmount]
                                      ,[Savings]
                                      ,tbl_CollectionStatus_Model.[Status] as [Payment_Status]
                                      ,tbl_Collection_AreaMember_Model.Payment_Status as Payment_StatusId
                                      ,[Payment_Method]
                                      ,[DateCollected]
                                      ,[Area_RefNo]
                                  FROM [GoldOne].[dbo].[tbl_Collection_AreaMember_Model] left JOIN
                                  tbl_CollectionStatus_Model on tbl_Collection_AreaMember_Model.Payment_Status = tbl_CollectionStatus_Model.Id 
                                where tbl_Collection_AreaMember_Model.NAID = '" + dr_meminfo["NAID"].ToString() + "' and " +
                            "tbl_Collection_AreaMember_Model.DateCollected = '" + dr["DateCreated"].ToString() + "'";
                            DataTable tbl_sql_collection_status = db.SelectDb(sql_collection_status).Tables[0];
                            if (tbl_sql_collection_status.Rows.Count != 0)
                            {
                                CollectedAmount = double.Parse(tbl_sql_collection_status.Rows[0]["CollectedAmount"].ToString());
                                LapsePayment = double.Parse(tbl_sql_collection_status.Rows[0]["LapsePayment"].ToString());
                                AdvancePayment = double.Parse(tbl_sql_collection_status.Rows[0]["AdvancePayment"].ToString());
                                Payment_Status_Id = tbl_sql_collection_status.Rows[0]["AdvancePayment"].ToString();
                                Payment_Status = tbl_sql_collection_status.Rows[0]["Payment_Status"].ToString();
                                Payment_Method = tbl_sql_collection_status.Rows[0]["Payment_Method"].ToString();
                                DateCollected = tbl_sql_collection_status.Rows[0]["DateCollected"].ToString();
                            }
                            m_item.CollectedAmount = CollectedAmount.ToString();
                            m_item.LapsePayment = LapsePayment.ToString();
                            m_item.AdvancePayment = AdvancePayment.ToString();
                            m_item.Payment_Status_Id = Payment_Status_Id.ToString();
                            m_item.Payment_Status = Payment_Status.ToString();
                            m_item.Payment_Method = Payment_Method.ToString();
                            m_item.DateCollected = DateCollected.ToString();
                            mem_info.Add(m_item);

                        }
                    }
                    a_item.Collection = mem_info;
                    area_res.Add(a_item);
                }
                items.ColArea = area_res;



                res.Add(items);
            }
            return res;
        }
        public List<ApplicationVM> GetApplicationListFilter(string naid)
        {
            var result = new List<ApplicationVM>();

            var param = new IDataParameter[]
            {
                    new SqlParameter("@ApplicationId",naid)
            };
            DataTable dt = db.SelectDb_SP("sp_MemberApplicationList", param).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datea = dr["DateApproval"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateApproval"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");


                var CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                //var type = dr["LoanTypeName"].ToString() == "" ? "Group" : dr["LoanTypeName"].ToString();
                var item = new ApplicationVM();
                item.MemId = dr["MemId"].ToString();
                item.DateCreated = datec;
                item.DateApproval = datea;
                item.Remarks = dr["Remarks"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Status = dr["Status"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                item.Borrower = dr["Borrower"].ToString();

                item.RefNo = dr["RefNo"].ToString();
                item.AreaName = dr["Area"].ToString();
                item.BorrowerCno = dr["BorrowerCno"].ToString();
                item.CoBorrower = dr["CoBorrower"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.CI_ApprovalDate = CI_ApprovalDate;
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.ReleasingDate = ReleasingDate;
                item.DeclineDate = DeclineDate;
                item.DeclinedBy = dr["DeclinedBy"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovalDate_1 = App_ApprovalDate_1;
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovalDate_2 = App_ApprovalDate_2;
                item.App_Note = dr["App_Note"].ToString();
                item.App_Notedby = dr["App_Notedby"].ToString();
                item.App_NotedDate = App_NotedDate;
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.SubmittedBy = dr["SubmittedBy"].ToString();
                item.DateSubmitted = DateSubmitted;
                item.ReleasedBy = dr["ReleasedBy"].ToString();


                item.LDID = dr["LDID"].ToString();
                item.LoanAmount = dr["LoanAmount"].ToString();
                item.LoanType = dr["LoanTypeName"].ToString();
                item.LoanTypeID = dr["LoanTypeID"].ToString();
                var type = dr["InterestType"].ToString() == "Percentage" ? "Percentage" : dr["InterestType"].ToString();
                item.Interest = dr["InterestRate"].ToString() + " Percentage";
                item.TermsOfPayment = dr["TermsOfPayment"].ToString() + " " + dr["Days"].ToString() + " Days";
                item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                item.NameOfTerms = dr["NameOfTerms"].ToString();
                item.ModeOfReleaseReference = dr["ModeOfReleaseReference"].ToString();
                item.Courerier = dr["Courerier"].ToString();
                item.CourierCNo = dr["CourierCNo"].ToString();
                item.CourerierName = dr["CourerierName"].ToString();
                item.Denomination = dr["Denomination"].ToString();
                item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString();
                item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString();
                item.Days = dr["Days"].ToString();

                result.Add(item);
            }

            return result;
        }

        public List<AreaVM> GetAreaList()
        {

            DataTable table = db.SelectDb_SP("sp_GetAreaList").Tables[0];
            var result = new List<AreaVM>();
            foreach (DataRow dr in table.Rows)
            {
                string city = "";
                string fullname = "";
                string foid = "";
                string areaid = "";
                char targetCharacter = ',';
                var item = new AreaVM();
                item.AreaName = dr["Area"].ToString();

                var param = new IDataParameter[]
                {
                    new SqlParameter("@Area",dr["Area"].ToString())
                };
                DataTable areafo = db.SelectDb_SP("sp_GetAreaFilterbyAreaList", param).Tables[0];

                if (areafo.Rows.Count != 0)
                {
                    foreach (DataRow fo_dr in areafo.Rows)
                    {
                        if (areafo.Rows.Count == 1)
                        {
                            city = fo_dr["City"].ToString();
                            fullname = fo_dr["Fullname"].ToString();
                            foid = fo_dr["FOID"].ToString();
                            areaid = fo_dr["AreaID"].ToString();
                        }
                        else
                        {
                            foid = fo_dr["FOID"].ToString();
                            city += fo_dr["City"].ToString() + ",";
                            fullname = fo_dr["Fullname"].ToString();
                            areaid += fo_dr["AreaID"].ToString() + ",";
                        }
                    }
                }
                item.FOID = foid;
                item.Location = city == "" ? "" : city.EndsWith(targetCharacter.ToString()) == true ? city.Substring(0, city.Length - 1) : city;
                item.Fullname = fullname;
                item.AreaID = areaid == "" ? "" : areaid.EndsWith(targetCharacter.ToString()) == true ? areaid.Substring(0, areaid.Length - 1) : areaid;
                item.Status = dr["Status"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                result.Add(item);
            }

            return result;
        }
        public List<AreaVM> GetFieldAreas()
        {

            DataTable table = db.SelectDb_SP("sp_fieldareas").Tables[0];
            var result = new List<AreaVM>();
            foreach (DataRow dr in table.Rows)
            {

                string fullname = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                string originalString = dr["City"].ToString();
                char charToReplace = '|';
                char replacementChar = ',';

                // Convert the string to a char array
                char[] charArray = originalString.ToCharArray();

                // Replace the specified character with the replacement character
                for (int i = 0; i < charArray.Length; i++)
                {
                    if (charArray[i] == charToReplace)
                        charArray[i] = replacementChar;
                }

                // Create a new string from the modified char array
                string replacedString = new string(charArray);

                var item = new AreaVM();
                item.AreaName = dr["AreaName"].ToString();
                item.FOID = dr["FOID"].ToString();
                item.Location = replacedString;
                item.Fullname = fullname;
                item.AreaID = dr["AreaID"].ToString();
                item.Status = dr["AreaStatus"].ToString();
                item.StatusId = dr["AreaStatusId"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                result.Add(item);
            }

            return result;
        }
        public List<Fieldexpenses> GetFieldExpenses(string AreaRefNo, string FOID)
        {
            var param = new IDataParameter[]
            {
                    new SqlParameter("@AreaRefNo",AreaRefNo),
                    new SqlParameter("@FOID",FOID)
            };
            DataTable table = db.SelectDb_SP("sp_fieldexpenses", param).Tables[0];
            var result = new List<Fieldexpenses>();
            foreach (DataRow dr in table.Rows)
            {

                string[] citiesArray = dr["FieldExpenses"].ToString().Split('|');
                string string_re = "";

                string res = "";
                foreach (string city in citiesArray)
                {

                    string[] description = city.Split(',');

                    var items = new Fieldexpenses();
                    items.ExpensesDescription = description[0];
                    items.FieldExpenses = description[1];
                    items.AreaId = dr["AreaId"].ToString();
                    result.Add(items);


                }




            }


            return result;
        }
        public List<AreaVM2> GetAreaViewing(string AreaID)
        {
            var param = new IDataParameter[]
            {
                    new SqlParameter("@AreaID",AreaID)
            };
            DataTable table = db.SelectDb_SP("sp_fieldareas_getdetails", param).Tables[0];
            var result = new List<AreaVM2>();
            foreach (DataRow dr in table.Rows)
            {

                string fullname = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                string citiesString = dr["City"].ToString();

                // Split the string based on the comma (',') delimiter
                string[] citiesArray = citiesString.Split('|');
                string string_re = "";
                // Print each city name
                var item = new AreaVM2();
                item.AreaName = dr["AreaName"].ToString();
                item.FOID = dr["FOID"].ToString();

                item.Fullname = fullname;
                item.AreaID = dr["AreaID"].ToString();
                item.Status = dr["AreaStatus"].ToString();
                item.StatusId = dr["AreaStatusId"].ToString();
                var location = new List<LocVM>();
                foreach (string city in citiesArray)
                {
                    var items = new LocVM();
                    items.Location = city;
                    location.Add(items);
                }

                item.Location = location;
                result.Add(item);
            }


            return result;
        }
        public List<AreaVM> GetUnAssignedLocationList()
        {

            var result = new List<AreaVM>();
            DataTable table = db.SelectDb_SP("sp_fieldareas_unassigned").Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new AreaVM();
                string city = "";
                //string fullname = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                string foid = "";
                string areaid = "";
                //item.FOID = dr["FOID"].ToString();
                item.Location = dr["City"].ToString();
                //item.Fullname = fullname;
                item.Status = dr["AreaStatus"].ToString();
                item.StatusId = dr["AreaStatusId"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                result.Add(item);
            }

            return result;
        }
        public List<MemberModelVM> GetApplicationMemberList()
        {

            var result = new List<MemberModelVM>();
            DataTable table = db.SelectDb_SP("sp_ApplicationMemberList").Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string BOstatus = dr["BO_Status"].ToString() == "true" ? "1" : dr["BO_Status"].ToString();
                var item = new MemberModelVM();
                item.Fullname = dr["Fullname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Age = dr["Age"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.City = dr["City"].ToString();
                item.Civil_Status = dr["Civil_Status"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.House_Stats = dr["House_Stats"].ToString();

                item.HouseStatusId = dr["HouseStatusId"].ToString();
                item.Country = dr["Country"].ToString();
                item.DOB = dr["DOB"].ToString();
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.POB = dr["POB"].ToString();
                item.Province = dr["Province"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Status = dr["MemberStatus"].ToString();
                item.DateCreated = datec;
                item.YearsStay = dr["YearsStay"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.WaterBill = dr["WaterBill"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.OtherBills = dr["OtherBills"].ToString();
                item.DailyExpenses = dr["DailyExpenses"].ToString();
                item.Emp_Status = dr["Emp_Status"].ToString();
                item.BO_Status = BOstatus;
                item.OtherSOC = dr["OtherSOC"].ToString();
                item.MonthlySalary = dr["MonthlySalary"].ToString();
                item.CompanyName = dr["CompanyName"].ToString();
                item.CompanyAddress = dr["CompanyAddress"].ToString();
                item.YOS = dr["YOS"].ToString();
                item.JobDescription = dr["JobDescription"].ToString();
                var famnod = dr["Fam_NOD"].ToString() == "" ? "0" : dr["Fam_NOD"].ToString();
                var F_YOS = dr["Fam_YOS"].ToString() == "" ? "0" : dr["Fam_YOS"].ToString();
                var F_Age = dr["Fam_Age"].ToString() == "" ? "0" : dr["Fam_Age"].ToString();
                var F_Emp_Status = dr["Fam_EmpStatus"].ToString() == "" ? "0" : dr["Fam_EmpStatus"].ToString();
                var F_DOB = dr["Fam_DOB"].ToString() == "" ? "" : Convert.ToDateTime(dr["Fam_DOB"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item.F_Fname = dr["Fam_Fname"].ToString();
                item.F_Lname = dr["Fam_Lname"].ToString();
                item.F_Mname = dr["Fam_Mname"].ToString();
                item.F_Suffix = dr["Fam_Suffix"].ToString();
                item.F_RTTB = dr["Fam_RTTB"].ToString();
                item.F_NOD = famnod.ToString();
                item.F_CompanyName = dr["Fam_CompanyName"].ToString();
                item.F_YOS = F_YOS;
                item.F_Job = dr["Position"].ToString();
                item.F_Emp_Status = F_Emp_Status;
                item.F_Age = F_Age;
                item.F_DOB = F_DOB;
                item.FamId = dr["FamId"].ToString();
                item.ApplicationStatus = dr["ApplicationStatus"].ToString();

                string sql_child = $@"SELECT        Id, Fname, Mname, Lname, Age, NOS, FamId, Status, DateCreated, DateUpdated
                            FROM            tbl_ChildInfo_Model
                            WHERE        (FamId = '" + dr["FamId"].ToString() + "')";

                DataTable child_table = db.SelectDb(sql_child).Tables[0];
                var child_res = new List<ChildModel>();
                foreach (DataRow c_dr in child_table.Rows)
                {
                    var items = new ChildModel();
                    items.Fname = c_dr["Fname"].ToString();
                    items.Lname = c_dr["Mname"].ToString();
                    items.Mname = c_dr["Lname"].ToString();
                    items.Age = int.Parse(c_dr["Age"].ToString());
                    items.NOS = c_dr["NOS"].ToString();
                    items.FamId = c_dr["FamId"].ToString();
                    child_res.Add(items);

                }
                item.Child = child_res;
                //business
                string sql_business = $@"SELECT        tbl_BusinessInformation_Model.Id, tbl_BusinessInformation_Model.BusinessName, tbl_BusinessInformation_Model.BusinessAddress, tbl_BusinessInformation_Model.YOB, tbl_BusinessInformation_Model.NOE, 
                         tbl_BusinessInformation_Model.Salary, tbl_BusinessInformation_Model.VOS, tbl_BusinessInformation_Model.AOS, tbl_BusinessInformation_Model.DateCreated, tbl_BusinessInformation_Model.DateUpdated, 
                         tbl_BusinessInformation_Model.BIID, tbl_Status_Model.Name AS Business_Status, tbl_Status_Model_1.Name AS Status, tbl_BusinessInformation_Model.BusinessType,tbl_BusinessInformation_Model.B_status AS B_statusID,tbl_BusinessInformation_Model.FilesUploaded

                        FROM            tbl_BusinessInformation_Model INNER JOIN
                                                 tbl_Status_Model ON tbl_BusinessInformation_Model.B_status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_BusinessInformation_Model.Status = tbl_Status_Model_1.Id
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable b_table = db.SelectDb(sql_business).Tables[0];
                var b_res = new List<BusinessModelVM>();
                foreach (DataRow b_dr in b_table.Rows)
                {
                    var b_item = new BusinessModelVM();
                    b_item.BusinessName = b_dr["BusinessName"].ToString();
                    b_item.BusinessType = b_dr["BusinessType"].ToString();
                    b_item.BusinessAddress = b_dr["BusinessAddress"].ToString();
                    b_item.B_statusID = b_dr["B_statusID"].ToString();
                    b_item.YOB = int.Parse(b_dr["YOB"].ToString());
                    b_item.NOE = int.Parse(b_dr["NOE"].ToString());
                    b_item.Salary = decimal.Parse(b_dr["Salary"].ToString());
                    b_item.VOS = decimal.Parse(b_dr["VOS"].ToString());
                    b_item.AOS = decimal.Parse(b_dr["AOS"].ToString());
                    var b_files = new List<FileModel>();
                    b_item.B_status = b_dr["Business_Status"].ToString();
                    if (b_dr["FilesUploaded"].ToString() != null)
                    {
                        var files = b_dr["FilesUploaded"].ToString().Split('^');

                        for (int x = 0; x < files.ToList().Count; x++)
                        {
                            var items = new FileModel();
                            items.FilePath = files[x];
                            b_files.Add(items);
                        }

                    }
                    b_item.BusinessFiles = b_files;
                    b_res.Add(b_item);
                }

                string sql_assets = $@"SELECT        MotorVehicles FROM            tbl_AssetsProperties_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable assets_table = db.SelectDb(sql_assets).Tables[0];
                var assest_res = new List<AssetsModel>();
                foreach (DataRow b_dr in assets_table.Rows)
                {
                    var assets_item = new AssetsModel();
                    assets_item.MotorVehicles = b_dr["MotorVehicles"].ToString();
                    assest_res.Add(assets_item);
                }

                //Property
                string sql_property = $@"SELECT     Property  FROM   tbl_Property_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable property_table = db.SelectDb(sql_property).Tables[0];
                var property_res = new List<PropertyDetailsModel>();
                foreach (DataRow b_dr in property_table.Rows)
                {
                    var property_item = new PropertyDetailsModel();
                    property_item.Property = b_dr["Property"].ToString();
                    property_res.Add(property_item);
                }

                string sql_bank = $@"SELECT        BankName, Address, DateCreated, DateUpdated, BankID, Status, MemId
                                        FROM            tbl_BankAccounts_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable bank_table = db.SelectDb(sql_bank).Tables[0];
                var bank_res = new List<BankModel>();
                foreach (DataRow b_dr in bank_table.Rows)
                {
                    var bank_item = new BankModel();
                    bank_item.BankName = b_dr["BankName"].ToString();
                    bank_item.Address = b_dr["Address"].ToString();
                    bank_res.Add(bank_item);
                }
                string sql_appliances = $@"SELECT        tbl_Appliance_Model.Brand, tbl_Appliance_Model.Description, tbl_Appliance_Model.NAID
                         FROM            tbl_Application_Model INNER JOIN
                         tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_Appliance_Model ON tbl_Application_Model.NAID = tbl_Appliance_Model.NAID
                                        WHERE        (tbl_Member_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable appliances_table = db.SelectDb(sql_appliances).Tables[0];
                var appliances_res = new List<ApplianceModel>();
                foreach (DataRow b_dr in appliances_table.Rows)
                {
                    var appliances_item = new ApplianceModel();
                    appliances_item.Appliances = b_dr["Description"].ToString();
                    appliances_item.Brand = b_dr["Brand"].ToString();
                    appliances_item.NAID = b_dr["NAID"].ToString();
                    appliances_res.Add(appliances_item);
                }
                //files

                string sql_files = $@"SELECT        tbl_fileupload_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_TypesModel.TypeName, tbl_Status_Model.Name AS Status
                         FROM            tbl_fileupload_Model INNER JOIN
                         tbl_TypesModel ON tbl_fileupload_Model.Type = tbl_TypesModel.Id INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                                        WHERE        (tbl_fileupload_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable file_table = db.SelectDb(sql_files).Tables[0];
                var file_res = new List<FileModel>();
                if (file_table.Rows.Count != 0)
                {
                    foreach (DataRow b_dr in file_table.Rows)
                    {
                        var file_item = new FileModel();
                        file_item.FilePath = b_dr["FilePath"].ToString();
                        file_res.Add(file_item);
                    }
                }
                item.Files = file_res;
                item.Property = property_res;
                item.Appliances = appliances_res;
                item.Bank = bank_res;
                item.Assets = assest_res;
                item.Business = b_res;
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                var grouploan = GetGroupApplicationList().Where(a => a.MemId == dr["MemId"].ToString()).ToList();
                var individualloan = GetApplicationList().Where(a => a.MemId == dr["MemId"].ToString()).ToList();
                var group = new List<GroupApplicationVM2>();
                var individual_ = new List<ApplicationVM2>();


                if (grouploan.Count != 0)
                {
                    for (int x = 0; x < grouploan.Count; x++)
                    {
                        var g_item = new GroupApplicationVM2();
                        g_item.LoanAmount = grouploan[x].Loandetails[0].LoanAmount;
                        g_item.Terms = grouploan[x].Loandetails[0].Terms;
                        g_item.LoanType = grouploan[x].Loandetails[0].LoanType;
                        g_item.InterestRate = grouploan[x].Loandetails[0].InterestRate;
                        g_item.GroupId = grouploan[x].GroupId;
                        g_item.LDID = grouploan[x].Loandetails[0].LDID;

                        g_item.CI_ApprovedBy = grouploan[x].Loandetails[0].CI_ApprovedBy;
                        g_item.CI_ApprovalDate = grouploan[x].Loandetails[0].CI_ApprovalDate;
                        g_item.ReleasingDate = grouploan[x].Loandetails[0].ReleasingDate;
                        g_item.DeclineDate = grouploan[x].Loandetails[0].DeclineDate;
                        g_item.App_ApprovedBy_1 = grouploan[x].Loandetails[0].App_ApprovedBy_1;
                        g_item.App_ApprovalDate_1 = grouploan[x].Loandetails[0].App_ApprovalDate_1;
                        g_item.App_ApprovedBy_2 = grouploan[x].Loandetails[0].App_ApprovedBy_2;
                        g_item.App_ApprovalDate_2 = grouploan[x].Loandetails[0].App_ApprovalDate_2;
                        g_item.App_Note = grouploan[x].Loandetails[0].App_Note;
                        g_item.App_Notedby = grouploan[x].Loandetails[0].App_Notedby;
                        g_item.App_NotedDate = grouploan[x].Loandetails[0].App_NotedDate;
                        g_item.CreatedBy = grouploan[x].Loandetails[0].CreatedBy;
                        g_item.SubmittedBy = grouploan[x].Loandetails[0].SubmittedBy;
                        g_item.DateSubmitted = grouploan[x].Loandetails[0].DateSubmitted;
                        g_item.ReleasedBy = grouploan[x].Loandetails[0].ReleasedBy;

                        g_item.ModeOfRelease = grouploan[x].Loandetails[0].ModeOfRelease;
                        g_item.ModeOfReleaseReference = grouploan[x].Loandetails[0].ModeOfReleaseReference;
                        g_item.Courerier = grouploan[x].Loandetails[0].Courerier;
                        g_item.CourierCNo = grouploan[x].Loandetails[0].CourierCNo;
                        g_item.CourerierName = grouploan[x].Loandetails[0].CourerierName;
                        g_item.Denomination = grouploan[x].Loandetails[0].Denomination;
                        g_item.AreaName = grouploan[x].Loandetails[0].AreaName;
                        g_item.Remarks = grouploan[x].Loandetails[0].Remarks;
                        g_item.ApprovedLoanAmount = grouploan[x].Loandetails[0].ApprovedLoanAmount;
                        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                        g_item.Days = grouploan[x].Loandetails[0].Days;
                        group.Add(g_item);
                    }

                }
                else
                {
                    for (int x = 0; x < individualloan.Count; x++)
                    {
                        var i_item = new ApplicationVM2();
                        i_item.LoanAmount = individualloan[x].LoanAmount;
                        i_item.Terms = individualloan[x].TermsOfPayment;
                        i_item.InterestRate = individualloan[x].Interest;
                        i_item.LoanType = individualloan[x].LoanType;
                        i_item.LoanTypeID = individualloan[x].LoanTypeID;
                        i_item.LDID = individualloan[x].LDID;

                        i_item.CI_ApprovedBy = individualloan[x].CI_ApprovedBy;
                        i_item.CI_ApprovalDate = individualloan[x].CI_ApprovalDate;
                        i_item.ReleasingDate = individualloan[x].ReleasingDate;
                        i_item.DeclineDate = individualloan[x].DeclineDate;
                        i_item.DeclinedBy = individualloan[x].DeclinedBy;
                        i_item.App_ApprovedBy_1 = individualloan[x].App_ApprovedBy_1;
                        i_item.App_ApprovalDate_1 = individualloan[x].App_ApprovalDate_1;
                        i_item.App_ApprovedBy_2 = individualloan[x].App_ApprovedBy_2;
                        i_item.App_ApprovalDate_2 = individualloan[x].App_ApprovalDate_2;
                        i_item.App_Note = individualloan[x].App_Note;
                        i_item.App_Notedby = individualloan[x].App_Notedby;
                        i_item.App_NotedDate = individualloan[x].App_NotedDate;
                        i_item.CreatedBy = individualloan[x].CreatedBy;
                        i_item.SubmittedBy = individualloan[x].SubmittedBy;
                        i_item.DateSubmitted = individualloan[x].DateSubmitted;
                        i_item.ReleasedBy = individualloan[x].ReleasedBy;

                        i_item.ModeOfRelease = individualloan[x].ModeOfRelease;
                        i_item.ModeOfReleaseReference = individualloan[x].ModeOfReleaseReference;
                        i_item.Courerier = individualloan[x].Courerier;
                        i_item.CourierCNo = individualloan[x].CourierCNo;
                        i_item.CourerierName = individualloan[x].CourerierName;
                        i_item.Denomination = individualloan[x].Denomination;
                        i_item.AreaName = individualloan[x].AreaName;
                        i_item.Remarks = individualloan[x].Remarks;
                        i_item.ApprovedLoanAmount = individualloan[x].ApprovedLoanAmount;
                        i_item.ApprovedTermsOfPayment = individualloan[x].ApprovedTermsOfPayment;
                        i_item.Days = individualloan[x].Days;

                        individual_.Add(i_item);
                    }
                }
                item.GroupLoan = group;
                item.IndividualLoan = individual_;
                item.TermsOfPayment = dr["TermsOfPayment"].ToString();
                item.Purpose = dr["Purpose"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Lnam"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Gender = dr["Co_Gender"].ToString();
                var co_dob = dr["Co_DOB"].ToString() == "" ? "0.00" : Convert.ToDateTime(dr["Co_DOB"].ToString()).ToString("yyyy-MM-dd");
                item.Co_DOB = co_dob;
                item.Co_POB = dr["Co_POB"].ToString();
                item.Co_Age = dr["Co_Age"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.Co_Civil_Status = dr["CivilStatus"].ToString();
                item.Co_EmailAddress = dr["Co_EmailAddress"].ToString();
                item.Co_HouseNo = dr["Co_HouseNo"].ToString();
                item.Co_Barangay = dr["Co_Barangay"].ToString();
                item.Co_City = dr["Co_City"].ToString();
                item.Co_Province = dr["Region"].ToString();
                item.Co_Country = dr["Co_Country"].ToString();
                item.Co_ZipCode = dr["Co_ZipCode"].ToString();
                item.Co_YearsStay = dr["Co_YOS"].ToString();
                item.Co_RTTB = dr["Co_RTTB"].ToString();
                item.CMID = dr["CMID"].ToString();
                item.Co_JobDescription = dr["Co_JobDescription"].ToString();
                item.Coj_YOS = dr["Coj_YOS"].ToString();
                item.Co_CompanyName = dr["Co_CompanyName"].ToString();
                item.Co_CompanyAddress = dr["Co_CompanyAddress"].ToString();
                item.Co_MonthlySalary = dr["Co_MonthlySalary"].ToString();
                item.Co_OtherSOC = dr["Co_OtherSOC"].ToString();
                item.Co_Emp_Status = dr["Co_Emp_Status"].ToString();
                item.Co_BO_Status = dr["Co_BO_Status"].ToString();
                item.Co_House_Stats = dr["Co_House_Stats"].ToString();

                item.Co_HouseStatusId = dr["Co_HouseStatusId"].ToString();
                result.Add(item);
            }

            return result;
        }
        public List<BusinessModelVM> GetBusinessMemberFiles(string memid)
        {
            string sql_business = $@"SELECT        tbl_BusinessInformation_Model.Id, tbl_BusinessInformation_Model.BusinessName, tbl_BusinessInformation_Model.BusinessAddress, tbl_BusinessInformation_Model.YOB, tbl_BusinessInformation_Model.NOE, 
                         tbl_BusinessInformation_Model.Salary, tbl_BusinessInformation_Model.VOS, tbl_BusinessInformation_Model.AOS, tbl_BusinessInformation_Model.DateCreated, tbl_BusinessInformation_Model.DateUpdated, 
                         tbl_BusinessInformation_Model.BIID, tbl_Status_Model.Name AS Business_Status, tbl_Status_Model_1.Name AS Status, tbl_BusinessInformation_Model.BusinessType,tbl_BusinessInformation_Model.B_status AS B_statusID,tbl_BusinessInformation_Model.FilesUploaded

                        FROM            tbl_BusinessInformation_Model INNER JOIN
                                                 tbl_Status_Model ON tbl_BusinessInformation_Model.B_status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_BusinessInformation_Model.Status = tbl_Status_Model_1.Id
                                        WHERE        (MemId = '" + memid + "')";

            DataTable b_table = db.SelectDb(sql_business).Tables[0];
            var b_res = new List<BusinessModelVM>();
            foreach (DataRow b_dr in b_table.Rows)
            {
                var b_item = new BusinessModelVM();
                b_item.BusinessName = b_dr["BusinessName"].ToString();
                b_item.BusinessType = b_dr["BusinessType"].ToString();
                b_item.BusinessAddress = b_dr["BusinessAddress"].ToString();
                b_item.B_statusID = b_dr["B_statusID"].ToString();
                b_item.YOB = int.Parse(b_dr["YOB"].ToString());
                b_item.NOE = int.Parse(b_dr["NOE"].ToString());
                b_item.Salary = decimal.Parse(b_dr["Salary"].ToString());
                b_item.VOS = decimal.Parse(b_dr["VOS"].ToString());
                b_item.AOS = decimal.Parse(b_dr["AOS"].ToString());
                var b_files = new List<FileModel>();
                b_item.B_status = b_dr["Business_Status"].ToString();
                if (b_dr["FilesUploaded"].ToString() != null)
                {
                    var files = b_dr["FilesUploaded"].ToString().Split('|');

                    for (int x = 0; x < files.ToList().Count; x++)
                    {
                        var items = new FileModel();
                        items.FilePath = files[x];
                        b_files.Add(items);
                    }

                }
                b_item.BusinessFiles = b_files;
                b_res.Add(b_item);
            }
            return b_res;
        }
        //TODO: Getapplication
        public List<MemberModelVM> GetApplicationMemberFilterList(string ApplicationId)
        {
            var result = new List<MemberModelVM>();
            var param = new IDataParameter[]
            {
                    new SqlParameter("@ApplicationID",ApplicationId)
            };
            DataTable table = db.SelectDb_SP("sp_MemberDetailsVM", param).Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dob = dr["DOB"].ToString() == "" ? "" : Convert.ToDateTime(dr["DOB"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string BOstatus = dr["BO_Status"].ToString() == "true" ? "1" : dr["BO_Status"].ToString();
                var item = new MemberModelVM();
                item.Fullname = dr["Fullname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Age = dr["Age"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.City = dr["City"].ToString();
                item.Civil_Status = dr["Civil_Status"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.House_Stats = dr["House_Stats"].ToString();

                item.HouseStatusId = dr["HouseStatusId"].ToString();
                item.Country = dr["Country"].ToString();
                item.DOB = dob;
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.POB = dr["POB"].ToString();
                item.Province = dr["Province"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Status = dr["MemberStatus"].ToString();
                item.DateCreated = datec;
                item.YearsStay = dr["YearsStay"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.WaterBill = dr["WaterBill"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.OtherBills = dr["OtherBills"].ToString();
                item.DailyExpenses = dr["DailyExpenses"].ToString();
                item.Emp_Status = dr["Emp_Status"].ToString();
                item.BO_Status = BOstatus;
                item.OtherSOC = dr["OtherSOC"].ToString();
                item.MonthlySalary = dr["MonthlySalary"].ToString();
                item.CompanyName = dr["CompanyName"].ToString();
                item.YOS = dr["YOS"].ToString();
                item.JobDescription = dr["JobDescription"].ToString();
                var famnod = dr["Fam_NOD"].ToString() == "" ? "0" : dr["Fam_NOD"].ToString();
                var F_YOS = dr["Fam_YOS"].ToString() == "" ? "0" : dr["Fam_YOS"].ToString();
                var F_Age = dr["Fam_Age"].ToString() == "" ? "0" : dr["Fam_Age"].ToString();
                var F_Emp_Status = dr["Fam_EmpStatus"].ToString() == "" ? "0" : dr["Fam_EmpStatus"].ToString();
                var F_DOB = dr["Fam_DOB"].ToString() == "" ? "" : Convert.ToDateTime(dr["Fam_DOB"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item.F_Fname = dr["Fam_Fname"].ToString();
                item.F_Lname = dr["Fam_Lname"].ToString();
                item.F_Mname = dr["Fam_Mname"].ToString();
                item.F_Suffix = dr["Fam_Suffix"].ToString();
                item.F_RTTB = dr["Fam_RTTB"].ToString();
                item.F_NOD = famnod.ToString();
                item.F_CompanyName = dr["Fam_CompanyName"].ToString();
                item.F_YOS = F_YOS;
                item.F_Job = dr["Position"].ToString();
                item.F_Emp_Status = F_Emp_Status;
                item.F_Age = F_Age;
                item.F_DOB = F_DOB;
                item.FamId = dr["FamId"].ToString();
                item.ApplicationStatus = dr["ApplicationStatus"].ToString();

                string sql_child = $@"SELECT        Id, Fname, Mname, Lname, Age, NOS, FamId, Status, DateCreated, DateUpdated
                            FROM            tbl_ChildInfo_Model
                            WHERE        (FamId = '" + dr["FamId"].ToString() + "')";

                DataTable child_table = db.SelectDb(sql_child).Tables[0];
                var child_res = new List<ChildModel>();
                foreach (DataRow c_dr in child_table.Rows)
                {
                    var items = new ChildModel();
                    items.Fname = c_dr["Fname"].ToString();
                    items.Lname = c_dr["Lname"].ToString();
                    items.Mname = c_dr["Mname"].ToString();
                    items.Age = int.Parse(c_dr["Age"].ToString());
                    items.NOS = c_dr["NOS"].ToString();
                    items.FamId = c_dr["FamId"].ToString();
                    child_res.Add(items);

                }
                item.Child = child_res;
                //business
                string sql_business = $@"SELECT        tbl_BusinessInformation_Model.Id, tbl_BusinessInformation_Model.BusinessName, tbl_BusinessInformation_Model.BusinessAddress, tbl_BusinessInformation_Model.YOB, tbl_BusinessInformation_Model.NOE, 
                         tbl_BusinessInformation_Model.Salary, tbl_BusinessInformation_Model.VOS, tbl_BusinessInformation_Model.AOS, tbl_BusinessInformation_Model.DateCreated, tbl_BusinessInformation_Model.DateUpdated, 
                         tbl_BusinessInformation_Model.BIID, tbl_Status_Model.Name AS Business_Status, tbl_Status_Model_1.Name AS Status, tbl_BusinessInformation_Model.BusinessType,tbl_BusinessInformation_Model.B_status AS B_statusID,tbl_BusinessInformation_Model.FilesUploaded

                        FROM            tbl_BusinessInformation_Model INNER JOIN
                                                 tbl_Status_Model ON tbl_BusinessInformation_Model.B_status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_BusinessInformation_Model.Status = tbl_Status_Model_1.Id
                                        WHERE        (tbl_BusinessInformation_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable b_table = db.SelectDb(sql_business).Tables[0];
                var b_res = new List<BusinessModelVM>();
                foreach (DataRow b_dr in b_table.Rows)
                {
                    var b_item = new BusinessModelVM();
                    b_item.BusinessName = b_dr["BusinessName"].ToString();
                    b_item.BusinessType = b_dr["BusinessType"].ToString();
                    b_item.BusinessAddress = b_dr["BusinessAddress"].ToString();
                    b_item.B_statusID = b_dr["B_statusID"].ToString();
                    b_item.YOB = int.Parse(b_dr["YOB"].ToString());
                    b_item.NOE = int.Parse(b_dr["NOE"].ToString());
                    b_item.Salary = decimal.Parse(b_dr["Salary"].ToString());
                    b_item.VOS = decimal.Parse(b_dr["VOS"].ToString());
                    b_item.AOS = decimal.Parse(b_dr["AOS"].ToString());
                    var b_files = new List<FileModel>();
                    b_item.B_status = b_dr["Business_Status"].ToString();
                    if (b_dr["FilesUploaded"].ToString() != null)
                    {
                        if (b_dr["FilesUploaded"].ToString().Contains("|"))
                        {
                            var files = b_dr["FilesUploaded"].ToString().Split('|');
                            string files_ = "";

                            for (int x = 0; x < files.ToList().Count; x++)
                            {
                                var items = new FileModel();
                                items.FilePath = files[x];
                                b_files.Add(items);
                            }
                        }
                        else
                        {
                            var items = new FileModel();
                            items.FilePath = b_dr["FilesUploaded"].ToString();
                            b_files.Add(items);
                        }


                    }
                    b_item.BusinessFiles = b_files;
                    b_res.Add(b_item);
                }

                string sql_assets = $@"SELECT        MotorVehicles FROM            tbl_AssetsProperties_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable assets_table = db.SelectDb(sql_assets).Tables[0];
                var assest_res = new List<AssetsModel>();
                foreach (DataRow b_dr in assets_table.Rows)
                {
                    var assets_item = new AssetsModel();
                    assets_item.MotorVehicles = b_dr["MotorVehicles"].ToString();
                    assest_res.Add(assets_item);
                }

                //Property
                string sql_property = $@"SELECT     Property  FROM   tbl_Property_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable property_table = db.SelectDb(sql_property).Tables[0];
                var property_res = new List<PropertyDetailsModel>();
                foreach (DataRow b_dr in property_table.Rows)
                {
                    var property_item = new PropertyDetailsModel();
                    property_item.Property = b_dr["Property"].ToString();
                    property_res.Add(property_item);
                }

                string sql_bank = $@"SELECT        BankName, Address, DateCreated, DateUpdated, BankID, Status, MemId
                                        FROM            tbl_BankAccounts_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable bank_table = db.SelectDb(sql_bank).Tables[0];
                var bank_res = new List<BankModel>();
                foreach (DataRow b_dr in bank_table.Rows)
                {
                    var bank_item = new BankModel();
                    bank_item.BankName = b_dr["BankName"].ToString();
                    bank_item.Address = b_dr["Address"].ToString();
                    bank_res.Add(bank_item);
                }
                string sql_appliances = $@"SELECT        tbl_Appliance_Model.Brand, tbl_Appliance_Model.Description, tbl_Appliance_Model.NAID
                         FROM            tbl_Application_Model INNER JOIN
                         tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_Appliance_Model ON tbl_Application_Model.NAID = tbl_Appliance_Model.NAID
                                        WHERE        (tbl_Member_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable appliances_table = db.SelectDb(sql_appliances).Tables[0];
                var appliances_res = new List<ApplianceModel>();
                foreach (DataRow b_dr in appliances_table.Rows)
                {
                    var appliances_item = new ApplianceModel();
                    appliances_item.Appliances = b_dr["Description"].ToString();
                    appliances_item.Brand = b_dr["Brand"].ToString();
                    appliances_item.NAID = b_dr["NAID"].ToString();
                    appliances_res.Add(appliances_item);
                }
                //files

                string sql_files = $@"SELECT        tbl_fileupload_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_TypesModel.TypeName, tbl_Status_Model.Name AS Status
                         FROM            tbl_fileupload_Model INNER JOIN
                         tbl_TypesModel ON tbl_fileupload_Model.Type = tbl_TypesModel.Id INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                                        WHERE        (tbl_fileupload_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable file_table = db.SelectDb(sql_files).Tables[0];
                var file_res = new List<FileModel>();
                if (file_table.Rows.Count != 0)
                {
                    foreach (DataRow b_dr in file_table.Rows)
                    {
                        var file_item = new FileModel();
                        file_item.FileName = b_dr["FileName"].ToString();
                        file_item.FilePath = b_dr["FilePath"].ToString();
                        file_item.FileType = b_dr["TypeName"].ToString();
                        file_res.Add(file_item);
                    }
                }
                item.Files = file_res;
                item.Property = property_res;
                item.Appliances = appliances_res;
                item.Bank = bank_res;
                item.Assets = assest_res;
                item.Business = b_res;
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                //var grouploan = GetGroupApplicationList().Where(a => a.NAID == ApplicationId).ToList();
                var individualloan = GetApplicationListFilter(ApplicationId).ToList();
                var group = new List<GroupApplicationVM2>();
                var individual_ = new List<ApplicationVM2>();


                for (int x = 0; x < individualloan.Count; x++)
                {
                    var i_item = new ApplicationVM2();
                    i_item.LoanAmount = individualloan[x].LoanAmount;
                    i_item.Terms = individualloan[x].TermsOfPayment;
                    i_item.InterestRate = individualloan[x].Interest;
                    i_item.LoanType = individualloan[x].LoanType;
                    i_item.LoanTypeID = individualloan[x].LoanTypeID;
                    i_item.LDID = individualloan[x].LDID;

                    i_item.CI_ApprovedBy = individualloan[x].CI_ApprovedBy;
                    i_item.CI_ApprovalDate = individualloan[x].CI_ApprovalDate;
                    i_item.ReleasingDate = individualloan[x].ReleasingDate;
                    i_item.DeclineDate = individualloan[x].DeclineDate;
                    i_item.DeclinedBy = individualloan[x].DeclinedBy;
                    i_item.App_ApprovedBy_1 = individualloan[x].App_ApprovedBy_1;
                    i_item.App_ApprovalDate_1 = individualloan[x].App_ApprovalDate_1;
                    i_item.App_ApprovedBy_2 = individualloan[x].App_ApprovedBy_2;
                    i_item.App_ApprovalDate_2 = individualloan[x].App_ApprovalDate_2;
                    i_item.App_Note = individualloan[x].App_Note;
                    i_item.App_Notedby = individualloan[x].App_Notedby;
                    i_item.App_NotedDate = individualloan[x].App_NotedDate;
                    i_item.CreatedBy = individualloan[x].CreatedBy;
                    i_item.SubmittedBy = individualloan[x].SubmittedBy;
                    i_item.DateSubmitted = individualloan[x].DateSubmitted;
                    i_item.ReleasedBy = individualloan[x].ReleasedBy;
                    i_item.NameOfTerms = individualloan[x].NameOfTerms;

                    i_item.ModeOfRelease = individualloan[x].ModeOfRelease;
                    i_item.ModeOfReleaseReference = individualloan[x].ModeOfReleaseReference;
                    i_item.Courerier = individualloan[x].Courerier;
                    i_item.CourierCNo = individualloan[x].CourierCNo;
                    i_item.CourerierName = individualloan[x].CourerierName;
                    i_item.Denomination = individualloan[x].Denomination;
                    i_item.AreaName = individualloan[x].AreaName;
                    i_item.Remarks = individualloan[x].Remarks;
                    i_item.ApprovedLoanAmount = individualloan[x].ApprovedLoanAmount;
                    i_item.ApprovedTermsOfPayment = individualloan[x].ApprovedTermsOfPayment;
                    i_item.Days = individualloan[x].Days;

                    individual_.Add(i_item);
                }
                //}
                //item.GroupLoan = group;
                item.IndividualLoan = individual_;
                item.TermsOfPayment = dr["TermsOfPayment"].ToString();
                item.Purpose = dr["Purpose"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Lnam"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Gender = dr["Co_Gender"].ToString();
                var co_dob = dr["Co_DOB"].ToString() == "" ? "0.00" : Convert.ToDateTime(dr["Co_DOB"].ToString()).ToString("yyyy-MM-dd");
                item.Co_DOB = co_dob;

                item.Co_POB = dr["Co_POB"].ToString();
                item.Co_Age = dr["Co_Age"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.Co_Civil_Status = dr["CivilStatus"].ToString();
                item.Co_EmailAddress = dr["Co_EmailAddress"].ToString();
                item.Co_HouseNo = dr["Co_HouseNo"].ToString();
                item.Co_Barangay = dr["Co_Barangay"].ToString();
                item.Co_City = dr["Co_City"].ToString();
                item.Co_Province = dr["Region"].ToString();
                item.Co_Country = dr["Co_Country"].ToString();
                item.Co_ZipCode = dr["Co_ZipCode"].ToString();
                item.Co_YearsStay = dr["Co_YOS"].ToString();
                item.Co_RTTB = dr["Co_RTTB"].ToString();
                item.CMID = dr["CMID"].ToString();
                item.Co_JobDescription = dr["Co_JobDescription"].ToString();
                item.Coj_YOS = dr["Coj_YOS"].ToString();
                item.Co_CompanyName = dr["Co_CompanyName"].ToString();
                item.Co_CompanyAddress = dr["Co_CompanyAddress"].ToString();
                item.Co_MonthlySalary = dr["Co_MonthlySalary"].ToString();
                item.Co_OtherSOC = dr["Co_OtherSOC"].ToString();
                item.Co_Emp_Status = dr["Co_Emp_Status"].ToString();
                item.Co_BO_Status = dr["Co_BO_Status"].ToString();
                item.Co_House_Stats = dr["Co_House_Stats"].ToString();
                item.CompanyAddress = dr["CompanyAddress"].ToString();
                item.Co_CompanyAddress = dr["Co_CompanyAddress"].ToString();

                item.Co_HouseStatusId = dr["Co_HouseStatusId"].ToString();

                string co_sql_files = $@"SELECT        tbl_CoMakerFileUpload_Model.Id, tbl_CoMakerFileUpload_Model.CMID, tbl_CoMakerFileUpload_Model.FileName, tbl_CoMakerFileUpload_Model.FilePath, tbl_CoMakerFileUpload_Model.DateCreated, 
                         tbl_TypesModel.TypeName, tbl_CoMakerFileUpload_Model.Status
                            FROM            tbl_CoMakerFileUpload_Model INNER JOIN
                         tbl_TypesModel ON tbl_CoMakerFileUpload_Model.Status = tbl_TypesModel.Id
                                        WHERE        (tbl_CoMakerFileUpload_Model.CMID = '" + dr["CMID"].ToString() + "')";

                DataTable co_file_table = db.SelectDb(co_sql_files).Tables[0];
                var co_file_res = new List<FileModel>();
                if (co_file_table.Rows.Count != 0)
                {
                    foreach (DataRow b_dr in co_file_table.Rows)
                    {
                        var file_item = new FileModel();
                        file_item.FileName = b_dr["FileName"].ToString();
                        file_item.FilePath = b_dr["FilePath"].ToString();
                        file_item.FileType = b_dr["TypeName"].ToString();
                        co_file_res.Add(file_item);
                    }
                }
                item.Co_Files = co_file_res;
                result.Add(item);
            }

            return result;
        }
        public List<FoVM> GetFieldOfficer()
        {

            var result = new List<FoVM>();

            DataTable table = db.SelectDb_SP("sp_FieldOfficerDetails").Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new FoVM();
                item.FOID = dr["FOID"].ToString();
                item.Id = dr["Id"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                item.Status = dr["Status"].ToString();
                item.Country = dr["Country"].ToString();
                item.Region = dr["Region"].ToString();
                item.City = dr["City"].ToString();
                item.StatusId = dr["StatusId"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.CivilStatus = dr["CivilStatus"].ToString();
                item.POB = dr["POB"].ToString();
                item.Age = dr["Age"].ToString();
                item.DOB = dr["DOB"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.Fullname = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Suffix = dr["Suffix"].ToString();

                string sql_file = $@"SELECT        Id, FOID, FilePath, FileType, DateCreated
                                FROM            tbl_FOFile_Model
                                        WHERE        (FOID= '" + dr["FOID"].ToString() + "')";

                DataTable file_table = db.SelectDb(sql_file).Tables[0];
                var file_res = new List<FileModel>();
                string file = "";
                foreach (DataRow b_dr in file_table.Rows)
                {
                    var assets_item = new FileModel();
                    if (file_table.Rows.Count != 0)
                    {
                        file = b_dr["FilePath"].ToString();
                        assets_item.FilePath = file;

                    }
                    else
                    {
                        assets_item.FilePath = "";
                    }
                    file_res.Add(assets_item);
                }
                item.Files = file_res;
                item.SSS = dr["SSS"].ToString();
                item.PagIbig = dr["PagIbig"].ToString();
                item.PhilHealth = dr["PhilHealth"].ToString();
                item.IdNum = dr["ID_Number"].ToString();
                item.IDType_Name = dr["IDType_Name"].ToString();
                item.TypeID = dr["TypeID"].ToString();
                item.ProfilePath = dr["ProfilePath"].ToString();
                item.FrontID_Path = dr["FrontID_Path"].ToString();
                item.BackID_Path = dr["BackID_Path"].ToString();

                //city.Substring(0, city.Length - 1)
                string area_list = $@"SELECT [Id]
                                      ,[Area]
                                      ,[City]
                                      ,[FOID]
                                      ,[Status]
                                      ,[DateCreated]
                                      ,[DateUpdated]
                                      ,[AreaID]
                                  FROM [GoldOne].[dbo].[tbl_Area_Model] where FOID='" + dr["FOID"].ToString() + "'";

                DataTable area_table = db.SelectDb(area_list).Tables[0];
                string city = "";
                foreach (DataRow fo_dr in area_table.Rows)
                {
                    if (area_table.Rows.Count == 1)
                    {
                        city = fo_dr["Area"].ToString() + " " + fo_dr["City"].ToString();
                    }
                    else
                    {

                        city += fo_dr["Area"].ToString() + " " + fo_dr["City"].ToString() + ",";

                    }
                }
                //}
                //item.FOID = foid;
                //item.Location = city == "" ? "" : city.EndsWith(targetCharacter.ToString()) == true ? city.Substring(0, city.Length - 1) : city;


                item.arealists = city;
                result.Add(item);
            }

            return result;
        }
        //public List<FoVM> GetFieldOfficerFilterbyFOID(string foid)
        //{

        //    var result = new List<FoVM>();
        //    var param = new IDataParameter[]
        //{
        //            new SqlParameter("@FOID",foid)
        //};
        //    DataTable table = db.SelectDb_SP("FieldOfficerFilterByID", param).Tables[0];

        //    foreach (DataRow dr in table.Rows)
        //    {
        //        var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
        //        var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
        //        var item = new FoVM();
        //        item.FOID = dr["FOID"].ToString();
        //        item.DateCreated = datec;
        //        item.DateUpdated = dateu;
        //        item.Status = dr["Status"].ToString();
        //        item.Country = dr["Country"].ToString();
        //        item.Region = dr["Region"].ToString();
        //        item.City = dr["City"].ToString();
        //        item.Barangay = dr["Barangay"].ToString();
        //        item.HouseNo = dr["HouseNo"].ToString();
        //        item.EmailAddress = dr["EmailAddress"].ToString();
        //        item.Cno = dr["Cno"].ToString();
        //        item.CivilStatus = dr["CivilStatus"].ToString();
        //        item.POB = dr["POB"].ToString();
        //        item.Age = dr["Age"].ToString();
        //        item.DOB = dr["DOB"].ToString();
        //        item.Gender = dr["Gender"].ToString();
        //        item.Fullname = dr["Fullname"].ToString();
        //        item.Fname = dr["Fname"].ToString();
        //        item.Lname = dr["Lname"].ToString();
        //        item.Mname = dr["Mname"].ToString();
        //        item.Suffix = dr["Suffix"].ToString();

        //        string sql_file = $@"SELECT        tbl_FOFile_Model.Id, tbl_FOFile_Model.FOID, tbl_FOFile_Model.FilePath, tbl_FOFile_Model.DateCreated, tbl_TypesModel.TypeName AS FileType
        //                                FROM            tbl_FOFile_Model INNER JOIN
        //                                                         tbl_TypesModel ON tbl_FOFile_Model.FileType = tbl_TypesModel.Id INNER JOIN
        //                                                         tbl_FieldOfficer_Model ON tbl_FOFile_Model.FOID = tbl_FieldOfficer_Model.FOID
        //                                WHERE        (tbl_FOFile_Model.FOID = '" + dr["FOID"].ToString() + "')";

        //        DataTable file_table = db.SelectDb(sql_file).Tables[0];
        //        var file_res = new List<FileModel>();
        //        foreach (DataRow b_dr in file_table.Rows)
        //        {
        //            var assets_item = new FileModel();
        //            assets_item.FilePath = b_dr["FilePath"].ToString();
        //            assets_item.FileType = b_dr["FileType"].ToString();
        //            file_res.Add(assets_item);
        //        }
        //        item.Files = file_res;
        //        item.SSS = dr["SSS"].ToString();
        //        item.PagIbig = dr["PagIbig"].ToString();
        //        item.PhilHealth = dr["PhilHealth"].ToString();
        //        item.IdNum = dr["IdNum"].ToString();
        //        item.IDType = dr["IDType"].ToString();
        //        item.TypeID = dr["TypeID"].ToString();

        //        result.Add(item);
        //    }

        //    return result;
        //}
        public List<IDTypeModel> GetIDTypeList()
        {

            sql = $@"SELECT [Id]
                          ,[Type]
                          ,[TypeID]
                      FROM [dbo].[tbl_IDType_Model]";

            DataTable table = db.SelectDb(sql).Tables[0];
            var result = new List<IDTypeModel>();
            foreach (DataRow dr in table.Rows)
            {
                var item = new IDTypeModel();
                item.Type = dr["Type"].ToString();
                item.TypeID = dr["TypeID"].ToString();
                result.Add(item);
            }

            return result;
        }
        public List<FormulaVM> GetLoanFormulaList()
        {
            int ctr = 0;
            string res = "";
            var result = new List<FormulaVM>();
            DataTable table = db.SelectDb_SP("sp_getformula").Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                string results = "";
                string Formula = dr["Formula"].ToString();
                int index1 = 0;
                string LoanAmount = "Loan Amount";
                int index2 = 0;
                string Interest = "Interest";
                int index3 = 0;
                string Days = "Terms";
                int countlength = dr["Formula"].ToString().Length;
                //if (ctr == 2)
                //{
                //    string results_ = "";
                //    string Formula_ = dr["Formula"].ToString();
                //    int index1_ = 0;
                //    string LoanAmount_ = "Loan Principal";
                //    int index2_ = 0;
                //    string Interest_ = "Interest";
                //    int index3_ = 0;
                //    string Days_ = "Terms";
                //    int countlength_ = dr["Formula"].ToString().Length;
                //    if (countlength == 4)
                //    {
                //        index1_ = 1;
                //        index2_ = 2;
                //        index3_ = 4;

                //    }
                //    else
                //    {
                //        index1_ = 2;
                //        index2_ = 3;
                //        index3_ = 5;
                //    }

                //    res = InsertStringsAtIndexes(Formula_, index1_, LoanAmount_, index2_, Interest_, index3_, Days_);
                //}
                //else
                //{
                //    if (countlength == 4)
                //    {
                //        index1 = 1;
                //        index2 = 2;
                //        index3 = 4;

                //    }
                //    else
                //    {
                //        index1 = 2;
                //        index2 = 3;
                //        index3 = 5;
                //    }
                //    res = InsertStringsAtIndexes(Formula, index1, LoanAmount, index2, Interest, index3, Days);
                //    ctr++;

                //}


                var item = new FormulaVM();
                item.FormulaID = dr["APFID"].ToString();
                item.Formula = Formula;

                result.Add(item);
            }

            return result;
        }
        public List<LoanTypeDetailsVM> GetLoanTypeDetails()
        {
            var result = new List<LoanTypeDetailsVM>();
            DataTable table = db.SelectDb_SP("sp_LoanTypes").Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                var item = new LoanTypeDetailsVM();
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dateu = dr["DateUpdated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateUpdated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item.LoanTypeName = dr["LoanTypeName"].ToString();




                item.Savings = dr["Savings"].ToString();

                item.LoanAmount_Min = dr["LoanAmount_Min"].ToString();
                item.LoanAmount_Max = dr["LoanAmount_Max"].ToString();
                item.LoanTypeID = dr["LoanTypeID"].ToString();
                item.DateCreated = datec;
                item.DateUpdated = dateu;
                item.Status = dr["Status"].ToString();





                int ctr = 0;
                string res = "";
                var param = new IDataParameter[]
                {
                    new SqlParameter("@LoanTypeId",dr["LoanTypeID"].ToString())
                };
                DataTable termsinfo = db.SelectDb_SP("sp_TermsFilterByLoanTypeID", param).Tables[0];
                var terms = new List<TermOfPaymentVM>();
                foreach (DataRow t_dr in termsinfo.Rows)
                {
                    var items = new TermOfPaymentVM();
                    items.NameOfTerms = t_dr["NameOfTerms"].ToString();
                    items.LoanTypeId = t_dr["LoanTypeId"].ToString();
                    items.TypeOfCollection = t_dr["TypeOfCollection"].ToString();
                    items.TopId = t_dr["TopId"].ToString();
                    items.Terms = t_dr["Terms"].ToString();
                    items.InterestRate = (double.Parse(t_dr["InterestRate"].ToString()) * 100).ToString();
                    items.IR_Type = t_dr["IR_Type"].ToString();

                    items.FormulaID = t_dr["APFID"].ToString();
                    string results = "";
                    string Formula = t_dr["Formula"].ToString();
                    int index1 = 0;
                    string LoanAmount = "Loan Amount";
                    int index2 = 0;
                    string Interest = "Interest";
                    int index3 = 0;
                    string Days = "Days";
                    int countlength = t_dr["Formula"].ToString().Length;

                    if (t_dr["APFID"].ToString() == "APFID-03")
                    {
                        string results_ = "";
                        string Formula_ = t_dr["Formula"].ToString();
                        int index1_ = 0;
                        string LoanAmount_ = "Loan Principal";
                        int index2_ = 0;
                        string Interest_ = "Interest";
                        int index3_ = 0;
                        string Days_ = "Days";
                        int countlength_ = t_dr["Formula"].ToString().Length;
                        if (countlength == 4)
                        {
                            index1_ = 1;
                            index2_ = 2;
                            index3_ = 4;

                        }
                        else
                        {
                            index1_ = 2;
                            index2_ = 3;
                            index3_ = 5;
                        }

                        res = InsertStringsAtIndexes(Formula_, index1_, LoanAmount_, index2_, Interest_, index3_, Days_);
                    }
                    else
                    {
                        if (countlength != 0)
                        {

                            if (countlength == 4)
                            {
                                index1 = 1;
                                index2 = 2;
                                index3 = 4;

                            }
                            else
                            {
                                index1 = 2;
                                index2 = 3;
                                index3 = 5;
                            }

                            res = InsertStringsAtIndexes(Formula, index1, LoanAmount, index2, Interest, index3, Days);
                        }
                        ctr++;

                    }
                    items.Formula = res;
                    items.NoAdvancePayment = t_dr["NoAdvancePayment"].ToString();
                    items.NotarialFeeOrigin = t_dr["NotarialFeeOrigin"].ToString();
                    items.LessThanNotarialAmount = t_dr["LALV_TypeID"].ToString() == "1" ? (double.Parse(t_dr["LessThanNotarialAmount"].ToString()) * 100).ToString() : t_dr["LessThanNotarialAmount"].ToString();
                    items.LALV_Type = t_dr["LALV_Type"].ToString();
                    items.LALV_TypeID = t_dr["LALV_TypeID"].ToString();
                    items.GreaterThanEqualNotarialAmount = t_dr["LAGEF_TypeID"].ToString() == "1" ? (double.Parse(t_dr["GreaterThanEqualNotarialAmount"].ToString()) * 100).ToString() : t_dr["GreaterThanEqualNotarialAmount"].ToString();
                    items.LAGEF_Type = t_dr["LAGEF_Type"].ToString();
                    items.LAGEF_TypeID = t_dr["LAGEF_TypeID"].ToString();
                    items.LoanInsuranceAmount = t_dr["LoanI_TypeID"].ToString() == "1" ? (double.Parse(t_dr["LoanInsuranceAmount"].ToString()) * 100).ToString() : t_dr["LoanInsuranceAmount"].ToString();
                    items.LoanI_Type = t_dr["LoanI_Type"].ToString();
                    items.LoanI_TypeID = t_dr["LoanI_TypeID"].ToString();
                    items.LifeInsuranceAmount = t_dr["LifeI_TypeID"].ToString() == "1" ? (double.Parse(t_dr["LifeInsuranceAmount"].ToString()) * 100).ToString() : t_dr["LifeInsuranceAmount"].ToString();
                    items.LifeI_Type = t_dr["LifeI_Type"].ToString();
                    items.LifeI_TypeID = t_dr["LifeI_TypeID"].ToString();
                    items.DeductInterest = t_dr["DeductInterest"].ToString();
                    items.OldFormula = t_dr["OldFormula"].ToString();
                    items.InterestApplied = t_dr["InterestApplied"].ToString();
                    items.TypeOfCollectionID = t_dr["TypeOfCollectionID"].ToString();
                    items.Status = t_dr["Status"].ToString();
                    terms.Add(items);

                }
                item.TermsofPayment = terms;
                result.Add(item);
            }


            return result;
        }

        public List<TermsVM> getTermsList()
        {
            var result = new List<TermsVM>();
            DataTable table = db.SelectDb_SP("sp_termslist").Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                var item = new TermsVM();
                item.TermsofPayment = dr["NameOfTerms"].ToString();
                item.TopId = dr["TopId"].ToString();
                item.LoanTypeId = dr["LoanTypeId"].ToString();

                result.Add(item);
            }


            return result;
        }
        public List<TermOfPaymentVM> getTermsListFilterbyLoanTypeID(string loantypeid)
        {
            int ctr = 0;
            string res = "";
            var param = new IDataParameter[]
                {
                    new SqlParameter("@LoanTypeId",loantypeid)
                };
            DataTable termsinfo = db.SelectDb_SP("sp_TermsFilterByLoanTypeID", param).Tables[0];
            var terms = new List<TermOfPaymentVM>();
            foreach (DataRow t_dr in termsinfo.Rows)
            {
                var items = new TermOfPaymentVM();
                items.NameOfTerms = t_dr["NameOfTerms"].ToString();
                items.LoanTypeId = t_dr["LoanTypeId"].ToString();
                items.TypeOfCollection = t_dr["TypeOfCollection"].ToString();
                items.TopId = t_dr["TopId"].ToString();
                items.Terms = t_dr["Terms"].ToString();
                items.InterestRate = t_dr["InterestRate"].ToString();
                items.IR_Type = t_dr["IR_Type"].ToString();

                items.FormulaID = t_dr["APFID"].ToString();
                string results = "";
                string Formula = t_dr["Formula"].ToString();
                int index1 = 0;
                string LoanAmount = "Loan Amount";
                int index2 = 0;
                string Interest = "Interest";
                int index3 = 0;
                string Days = "Days";
                int countlength = t_dr["Formula"].ToString().Length;

                if (t_dr["APFID"].ToString() == "APFID-03")
                {
                    string results_ = "";
                    string Formula_ = t_dr["Formula"].ToString();
                    int index1_ = 0;
                    string LoanAmount_ = "Loan Principal";
                    int index2_ = 0;
                    string Interest_ = "Interest";
                    int index3_ = 0;
                    string Days_ = "Days";
                    int countlength_ = t_dr["Formula"].ToString().Length;
                    if (countlength == 4)
                    {
                        index1_ = 1;
                        index2_ = 2;
                        index3_ = 4;

                    }
                    else
                    {
                        index1_ = 2;
                        index2_ = 3;
                        index3_ = 5;
                    }

                    res = InsertStringsAtIndexes(Formula_, index1_, LoanAmount_, index2_, Interest_, index3_, Days_);
                }
                else
                {
                    if (countlength != 0)
                    {

                        if (countlength == 4)
                        {
                            index1 = 1;
                            index2 = 2;
                            index3 = 4;

                        }
                        else
                        {
                            index1 = 2;
                            index2 = 3;
                            index3 = 5;
                        }

                        res = InsertStringsAtIndexes(Formula, index1, LoanAmount, index2, Interest, index3, Days);
                    }
                    ctr++;

                }
                items.Formula = res;
                items.NoAdvancePayment = t_dr["NoAdvancePayment"].ToString();
                items.NotarialFeeOrigin = t_dr["NotarialFeeOrigin"].ToString();
                items.LessThanNotarialAmount = t_dr["LessThanNotarialAmount"].ToString();
                items.LALV_Type = t_dr["LALV_Type"].ToString();
                items.LALV_TypeID = t_dr["LALV_TypeID"].ToString();
                items.GreaterThanEqualNotarialAmount = t_dr["GreaterThanEqualNotarialAmount"].ToString();
                items.LAGEF_Type = t_dr["LAGEF_Type"].ToString();
                items.LAGEF_TypeID = t_dr["LAGEF_TypeID"].ToString();
                items.LoanInsuranceAmount = t_dr["LoanInsuranceAmount"].ToString();
                items.LoanI_Type = t_dr["LoanI_Type"].ToString();
                items.LoanI_TypeID = t_dr["LoanI_TypeID"].ToString();
                items.LifeInsuranceAmount = t_dr["LifeInsuranceAmount"].ToString();
                items.LifeI_Type = t_dr["LifeI_Type"].ToString();
                items.LifeI_TypeID = t_dr["LifeI_TypeID"].ToString();
                items.DeductInterest = t_dr["DeductInterest"].ToString();
                items.OldFormula = t_dr["OldFormula"].ToString();
                items.InterestApplied = t_dr["InterestApplied"].ToString();
                items.TypeOfCollectionID = t_dr["TypeOfCollectionID"].ToString();
                items.Status = t_dr["Status"].ToString();
                terms.Add(items);

            }
            return terms;
        }
        public List<LoanHistoryVM> GetLoanHistory()
        {

            var result = new List<LoanHistoryVM>();
            DataTable table = db.SelectDb_SP("GetLoanHistory").Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                var datereleased = dr["DateReleased"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateReleased"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var duedate = dr["DueDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DueDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var dofp = dr["DateOfFullPayment"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateOfFullPayment"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                var item = new LoanHistoryVM();
                item.LoanAmount = dr["LoanAmount"].ToString();
                item.Savings = dr["Savings"].ToString();
                item.Penalty = dr["Penalty"].ToString();
                item.OutStandingBalance = dr["OutStandingBalance"].ToString();
                item.DateReleased = datereleased;
                item.DueDate = duedate;
                item.DateCreated = datec;
                item.DateOfFullPayment = dofp;
                item.MemId = dr["MemId"].ToString();
                item.RefNo = dr["RefNo"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Status = dr["Status"].ToString();
                result.Add(item);
            }

            return result;
        }
        public List<MemberModelVM> GetMemberList()
        {
            var result = new List<MemberModelVM>();
            DataTable table = db.SelectDb_SP("sp_Memberlist").Tables[0];

            foreach (DataRow dr in table.Rows)
            {

                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string BOstatus = dr["BO_Status"].ToString() == "true" ? "1" : "0";
                var item = new MemberModelVM();
                item.Fullname = dr["Fullname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Age = dr["Age"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.City = dr["City"].ToString();
                item.Civil_Status = dr["Civil_Status"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.House_Stats = dr["House_Stats"].ToString();
                item.Country = dr["Country"].ToString();
                item.DOB = dr["DOB"].ToString();
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.HouseStatusId = dr["HouseStatusId"].ToString();
                item.ApplicationStatus = dr["ApplicationStatus"].ToString();
                item.Co_HouseStatusId = dr["Co_HouseStatusId"].ToString();
                item.POB = dr["POB"].ToString();
                item.Province = dr["Province"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Status = dr["MemberStatus"].ToString();
                item.DateCreated = datec;
                item.YearsStay = dr["YearsStay"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                //monthlybills
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.WaterBill = dr["WaterBill"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.OtherBills = dr["OtherBills"].ToString();
                item.DailyExpenses = dr["DailyExpenses"].ToString();
                //job
                item.Emp_Status = dr["Emp_Status"].ToString();
                item.BO_Status = BOstatus;
                item.OtherSOC = dr["OtherSOC"].ToString();
                item.MonthlySalary = dr["MonthlySalary"].ToString();
                item.CompanyName = dr["CompanyName"].ToString();
                item.CompanyAddress = dr["CompanyAddress"].ToString();
                item.YOS = dr["YOS"].ToString();
                item.JobDescription = dr["JobDescription"].ToString();
                item.CompanyAddress = dr["CompanyAddress"].ToString();

                //famv
                var famnod = dr["Fam_NOD"].ToString() == "" ? "0" : dr["Fam_NOD"].ToString();
                var F_YOS = dr["Fam_YOS"].ToString() == "" ? "0" : dr["Fam_YOS"].ToString();
                var F_Age = dr["Fam_Age"].ToString() == "" ? "0" : dr["Fam_Age"].ToString();
                var F_Emp_Status = dr["Fam_EmpStatus"].ToString() == "" ? "0" : dr["Fam_EmpStatus"].ToString();
                var F_DOB = dr["Fam_DOB"].ToString() == "" ? "" : Convert.ToDateTime(dr["Fam_DOB"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item.F_Fname = dr["Fam_Fname"].ToString();
                item.F_Lname = dr["Fam_Lname"].ToString();
                item.F_Mname = dr["Fam_Mname"].ToString();
                item.F_Suffix = dr["Fam_Suffix"].ToString();
                item.F_RTTB = dr["Fam_RTTB"].ToString();
                item.F_NOD = famnod.ToString();
                item.F_CompanyName = dr["Fam_CompanyName"].ToString();
                item.F_YOS = F_YOS;
                item.F_Job = dr["Position"].ToString();
                item.F_Emp_Status = F_Emp_Status;
                item.F_Age = F_Age;
                item.F_DOB = F_DOB;
                item.FamId = dr["FamId"].ToString();

                string sql_child = $@"SELECT        Id, Fname, Mname, Lname, Age, NOS, FamId, Status, DateCreated, DateUpdated
                            FROM            tbl_ChildInfo_Model
                            WHERE        (FamId = '" + dr["FamId"].ToString() + "')";

                DataTable child_table = db.SelectDb(sql_child).Tables[0];
                var child_res = new List<ChildModel>();
                foreach (DataRow c_dr in child_table.Rows)
                {
                    var items = new ChildModel();
                    items.Fname = c_dr["Fname"].ToString();
                    items.Lname = c_dr["Mname"].ToString();
                    items.Mname = c_dr["Lname"].ToString();
                    items.Age = int.Parse(c_dr["Age"].ToString());
                    items.NOS = c_dr["NOS"].ToString();
                    items.FamId = c_dr["FamId"].ToString();
                    child_res.Add(items);

                }
                item.Child = child_res;
                //business
                string sql_business = $@"SELECT        tbl_BusinessInformation_Model.Id, tbl_BusinessInformation_Model.BusinessName, tbl_BusinessInformation_Model.BusinessAddress, tbl_BusinessInformation_Model.YOB, tbl_BusinessInformation_Model.NOE, 
                         tbl_BusinessInformation_Model.Salary, tbl_BusinessInformation_Model.VOS, tbl_BusinessInformation_Model.AOS, tbl_BusinessInformation_Model.DateCreated, tbl_BusinessInformation_Model.DateUpdated, 
                         tbl_BusinessInformation_Model.BIID, tbl_Status_Model.Name AS Business_Status, tbl_Status_Model_1.Name AS Status, tbl_BusinessInformation_Model.BusinessType,tbl_BusinessInformation_Model.B_status AS B_statusID,tbl_BusinessInformation_Model.FilesUploaded

                        FROM            tbl_BusinessInformation_Model INNER JOIN
                                                 tbl_Status_Model ON tbl_BusinessInformation_Model.B_status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_BusinessInformation_Model.Status = tbl_Status_Model_1.Id
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable b_table = db.SelectDb(sql_business).Tables[0];
                var b_res = new List<BusinessModelVM>();
                foreach (DataRow b_dr in b_table.Rows)
                {
                    var b_item = new BusinessModelVM();
                    b_item.BusinessName = b_dr["BusinessName"].ToString();
                    b_item.BusinessType = b_dr["BusinessType"].ToString();
                    b_item.BusinessAddress = b_dr["BusinessAddress"].ToString();
                    b_item.B_statusID = b_dr["B_statusID"].ToString();
                    b_item.YOB = int.Parse(b_dr["YOB"].ToString());
                    b_item.NOE = int.Parse(b_dr["NOE"].ToString());
                    b_item.Salary = decimal.Parse(b_dr["Salary"].ToString());
                    b_item.VOS = decimal.Parse(b_dr["VOS"].ToString());
                    b_item.AOS = decimal.Parse(b_dr["AOS"].ToString());
                    var b_files = new List<FileModel>();
                    b_item.B_status = b_dr["Business_Status"].ToString();
                    if (b_dr["FilesUploaded"].ToString() != null)
                    {
                        var files = b_dr["FilesUploaded"].ToString().Split('|');

                        for (int x = 0; x < files.ToList().Count; x++)
                        {
                            var items = new FileModel();
                            items.FilePath = files[x];
                            b_files.Add(items);
                        }

                    }
                    b_item.BusinessFiles = b_files;
                    b_res.Add(b_item);
                }

                //Motor
                string sql_assets = $@"SELECT        MotorVehicles FROM            tbl_AssetsProperties_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable assets_table = db.SelectDb(sql_assets).Tables[0];
                var assest_res = new List<AssetsModel>();
                foreach (DataRow b_dr in assets_table.Rows)
                {
                    var assets_item = new AssetsModel();
                    assets_item.MotorVehicles = b_dr["MotorVehicles"].ToString();
                    assest_res.Add(assets_item);
                }

                //Property
                string sql_property = $@"SELECT     Property  FROM   tbl_Property_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable property_table = db.SelectDb(sql_property).Tables[0];
                var property_res = new List<PropertyDetailsModel>();
                foreach (DataRow b_dr in property_table.Rows)
                {
                    var property_item = new PropertyDetailsModel();
                    property_item.Property = b_dr["Property"].ToString();
                    property_res.Add(property_item);
                }

                //Bank
                string sql_bank = $@"SELECT        BankName, Address, DateCreated, DateUpdated, BankID, Status, MemId
                                        FROM            tbl_BankAccounts_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable bank_table = db.SelectDb(sql_bank).Tables[0];
                var bank_res = new List<BankModel>();
                foreach (DataRow b_dr in bank_table.Rows)
                {
                    var bank_item = new BankModel();
                    bank_item.BankName = b_dr["BankName"].ToString();
                    bank_item.Address = b_dr["Address"].ToString();
                    bank_res.Add(bank_item);
                }
                string sql_appliances = $@"SELECT        tbl_Appliance_Model.Brand, tbl_Appliance_Model.Description, tbl_Appliance_Model.NAID
                         FROM            tbl_Application_Model INNER JOIN
                         tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_Appliance_Model ON tbl_Application_Model.NAID = tbl_Appliance_Model.NAID
                                        WHERE        (tbl_Member_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable appliances_table = db.SelectDb(sql_appliances).Tables[0];
                var appliances_res = new List<ApplianceModel>();
                foreach (DataRow b_dr in appliances_table.Rows)
                {
                    var appliances_item = new ApplianceModel();
                    appliances_item.Appliances = b_dr["Description"].ToString();
                    appliances_item.Brand = b_dr["Brand"].ToString();
                    appliances_item.NAID = b_dr["NAID"].ToString();
                    appliances_res.Add(appliances_item);
                }
                //files

                string sql_files = $@"SELECT        tbl_fileupload_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_TypesModel.TypeName, tbl_Status_Model.Name AS Status
                         FROM            tbl_fileupload_Model INNER JOIN
                         tbl_TypesModel ON tbl_fileupload_Model.Type = tbl_TypesModel.Id INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                                        WHERE        (tbl_fileupload_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable file_table = db.SelectDb(sql_files).Tables[0];
                var file_res = new List<FileModel>();
                if (file_table.Rows.Count != 0)
                {
                    foreach (DataRow b_dr in file_table.Rows)
                    {
                        var file_item = new FileModel();
                        file_item.FileName = b_dr["FileName"].ToString();
                        file_item.FilePath = b_dr["FilePath"].ToString();
                        file_item.FileType = b_dr["TypeName"].ToString();
                        file_res.Add(file_item);
                    }
                }
                item.Files = file_res;
                item.Property = property_res;
                item.Appliances = appliances_res;
                item.Bank = bank_res;
                item.Assets = assest_res;
                item.Business = b_res;
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                //    var grouploan = GetGroupApplicationList().Where(a => a.MemId == dr["MemId"].ToString()).ToList();
                var individualloan = GetApplicationListFilter(dr["NAID"].ToString()).ToList();
                var group = new List<GroupApplicationVM2>();
                var individual_ = new List<ApplicationVM2>();


                //if (grouploan.Count != 0)
                //{
                //    for (int x = 0; x < grouploan.Count; x++)
                //    {
                //        var g_item = new GroupApplicationVM2();
                //        g_item.LoanAmount = grouploan[x].Loandetails[0].LoanAmount;
                //        g_item.Terms = grouploan[x].Loandetails[0].Terms;
                //        g_item.LoanType = grouploan[x].Loandetails[0].LoanType;
                //        g_item.InterestRate = grouploan[x].Loandetails[0].InterestRate;
                //        g_item.GroupId = grouploan[x].GroupId;
                //        g_item.LDID = grouploan[x].Loandetails[0].LDID;

                //        g_item.CI_ApprovedBy = grouploan[x].Loandetails[0].CI_ApprovedBy;
                //        g_item.CI_ApprovalDate = grouploan[x].Loandetails[0].CI_ApprovalDate;
                //        g_item.ReleasingDate = grouploan[x].Loandetails[0].ReleasingDate;
                //        g_item.DeclineDate = grouploan[x].Loandetails[0].DeclineDate;
                //        g_item.App_ApprovedBy_1 = grouploan[x].Loandetails[0].App_ApprovedBy_1;
                //        g_item.App_ApprovalDate_1 = grouploan[x].Loandetails[0].App_ApprovalDate_1;
                //        g_item.App_ApprovedBy_2 = grouploan[x].Loandetails[0].App_ApprovedBy_2;
                //        g_item.App_ApprovalDate_2 = grouploan[x].Loandetails[0].App_ApprovalDate_2;
                //        g_item.App_Note = grouploan[x].Loandetails[0].App_Note;
                //        g_item.App_Notedby = grouploan[x].Loandetails[0].App_Notedby;
                //        g_item.App_NotedDate = grouploan[x].Loandetails[0].App_NotedDate;
                //        g_item.CreatedBy = grouploan[x].Loandetails[0].CreatedBy;
                //        g_item.SubmittedBy = grouploan[x].Loandetails[0].SubmittedBy;
                //        g_item.DateSubmitted = grouploan[x].Loandetails[0].DateSubmitted;
                //        g_item.ReleasedBy = grouploan[x].Loandetails[0].ReleasedBy;

                //        g_item.ModeOfRelease = grouploan[x].Loandetails[0].ModeOfRelease;
                //        g_item.ModeOfReleaseReference = grouploan[x].Loandetails[0].ModeOfReleaseReference;
                //        g_item.Courerier = grouploan[x].Loandetails[0].Courerier;
                //        g_item.CourierCNo = grouploan[x].Loandetails[0].CourierCNo;
                //        g_item.CourerierName = grouploan[x].Loandetails[0].CourerierName;
                //        g_item.Denomination = grouploan[x].Loandetails[0].Denomination;
                //        g_item.AreaName = grouploan[x].Loandetails[0].AreaName;
                //        g_item.Remarks = grouploan[x].Loandetails[0].Remarks;
                //        g_item.ApprovedLoanAmount = grouploan[x].Loandetails[0].ApprovedLoanAmount;
                //        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                //        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                //        g_item.Days = grouploan[x].Loandetails[0].Days;
                //        group.Add(g_item);
                //    }

                //}
                //else
                //{
                for (int x = 0; x < individualloan.Count; x++)
                {
                    var i_item = new ApplicationVM2();
                    i_item.LoanAmount = individualloan[x].LoanAmount;
                    i_item.Terms = individualloan[x].TermsOfPayment;
                    i_item.InterestRate = individualloan[x].Interest;
                    i_item.LoanType = individualloan[x].LoanType;
                    i_item.LoanTypeID = individualloan[x].LoanTypeID;
                    i_item.LDID = individualloan[x].LDID;
                    i_item.NameOfTerms = individualloan[x].NameOfTerms;

                    i_item.CI_ApprovedBy = individualloan[x].CI_ApprovedBy;
                    i_item.CI_ApprovalDate = individualloan[x].CI_ApprovalDate;
                    i_item.ReleasingDate = individualloan[x].ReleasingDate;
                    i_item.DeclineDate = individualloan[x].DeclineDate;
                    i_item.DeclinedBy = individualloan[x].DeclinedBy;
                    i_item.App_ApprovedBy_1 = individualloan[x].App_ApprovedBy_1;
                    i_item.App_ApprovalDate_1 = individualloan[x].App_ApprovalDate_1;
                    i_item.App_ApprovedBy_2 = individualloan[x].App_ApprovedBy_2;
                    i_item.App_ApprovalDate_2 = individualloan[x].App_ApprovalDate_2;
                    i_item.App_Note = individualloan[x].App_Note;
                    i_item.App_Notedby = individualloan[x].App_Notedby;
                    i_item.App_NotedDate = individualloan[x].App_NotedDate;
                    i_item.CreatedBy = individualloan[x].CreatedBy;
                    i_item.SubmittedBy = individualloan[x].SubmittedBy;
                    i_item.DateSubmitted = individualloan[x].DateSubmitted;
                    i_item.ReleasedBy = individualloan[x].ReleasedBy;

                    i_item.ModeOfRelease = individualloan[x].ModeOfRelease;
                    i_item.ModeOfReleaseReference = individualloan[x].ModeOfReleaseReference;
                    i_item.Courerier = individualloan[x].Courerier;
                    i_item.CourierCNo = individualloan[x].CourierCNo;
                    i_item.CourerierName = individualloan[x].CourerierName;
                    i_item.Denomination = individualloan[x].Denomination;
                    i_item.AreaName = individualloan[x].AreaName;
                    i_item.Remarks = individualloan[x].Remarks;
                    i_item.ApprovedLoanAmount = individualloan[x].ApprovedLoanAmount;
                    i_item.ApprovedTermsOfPayment = individualloan[x].ApprovedTermsOfPayment;
                    i_item.Days = individualloan[x].Days;

                    individual_.Add(i_item);
                }
                //}
                //item.GroupLoan = group;
                item.IndividualLoan = individual_;
                item.TermsOfPayment = dr["TermsOfPayment"].ToString();
                item.Purpose = dr["Purpose"].ToString();
                //co maker
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Lnam"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Gender = dr["Co_Gender"].ToString();
                var co_dob = dr["Co_DOB"].ToString() == "" ? "0.00" : Convert.ToDateTime(dr["Co_DOB"].ToString()).ToString("yyyy-MM-dd");
                item.Co_DOB = co_dob;
                item.Co_POB = dr["Co_POB"].ToString();
                item.Co_Age = dr["Co_Age"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.Co_Civil_Status = dr["CivilStatus"].ToString();
                item.Co_EmailAddress = dr["Co_EmailAddress"].ToString();
                item.Co_HouseNo = dr["Co_HouseNo"].ToString();
                item.Co_Barangay = dr["Co_Barangay"].ToString();
                item.Co_City = dr["Co_City"].ToString();
                item.Co_Province = dr["Region"].ToString();
                item.Co_Country = dr["Co_Country"].ToString();
                item.Co_ZipCode = dr["Co_ZipCode"].ToString();
                item.Co_YearsStay = dr["Co_YOS"].ToString();
                item.Co_RTTB = dr["Co_RTTB"].ToString();
                item.CMID = dr["CMID"].ToString();
                //Co_job
                item.Co_JobDescription = dr["Co_JobDescription"].ToString();
                item.Coj_YOS = dr["Coj_YOS"].ToString();
                item.Co_CompanyName = dr["Co_CompanyName"].ToString();
                item.Co_CompanyAddress = dr["Co_CompanyAddress"].ToString();
                item.Co_MonthlySalary = dr["Co_MonthlySalary"].ToString();
                item.Co_OtherSOC = dr["Co_OtherSOC"].ToString();
                item.Co_Emp_Status = dr["Co_Emp_Status"].ToString();
                item.Co_BO_Status = dr["Co_BO_Status"].ToString();
                item.Co_House_Stats = dr["Co_House_Stats"].ToString();


                ///


                result.Add(item);
            }

            return result;
        }

        public List<MemberModelVM> GetMembershipFilterByFullname(string fullname)
        {

            var result = new List<MemberModelVM>();
            var param = new IDataParameter[]
            {
                    new SqlParameter("@Fullname",fullname)
            };
            DataTable table = db.SelectDb_SP("sp_MemberFiltering", param).Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string BOstatus = dr["BO_Status"].ToString() == "true" ? "1" : dr["BO_Status"].ToString();
                var item = new MemberModelVM();
                item.Fullname = dr["Fullname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Age = dr["Age"].ToString();
                item.Barangay = dr["Barangay"].ToString();
                item.City = dr["City"].ToString();
                item.Civil_Status = dr["Civil_Status"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.House_Stats = dr["House_Stats"].ToString();
                item.Country = dr["Country"].ToString();
                item.DOB = dr["DOB"].ToString();
                item.EmailAddress = dr["EmailAddress"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.HouseNo = dr["HouseNo"].ToString();
                item.POB = dr["POB"].ToString();
                item.Province = dr["Province"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Status = dr["MemberStatus"].ToString();
                item.DateCreated = datec;
                item.YearsStay = dr["YearsStay"].ToString();
                item.ZipCode = dr["ZipCode"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.WaterBill = dr["WaterBill"].ToString();
                item.ElectricBill = dr["ElectricBill"].ToString();
                item.OtherBills = dr["OtherBills"].ToString();
                item.DailyExpenses = dr["DailyExpenses"].ToString();
                item.Emp_Status = dr["Emp_Status"].ToString();
                item.BO_Status = BOstatus;
                item.OtherSOC = dr["OtherSOC"].ToString();
                item.MonthlySalary = dr["MonthlySalary"].ToString();
                item.CompanyName = dr["CompanyName"].ToString();
                item.CompanyAddress = dr["CompanyAddress"].ToString();
                item.YOS = dr["YOS"].ToString();
                item.JobDescription = dr["JobDescription"].ToString();
                var famnod = dr["Fam_NOD"].ToString() == "" ? "0" : dr["Fam_NOD"].ToString();
                var F_YOS = dr["Fam_YOS"].ToString() == "" ? "0" : dr["Fam_YOS"].ToString();
                var F_Age = dr["Fam_Age"].ToString() == "" ? "0" : dr["Fam_Age"].ToString();
                var F_Emp_Status = dr["Fam_EmpStatus"].ToString() == "" ? "0" : dr["Fam_EmpStatus"].ToString();
                var F_DOB = dr["Fam_DOB"].ToString() == "" ? "" : Convert.ToDateTime(dr["Fam_DOB"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                item.F_Fname = dr["Fam_Fname"].ToString();
                item.F_Lname = dr["Fam_Lname"].ToString();
                item.F_Mname = dr["Fam_Mname"].ToString();
                item.F_Suffix = dr["Fam_Suffix"].ToString();
                item.F_RTTB = dr["Fam_RTTB"].ToString();
                item.F_NOD = famnod.ToString();
                item.F_CompanyName = dr["Fam_CompanyName"].ToString();
                item.F_YOS = F_YOS;
                item.F_Job = dr["Position"].ToString();
                item.F_Emp_Status = F_Emp_Status;
                item.F_Age = F_Age;
                item.F_DOB = F_DOB;
                item.FamId = dr["FamId"].ToString();

                string sql_child = $@"SELECT        Id, Fname, Mname, Lname, Age, NOS, FamId, Status, DateCreated, DateUpdated
                            FROM            tbl_ChildInfo_Model
                            WHERE        (FamId = '" + dr["FamId"].ToString() + "')";

                DataTable child_table = db.SelectDb(sql_child).Tables[0];
                var child_res = new List<ChildModel>();
                foreach (DataRow c_dr in child_table.Rows)
                {
                    var items = new ChildModel();
                    items.Fname = c_dr["Fname"].ToString();
                    items.Lname = c_dr["Mname"].ToString();
                    items.Mname = c_dr["Lname"].ToString();
                    items.Age = int.Parse(c_dr["Age"].ToString());
                    items.NOS = c_dr["NOS"].ToString();
                    items.FamId = c_dr["FamId"].ToString();
                    child_res.Add(items);

                }
                item.Child = child_res;
                //business
                string sql_business = $@"SELECT        tbl_BusinessInformation_Model.Id, tbl_BusinessInformation_Model.BusinessName, tbl_BusinessInformation_Model.BusinessAddress, tbl_BusinessInformation_Model.YOB, tbl_BusinessInformation_Model.NOE, 
                         tbl_BusinessInformation_Model.Salary, tbl_BusinessInformation_Model.VOS, tbl_BusinessInformation_Model.AOS, tbl_BusinessInformation_Model.DateCreated, tbl_BusinessInformation_Model.DateUpdated, 
                         tbl_BusinessInformation_Model.BIID, tbl_Status_Model.Name AS Business_Status, tbl_Status_Model_1.Name AS Status, tbl_BusinessInformation_Model.BusinessType,tbl_BusinessInformation_Model.B_status AS B_statusID,tbl_BusinessInformation_Model.FilesUploaded

                        FROM            tbl_BusinessInformation_Model INNER JOIN
                                                 tbl_Status_Model ON tbl_BusinessInformation_Model.B_status = tbl_Status_Model.Id INNER JOIN
                         tbl_Status_Model AS tbl_Status_Model_1 ON tbl_BusinessInformation_Model.Status = tbl_Status_Model_1.Id
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable b_table = db.SelectDb(sql_business).Tables[0];
                var b_res = new List<BusinessModelVM>();
                foreach (DataRow b_dr in b_table.Rows)
                {
                    var b_item = new BusinessModelVM();
                    b_item.BusinessName = b_dr["BusinessName"].ToString();
                    b_item.BusinessType = b_dr["BusinessType"].ToString();
                    b_item.BusinessAddress = b_dr["BusinessAddress"].ToString();
                    b_item.B_statusID = b_dr["B_statusID"].ToString();
                    b_item.YOB = int.Parse(b_dr["YOB"].ToString());
                    b_item.NOE = int.Parse(b_dr["NOE"].ToString());
                    b_item.Salary = decimal.Parse(b_dr["Salary"].ToString());
                    b_item.VOS = decimal.Parse(b_dr["VOS"].ToString());
                    b_item.AOS = decimal.Parse(b_dr["AOS"].ToString());
                    var b_files = new List<FileModel>();
                    b_item.B_status = b_dr["Business_Status"].ToString();
                    if (b_dr["FilesUploaded"].ToString() != null)
                    {
                        var files = b_dr["FilesUploaded"].ToString().Split('|');

                        for (int x = 0; x < files.ToList().Count; x++)
                        {
                            var items = new FileModel();
                            items.FilePath = files[x];
                            b_files.Add(items);
                        }

                    }
                    b_item.BusinessFiles = b_files;
                    b_res.Add(b_item);
                }

                string sql_assets = $@"SELECT        MotorVehicles FROM            tbl_AssetsProperties_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable assets_table = db.SelectDb(sql_assets).Tables[0];
                var assest_res = new List<AssetsModel>();
                foreach (DataRow b_dr in assets_table.Rows)
                {
                    var assets_item = new AssetsModel();
                    assets_item.MotorVehicles = b_dr["MotorVehicles"].ToString();
                    assest_res.Add(assets_item);
                }

                //Property
                string sql_property = $@"SELECT     Property  FROM   tbl_Property_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable property_table = db.SelectDb(sql_property).Tables[0];
                var property_res = new List<PropertyDetailsModel>();
                foreach (DataRow b_dr in property_table.Rows)
                {
                    var property_item = new PropertyDetailsModel();
                    property_item.Property = b_dr["Property"].ToString();
                    property_res.Add(property_item);
                }

                string sql_bank = $@"SELECT        BankName, Address, DateCreated, DateUpdated, BankID, Status, MemId
                                        FROM            tbl_BankAccounts_Model
                                        WHERE        (MemId = '" + dr["MemId"].ToString() + "')";

                DataTable bank_table = db.SelectDb(sql_bank).Tables[0];
                var bank_res = new List<BankModel>();
                foreach (DataRow b_dr in bank_table.Rows)
                {
                    var bank_item = new BankModel();
                    bank_item.BankName = b_dr["BankName"].ToString();
                    bank_item.Address = b_dr["Address"].ToString();
                    bank_res.Add(bank_item);
                }
                string sql_appliances = $@"SELECT        tbl_Appliance_Model.Brand, tbl_Appliance_Model.Description, tbl_Appliance_Model.NAID
                         FROM            tbl_Application_Model INNER JOIN
                         tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_Appliance_Model ON tbl_Application_Model.NAID = tbl_Appliance_Model.NAID
                                        WHERE        (tbl_Member_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable appliances_table = db.SelectDb(sql_appliances).Tables[0];
                var appliances_res = new List<ApplianceModel>();
                foreach (DataRow b_dr in appliances_table.Rows)
                {
                    var appliances_item = new ApplianceModel();
                    appliances_item.Appliances = b_dr["Description"].ToString();
                    appliances_item.Brand = b_dr["Brand"].ToString();
                    appliances_item.NAID = b_dr["NAID"].ToString();
                    appliances_res.Add(appliances_item);
                }  //files

                string sql_files = $@"SELECT        tbl_fileupload_Model.MemId, tbl_fileupload_Model.FileName, tbl_fileupload_Model.FilePath, tbl_TypesModel.TypeName, tbl_Status_Model.Name AS Status
                         FROM            tbl_fileupload_Model INNER JOIN
                         tbl_TypesModel ON tbl_fileupload_Model.Type = tbl_TypesModel.Id INNER JOIN
                         tbl_Status_Model ON tbl_fileupload_Model.Status = tbl_Status_Model.Id
                                        WHERE        (tbl_fileupload_Model.MemId = '" + dr["MemId"].ToString() + "')";

                DataTable file_table = db.SelectDb(sql_files).Tables[0];
                var file_res = new List<FileModel>();
                if (file_table.Rows.Count != 0)
                {
                    foreach (DataRow b_dr in file_table.Rows)
                    {
                        var file_item = new FileModel();
                        file_item.FileName = b_dr["FileName"].ToString();
                        file_item.FilePath = b_dr["FilePath"].ToString();
                        file_item.FileType = b_dr["TypeName"].ToString();
                        file_res.Add(file_item);
                    }
                }
                item.Files = file_res;
                item.Property = property_res;
                item.Appliances = appliances_res;
                item.Bank = bank_res;
                item.Assets = assest_res;
                item.Business = b_res;
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                //item.LoanAmount = decimal.Parse(dr["LoanAmount"].ToString());
                //var amount = dr["LoanAmount"].ToString() == "" ? "0.00" : dr["LoanAmount"].ToString();
                // item.LoanAmount = decimal.Parse(amount);
                var grouploan = GetGroupApplicationList().Where(a => a.MemId == dr["MemId"].ToString()).ToList();
                var individualloan = GetApplicationListFilter(dr["NAID"].ToString()).ToList();
                var group = new List<GroupApplicationVM2>();
                var individual_ = new List<ApplicationVM2>();


                if (grouploan.Count != 0)
                {
                    for (int x = 0; x < grouploan.Count; x++)
                    {
                        var g_item = new GroupApplicationVM2();
                        g_item.LoanAmount = grouploan[x].Loandetails[0].LoanAmount;
                        g_item.Terms = grouploan[x].Loandetails[0].Terms;
                        g_item.LoanType = grouploan[x].Loandetails[0].LoanType;
                        g_item.InterestRate = grouploan[x].Loandetails[0].InterestRate;
                        g_item.GroupId = grouploan[x].GroupId;
                        g_item.LDID = grouploan[x].Loandetails[0].LDID;

                        g_item.CI_ApprovedBy = grouploan[x].Loandetails[0].CI_ApprovedBy;
                        g_item.CI_ApprovalDate = grouploan[x].Loandetails[0].CI_ApprovalDate;
                        g_item.ReleasingDate = grouploan[x].Loandetails[0].ReleasingDate;
                        g_item.DeclineDate = grouploan[x].Loandetails[0].DeclineDate;
                        g_item.App_ApprovedBy_1 = grouploan[x].Loandetails[0].App_ApprovedBy_1;
                        g_item.App_ApprovalDate_1 = grouploan[x].Loandetails[0].App_ApprovalDate_1;
                        g_item.App_ApprovedBy_2 = grouploan[x].Loandetails[0].App_ApprovedBy_2;
                        g_item.App_ApprovalDate_2 = grouploan[x].Loandetails[0].App_ApprovalDate_2;
                        g_item.App_Note = grouploan[x].Loandetails[0].App_Note;
                        g_item.App_Notedby = grouploan[x].Loandetails[0].App_Notedby;
                        g_item.App_NotedDate = grouploan[x].Loandetails[0].App_NotedDate;
                        g_item.CreatedBy = grouploan[x].Loandetails[0].CreatedBy;
                        g_item.SubmittedBy = grouploan[x].Loandetails[0].SubmittedBy;
                        g_item.DateSubmitted = grouploan[x].Loandetails[0].DateSubmitted;
                        g_item.ReleasedBy = grouploan[x].Loandetails[0].ReleasedBy;

                        g_item.ModeOfRelease = grouploan[x].Loandetails[0].ModeOfRelease;
                        g_item.ModeOfReleaseReference = grouploan[x].Loandetails[0].ModeOfReleaseReference;
                        g_item.Courerier = grouploan[x].Loandetails[0].Courerier;
                        g_item.CourierCNo = grouploan[x].Loandetails[0].CourierCNo;
                        g_item.CourerierName = grouploan[x].Loandetails[0].CourerierName;
                        g_item.Denomination = grouploan[x].Loandetails[0].Denomination;
                        g_item.AreaName = grouploan[x].Loandetails[0].AreaName;
                        g_item.Remarks = grouploan[x].Loandetails[0].Remarks;
                        g_item.ApprovedLoanAmount = grouploan[x].Loandetails[0].ApprovedLoanAmount;
                        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                        g_item.ApprovedTermsOfPayment = grouploan[x].Loandetails[0].ApprovedTermsOfPayment;
                        g_item.Days = grouploan[x].Loandetails[0].Days;
                        group.Add(g_item);
                    }

                }
                else
                {
                    for (int x = 0; x < individualloan.Count; x++)
                    {
                        var i_item = new ApplicationVM2();
                        i_item.LoanAmount = individualloan[x].LoanAmount;
                        i_item.Terms = individualloan[x].TermsOfPayment;
                        i_item.NameOfTerms = individualloan[x].NameOfTerms;
                        i_item.InterestRate = individualloan[x].Interest;
                        i_item.LoanType = individualloan[x].LoanType;
                        i_item.LoanTypeID = individualloan[x].LoanTypeID;
                        i_item.LDID = individualloan[x].LDID;

                        i_item.CI_ApprovedBy = individualloan[x].CI_ApprovedBy;
                        i_item.CI_ApprovalDate = individualloan[x].CI_ApprovalDate;
                        i_item.ReleasingDate = individualloan[x].ReleasingDate;
                        i_item.DeclineDate = individualloan[x].DeclineDate;
                        i_item.DeclinedBy = individualloan[x].DeclinedBy;
                        i_item.App_ApprovedBy_1 = individualloan[x].App_ApprovedBy_1;
                        i_item.App_ApprovalDate_1 = individualloan[x].App_ApprovalDate_1;
                        i_item.App_ApprovedBy_2 = individualloan[x].App_ApprovedBy_2;
                        i_item.App_ApprovalDate_2 = individualloan[x].App_ApprovalDate_2;
                        i_item.App_Note = individualloan[x].App_Note;
                        i_item.App_Notedby = individualloan[x].App_Notedby;
                        i_item.App_NotedDate = individualloan[x].App_NotedDate;
                        i_item.CreatedBy = individualloan[x].CreatedBy;
                        i_item.SubmittedBy = individualloan[x].SubmittedBy;
                        i_item.DateSubmitted = individualloan[x].DateSubmitted;
                        i_item.ReleasedBy = individualloan[x].ReleasedBy;

                        i_item.ModeOfRelease = individualloan[x].ModeOfRelease;
                        i_item.ModeOfReleaseReference = individualloan[x].ModeOfReleaseReference;
                        i_item.Courerier = individualloan[x].Courerier;
                        i_item.CourierCNo = individualloan[x].CourierCNo;
                        i_item.CourerierName = individualloan[x].CourerierName;
                        i_item.Denomination = individualloan[x].Denomination;
                        i_item.AreaName = individualloan[x].AreaName;
                        i_item.Remarks = individualloan[x].Remarks;
                        i_item.ApprovedLoanAmount = individualloan[x].ApprovedLoanAmount;
                        i_item.ApprovedTermsOfPayment = individualloan[x].ApprovedTermsOfPayment;
                        i_item.Days = individualloan[x].Days;

                        individual_.Add(i_item);
                    }
                }
                item.GroupLoan = group;
                item.IndividualLoan = individual_;
                item.TermsOfPayment = dr["TermsOfPayment"].ToString();
                item.Purpose = dr["Purpose"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Lnam"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Gender = dr["Co_Gender"].ToString();
                var co_dob = dr["Co_DOB"].ToString() == "" ? "0.00" : Convert.ToDateTime(dr["Co_DOB"].ToString()).ToString("yyyy-MM-dd");
                item.Co_DOB = co_dob;
                item.Co_POB = dr["Co_POB"].ToString();
                item.Co_Age = dr["Co_Age"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.Co_Civil_Status = dr["CivilStatus"].ToString();
                item.Co_EmailAddress = dr["Co_EmailAddress"].ToString();
                item.Co_HouseNo = dr["Co_HouseNo"].ToString();
                item.Co_Barangay = dr["Co_Barangay"].ToString();
                item.Co_City = dr["Co_City"].ToString();
                item.Co_Province = dr["Region"].ToString();
                item.Co_Country = dr["Co_Country"].ToString();
                item.Co_ZipCode = dr["Co_ZipCode"].ToString();
                item.Co_YearsStay = dr["Co_YOS"].ToString();
                item.Co_RTTB = dr["Co_RTTB"].ToString();
                item.CMID = dr["CMID"].ToString();
                item.Co_JobDescription = dr["Co_JobDescription"].ToString();
                item.Coj_YOS = dr["Coj_YOS"].ToString();
                item.Co_CompanyName = dr["Co_CompanyName"].ToString();
                item.Co_CompanyAddress = dr["Co_CompanyAddress"].ToString();
                item.Co_MonthlySalary = dr["Co_MonthlySalary"].ToString();
                item.Co_OtherSOC = dr["Co_OtherSOC"].ToString();
                item.Co_Emp_Status = dr["Co_Emp_Status"].ToString();
                item.Co_BO_Status = dr["Co_BO_Status"].ToString();
                item.Co_House_Stats = dr["Co_House_Stats"].ToString();
                result.Add(item);
            }

            return result;
        }


        public class MultipleStringss
        {
            public string advance_payment { get; set; }
            public string dailyCollectiblesSum { get; set; }
            public string balance { get; set; }
            public string advance { get; set; }
            public string lapses { get; set; }
            public string collectedamount { get; set; }
            public string AreaName { get; set; }
            public string AreaID { get; set; }
            public string Area_RefNo { get; set; }
            public string Collection_RefNo { get; set; }
            public string FOID { get; set; }
            public string FieldOfficer { get; set; }
            public string DateCreated { get; set; }

        }

        public List<MultipleStringss> getCollectionValue()
        {
            var areas = getAreaLoanSummary().GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.FOID, a.Area_RefNo, a.Collection_RefNo, a.DateCreated }).ToList();
            var result = new List<MultipleStringss>();
            foreach (var group in areas)
            {

                var advance_payment = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID && a.AdvancePayment != null).Select(a => double.Parse(a.AdvancePayment)).Sum();
                var dailyCollectiblesSum = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.DailyCollectibles)).Sum();
                var savings = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.TotalSavingsAmount)).Sum();
                var balance = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.AmountDue)).Sum();
                var advance = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                var lapses = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.LapsePayment)).Sum();
                var collectedamount = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.CollectedAmount)).Sum();

                var items = new MultipleStringss();
                items.advance_payment = advance_payment.ToString();
                items.dailyCollectiblesSum = dailyCollectiblesSum.ToString();
                items.balance = balance.ToString();
                items.lapses = lapses.ToString();
                items.advance = advance.ToString();
                items.collectedamount = collectedamount.ToString();
                items.AreaName = group.Key.AreaName;
                items.AreaID = group.Key.AreaID;
                items.FieldOfficer = group.Key.FieldOfficer;
                items.FOID = group.Key.FOID;
                items.Area_RefNo = group.Key.Area_RefNo;
                items.Collection_RefNo = group.Key.Collection_RefNo;
                items.DateCreated = group.Key.DateCreated;
                result.Add(items);
            }


            return result;

            //MultipleStrings strings = new MultipleStrings
            //{
            //    add_dueday = add_dueday,
            //    total_days = total_days.ToString(),
            //    total_loan_amount = total_loan_amount.ToString(),
            //    total = interest_rate.ToString(),
            //    total_notarial = total_notarial.ToString(),
            //    total_loaninsurance = total_loaninsurance.ToString(),
            //    loanreceivable = loanreceivable.ToString(),
            //    daily_collectibles = daily_collectibles.ToString(),
            //    LifeInsurance = LifeInsurances.ToString(),
            //    LoanBalance = loan_bal.ToString(),
            //    AdvancePayment = AdvancePayment.ToString()
            //};
            return result;
        }
        public class MultipleStrings
        {
            public string add_dueday { get; set; }
            public string total_days { get; set; }
            public string total_loan_amount { get; set; }
            public string total { get; set; }
            public string total_notarial { get; set; }
            public string total_loaninsurance { get; set; }
            public string loanreceivable { get; set; }
            public string daily_collectibles { get; set; }
            public string LifeInsurance { get; set; }
            public string AdvancePayment { get; set; }
            public string Savings { get; set; }
            public string LoanBalance { get; set; }
            public string HolidayAmount { get; set; }
            public string HolidayDaysCount { get; set; }
            public string DayOfHoliday { get; set; }
            public string DateOfHoliday { get; set; }
            public string Deduct_Interest { get; set; }

        }
        public class dayresult
        {
            public string totaldays { get; set; }

        }
        public dayresult datecomputation(string date, int value)
        {
            int total_days = 0;
            double holiday = 0;
            int ctr = 0;
            int payment_count = 0;
            string add_dueday = "0";
            string DayOfHoliday = "";
            string DateOfHoliday = "";
            int ctr_holiday = 0;
            add_dueday = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            int sundaycount = CountSundaysBetweenDates(DateTime.Parse(date), DateTime.Parse(add_dueday).AddDays(value));

            for (DateTime currentDate = DateTime.Parse(add_dueday); currentDate <= DateTime.Parse(add_dueday).AddDays(value); currentDate = currentDate.AddDays(1))
            {
                ctr_holiday++;
                var dates = Convert.ToDateTime(currentDate).ToString("yyyy-MM-dd");

                var results = GetHolidayList().Where(a => a.Date == dates).ToList().Count;
                if (results != 0)
                {
                    ctr++;
                    DayOfHoliday += ctr_holiday + ", ";
                    DateOfHoliday += ctr_holiday + ": " + dates + "| ";
                }
            }
            total_days = value + sundaycount + ctr;
            dayresult strings = new dayresult
            {
                totaldays = total_days.ToString()
            };
            return strings;
        }
        public MultipleStrings GetMultipleStrings(string App_ApprovalDate, string Days, string memid, string interestrate, string loanprincipal
             , string loanamountgreaterthan_val, string loanamountgreaterequal, string lagef_type, string loanamountgreatethan_val
             , string loanamountlessthan, string lav_type, string loaninsurance, string loani_type, string formulas, string apfid
             , string InterestType, string OldFormula, string NoAdvancePayment, string InterestApplied, string DeductInterest, string TypeOfCollection, string NotarialFeeOrigin, string LifeInsurance, string Savings, string value)
        {
            int total_days = 0;
            double total = 0;
            double total_loan_amount = 0;
            double loan_bal = 0;
            double get_notiral = 0;
            double total_notarial = 0;
            double loanreceivable = 0;
            double loan_amount = 0;
            double total_loaninsurance = 0;
            double total_deduction = 0;
            var formula = formulas;
            double final_formula = 0;
            double amountreceived = 0;
            double interest = 0;
            double interest_rate = 0;
            double LifeInsurances = 0;
            double AdvancePayment = 0;
            double daily_collectibles = 0;
            double deduct_interest = 0;
            double advance_totalamount = 0;

            double holiday = 0;
            int days = 0;
            int ctr = 0;
            int payment_count = 0;
            string add_dueday = "0";
            string DayOfHoliday = "";
            string DateOfHoliday = "";
            int ctr_holiday = 0;
            if (TypeOfCollection == "Daily")
            {
                days = int.Parse(Days);
            }
            else
            {

                days = int.Parse(Days) * int.Parse(value);
            }
            if (App_ApprovalDate != "")
            {

                add_dueday = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                int sundaycount = CountSundaysBetweenDates(DateTime.Parse(App_ApprovalDate), DateTime.Parse(add_dueday).AddDays(days));

                for (DateTime currentDate = DateTime.Parse(add_dueday); currentDate <= DateTime.Parse(add_dueday).AddDays(days); currentDate = currentDate.AddDays(1))
                {
                    ctr_holiday++;
                    var dates = Convert.ToDateTime(currentDate).ToString("yyyy-MM-dd");
                    var getloanhistory = GetLoanHistory().Where(a => a.MemId == memid && Convert.ToDateTime(a.DateCreated).ToString("yyyy-MM-dd") == dates).ToList().Count;
                    if (getloanhistory != 0)
                    {
                        payment_count++;
                    }
                    var results = GetHolidayList().Where(a => a.Date == dates).ToList().Count;
                    if (results != 0)
                    {
                        ctr++;
                        DayOfHoliday += ctr_holiday + ", ";
                        DateOfHoliday += ctr_holiday + ": " + dates + "| ";
                    }
                }
                total_days = days + sundaycount + ctr - payment_count;
            }

            //------------------------------------------------>

            // Computation of Loan Amount

            if (DeductInterest == "2")
            {

                interest = double.Parse(interestrate);
                interest_rate = Convert.ToDouble(loanprincipal) * Convert.ToDouble(interest);

                if (TypeOfCollection == "Daily")
                {
                    if (InterestType == "Compound")
                    {


                        advance_totalamount = formulas == "Loan Amount/Terms" ? Convert.ToDouble(loanprincipal) + interest_rate : Convert.ToDouble(loanprincipal);
                        if (NotarialFeeOrigin == "2")
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal);
                        }
                        else
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal) + interest_rate;
                        }

                        if (double.Parse(loanamountgreaterthan_val) < total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountgreaterequal);
                            if (lagef_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }

                        }
                        else if (double.Parse(loanamountgreatethan_val) >= total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountlessthan);
                            if (lav_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }
                        }
                        double getloanInsurance = double.Parse(loaninsurance);
                        if (loani_type == "Percentage")
                        {
                            total_loaninsurance = Convert.ToDouble(loanprincipal) * getloanInsurance;
                        }
                        else
                        {
                            total_loaninsurance = getloanInsurance;
                        }
                        double getLifeInsurance = double.Parse(LifeInsurance);
                        if (loani_type == "Percentage")
                        {
                            LifeInsurances = Convert.ToDouble(loanprincipal) * getLifeInsurance;
                        }
                        else
                        {
                            LifeInsurances = getLifeInsurance;
                        }

                        if (OldFormula == "2")
                        {

                            if (NotarialFeeOrigin == "1")
                            {
                                double notarial_total = Math.Ceiling(advance_totalamount / double.Parse(Days));
                                daily_collectibles = notarial_total;
                            }
                            else
                            {
                                double notarial_total = Math.Ceiling(advance_totalamount / double.Parse(Days));
                                daily_collectibles = notarial_total;
                            }

                            AdvancePayment = double.Parse(NoAdvancePayment) == 2 && double.Parse(OldFormula) == 1 ? Math.Ceiling(advance_totalamount / (double.Parse(Days)) * 2)
                              : double.Parse(NoAdvancePayment) == 1 && double.Parse(OldFormula) == 2 ? Math.Ceiling(advance_totalamount / (double.Parse(Days))) : 0;
                            holiday = double.Parse(ctr.ToString()) * daily_collectibles;
                            total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday);
                            loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));
                            loan_bal = Math.Abs((total_loan_amount) - (AdvancePayment + holiday));
                        }
                        else
                        {
                            //Computation for Old Formula

                            double notarial_total = Math.Ceiling(((double.Parse(loanprincipal) + interest_rate) / days) * 2);
                            daily_collectibles = Math.Ceiling(advance_totalamount / days);


                            AdvancePayment = double.Parse(NoAdvancePayment) == 2 && double.Parse(OldFormula) == 1 ? Math.Ceiling(advance_totalamount / (double.Parse(Days)) * 2)
                                           : double.Parse(NoAdvancePayment) == 1 && double.Parse(OldFormula) == 2 ? Math.Ceiling(advance_totalamount / (double.Parse(Days))) : 0;
                            holiday = double.Parse(ctr.ToString()) * daily_collectibles;
                            total_deduction = Math.Ceiling(total_notarial + 0 + total_loaninsurance + AdvancePayment + holiday);

                            loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));
                            loan_bal = Math.Abs((total_loan_amount) - (AdvancePayment + holiday));
                        }

                    }
                    else
                    {

                        loan_amount = Convert.ToDouble(loanprincipal) * Convert.ToDouble(interest);
                        total_loan_amount = Convert.ToDouble(loanprincipal) + loan_amount;
                        interest_rate = Convert.ToDouble(loanprincipal) * Convert.ToDouble(interest);
                        if (NotarialFeeOrigin == "Principal")
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal);
                        }
                        else
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal) + interest_rate;
                        }
                        if (double.Parse(loanamountgreaterthan_val) < total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountgreaterequal);
                            if (lagef_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }

                        }
                        else if (double.Parse(loanamountgreatethan_val) >= total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountlessthan);
                            if (lav_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }
                        }
                        double getloanInsurance = double.Parse(loaninsurance);
                        if (loani_type == "Percentage")
                        {
                            total_loaninsurance = total_loan_amount * getloanInsurance;
                        }
                        else
                        {
                            total_loaninsurance = getloanInsurance;
                        }
                        double getLifeInsurance = double.Parse(LifeInsurance);
                        if (loani_type == "Percentage")
                        {
                            LifeInsurances = total_loan_amount * getLifeInsurance;
                        }
                        else
                        {
                            LifeInsurances = getLifeInsurance;
                        }
                        total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday);

                        loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));

                        if (OldFormula == "2")
                        {

                            if (NotarialFeeOrigin == "1")
                            {
                                double notarial_total = Math.Ceiling(advance_totalamount / days);
                                daily_collectibles = notarial_total;
                            }
                            else
                            {
                                double notarial_total = Math.Ceiling(advance_totalamount / days);
                                daily_collectibles = notarial_total;
                            }

                        }
                        else
                        {
                            //Computation for Old Formula
                            double notarial_total = Math.Ceiling(((double.Parse(loanprincipal) + interest_rate) / days) * 2);
                            daily_collectibles = Math.Ceiling(advance_totalamount / days);
                            AdvancePayment = double.Parse(NoAdvancePayment) == 2 && double.Parse(OldFormula) == 1 ? Math.Ceiling(advance_totalamount / (double.Parse(Days)) * 2)
                                       : double.Parse(NoAdvancePayment) == 1 && double.Parse(OldFormula) == 2 ? Math.Ceiling(advance_totalamount / (double.Parse(Days))) : 0;
                            holiday = double.Parse(ctr.ToString()) * daily_collectibles;
                            total_deduction = total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + interest_rate + holiday + double.Parse(Savings);
                            //total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday );

                            loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));
                            loan_bal = Math.Abs((total_loan_amount) - (AdvancePayment + interest_rate + holiday));

                        }

                    }



                    amountreceived = (double.Parse(loanprincipal) + interest_rate) - total_deduction;


                }
                else
                {
                    if (InterestType == "Compound")
                    {



                        if (NotarialFeeOrigin == "Principal")
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal);
                        }
                        else
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal) + interest_rate;
                        }

                        if (double.Parse(loanamountgreaterthan_val) < total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountgreaterequal);
                            if (lagef_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }

                        }
                        else if (double.Parse(loanamountgreatethan_val) >= total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountlessthan);
                            if (lav_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }
                        }
                        double getloanInsurance = double.Parse(loaninsurance);
                        if (loani_type == "Percentage")
                        {
                            total_loaninsurance = total_loan_amount * getloanInsurance;
                        }
                        else
                        {
                            total_loaninsurance = getloanInsurance;
                        }
                        double getLifeInsurance = double.Parse(LifeInsurance);
                        if (loani_type == "Percentage")
                        {
                            LifeInsurances = total_loan_amount * getLifeInsurance;
                        }
                        else
                        {
                            LifeInsurances = getLifeInsurance;
                        }
                        //loanreceivable = Math.Abs(total_loan_amount - total - total_notarial - total_loaninsurance);
                        total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday);

                        loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));

                    }
                    else
                    {

                        loan_amount = Convert.ToDouble(loanprincipal) * Convert.ToDouble(interest);
                        total_loan_amount = Convert.ToDouble(loanprincipal) + loan_amount;
                        interest_rate = (Convert.ToDouble(loanprincipal) * Convert.ToDouble(interest) * (double.Parse(Days)) / 2);
                        if (NotarialFeeOrigin == "Principal")
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal);
                        }
                        else
                        {
                            total_loan_amount = Convert.ToDouble(loanprincipal) + interest_rate;
                        }
                        if (double.Parse(loanamountgreaterthan_val) < total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountgreaterequal);
                            if (lagef_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }

                        }
                        else if (double.Parse(loanamountgreatethan_val) >= total_loan_amount)
                        {
                            get_notiral = double.Parse(loanamountlessthan);
                            if (lav_type == "Percentage")
                            {
                                total_notarial = total_loan_amount * get_notiral;
                            }
                            else
                            {
                                total_notarial = get_notiral;
                            }
                        }
                        double getloanInsurance = double.Parse(loaninsurance);
                        if (loani_type == "Percentage")
                        {
                            total_loaninsurance = total_loan_amount * getloanInsurance;
                        }
                        else
                        {
                            total_loaninsurance = getloanInsurance;
                        }
                        double getLifeInsurance = double.Parse(LifeInsurance);
                        if (loani_type == "Percentage")
                        {
                            LifeInsurances = total_loan_amount * getLifeInsurance;
                        }
                        else
                        {
                            LifeInsurances = getLifeInsurance;
                        }
                        //loanreceivable = Math.Abs(total_loan_amount - total - total_notarial - total_loaninsurance);
                        total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday);

                        loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));


                    }

                    if (OldFormula == "2")
                    {

                        if (NotarialFeeOrigin == "1")
                        {
                            double notarial_total = Math.Ceiling(advance_totalamount / double.Parse(Days));
                            daily_collectibles = notarial_total;
                        }
                        else
                        {
                            double notarial_total = Math.Ceiling(advance_totalamount / double.Parse(Days));
                            daily_collectibles = notarial_total;
                        }

                    }
                    else
                    {
                        //Computation for Old Formula
                        double notarial_total = Math.Ceiling(((double.Parse(loanprincipal) + interest_rate) / days) * 2);
                        daily_collectibles = Math.Ceiling(advance_totalamount / days);
                        AdvancePayment = double.Parse(NoAdvancePayment) == 2 && double.Parse(OldFormula) == 1 ? Math.Ceiling(advance_totalamount / (double.Parse(Days)) * 2)
                                     : double.Parse(NoAdvancePayment) == 1 && double.Parse(OldFormula) == 2 ? Math.Ceiling(advance_totalamount / (double.Parse(Days))) : 0;
                        holiday = double.Parse(ctr.ToString()) * daily_collectibles;
                        total_deduction = total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + interest_rate + holiday + double.Parse(Savings);
                        //total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday );

                        loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));
                        loan_bal = Math.Abs((total_loan_amount) - (AdvancePayment + interest_rate + holiday));
                    }

                    amountreceived = (double.Parse(loanprincipal) + interest_rate) - total_deduction;



                }
            }
            else
            {
                interest = double.Parse(interestrate);
                deduct_interest = interest;
                daily_collectibles = Math.Ceiling(advance_totalamount / Convert.ToDouble(days));
                interest_rate = Convert.ToDouble(loanprincipal) * Convert.ToDouble(interest);
                total_loan_amount = Convert.ToDouble(loanprincipal) + interest_rate;
                double origin_amount = 0;
                if (NotarialFeeOrigin == "2")
                {
                    origin_amount = Convert.ToDouble(loanprincipal);
                }
                else
                {
                    origin_amount = Convert.ToDouble(loanprincipal) + interest_rate;
                }
                if (double.Parse(loanamountgreaterthan_val) < origin_amount)
                {
                    get_notiral = double.Parse(loanamountgreaterequal);
                    if (lagef_type == "Percentage")
                    {
                        total_notarial = origin_amount * get_notiral;
                    }
                    else
                    {
                        total_notarial = get_notiral;
                    }

                }
                else if (double.Parse(loanamountgreatethan_val) >= origin_amount)
                {
                    get_notiral = double.Parse(loanamountlessthan);
                    if (lav_type == "Percentage")
                    {
                        total_notarial = origin_amount * get_notiral;
                    }
                    else
                    {
                        total_notarial = get_notiral;
                    }
                }
                double getloanInsurance = double.Parse(loaninsurance);
                if (loani_type == "Percentage")
                {
                    total_loaninsurance = origin_amount * getloanInsurance;
                }
                else
                {
                    total_loaninsurance = getloanInsurance;
                }
                double getLifeInsurance = double.Parse(LifeInsurance);
                if (loani_type == "Percentage")
                {
                    LifeInsurances = origin_amount * getLifeInsurance;
                }
                else
                {
                    LifeInsurances = getLifeInsurance;
                }
                AdvancePayment = double.Parse(NoAdvancePayment) == 2 && double.Parse(OldFormula) == 1 ? Math.Ceiling(Convert.ToDouble(loanprincipal) / (double.Parse(Days)) * 2)
                                   : double.Parse(NoAdvancePayment) == 1 && double.Parse(OldFormula) == 2 ? Math.Ceiling(Convert.ToDouble(loanprincipal) / (double.Parse(Days))) : 0;
                holiday = double.Parse(ctr.ToString()) * daily_collectibles;
                total_deduction = total_notarial + LifeInsurances + total_loaninsurance + Math.Ceiling(AdvancePayment) + interest_rate + holiday;
                //total_deduction = Math.Ceiling(total_notarial + LifeInsurances + total_loaninsurance + AdvancePayment + holiday );

                loanreceivable = Math.Ceiling((Convert.ToDouble(loanprincipal) - total_deduction));
                amountreceived = Math.Ceiling((double.Parse(loanprincipal) + interest_rate) - total_deduction);
                loan_bal = Math.Abs((total_loan_amount) - (AdvancePayment + interest_rate + holiday));
            }
            string day_holiday = "";
            string dateholiday = "";
            if (DayOfHoliday.ToString() == "" || DateOfHoliday.ToString() == "")
            {
                day_holiday = "";
                dateholiday = "";
            }
            else
            {
                day_holiday = DayOfHoliday.ToString().Substring(0, DayOfHoliday.ToString().Length - 1);
                dateholiday = DateOfHoliday.ToString().Substring(0, DateOfHoliday.ToString().Length - 1);
            }
            MultipleStrings strings = new MultipleStrings
            {
                add_dueday = add_dueday.ToString(),
                total_days = total_days.ToString(),
                total_loan_amount = total_loan_amount.ToString(),
                total = interest_rate.ToString(),
                total_notarial = total_notarial.ToString(),
                total_loaninsurance = total_loaninsurance.ToString(),
                loanreceivable = loanreceivable.ToString(),
                daily_collectibles = daily_collectibles.ToString(),
                LifeInsurance = LifeInsurances.ToString(),
                LoanBalance = loan_bal.ToString(),
                AdvancePayment = AdvancePayment.ToString(),
                HolidayAmount = holiday.ToString(),
                HolidayDaysCount = ctr.ToString(),
                DayOfHoliday = day_holiday,
                DateOfHoliday = dateholiday,
                Deduct_Interest = deduct_interest.ToString()
            };
            return strings;
        }
        public List<LoanSummaryVM> GetLoanSummary(string NAID)
        {

            var result = new List<LoanSummaryVM>();
            var param = new IDataParameter[]
            {
            new SqlParameter("@ApplicationID", NAID)
            };

            DataTable table = db.SelectDb_SP("sp_LoanSummary", param).Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                string CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd");
                string DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd");
                string App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd");
                string App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd");
                string App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd");
                string DateCreated = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd");
                string DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd");
                string ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd");

                var item = new LoanSummaryVM();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.AreaName = dr["AreaName"].ToString();
                item.AreaID = dr["AreaID"].ToString();
                item.City = dr["City"].ToString();
                item.TotalSavingUsed = dr["UsedSavings"].ToString();
                //computation of duedate
                string approved_date = "";
                if (App_ApprovalDate_1 == "")
                {
                    approved_date = App_ApprovalDate_2;
                }
                else
                {
                    approved_date = App_ApprovalDate_1;
                }
                string loan_amount = dr["ApprovedLoanAmount"].ToString() == "" ? dr["LoanAmount"].ToString() : dr["ApprovedLoanAmount"].ToString();

                //    dr["InterestRate"].ToString(), );
                var computation_res = GetMultipleStrings(approved_date, dr["Days"].ToString(), dr["MemId"].ToString(), dr["InterestRate"].ToString(),loan_amount
                                    , dr["Loan_amount_GreaterEqual_Value"].ToString(), dr["Loan_amount_GreaterEqual"].ToString(), dr["LAGEF_Type"].ToString(), dr["Loan_amount_Lessthan_Value"].ToString()
                                    , dr["Loan_amount_Lessthan"].ToString(), dr["LALV_Type"].ToString(), dr["LoanInsurance"].ToString(), dr["LoanI_Type"].ToString(), dr["Formula"].ToString(), dr["APFID"].ToString()
                                    , dr["InterestType"].ToString(), dr["OldFormula"].ToString(), dr["NoAdvancePayment"].ToString(), dr["InterestApplied"].ToString(), dr["DeductInterest"].ToString(), dr["TypeOfCollection"].ToString()
                                    , dr["NotarialFeeOrigin"].ToString(), dr["LifeInsurance"].ToString(), dr["Savings"].ToString(), dr["Value"].ToString());
                //------------------------------------------------->
                item.Date = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                item.DueDate = computation_res.total_days != "0" ? Convert.ToDateTime(computation_res.add_dueday).AddDays(int.Parse(computation_res.total_days)).ToString("yyyy-MM-dd") : "";
                item.NAID = dr["NAID"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Days = dr["Days"].ToString();
                item.DeductInterest = String.Format("{0:0.00}", computation_res.Deduct_Interest);
                item.PrincipalLoan = dr["LoanAmount"].ToString();
                item.LoanAmount = String.Format("{0:0.00}", computation_res.total_loan_amount);
                item.Total_InterestAmount = String.Format("{0:0.00}", computation_res.total);
                item.NotarialFee = String.Format("{0:0.00}", computation_res.total_notarial);
                item.InterestRate = dr["InterestRate"].ToString();
                item.Savings = dr["Savings"].ToString();
                item.LoanBalance = String.Format("{0:0.00}", computation_res.LoanBalance);
                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                //item.InterestType = dr["InterestType"].ToString();
                item.LoanInsurance = String.Format("{0:0.00}", computation_res.total_loaninsurance);
                //item.LoanInsurance =  dr["LoanInsurance"].ToString();
                item.LoanI_Type = dr["LoanI_Type"].ToString();
                item.Total_LoanReceivable = String.Format("{0:0.00}", computation_res.loanreceivable);
                item.DailyCollectibles = computation_res.daily_collectibles;
                item.AdvancePayment = computation_res.AdvancePayment;
                item.LifeInsurance = computation_res.LifeInsurance;
                item.HolidayAmount = String.Format("{0:0.00}", computation_res.HolidayAmount);
                item.HolidayDaysCount = computation_res.HolidayDaysCount;
                item.DayOfHoliday = computation_res.DayOfHoliday;
                item.DateOfHoliday = computation_res.DateOfHoliday;
                //item.LifeInsuranceType = dr["LifeInsuranceType"].ToString();
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.CI_ApprovedBy_UserId = dr["CI_ApprovedBy_UserId"].ToString();
                item.CI_ApprovalDate = dr["CI_ApprovalDate"].ToString();
                item.DeclinedBy = dr["DeclinedBy"].ToString();
                item.DeclinedBy_UserId = dr["DeclinedBy_UserId"].ToString();
                item.DeclineDate = dr["DeclineDate"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovedBy_1_UserId = dr["App_ApprovedBy_1_UserId"].ToString();
                item.App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString();
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovedBy_2_UserId = dr["App_ApprovedBy_2_UserId"].ToString();
                item.App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString();
                item.App_Notedby = dr["App_Notedby"].ToString();
                item.App_Notedby_UserId = dr["App_Notedby_UserId"].ToString();
                item.App_NotedDate = dr["App_NotedDate"].ToString();
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.CreatedBy_UserId = dr["CreatedBy_UserId"].ToString();
                item.DateCreated = dr["DateCreated"].ToString();
                item.SubmittedBy = dr["SubmittedBy"].ToString();
                item.SubmittedBy_UserId = dr["SubmittedBy_UserId"].ToString();
                item.DateSubmitted = dr["DateSubmitted"].ToString();
                item.ReleasedBy = dr["ReleasedBy"].ToString();
                item.ReleasedBy_UserId = dr["ReleasedBy_UserId"].ToString();
                item.ReleasingDate = dr["ReleasingDate"].ToString();
                //item.Formula = dr["Formula"].ToString();
                item.APFID = dr["APFID"].ToString();
                //item.Loan_amount_GreaterEqual_Value = dr["Loan_amount_GreaterEqual_Value"].ToString();
                //item.Loan_amount_GreaterEqual = dr["Loan_amount_GreaterEqual"].ToString();
                //item.LAGEF_Type = dr["LAGEF_Type"].ToString();
                //item.Loan_amount_Lessthan_Value = dr["Loan_amount_Lessthan_Value"].ToString();
                //item.Loan_amount_Lessthan = dr["Loan_amount_Lessthan"].ToString();
                //item.LALV_Type = dr["LALV_Type"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Co_Lname"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();

                //item.OldFormula = dr["OldFormula"].ToString();
                item.NoAdvancePayment = dr["NoAdvancePayment"].ToString();
                //item.InterestApplied = dr["InterestApplied"].ToString();
                item.DeductInterest = dr["DeductInterest"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();

                item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString();
                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                item.ApproveedInterest = dr["ApproveedInterest"].ToString();
                item.ApprovedReleasingAmount = String.Format("{0:0.00}", computation_res.loanreceivable);
                item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString();
                item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString();
                item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString();
                item.ModeOfReleaseReference = dr["ModeOfReleaseReference"].ToString();
                item.ModeOfRelease = dr["ModeOfRelease"].ToString();


                result.Add(item);

            }


            return result;
        }


        public List<LoanSummaryVM> LoanSummaryRecompute(string NAID, string loanamount, string loantype)
        {

            var result = new List<LoanSummaryVM>();
            var param = new IDataParameter[]
            {
            new SqlParameter("@ApplicationID", NAID)
            };

            DataTable table = db.SelectDb_SP("sp_LoanSummary", param).Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                string CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd");
                string DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd");
                string App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd");
                string App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd");
                string App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd");
                string DateCreated = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd");
                string DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd");
                string ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd");

                var item = new LoanSummaryVM();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.AreaName = dr["AreaName"].ToString();
                item.AreaID = dr["AreaID"].ToString();
                item.City = dr["City"].ToString();
                //computation of duedate
                string approved_date = "";
                if (App_ApprovalDate_1 == "")
                {
                    approved_date = App_ApprovalDate_2;
                }
                else
                {
                    approved_date = App_ApprovalDate_1;
                }

                //    dr["InterestRate"].ToString(), );
                var computation_res = GetMultipleStrings(approved_date, dr["Days"].ToString(), dr["MemId"].ToString(), dr["InterestRate"].ToString(), loanamount
                                    , dr["Loan_amount_GreaterEqual_Value"].ToString(), dr["Loan_amount_GreaterEqual"].ToString(), dr["LAGEF_Type"].ToString(), dr["Loan_amount_Lessthan_Value"].ToString()
                                    , dr["Loan_amount_Lessthan"].ToString(), dr["LALV_Type"].ToString(), dr["LoanInsurance"].ToString(), loantype, dr["Formula"].ToString(), dr["APFID"].ToString()
                                    , dr["InterestType"].ToString(), dr["OldFormula"].ToString(), dr["NoAdvancePayment"].ToString(), dr["InterestApplied"].ToString(), dr["DeductInterest"].ToString(), dr["TypeOfCollection"].ToString()
                                    , dr["NotarialFeeOrigin"].ToString(), dr["LifeInsurance"].ToString(), dr["Savings"].ToString(), dr["Value"].ToString());
                //------------------------------------------------->
                item.Date = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                item.DueDate = computation_res.total_days != "0" ? Convert.ToDateTime(computation_res.add_dueday).AddDays(int.Parse(computation_res.total_days)).ToString("yyyy-MM-dd") : "";
                item.NAID = dr["NAID"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Days = dr["Days"].ToString();
                item.PrincipalLoan = dr["LoanAmount"].ToString();
                item.LoanAmount = String.Format("{0:0.00}", computation_res.total_loan_amount);
                item.Total_InterestAmount = String.Format("{0:0.00}", computation_res.total);
                item.NotarialFee = String.Format("{0:0.00}", computation_res.total_notarial);
                item.InterestRate = dr["InterestRate"].ToString();
                item.Savings = dr["Savings"].ToString();
                item.LoanBalance = String.Format("{0:0.00}", computation_res.LoanBalance);
                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                //item.InterestType = dr["InterestType"].ToString();
                item.LoanInsurance = String.Format("{0:0.00}", computation_res.total_loaninsurance);
                //item.LoanInsurance =  dr["LoanInsurance"].ToString();
                item.LoanI_Type = dr["LoanI_Type"].ToString();
                item.Total_LoanReceivable = String.Format("{0:0.00}", computation_res.loanreceivable);
                item.DailyCollectibles = computation_res.daily_collectibles;
                item.AdvancePayment = computation_res.AdvancePayment;
                item.LifeInsurance = computation_res.LifeInsurance;
                //item.LifeInsuranceType = dr["LifeInsuranceType"].ToString();
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.CI_ApprovedBy_UserId = dr["CI_ApprovedBy_UserId"].ToString();
                item.CI_ApprovalDate = dr["CI_ApprovalDate"].ToString();
                item.DeclinedBy = dr["DeclinedBy"].ToString();
                item.DeclinedBy_UserId = dr["DeclinedBy_UserId"].ToString();
                item.DeclineDate = dr["DeclineDate"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovedBy_1_UserId = dr["App_ApprovedBy_1_UserId"].ToString();
                item.App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString();
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovedBy_2_UserId = dr["App_ApprovedBy_2_UserId"].ToString();
                item.App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString();
                item.App_Notedby = dr["App_Notedby"].ToString();
                item.App_Notedby_UserId = dr["App_Notedby_UserId"].ToString();
                item.App_NotedDate = dr["App_NotedDate"].ToString();
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.CreatedBy_UserId = dr["CreatedBy_UserId"].ToString();
                item.DateCreated = dr["DateCreated"].ToString();
                item.SubmittedBy = dr["SubmittedBy"].ToString();
                item.SubmittedBy_UserId = dr["SubmittedBy_UserId"].ToString();
                item.DateSubmitted = dr["DateSubmitted"].ToString();
                item.ReleasedBy = dr["ReleasedBy"].ToString();
                item.ReleasedBy_UserId = dr["ReleasedBy_UserId"].ToString();
                item.ReleasingDate = dr["ReleasingDate"].ToString();
                //item.Formula = dr["Formula"].ToString();
                item.APFID = dr["APFID"].ToString();
                //item.Loan_amount_GreaterEqual_Value = dr["Loan_amount_GreaterEqual_Value"].ToString();
                //item.Loan_amount_GreaterEqual = dr["Loan_amount_GreaterEqual"].ToString();
                //item.LAGEF_Type = dr["LAGEF_Type"].ToString();
                //item.Loan_amount_Lessthan_Value = dr["Loan_amount_Lessthan_Value"].ToString();
                //item.Loan_amount_Lessthan = dr["Loan_amount_Lessthan"].ToString();
                //item.LALV_Type = dr["LALV_Type"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Co_Lname"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();

                //item.OldFormula = dr["OldFormula"].ToString();
                item.NoAdvancePayment = dr["NoAdvancePayment"].ToString();
                //item.InterestApplied = dr["InterestApplied"].ToString();
                //item.DeductInterest = dr["DeductInterest"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();

                item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString();
                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                item.ApproveedInterest = dr["ApproveedInterest"].ToString();
                item.ApprovedReleasingAmount = dr["ApprovedReleasingAmount"].ToString();
                item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString();
                item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString();
                item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString();


                result.Add(item);

            }


            return result;
        }
        public List<CollectionVM> GetCollectionList(string AreaID)
        {

            var result = new List<CollectionVM>();

            var param = new IDataParameter[]
           {
            new SqlParameter("@AreaID", AreaID)
           };

            DataTable table = db.SelectDb_SP("sp_collectionList", param).Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                var item = new CollectionVM();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.FilePath = dr["FilePath"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Co_Lname"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                item.AmountDue = dr["AmountDue"].ToString();
                item.DueDate = dr["DueDate"].ToString();
                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                item.ReleasingDate = dr["ReleasingDate"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                item.CollectedAmount = dr["CollectedAmount"].ToString();
                item.LapsePayment = dr["LapsePayment"].ToString();
                item.AdvancePayment = dr["AdvancePayment"].ToString();
                item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                item.Payment_Status = dr["Payment_Status"].ToString();
                item.Collection_Status = dr["Collection_Status"].ToString();
                item.Collection_Status_Id = dr["Collection_Status_Id"].ToString();
                item.Payment_Method = dr["Payment_Method"].ToString();
                item.AreaName = dr["AreaName"].ToString();
                item.AreaID = dr["AreaID"].ToString();
                item.Area_RefNo = dr["Area_RefNo"].ToString();
                item.Collection_RefNo = dr["Collection_RefNo"].ToString();
                result.Add(item);

            }


            return result;
        }
        public List<CollectionVM> getAmount(string areaid)
        {
            var result = new List<CollectionVM>();

            string sql_count = $@"select 
                        sum(tbl_LoanDetails_Model.ApprovedDailyAmountDue) as DailyCollectibles,
                        sum(tbl_LoanHistory_Model.OutstandingBalance) as AmountDue,
                        sum(tbl_Collection_AreaMember_Model.Savings) as Savings,
                        sum(tbl_Collection_AreaMember_Model.AdvancePayment) as AdvancePayment,
                        sum(tbl_Collection_AreaMember_Model.LapsePayment) as LapsePayment,
                        sum(tbl_Collection_AreaMember_Model.CollectedAmount) as CollectedAmount
                        from
                        tbl_Application_Model left join
                        tbl_Collection_AreaMember_Model on tbl_Collection_AreaMember_Model.NAID = tbl_Application_Model.NAID left join
                        tbl_Member_Model on tbl_Member_Model.MemId = tbl_Application_Model.MemId left join
                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                        tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join
                        tbl_LoanHistory_Model on tbl_Application_Model.MemId = tbl_LoanHistory_Model.MemId

                                where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + areaid + "'";
            DataTable table_ = db.SelectDb(sql_count).Tables[0];
            var item = new CollectionVM();
            //item.Fname = dr["Fname"].ToString();
            //item.Mname = dr["Mname"].ToString();
            //item.Lname = dr["Lname"].ToString();
            //item.Suffix = dr["Suffix"].ToString();
            //item.DateCreated = datec;

            //item.MemId = dr["MemId"].ToString();
            //item.Cno = dr["Cno"].ToString();
            //item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
            //item.NAID = dr["NAID"].ToString();
            //item.Co_Fname = dr["Co_Fname"].ToString();
            //item.Co_Mname = dr["Co_Mname"].ToString();
            //item.Co_Lname = dr["Co_Lname"].ToString();
            //item.Co_Suffix = dr["Co_Suffix"].ToString();
            //item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
            //item.Co_Cno = dr["Co_Cno"].ToString();
            item.DailyCollectibles = table_.Rows[0]["DailyCollectibles"].ToString();
            item.AmountDue = table_.Rows[0]["AmountDue"].ToString();
            //item.DueDate = dr["DueDate"].ToString();
            //item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
            item.TotalSavingsAmount = table_.Rows[0]["Savings"].ToString();
            //item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
            //item.LoanPrincipal = dr["LoanPrincipal"].ToString();
            //item.ReleasingDate = dr["ReleasingDate"].ToString();
            //item.TypeOfCollection = dr["TypeOfCollection"].ToString();
            item.CollectedAmount = table_.Rows[0]["CollectedAmount"].ToString();
            item.LapsePayment = table_.Rows[0]["LapsePayment"].ToString();
            item.AdvancePayment = table_.Rows[0]["AdvancePayment"].ToString() == "" ? "0" : table_.Rows[0]["AdvancePayment"].ToString();
            //item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
            //item.Payment_Status = dr["Payment_Status"].ToString();
            //item.Collection_Status = dr["Collection_Status"].ToString();
            //item.Collection_Status_Id = dr["Collection_Status_Id"].ToString();
            //item.Payment_Method = dr["Payment_Method"].ToString();
            //item.AreaName = dr["AreaName"].ToString();
            //item.AreaID = dr["AreaID"].ToString();
            //item.Area_RefNo = dr["Area_RefNo"].ToString();
            //item.Collection_RefNo = dr["Collection_RefNo"].ToString();
            //item.FieldOfficer = dr["FO_Fname"].ToString() + " " + dr["FO_Mname"].ToString() + " " + dr["FO_Lname"].ToString() + " " + dr["FO_Suffix"].ToString();
            //item.FOID = dr["FOID"].ToString();
            //item.FilePath = dr["FilePath"].ToString();
            //item.DateCollected = dr["DateCollected"].ToString();
            result.Add(item);


            return result;
        }

        public List<AreaDetailsVM> getMAkeCollectionList()
        {
            DataTable table = db.SelectDb_SP("sp_fieldarealist").Tables[0];
            var res = new List<AreaDetailsVM>();
            foreach (DataRow dr in table.Rows)
            {


                var items = new AreaDetailsVM();
                items.TotalCollectible = double.Parse(dr["DailyCollectibles"].ToString()) == null ? 0 : double.Parse(dr["DailyCollectibles"].ToString());
                items.Total_Balance = 0.00;
                items.Total_savings = 0.00;
                items.Total_advance = 0.00;
                items.Total_lapses = double.Parse(dr["Penalty"].ToString());
                items.Total_collectedAmount = 0.00;
                items.AreaName = dr["AreaName"].ToString();
                items.AreaID = dr["AreaID"].ToString();
                items.FieldOfficer = dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString(); ;
                items.FOID = dr["FOID"].ToString();
                items.Area_RefNo = "";
                items.Collection_RefNo = "";
                items.DateCreated = "";

                string sql_count = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr["AreaID"].ToString() + "'";
                DataTable table_ = db.SelectDb(sql_count).Tables[0];
                items.TotalItems = table_.Rows.Count.ToString();
                items.ExpectedCollection = 0.00;
                items.AdvancePayment = 0.00;
                items.Total_FieldExpenses = 0.00;
                res.Add(items);
            }
            return res;
        }

        public List<CollectionPrintedVM> CollectionGroupby()
        {

            var areas = Collection_PrintedResult().GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.FOID, a.Area_RefNo, a.RefNo, a.DateCreated, a.Collection_Status }).ToList();
            var list = Collection_PrintedResult().ToList();
            bool containsNow = list.Any(dt => dt.ToString() == DateTime.Parse(Convert.ToDateTime(list[0].DateCreated).ToString("yyyy-MM-dd")).ToString());
            var res = new List<CollectionPrintedVM>();
            foreach (var group in areas)
            {

                var advance_payment = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.AdvancePayment != null && a.AdvancePayment != "").Select(a => double.Parse(a.AdvancePayment)).Sum();

                var dailyCollectiblesSum = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.ApprovedDailyAmountDue != "").Select(a => double.Parse(a.ApprovedDailyAmountDue)).Sum();
                var savings = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.Savings != "").Select(a => double.Parse(a.Savings)).Sum();
                var balance = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.OutstandingBalance != "").Select(a => double.Parse(a.OutstandingBalance)).Sum();
                var advance = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.ApprovedAdvancePayment != "").Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                var lapses = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.LapsePayment != "").Select(a => double.Parse(a.LapsePayment)).Sum();
                var collectedamount = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.CollectedAmount != "").Select(a => double.Parse(a.CollectedAmount)).Sum();
                var fieldexpenses = list.Where(a => a.AreaID == group.Key.AreaID && a.Area_RefNo == group.Key.Area_RefNo && a.FieldExpenses != "").Select(a => double.Parse(a.FieldExpenses)).Sum();

                var items = new CollectionPrintedVM();
                items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
                items.OutstandingBalance = Math.Ceiling(double.Parse(balance.ToString())).ToString();
                items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
                items.Savings = Math.Ceiling(double.Parse(savings.ToString())).ToString();
                items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
                items.FieldExpenses = Math.Ceiling(double.Parse(fieldexpenses.ToString())).ToString();
                items.Total_lapses = Math.Ceiling(lapses);
                items.LapsePayment = Math.Ceiling(lapses).ToString();
                items.Total_collectedAmount = Math.Ceiling(collectedamount);
                items.CollectedAmount = Math.Ceiling(collectedamount).ToString();
                items.AreaName = group.Key.AreaName;
                items.AreaID = group.Key.AreaID;
                items.FieldOfficer = group.Key.FieldOfficer;
                items.FOID = group.Key.FOID;
                items.Collection_Status = group.Key.Collection_Status;
                items.Area_RefNo = group.Key.Area_RefNo;
                items.RefNo = group.Key.RefNo;
                items.DateCreated = group.Key.DateCreated;
                items.TotalItems = list.Count.ToString();
                items.ExpectedCollection = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.AdvancePayment = Math.Ceiling(advance_payment).ToString();
                res.Add(items);
                //}

                //var dailyCollectiblesSum = dbmet.getAreaLoanSummary().Where(a=>a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.DailyCollectibles)).Sum();

            }
            return res;

        }
        public List<CollectionPrintedVM> Collection_PrintedResult()
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = new List<CollectionPrintedVM>();
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {
                string sql_count = $@"SELECT        tbl_CollectionModel.RefNo, tbl_CollectionModel.DateCreated, tbl_CollectionArea_Model.Area_RefNo, tbl_CollectionArea_Model.AreaId, tbl_CollectionArea_Model.Denomination, 
                         CASE WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN 0 ELSE tbl_CollectionArea_Model.FieldExpenses END AS FieldExpenses, tbl_CollectionArea_Model.Remarks, tbl_Collection_AreaMember_Model.NAID, 
                         tbl_Collection_AreaMember_Model.AdvancePayment, tbl_Collection_AreaMember_Model.LapsePayment, tbl_Collection_AreaMember_Model.CollectedAmount, tbl_Collection_AreaMember_Model.Savings, 
                         tbl_Collection_AreaMember_Model.DateCollected, tbl_Collection_AreaMember_Model.AdvanceStatus, tbl_CollectionStatus_Model.Status AS PrintedStatus, CASE WHEN col_stats.[Status] IS NULL 
                         THEN 'NO PAYMENT' ELSE col_stats.[Status] END AS Collection_Status, CASE WHEN tbl_CollectionStatus_Model.Status IS NULL THEN 'NO PAYMENT' ELSE tbl_CollectionStatus_Model.Status END AS Payment_Status, 
                         CASE WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT' ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method, tbl_Application_Model.MemId, 
                         CASE WHEN tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0 ELSE tbl_LoanHistory_Model.OutstandingBalance END AS OutstandingBalance, CASE WHEN tbl_LoanHistory_Model.Penalty IS NULL 
                         THEN 0 ELSE tbl_LoanHistory_Model.Penalty END AS Penalty, tbl_LoanHistory_Model.DateReleased, tbl_LoanHistory_Model.DueDate, tbl_LoanHistory_Model.DateOfFullPayment, 
                         CASE WHEN tbl_LoanHistory_Model.UsedSavings IS NULL THEN 0 ELSE tbl_LoanHistory_Model.UsedSavings END AS UsedSavings, tbl_LoanDetails_Model.ApprovedTermsOfPayment, 
                         tbl_LoanDetails_Model.ApprovedLoanAmount, tbl_LoanDetails_Model.ApprovedNotarialFee, tbl_LoanDetails_Model.ApprovedAdvancePayment, tbl_LoanDetails_Model.ApprovedReleasingAmount, 
                         tbl_LoanDetails_Model.ApproveedInterest, tbl_LoanDetails_Model.ApprovedDailyAmountDue, tbl_LoanDetails_Model.ModeOfRelease, tbl_LoanDetails_Model.LoanTypeID, tbl_TermsTypeOfCollection_Model.TypeOfCollection, 
                         tbl_TermsTypeOfCollection_Model.Value, tbl_Application_Model.ReleasingDate
FROM            tbl_TermsOfPayment_Model INNER JOIN
                         tbl_LoanDetails_Model ON tbl_TermsOfPayment_Model.TopId = tbl_LoanDetails_Model.ApprovedTermsOfPayment INNER JOIN
                         tbl_TermsTypeOfCollection_Model ON tbl_TermsOfPayment_Model.CollectionTypeId = tbl_TermsTypeOfCollection_Model.Id RIGHT OUTER JOIN
                         tbl_CollectionModel INNER JOIN
                         tbl_CollectionArea_Model ON tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo LEFT OUTER JOIN
                         tbl_Collection_AreaMember_Model ON tbl_Collection_AreaMember_Model.Area_RefNo = tbl_CollectionArea_Model.Area_RefNo LEFT OUTER JOIN
                         tbl_CollectionStatus_Model ON tbl_CollectionArea_Model.Printed_Status = tbl_CollectionStatus_Model.Id LEFT OUTER JOIN
                         tbl_CollectionStatus_Model AS pay_stats ON tbl_Collection_AreaMember_Model.Payment_Status = pay_stats.Id LEFT OUTER JOIN
                         tbl_CollectionStatus_Model AS col_stats ON tbl_CollectionArea_Model.Collection_Status = col_stats.Id LEFT OUTER JOIN
                         tbl_Application_Model ON tbl_Application_Model.NAID = tbl_Collection_AreaMember_Model.NAID LEFT OUTER JOIN
                         tbl_LoanHistory_Model ON tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId ON tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID
                            where tbl_CollectionArea_Model.AreaId = '" + dr_area["AreaID"].ToString() + "' and tbl_Application_Model.Status = 14 ";
                DataTable table_ = db.SelectDb(sql_count).Tables[0];
                if (table_.Rows.Count != 0)
                {

                    foreach (DataRow dr in table_.Rows)
                    {
                        string date_2 = dr["DateCollected"].ToString() == "" ? dr["ReleasingDate"].ToString() : dr["DateCollected"].ToString();
                        if (dr["TypeOfCollection"].ToString() == "Daily")
                        {
                            var item = new CollectionPrintedVM();
                            item.RefNo = dr["RefNo"].ToString();
                            item.DateCreated = dr["DateCreated"].ToString();
                            item.Area_RefNo = dr["Area_RefNo"].ToString();
                            item.AreaID = dr["AreaId"].ToString();
                            item.AreaName = dr_area["Area"].ToString();
                            item.FOID = dr_area["FOID"].ToString();
                            item.DateCollected = date_2;
                            item.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString();
                            item.Denomination = dr["Denomination"].ToString();
                            item.FieldExpenses = dr["FieldExpenses"].ToString() == "" ? "0" : dr["FieldExpenses"].ToString();
                            item.Remarks = dr["Remarks"].ToString();
                            item.NAID = dr["NAID"].ToString();
                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                            item.LapsePayment = dr["LapsePayment"].ToString() == "" ? "0" : dr["LapsePayment"].ToString();
                            item.CollectedAmount = dr["CollectedAmount"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString();
                            item.Savings = dr["Savings"].ToString() == "" ? "0" : dr["Savings"].ToString();
                            item.PrintedStatus = dr["PrintedStatus"].ToString();
                            item.AdvanceStatus = dr["AdvanceStatus"].ToString();
                            item.Collection_Status = dr["Collection_Status"].ToString();
                            item.Payment_Status = dr["Payment_Status"].ToString();
                            item.Payment_Method = dr["Payment_Method"].ToString();
                            item.MemId = dr["MemId"].ToString();
                            item.OutstandingBalance = dr["OutstandingBalance"].ToString();
                            item.Penalty = dr["Penalty"].ToString();
                            item.DateReleased = dr["DateReleased"].ToString();
                            item.DueDate = dr["DueDate"].ToString();
                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                            item.UsedSavings = dr["UsedSavings"].ToString() == "" ? "0" : dr["UsedSavings"].ToString();
                            item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString() == "" ? "0" : dr["ApprovedTermsOfPayment"].ToString();
                            item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString() == "" ? "0" : dr["ApprovedLoanAmount"].ToString();
                            item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString() == "" ? "0" : dr["ApprovedNotarialFee"].ToString();
                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["ApprovedAdvancePayment"].ToString();
                            item.ApprovedReleasingAmount = dr["ApprovedReleasingAmount"].ToString() == "" ? "0" : dr["ApprovedReleasingAmount"].ToString();
                            item.ApproveedInterest = dr["ApproveedInterest"].ToString() == "" ? "0" : dr["ApproveedInterest"].ToString();
                            item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString() == "" ? "0" : dr["ApprovedDailyAmountDue"].ToString();
                            item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                            item.LoanTypeID = dr["LoanTypeID"].ToString();
                            string sql_count1 = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr["AreaId"].ToString() + "'";
                            DataTable table1_ = db.SelectDb(sql_count1).Tables[0];
                            item.TotalItems = table1_.Rows.Count.ToString();
                            result.Add(item);
                        }
                        else
                        {
                            int day_val = int.Parse(dr["Value"].ToString());
                            DateTime date1 = Convert.ToDateTime(currentDate.ToString());
                            DateTime date2 = Convert.ToDateTime(date_2).AddDays(day_val);
                            TimeSpan difference = date2 - date1;
                            int dayDifference = difference.Days;
                            if (dayDifference == 0)
                            {

                                var item = new CollectionPrintedVM();
                                item.RefNo = dr["RefNo"].ToString();
                                item.DateCollected = date_2;
                                item.DateCreated = dr["DateCreated"].ToString();
                                item.Area_RefNo = dr["Area_RefNo"].ToString();
                                item.AreaID = dr["AreaId"].ToString();
                                item.AreaName = dr_area["Area"].ToString();
                                item.FOID = dr_area["FOID"].ToString();
                                item.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString();
                                item.Denomination = dr["Denomination"].ToString();
                                item.FieldExpenses = dr["FieldExpenses"].ToString() == "" ? "0" : dr["FieldExpenses"].ToString();
                                item.Remarks = dr["Remarks"].ToString();
                                item.NAID = dr["NAID"].ToString();
                                item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                item.LapsePayment = dr["LapsePayment"].ToString() == "" ? "0" : dr["LapsePayment"].ToString();
                                item.CollectedAmount = dr["CollectedAmount"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString();
                                item.Savings = dr["Savings"].ToString() == "" ? "0" : dr["Savings"].ToString();
                                item.PrintedStatus = dr["PrintedStatus"].ToString();
                                item.AdvanceStatus = dr["AdvanceStatus"].ToString();
                                item.Collection_Status = dr["Collection_Status"].ToString();
                                item.Payment_Status = dr["Payment_Status"].ToString();
                                item.Payment_Method = dr["Payment_Method"].ToString();
                                item.MemId = dr["MemId"].ToString();
                                item.OutstandingBalance = dr["OutstandingBalance"].ToString();
                                item.Penalty = dr["Penalty"].ToString();
                                item.DateReleased = dr["DateReleased"].ToString();
                                item.DueDate = dr["DueDate"].ToString();
                                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                item.UsedSavings = dr["UsedSavings"].ToString() == "" ? "0" : dr["UsedSavings"].ToString();
                                item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString() == "" ? "0" : dr["ApprovedTermsOfPayment"].ToString();
                                item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString() == "" ? "0" : dr["ApprovedLoanAmount"].ToString();
                                item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString() == "" ? "0" : dr["ApprovedNotarialFee"].ToString();
                                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["ApprovedAdvancePayment"].ToString();
                                item.ApprovedReleasingAmount = dr["ApprovedReleasingAmount"].ToString() == "" ? "0" : dr["ApprovedReleasingAmount"].ToString();
                                item.ApproveedInterest = dr["ApproveedInterest"].ToString() == "" ? "0" : dr["ApproveedInterest"].ToString();
                                item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString() == "" ? "0" : dr["ApprovedDailyAmountDue"].ToString();
                                item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                                item.LoanTypeID = dr["LoanTypeID"].ToString();
                                string sql_count1 = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr["AreaId"].ToString() + "'";
                                DataTable table1_ = db.SelectDb(sql_count1).Tables[0];
                                item.TotalItems = table1_.Rows.Count.ToString();
                                result.Add(item);

                            }

                        }
                    }


                }

            }
            return result;
        }

        public List<CollectionPrintedVM> GetLoanHistoryNew()
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = new List<CollectionPrintedVM>();
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {
                string sql_count = $@"SELECT        tbl_CollectionModel.RefNo, tbl_CollectionModel.DateCreated, tbl_CollectionArea_Model.Area_RefNo, tbl_CollectionArea_Model.AreaId, tbl_CollectionArea_Model.Denomination, 
                         CASE WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN 0 ELSE tbl_CollectionArea_Model.FieldExpenses END AS FieldExpenses, tbl_CollectionArea_Model.Remarks, tbl_Collection_AreaMember_Model.NAID, 
                         tbl_Collection_AreaMember_Model.AdvancePayment, tbl_Collection_AreaMember_Model.LapsePayment, tbl_Collection_AreaMember_Model.CollectedAmount, tbl_Collection_AreaMember_Model.Savings, 
                         tbl_Collection_AreaMember_Model.DateCollected, tbl_Collection_AreaMember_Model.AdvanceStatus, tbl_CollectionStatus_Model.Status AS PrintedStatus, CASE WHEN col_stats.[Status] IS NULL 
                         THEN 'NO PAYMENT' ELSE col_stats.[Status] END AS Collection_Status, CASE WHEN tbl_CollectionStatus_Model.Status IS NULL THEN 'NO PAYMENT' ELSE tbl_CollectionStatus_Model.Status END AS Payment_Status, 
                         CASE WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT' ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method, tbl_Application_Model.MemId, 
                         CASE WHEN tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0 ELSE tbl_LoanHistory_Model.OutstandingBalance END AS OutstandingBalance, CASE WHEN tbl_LoanHistory_Model.Penalty IS NULL 
                         THEN 0 ELSE tbl_LoanHistory_Model.Penalty END AS Penalty, tbl_LoanHistory_Model.DateReleased, tbl_LoanHistory_Model.DueDate, tbl_LoanHistory_Model.DateOfFullPayment, 
                         CASE WHEN tbl_LoanHistory_Model.UsedSavings IS NULL THEN 0 ELSE tbl_LoanHistory_Model.UsedSavings END AS UsedSavings, tbl_LoanDetails_Model.ApprovedTermsOfPayment, 
                         tbl_LoanDetails_Model.ApprovedLoanAmount, tbl_LoanDetails_Model.ApprovedNotarialFee, tbl_LoanDetails_Model.ApprovedAdvancePayment, tbl_LoanDetails_Model.ApprovedReleasingAmount, 
                         tbl_LoanDetails_Model.ApproveedInterest, tbl_LoanDetails_Model.ApprovedDailyAmountDue, tbl_LoanDetails_Model.ModeOfRelease, tbl_LoanDetails_Model.LoanTypeID, tbl_TermsTypeOfCollection_Model.TypeOfCollection, 
                         tbl_TermsTypeOfCollection_Model.Value, tbl_Application_Model.ReleasingDate, tbl_Status_Model.Name as ApplicationStatus
FROM            tbl_LoanHistory_Model RIGHT OUTER JOIN
                         tbl_Status_Model INNER JOIN
                         tbl_Application_Model ON tbl_Status_Model.Id = tbl_Application_Model.Status LEFT OUTER JOIN
                         tbl_TermsOfPayment_Model INNER JOIN
                         tbl_LoanDetails_Model ON tbl_TermsOfPayment_Model.TopId = tbl_LoanDetails_Model.ApprovedTermsOfPayment INNER JOIN
                         tbl_TermsTypeOfCollection_Model ON tbl_TermsOfPayment_Model.CollectionTypeId = tbl_TermsTypeOfCollection_Model.Id ON tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID RIGHT OUTER JOIN
                         tbl_CollectionModel INNER JOIN
                         tbl_CollectionArea_Model ON tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo LEFT OUTER JOIN
                         tbl_Collection_AreaMember_Model ON tbl_Collection_AreaMember_Model.Area_RefNo = tbl_CollectionArea_Model.Area_RefNo LEFT OUTER JOIN
                         tbl_CollectionStatus_Model ON tbl_CollectionArea_Model.Printed_Status = tbl_CollectionStatus_Model.Id LEFT OUTER JOIN
                         tbl_CollectionStatus_Model AS pay_stats ON tbl_Collection_AreaMember_Model.Payment_Status = pay_stats.Id LEFT OUTER JOIN
                         tbl_CollectionStatus_Model AS col_stats ON tbl_CollectionArea_Model.Collection_Status = col_stats.Id ON tbl_Application_Model.NAID = tbl_Collection_AreaMember_Model.NAID ON 
                         tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId where tbl_CollectionArea_Model.AreaId = '" + dr_area["AreaID"].ToString() + "' ";
                DataTable table_ = db.SelectDb(sql_count).Tables[0];
                if (table_.Rows.Count != 0)
                {

                    foreach (DataRow dr in table_.Rows)
                    {
                        string date_2 = dr["DateCollected"].ToString() == "" ? dr["ReleasingDate"].ToString() : dr["DateCollected"].ToString();
                        if (dr["TypeOfCollection"].ToString() == "Daily")
                        {
                            var item = new CollectionPrintedVM();
                            item.RefNo = dr["RefNo"].ToString();
                            item.DateCreated = dr["DateCreated"].ToString();
                            item.Area_RefNo = dr["Area_RefNo"].ToString();
                            item.AreaID = dr["AreaId"].ToString();
                            item.AreaName = dr_area["Area"].ToString();
                            item.FOID = dr_area["FOID"].ToString();
                            item.DateCollected = date_2;
                            item.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString();
                            item.Denomination = dr["Denomination"].ToString();
                            item.FieldExpenses = dr["FieldExpenses"].ToString() == "" ? "0" : dr["FieldExpenses"].ToString();
                            item.Remarks = dr["Remarks"].ToString();
                            item.NAID = dr["NAID"].ToString();
                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                            item.LapsePayment = dr["LapsePayment"].ToString() == "" ? "0" : dr["LapsePayment"].ToString();
                            item.CollectedAmount = dr["CollectedAmount"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString();
                            item.Savings = dr["Savings"].ToString() == "" ? "0" : dr["Savings"].ToString();
                            item.PrintedStatus = dr["PrintedStatus"].ToString();
                            item.AdvanceStatus = dr["AdvanceStatus"].ToString();
                            item.Collection_Status = dr["Collection_Status"].ToString();
                            item.Payment_Status = dr["Payment_Status"].ToString();
                            item.Payment_Method = dr["Payment_Method"].ToString();
                            item.MemId = dr["MemId"].ToString();
                            item.OutstandingBalance = dr["OutstandingBalance"].ToString();
                            item.Penalty = dr["Penalty"].ToString();
                            item.DateReleased = dr["DateReleased"].ToString();
                            item.DueDate = dr["DueDate"].ToString();
                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                            item.UsedSavings = dr["UsedSavings"].ToString() == "" ? "0" : dr["UsedSavings"].ToString();
                            item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString() == "" ? "0" : dr["ApprovedTermsOfPayment"].ToString();
                            item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString() == "" ? "0" : dr["ApprovedLoanAmount"].ToString();
                            item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString() == "" ? "0" : dr["ApprovedNotarialFee"].ToString();
                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["ApprovedAdvancePayment"].ToString();
                            item.ApprovedReleasingAmount = dr["ApprovedReleasingAmount"].ToString() == "" ? "0" : dr["ApprovedReleasingAmount"].ToString();
                            item.ApproveedInterest = dr["ApproveedInterest"].ToString() == "" ? "0" : dr["ApproveedInterest"].ToString();
                            item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString() == "" ? "0" : dr["ApprovedDailyAmountDue"].ToString();
                            item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                            item.LoanTypeID = dr["LoanTypeID"].ToString();
                            string sql_count1 = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr["AreaId"].ToString() + "'";
                            DataTable table1_ = db.SelectDb(sql_count1).Tables[0];
                            item.TotalItems = table1_.Rows.Count.ToString();
                            result.Add(item);
                        }
                        else
                        {
                            int day_val = int.Parse(dr["Value"].ToString());
                            DateTime date1 = Convert.ToDateTime(currentDate.ToString());
                            DateTime date2 = Convert.ToDateTime(date_2).AddDays(day_val);
                            TimeSpan difference = date2 - date1;
                            int dayDifference = difference.Days;
                            if (dayDifference == 0)
                            {

                                var item = new CollectionPrintedVM();
                                item.RefNo = dr["RefNo"].ToString();
                                item.DateCollected = date_2;
                                item.DateCreated = dr["DateCreated"].ToString();
                                item.Area_RefNo = dr["Area_RefNo"].ToString();
                                item.AreaID = dr["AreaId"].ToString();
                                item.AreaName = dr_area["Area"].ToString();
                                item.FOID = dr_area["FOID"].ToString();
                                item.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString();
                                item.Denomination = dr["Denomination"].ToString();
                                item.FieldExpenses = dr["FieldExpenses"].ToString() == "" ? "0" : dr["FieldExpenses"].ToString();
                                item.Remarks = dr["Remarks"].ToString();
                                item.NAID = dr["NAID"].ToString();
                                item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                item.LapsePayment = dr["LapsePayment"].ToString() == "" ? "0" : dr["LapsePayment"].ToString();
                                item.CollectedAmount = dr["CollectedAmount"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString();
                                item.Savings = dr["Savings"].ToString() == "" ? "0" : dr["Savings"].ToString();
                                item.PrintedStatus = dr["PrintedStatus"].ToString();
                                item.AdvanceStatus = dr["AdvanceStatus"].ToString();
                                item.Collection_Status = dr["Collection_Status"].ToString();
                                item.Payment_Status = dr["Payment_Status"].ToString();
                                item.Payment_Method = dr["Payment_Method"].ToString();
                                item.MemId = dr["MemId"].ToString();
                                item.OutstandingBalance = dr["OutstandingBalance"].ToString();
                                item.Penalty = dr["Penalty"].ToString();
                                item.DateReleased = dr["DateReleased"].ToString();
                                item.DueDate = dr["DueDate"].ToString();
                                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                item.UsedSavings = dr["UsedSavings"].ToString() == "" ? "0" : dr["UsedSavings"].ToString();
                                item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString() == "" ? "0" : dr["ApprovedTermsOfPayment"].ToString();
                                item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString() == "" ? "0" : dr["ApprovedLoanAmount"].ToString();
                                item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString() == "" ? "0" : dr["ApprovedNotarialFee"].ToString();
                                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["ApprovedAdvancePayment"].ToString();
                                item.ApprovedReleasingAmount = dr["ApprovedReleasingAmount"].ToString() == "" ? "0" : dr["ApprovedReleasingAmount"].ToString();
                                item.ApproveedInterest = dr["ApproveedInterest"].ToString() == "" ? "0" : dr["ApproveedInterest"].ToString();
                                item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString() == "" ? "0" : dr["ApprovedDailyAmountDue"].ToString();
                                item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                                item.LoanTypeID = dr["LoanTypeID"].ToString();
                                string sql_count1 = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr["AreaId"].ToString() + "'";
                                DataTable table1_ = db.SelectDb(sql_count1).Tables[0];
                                item.TotalItems = table1_.Rows.Count.ToString();
                                result.Add(item);

                            }

                        }
                    }


                }

            }
            return result;
        }
        public List<CollectionPrintedVM> Collection_NotPrintedResult()
        {
            var result = new List<CollectionPrintedVM>();
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {
                var item = new CollectionPrintedVM();
                item.TotalCollectible = 0;
                item.Total_Balance = 0;
                item.Total_savings = 0;
                item.Total_advance = 0;
                item.Total_lapses = 0;
                item.ExpectedCollection = 0;
                item.Total_collectedAmount = 0;
                item.RefNo = "PENDING";
                item.DateCreated = "";
                item.Area_RefNo = "PENDING";
                item.AreaID = dr_area["AreaId"].ToString();
                item.AreaName = dr_area["Area"].ToString();
                item.FOID = dr_area["FOID"].ToString();
                item.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString();
                item.Denomination = "";
                item.FieldExpenses = "0";
                item.Remarks = "";
                item.NAID = "";
                item.AdvancePayment = "0";
                item.LapsePayment = "0";
                item.CollectedAmount = "0";
                item.Savings = "0";
                item.PrintedStatus = "";
                item.AdvanceStatus = "0";
                item.Collection_Status = "NO PAYMENT";
                item.Payment_Status = "NO PAYMENT";
                item.Payment_Method = "NO PAYMENT";
                item.MemId = "";
                item.OutstandingBalance = "0";
                item.Penalty = "0";
                item.DateReleased = "";
                item.DueDate = "";
                item.DateOfFullPayment = "";
                item.UsedSavings = "0";
                item.ApprovedTermsOfPayment = "";
                item.ApprovedLoanAmount = "0";
                item.ApprovedNotarialFee = "0";
                item.ApprovedAdvancePayment = "0";
                item.ApprovedReleasingAmount = "0";
                item.ApproveedInterest = "0";
                item.ApprovedDailyAmountDue = "0";
                item.ModeOfRelease = "0";
                item.LoanTypeID = "";
                string sql_count1 = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr_area["AreaId"].ToString() + "'";
                DataTable table1_ = db.SelectDb(sql_count1).Tables[0];
                item.TotalItems = table1_.Rows.Count.ToString();
                result.Add(item);



            }
            return result;
        }
        public List<AreaDetailsVM> getPrintedArea2()
        {
            //getPrintedArea
            //DataTable table = db.SelectDb_SP("sp_fieldarealist").Tables[0];


            //var items = new CollectionTotals();
            //items.Total_FieldExpenses = Math.Ceiling(double.Parse(fieldexpenses.ToString()));
            //items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
            //items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
            //items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
            //items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
            //items.Total_lapses = Math.Ceiling(lapses);
            //items.Total_collectedAmount = Math.Ceiling(collectedamount);
            var datenow = DateTime.Now.ToString("yyyy-MM-dd");
            var list = getPrintedArea().Where(a => a.DateCreated != "" && a.DateCreated == datenow).ToList();
            var areas = list.GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.FOID, a.Area_RefNo, a.Collection_RefNo }).ToList();
            var res = new List<AreaDetailsVM>();
            //foreach (DataRow dr in table.Rows)
            for (int x = 0; x < areas.Count; x++)
            {
                var dailyCollectiblesSum = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.TotalCollectible).Sum();
                var savings = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.Total_savings).Sum();
                var balance = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.Total_Balance).Sum();
                var advance = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.Total_advance).Sum();
                var lapses = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.Total_lapses).Sum();
                var collectedamount = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.Total_collectedAmount).Sum();
                var Fieldexpenses = list.Where(a => a.AreaID == areas[x].Key.AreaID).Select(a => a.Total_FieldExpenses).FirstOrDefault();
                var items = new AreaDetailsVM();
                items.TotalCollectible = dailyCollectiblesSum;
                items.Total_Balance = savings;
                items.Total_savings = balance;
                items.Total_advance = advance;
                items.Total_lapses = lapses;
                items.Total_collectedAmount = collectedamount;
                items.AreaName = areas[x].Key.AreaName;
                items.AreaID = areas[x].Key.AreaID;
                items.FieldOfficer = areas[x].Key.FieldOfficer;
                items.FOID = areas[x].Key.FOID;
                items.Area_RefNo = areas[x].Key.Area_RefNo;
                items.Collection_RefNo = areas[x].Key.Collection_RefNo;
                items.DateCreated = "";
                items.Total_FieldExpenses = Fieldexpenses;
                string sql_count = $@"
                                                    select 
                                                    tbl_Member_Model.MemId,
                                                    tbl_Area_Model.AreaID
                                                    from
                                                    tbl_Application_Model inner join
                                                    tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                                    tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                                    tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                                    tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                                    tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                                where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + areas[x].Key.AreaID + "'";
                DataTable table_ = db.SelectDb(sql_count).Tables[0];
                items.TotalItems = table_.Rows.Count.ToString();
                items.ExpectedCollection = 0.00;
                items.AdvancePayment = advance;
                res.Add(items);
            }
            //{



            //}
            return res;
        }
        public List<AreaDetailsVM> getPrintedArea()
        {
            var res = new List<AreaDetailsVM>();

            string datec = "";
            string Collection_Status_Id = "";
            string FieldExpenses = "0";
            string Area_RefNo = "PENDING";
            string Collection_RefNo = "PENDING";
            string Collection_Status = "NO PAYMENT";
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {

                string reference = $@"select
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status ,
                CASE 
                WHEN tbl_CollectionArea_Model.Collection_Status IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.Collection_Status END as Collection_Status_Id,
                CASE 
                WHEN tbl_CollectionArea_Model.Area_RefNo IS NULL THEN 'PENDING'
                ELSE tbl_CollectionArea_Model.Area_RefNo END AS Area_RefNo,
                CASE
                WHEN tbl_CollectionModel.RefNo IS NULL THEN 'PENDING' 
                ELSE  tbl_CollectionModel.RefNo  END as Collection_RefNo,
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status,tbl_CollectionModel.DateCreated,
				 CASE 
                WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.FieldExpenses END as FieldExpenses
                from 
                tbl_CollectionArea_Model left join
                tbl_CollectionModel on tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo left join
                tbl_CollectionStatus_Model as col_stats on col_stats.Id = tbl_CollectionArea_Model.Collection_Status 
                where tbl_CollectionArea_Model.AreaID = '" + dr_area["AreaID"].ToString() + "' ";
                DataTable tbl_reference = db.SelectDb(reference).Tables[0];
                if (tbl_reference.Rows.Count != 0)
                {
                    datec = tbl_reference.Rows[0]["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(tbl_reference.Rows[0]["DateCreated"].ToString()).ToString("yyyy-MM-dd");//
                    Collection_Status_Id = "0";
                    Area_RefNo = tbl_reference.Rows[0]["Area_RefNo"].ToString();
                    Collection_RefNo = tbl_reference.Rows[0]["Collection_RefNo"].ToString();
                    Collection_Status = tbl_reference.Rows[0]["Collection_Status"].ToString();
                    FieldExpenses = tbl_reference.Rows[0]["FieldExpenses"].ToString();
                }
                else
                {
                    datec = "";
                    Collection_Status_Id = "0";
                    Area_RefNo = "PENDING";
                    Collection_RefNo = "PENDING";
                    Collection_Status = "NO PAYMENT";
                    FieldExpenses = "";
                }
                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    if (area_city.Count > 1)
                    {

                    }
                    var spliter = area_city[x].Split(",");
                    string barangay = spliter[0].Trim();
                    string city = spliter[1].Trim();
                    var param = new IDataParameter[]
                  {
                    new SqlParameter("@barangay", barangay),
                    new SqlParameter("@city", city)
                  };

                    DataTable table = db.SelectDb_SP("sp_fieldarealist_printed", param).Tables[0];
                    if (table.Rows.Count != 0)
                    {

                        foreach (DataRow dr in table.Rows)
                        {
                            var test = dr["DailyCollectibles"].ToString();

                            var items = new AreaDetailsVM();
                            items.TotalCollectible = double.Parse(dr["DailyCollectibles"].ToString() == "" ? "0" : dr["DailyCollectibles"].ToString());
                            items.Total_Balance = double.Parse(dr["AmountDue"].ToString() == "" ? "0" : dr["AmountDue"].ToString());
                            items.Total_savings = double.Parse(dr["TotalSavingsAmount"].ToString() == "" ? "0" : dr["TotalSavingsAmount"].ToString());
                            items.Total_advance = double.Parse(dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["ApprovedAdvancePayment"].ToString());
                            items.Total_lapses = double.Parse(dr["Lapses"].ToString() == "" ? "0" : dr["Lapses"].ToString());
                            items.Total_collectedAmount = double.Parse(dr["CollectedAmount"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString());
                            items.AreaName = dr_area["Area"].ToString();
                            items.AreaID = dr_area["AreaID"].ToString();
                            items.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString(); ;
                            items.FOID = dr_area["FOID"].ToString();
                            items.Area_RefNo = Area_RefNo;
                            items.Collection_RefNo = Collection_RefNo;
                            items.DateCreated = datec;

                            string sql_count = $@"
                                                    select 
                                                    tbl_Member_Model.MemId,
                                                    tbl_Area_Model.AreaID
                                                    from
                                                    tbl_Application_Model inner join
                                                    tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                                    tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                                    tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                                    tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                                    tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                                where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr_area["AreaID"].ToString() + "'";
                            DataTable table_ = db.SelectDb(sql_count).Tables[0];
                            items.TotalItems = table_.Rows.Count.ToString();
                            items.ExpectedCollection = 0.00;
                            items.Total_FieldExpenses = double.Parse(FieldExpenses == "" ? "0" : FieldExpenses);
                            items.AdvancePayment = double.Parse(dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString());
                            res.Add(items);
                        }
                    }
                }
            }

            return res;
        }
        public List<CollectionVM> getAreaLoanSummary()
        {


            var result = new List<CollectionVM>();


            DataTable table = db.SelectDb_SP("sp_collectionlist_nofilter").Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                //DateTime dateToCompare = DateTime.Parse(dr["DateCreated"].ToString()); // Replace with your specific date
                //DateTime currentDate = DateTime.Now;
                var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                var item = new CollectionVM();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Remarks = dr["Remarks"].ToString();
                item.Penalty = dr["Penalty"].ToString();
                item.DateCreated = datec;

                item.MemId = dr["MemId"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Co_Lname"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                item.AmountDue = dr["AmountDue"].ToString();
                item.DueDate = dr["DueDate"].ToString();
                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                item.ReleasingDate = dr["ReleasingDate"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                item.CollectedAmount = dr["CollectedAmount"].ToString();
                item.LapsePayment = dr["LapsePayment"].ToString();
                item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                item.Payment_Status = dr["Payment_Status"].ToString();
                item.Collection_Status = dr["Collection_Status"].ToString();
                item.Collection_Status_Id = dr["Collection_Status_Id"].ToString();
                item.Payment_Method = dr["Payment_Method"].ToString();
                item.AreaName = dr["AreaName"].ToString();
                item.AreaID = dr["AreaID"].ToString();
                item.Area_RefNo = dr["Area_RefNo"].ToString();
                item.Collection_RefNo = dr["Collection_RefNo"].ToString();
                item.FieldOfficer = dr["FO_Fname"].ToString() + " " + dr["FO_Mname"].ToString() + " " + dr["FO_Lname"].ToString() + " " + dr["FO_Suffix"].ToString();
                item.FOID = dr["FOID"].ToString();
                item.FilePath = dr["FilePath"].ToString();
                item.DateCollected = dr["DateCollected"].ToString();
                item.LoanInsurance = dr["LoanInsuranceAmount"].ToString() == "" ? "0" : dr["LoanInsuranceAmount"].ToString();
                item.LifeInsurance = dr["LifeInsuranceAmount"].ToString() == "" ? "0" : dr["LifeInsuranceAmount"].ToString();
                result.Add(item);

            }


            return result;

        }
        public List<CollectionVM> getLoanhistoryAll()
        {

            string w_column = "";
            string r_column = "";
            string datec = "";
            string Collection_Status_Id = "";
            string FieldExpenses = "0";
            string Area_RefNo = "PENDING";
            string Collection_RefNo = "PENDING";
            string Collection_Status = "NO PAYMENT";
            var result = new List<CollectionVM>();

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {
                string reference = $@"select
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status ,
                CASE 
                WHEN tbl_CollectionArea_Model.Collection_Status IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.Collection_Status END as Collection_Status_Id,
                CASE 
                WHEN tbl_CollectionArea_Model.Area_RefNo IS NULL THEN 'PENDING'
                ELSE tbl_CollectionArea_Model.Area_RefNo END AS Area_RefNo,
                CASE
                WHEN tbl_CollectionModel.RefNo IS NULL THEN 'PENDING' 
                ELSE  tbl_CollectionModel.RefNo  END as Collection_RefNo,
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status,tbl_CollectionModel.DateCreated,
				 CASE 
                WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.FieldExpenses END as FieldExpenses
                from 
                tbl_CollectionArea_Model left join
                tbl_CollectionModel on tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo left join
                tbl_CollectionStatus_Model as col_stats on col_stats.Id = tbl_CollectionArea_Model.Collection_Status 
                where tbl_CollectionArea_Model.AreaID = '" + dr_area["AreaID"].ToString() + "' ";
                DataTable tbl_reference = db.SelectDb(reference).Tables[0];
                if (tbl_reference.Rows.Count != 0)
                {
                    datec = tbl_reference.Rows[0]["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(tbl_reference.Rows[0]["DateCreated"].ToString()).ToString("yyyy-MM-dd");//
                    Collection_Status_Id = "0";
                    Area_RefNo = tbl_reference.Rows[0]["Area_RefNo"].ToString();
                    Collection_RefNo = tbl_reference.Rows[0]["Collection_RefNo"].ToString();
                    Collection_Status = tbl_reference.Rows[0]["Collection_Status"].ToString();
                    FieldExpenses = tbl_reference.Rows[0]["FieldExpenses"].ToString();
                }
                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    string barangay = spliter[0];
                    string city = spliter[1];


                    string sql_count = $@"SELECT        tbl_Member_Model.Fname, tbl_Member_Model.Mname, tbl_Member_Model.Lname, tbl_Member_Model.Suffix, tbl_Member_Model.Cno, tbl_Member_Model.MemId, tbl_Application_Model.NAID, 
                         tbl_CoMaker_Model.Fname AS Co_Fname, tbl_CoMaker_Model.Mname AS Co_Mname, tbl_CoMaker_Model.Lnam AS Co_Lname, tbl_CoMaker_Model.Suffi AS Co_Suffix, tbl_CoMaker_Model.Cno AS Co_Cno, 
                         tbl_Application_Model.Remarks, tbl_LoanHistory_Model.Penalty, CASE WHEN tbl_LoanDetails_Model.ApprovedDailyAmountDue IS NULL 
                         THEN 0 ELSE tbl_LoanDetails_Model.ApprovedDailyAmountDue END AS DailyCollectibles, CASE WHEN tbl_LoanHistory_Model.OutstandingBalance IS NULL 
                         THEN 0 ELSE tbl_LoanHistory_Model.OutstandingBalance END AS AmountDue, CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                         ELSE tbl_LoanHistory_Model.DueDate END AS DueDate, CASE WHEN tbl_LoanHistory_Model.DateOfFullPayment IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                         ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS DateOfFullPayment, CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL 
                         THEN 0 ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount, CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL 
                         THEN 0 ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS ApprovedAdvancePayment, tbl_LoanDetails_Model.LoanAmount AS LoanPrincipal, tbl_Application_Model.ReleasingDate, 
                         tbl_TermsTypeOfCollection_Model.TypeOfCollection, CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0 ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount, 
                         CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0 ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment, 
                         CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0 ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment, 
                         tbl_Collection_AreaMember_Model.Payment_Status AS Payment_Status_Id, CASE WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING' ELSE tbl_CollectionStatus_Model.[Status] END AS Payment_Status, 
                         CASE WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL 
                         THEN 'NO PAYMENT' WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT' ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method, 
                         CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') ELSE tbl_Collection_AreaMember_Model.DateCollected END AS DateCollected, 
                         CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND' ELSE file_.FilePath END AS FilePath, tbl_LoanDetails_Model.ModeOfRelease, tbl_LoanDetails_Model.ModeOfReleaseReference, 
                         tbl_TermsOfPayment_Model.LoanInsuranceAmount, tbl_TermsOfPayment_Model.InterestRate, tbl_TermsOfPayment_Model.LifeInsuranceAmount, tbl_TermsTypeOfCollection_Model.Value, 
                         tbl_Status_Model.Name AS Status
FROM            tbl_Application_Model INNER JOIN
                         tbl_Status_Model ON tbl_Application_Model.Status = tbl_Status_Model.Id LEFT OUTER JOIN
                         tbl_LoanDetails_Model ON tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID LEFT OUTER JOIN
                         tbl_Member_Model ON tbl_LoanDetails_Model.MemId = tbl_Member_Model.MemId LEFT OUTER JOIN
                         tbl_LoanHistory_Model ON tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId LEFT OUTER JOIN
                         tbl_Collection_AreaMember_Model ON tbl_Collection_AreaMember_Model.NAID = tbl_Application_Model.NAID LEFT OUTER JOIN
                         tbl_CoMaker_Model ON tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId LEFT OUTER JOIN
                         tbl_MemberSavings_Model ON tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId LEFT OUTER JOIN
                         tbl_TermsOfPayment_Model ON tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId LEFT OUTER JOIN
                         tbl_TermsTypeOfCollection_Model ON tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId LEFT OUTER JOIN
                         tbl_CollectionStatus_Model ON tbl_CollectionStatus_Model.Id = tbl_Collection_AreaMember_Model.Payment_Status LEFT OUTER JOIN
                             (SELECT        FilePath, MemId
                               FROM            tbl_fileupload_Model
                               WHERE        (Type = 1)) AS file_ ON file_.MemId = tbl_Member_Model.MemId   where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "'";
                    DataTable table_ = db.SelectDb(sql_count).Tables[0];
                    if (table_.Rows.Count != 0)
                    {

                        foreach (DataRow dr in table_.Rows)
                        {
                            var item = new CollectionVM();
                            item.Fname = dr["Fname"].ToString();
                            item.Mname = dr["Mname"].ToString();
                            item.Lname = dr["Lname"].ToString();
                            item.Suffix = dr["Suffix"].ToString();
                            item.Remarks = dr["Remarks"].ToString();
                            item.Penalty = dr["Penalty"].ToString();
                            //item.DateCreated = datec;
                            item.Status = dr["Status"].ToString();
                            item.MemId = dr["MemId"].ToString();
                            item.Cno = dr["Cno"].ToString();
                            item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                            item.NAID = dr["NAID"].ToString();
                            item.Co_Fname = dr["Co_Fname"].ToString();
                            item.Co_Mname = dr["Co_Mname"].ToString();
                            item.Co_Lname = dr["Co_Lname"].ToString();
                            item.Co_Suffix = dr["Co_Suffix"].ToString();
                            item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                            item.Co_Cno = dr["Co_Cno"].ToString();
                            item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                            item.AmountDue = dr["AmountDue"].ToString();
                            item.DueDate = dr["DueDate"].ToString();
                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                            item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                            item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                            item.ReleasingDate = dr["ReleasingDate"].ToString();
                            item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                            item.CollectedAmount = dr["CollectedAmount"].ToString();
                            item.LapsePayment = dr["LapsePayment"].ToString();
                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                            item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                            item.Payment_Status = dr["Payment_Status"].ToString();
                            item.Collection_Status = Collection_Status;
                            item.Collection_Status_Id = Collection_Status_Id;
                            item.Payment_Method = dr["Payment_Method"].ToString();
                            item.AreaName = dr_area["Area"].ToString();
                            item.AreaID = dr_area["AreaID"].ToString();
                            item.Area_RefNo = Area_RefNo;
                            item.Collection_RefNo = Collection_RefNo;
                            item.FieldOfficer = dr_area["Lname"].ToString() + ", " + dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Suffix"].ToString();
                            item.FOID = dr_area["FOID"].ToString();
                            item.FilePath = dr["FilePath"].ToString();
                            item.DateCollected = dr["DateCollected"].ToString();
                            item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();
                            item.LifeInsurance = dr["LifeInsuranceAmount"].ToString();
                            string sql_count1 = $@"
                                        select 
                                        tbl_Member_Model.MemId,
                                        tbl_Area_Model.AreaID
                                        from
                                        tbl_Application_Model inner join
                                        tbl_Member_Model on tbl_Application_Model.MemId = tbl_Member_Model.MemId left join
                                        tbl_LoanDetails_Model on tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID inner join
                                        tbl_Area_Model on tbl_Area_Model.City LIKE '%' + tbl_Member_Model.Barangay + '%' and tbl_Area_Model.FOID is not null left join
                                        tbl_LoanHistory_Model on tbl_LoanHistory_Model.MemId = tbl_Application_Model.MemId inner join
                                        tbl_FieldOfficer_Model on tbl_FieldOfficer_Model.FOID = tbl_Area_Model.FOID
                                    where tbl_Application_Model.Status =14 and tbl_Area_Model.AreaID='" + dr_area["AreaId"].ToString() + "'";
                            DataTable table1_ = db.SelectDb(sql_count1).Tables[0];
                            item.TotalItems = table1_.Rows.Count.ToString();
                            result.Add(item);
                            //var item = new CollectionVM();
                            //    item.RefNo = dr["RefNo"].ToString();
                            //    item.DateCreated = dr["DateCreated"].ToString();
                            //    item.Area_RefNo = dr["Area_RefNo"].ToString();
                            //    item.AreaID = dr["AreaId"].ToString();
                            //    item.AreaName = dr_area["Area"].ToString();
                            //    item.FOID = dr_area["FOID"].ToString();
                            //    item.DateCollected = date_2;
                            //    item.FieldOfficer = dr_area["Fname"].ToString() + ", " + dr_area["Mname"].ToString() + ", " + dr_area["Lname"].ToString();
                            //    item.Denomination = dr["Denomination"].ToString();
                            //    item.FieldExpenses = dr["FieldExpenses"].ToString() == "" ? "0" : dr["FieldExpenses"].ToString();
                            //    item.Remarks = dr["Remarks"].ToString();
                            //    item.NAID = dr["NAID"].ToString();
                            //    item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                            //    item.LapsePayment = dr["LapsePayment"].ToString() == "" ? "0" : dr["LapsePayment"].ToString();
                            //    item.CollectedAmount = dr["CollectedAmount"].ToString() == "" ? "0" : dr["CollectedAmount"].ToString();
                            //    item.Savings = dr["Savings"].ToString() == "" ? "0" : dr["Savings"].ToString();
                            //    item.PrintedStatus = dr["PrintedStatus"].ToString();
                            //    item.AdvanceStatus = dr["AdvanceStatus"].ToString();
                            //    item.Collection_Status = dr["Collection_Status"].ToString();
                            //    item.Payment_Status = dr["Payment_Status"].ToString();
                            //    item.Payment_Method = dr["Payment_Method"].ToString();
                            //    item.MemId = dr["MemId"].ToString();
                            //    item.OutstandingBalance = dr["OutstandingBalance"].ToString();
                            //    item.Penalty = dr["Penalty"].ToString();
                            //    item.DateReleased = dr["DateReleased"].ToString();
                            //    item.DueDate = dr["DueDate"].ToString();
                            //    item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                            //    item.UsedSavings = dr["UsedSavings"].ToString() == "" ? "0" : dr["UsedSavings"].ToString();
                            //    item.ApprovedTermsOfPayment = dr["ApprovedTermsOfPayment"].ToString() == "" ? "0" : dr["ApprovedTermsOfPayment"].ToString();
                            //    item.ApprovedLoanAmount = dr["ApprovedLoanAmount"].ToString() == "" ? "0" : dr["ApprovedLoanAmount"].ToString();
                            //    item.ApprovedNotarialFee = dr["ApprovedNotarialFee"].ToString() == "" ? "0" : dr["ApprovedNotarialFee"].ToString();
                            //    item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString() == "" ? "0" : dr["ApprovedAdvancePayment"].ToString();
                            //    item.ApprovedReleasingAmount = dr["ApprovedReleasingAmount"].ToString() == "" ? "0" : dr["ApprovedReleasingAmount"].ToString();
                            //    item.ApproveedInterest = dr["ApproveedInterest"].ToString() == "" ? "0" : dr["ApproveedInterest"].ToString();
                            //    item.ApprovedDailyAmountDue = dr["ApprovedDailyAmountDue"].ToString() == "" ? "0" : dr["ApprovedDailyAmountDue"].ToString();
                            //    item.ModeOfRelease = dr["ModeOfRelease"].ToString();
                            //    item.LoanTypeID = dr["LoanTypeID"].ToString();

                            //result.Add(item);



                        }


                    }

                }
            }
            //    DataTable table = db.SelectDb_SP("sp_loanhistory_all").Tables[0];
            //foreach (DataRow dr in table.Rows)
            //{

            //    //DateTime dateToCompare = DateTime.Parse(dr["DateCreated"].ToString()); // Replace with your specific date
            //    //DateTime currentDate = DateTime.Now;
            //    var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

            

            //}


            return result;

        }
        public List<AreaDetailsVM> ShowArea()
        {
            var areas = Collection_PrintedResult().GroupBy(a => new { a.RefNo, a.DateCreated }).ToList();
            var list = Collection_PrintedResult().ToList();
            bool containsNow = list.Any(dt => dt.ToString() == DateTime.Parse(Convert.ToDateTime(list[0].DateCreated).ToString("yyyy-MM-dd")).ToString());
            var res = new List<AreaDetailsVM>();
            foreach (var group in areas)
            {

                var advance_payment = list.Where(a => a.RefNo == group.Key.RefNo && a.AdvancePayment != null && a.AdvancePayment != "").Select(a => double.Parse(a.AdvancePayment)).Sum();

                var dailyCollectiblesSum = list.Where(a => a.RefNo == group.Key.RefNo && a.ApprovedDailyAmountDue != "").Select(a => double.Parse(a.ApprovedDailyAmountDue)).Sum();
                var savings = list.Where(a => a.RefNo == group.Key.RefNo && a.Savings != "").Select(a => double.Parse(a.Savings)).Sum();
                var balance = list.Where(a => a.RefNo == group.Key.RefNo && a.OutstandingBalance != "").Select(a => double.Parse(a.OutstandingBalance)).Sum();
                var advance = list.Where(a => a.RefNo == group.Key.RefNo && a.ApprovedAdvancePayment != "").Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                var lapses = list.Where(a => a.RefNo == group.Key.RefNo && a.LapsePayment != "").Select(a => double.Parse(a.LapsePayment)).Sum();
                var collectedamount = list.Where(a => a.RefNo == group.Key.RefNo && a.CollectedAmount != "").Select(a => double.Parse(a.CollectedAmount)).Sum();
                var fieldexpenses = list.Where(a => a.RefNo == group.Key.RefNo && a.FieldExpenses != "").Select(a => double.Parse(a.FieldExpenses)).Sum();

                var items = new AreaDetailsVM();
                items.TotalCollectible = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.Total_Balance = Math.Ceiling(double.Parse(balance.ToString()));
                items.Total_savings = Math.Ceiling(double.Parse(savings.ToString()));
                items.Total_advance = Math.Ceiling(double.Parse(advance.ToString()));
                items.Total_lapses = Math.Ceiling(lapses);
                items.Total_collectedAmount = Math.Ceiling(collectedamount);
                items.Collection_RefNo = group.Key.RefNo;
                items.DateCreated = group.Key.DateCreated;
                items.TotalItems = list.Count.ToString();
                items.ExpectedCollection = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.AdvancePayment = Math.Ceiling(advance_payment);
                res.Add(items);
            }
            return res;
        }
        public List<CollectionVM> getAreaLoanSummary_2(string areaid, string? arearefno)
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = new List<CollectionVM>();
            string w_column = "";
            string r_column = "";
            string datec = "";
            string Collection_Status_Id = "";
            string FieldExpenses = "0";
            string Area_RefNo = "PENDING";
            string Collection_RefNo = "PENDING";
            string Collection_Status = "NO PAYMENT";
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID
                        where tbl_Area_Model.AreaID ='" + areaid + "'";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {

                if (arearefno != "")
                {
                    w_column = " and tbl_CollectionArea_Model.Area_RefNo = '" + arearefno + "'  ";
                    r_column = " and tbl_Collection_AreaMember_Model.Area_RefNo = '" + arearefno + "'  ";
                }

                string reference = $@"select
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status ,
                CASE 
                WHEN tbl_CollectionArea_Model.Collection_Status IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.Collection_Status END as Collection_Status_Id,
                CASE 
                WHEN tbl_CollectionArea_Model.Area_RefNo IS NULL THEN 'PENDING'
                ELSE tbl_CollectionArea_Model.Area_RefNo END AS Area_RefNo,
                CASE
                WHEN tbl_CollectionModel.RefNo IS NULL THEN 'PENDING' 
                ELSE  tbl_CollectionModel.RefNo  END as Collection_RefNo,
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status,tbl_CollectionModel.DateCreated,
				 CASE 
                WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.FieldExpenses END as FieldExpenses
                from 
                tbl_CollectionArea_Model left join
                tbl_CollectionModel on tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo left join
                tbl_CollectionStatus_Model as col_stats on col_stats.Id = tbl_CollectionArea_Model.Collection_Status 
                where tbl_CollectionArea_Model.AreaID = '" + dr_area["AreaID"].ToString() + "' " + w_column + " ";
                DataTable tbl_reference = db.SelectDb(reference).Tables[0];
                if (tbl_reference.Rows.Count != 0)
                {
                    datec = tbl_reference.Rows[0]["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(tbl_reference.Rows[0]["DateCreated"].ToString()).ToString("yyyy-MM-dd");//
                    Collection_Status_Id = "0";
                    Area_RefNo = tbl_reference.Rows[0]["Area_RefNo"].ToString();
                    Collection_RefNo = tbl_reference.Rows[0]["Collection_RefNo"].ToString();
                    Collection_Status = tbl_reference.Rows[0]["Collection_Status"].ToString();
                    FieldExpenses = tbl_reference.Rows[0]["FieldExpenses"].ToString();
                }

                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    string barangay = spliter[0];
                    string city = spliter[1];


                    if (arearefno == null || arearefno == "")
                    {
                        string sql_applicationdetails2 = $@"
                
                                 select 
                                tbl_Member_Model.Fname,
                                tbl_Member_Model.Mname,
                                tbl_Member_Model.Lname,
                                tbl_Member_Model.Suffix,
                                tbl_Member_Model.Cno,
                                tbl_Member_Model.MemId,
                                tbl_Application_Model.NAID,
                                tbl_CoMaker_Model.Fname as Co_Fname,
                                tbl_CoMaker_Model.Mname as Co_Mname,
                                tbl_CoMaker_Model.lnam  as Co_Lname,
                                tbl_CoMaker_Model.Suffi as Co_Suffix,
                                tbl_CoMaker_Model.Cno as Co_Cno,
                                tbl_Application_Model.Remarks ,

                                tbl_LoanHistory_Model.Penalty,
                                Case when tbl_LoanDetails_Model.ApprovedDailyAmountDue  IS NULL then 0 
                                else tbl_LoanDetails_Model.ApprovedDailyAmountDue end
                                as DailyCollectibles,
                                case when tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0
                                ELSE tbl_LoanHistory_Model.OutstandingBalance end as AmountDue ,
                                CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                                ELSE tbl_LoanHistory_Model.DueDate END AS DueDate,
                                case when tbl_LoanHistory_Model.DateOfFullPayment  IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                                ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS  DateOfFullPayment,
                                CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL THEN 0 
                                ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount,
                              -- CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0 
                   -- ELSE tbl_Collection_AreaMember_Model.AdvancePayment  END AS ApprovedAdvancePayment,
                                tbl_LoanDetails_Model.LoanAmount as LoanPrincipal,
                                CASE WHEN tbl_Application_Model.ReleasingDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                                ELSE tbl_Application_Model.ReleasingDate  END AS ReleasingDate, 
                                tbl_TermsTypeOfCollection_Model.TypeOfCollection,
                            ---    CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0
                         ----       ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount,
                        -- --       CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0
                        --        ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment,
                        --        CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0
                        --        ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment,
                         --       tbl_Collection_AreaMember_Model.Payment_Status as Payment_Status_Id,
                       ---             CASE
                        --                WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING'
                         --               ELSE tbl_CollectionStatus_Model.[Status]
                             --       END as Payment_Status,


                        --    CASE 
                         --   WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT'
                         --   WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT'
                       --     ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method,




                          --  CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                        --    ELSE tbl_Collection_AreaMember_Model.DateCollected  END AS DateCollected,

                            CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND'
                            ELSE file_.FilePath END AS FilePath,

                            tbl_LoanDetails_Model.ModeOfRelease,
                            tbl_LoanDetails_Model.ModeOfReleaseReference,
                            tbl_TermsOfPayment_Model.LoanInsuranceAmount,
		                    tbl_TermsOfPayment_Model.InterestRate,
                            tbl_TermsOfPayment_Model.LifeInsuranceAmount, tbl_TermsTypeOfCollection_Model.Value
                            from
                            tbl_Application_Model left join 
                            tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join 
                            tbl_Member_Model on tbl_LoanDetails_Model.MemId  = tbl_Member_Model.MemId left join 
                            tbl_LoanHistory_Model on tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId left JOIN 
                            tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId left JOIN
                            tbl_MemberSavings_Model on tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId left JOIN
                            tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId left JOIN
                            tbl_TermsTypeOfCollection_Model on tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId left JOIN
                            (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId
                            where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "' and tbl_Application_Model.Status = 14 ";
                        DataTable tbl_application_details2 = db.SelectDb(sql_applicationdetails2).Tables[0];
                        if (tbl_application_details2.Rows.Count != 0)
                        {

                            foreach (DataRow dr in tbl_application_details2.Rows)
                            {
                                if (dr["TypeOfCollection"].ToString() == "Daily")
                                {
                                    double interestrate = double.Parse(dr["InterestRate"].ToString());
                                    double bal = double.Parse(dr["LoanPrincipal"].ToString() == "" ? "0" : dr["LoanPrincipal"].ToString());
                                    double col = 0;
                                    double total_bal = 0;
                                    double pastdueamount = 0;
                                    double interest_amount = 0;
                                    if (Convert.ToDateTime(currentDate.ToString()) > Convert.ToDateTime(dr["DueDate"].ToString()))
                                    {
                                        total_bal = double.Parse(dr["AmountDue"].ToString()) + (double.Parse(dr["AmountDue"].ToString()) * interestrate);
                                        interest_amount = double.Parse(dr["AmountDue"].ToString()) * interestrate;
                                    }
                                    else
                                    {
                                        total_bal = double.Parse(dr["AmountDue"].ToString());
                                        interest_amount = 0;
                                    }
                                    var item = new CollectionVM();
                                    item.Fname = dr["Fname"].ToString();
                                    item.Mname = dr["Mname"].ToString();
                                    item.Lname = dr["Lname"].ToString();
                                    item.Suffix = dr["Suffix"].ToString();
                                    item.Remarks = dr["Remarks"].ToString();
                                    item.Penalty = dr["Penalty"].ToString();
                                    item.DateCreated = datec;

                                    item.MemId = dr["MemId"].ToString();
                                    item.Cno = dr["Cno"].ToString();
                                    item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                    item.NAID = dr["NAID"].ToString();
                                    item.Co_Fname = dr["Co_Fname"].ToString();
                                    item.Co_Mname = dr["Co_Mname"].ToString();
                                    item.Co_Lname = dr["Co_Lname"].ToString();
                                    item.Co_Suffix = dr["Co_Suffix"].ToString();
                                    item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                    item.Co_Cno = dr["Co_Cno"].ToString();
                                    item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                    item.AmountDue = total_bal.ToString();
                                    item.DueDate = dr["DueDate"].ToString();
                                    item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                    item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                    item.ApprovedAdvancePayment = "0";
                                    item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                    item.ReleasingDate = dr["ReleasingDate"].ToString();
                                    item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                    item.CollectedAmount = "0";
                                    item.LapsePayment = "0";
                                    item.AdvancePayment = "0";
                                    item.Payment_Status_Id = "0";
                                    item.Payment_Status = "PENDING";
                                    item.Collection_Status = "NO PAYMENT";
                                    item.Collection_Status_Id = "0";//
                                    item.Payment_Method = "NO PAYMENT";
                                    item.AreaName = dr_area["Area"].ToString();
                                    item.AreaID = dr_area["AreaID"].ToString();
                                    item.FieldExpenses = FieldExpenses;
                                    item.Area_RefNo = "PENDING"; ;//
                                    item.PastDue = interest_amount.ToString();
                                    item.Collection_RefNo = "PENDING"; ;//
                                    item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                    item.FOID = dr_area["FOID"].ToString();
                                    item.FilePath = dr["FilePath"].ToString();
                                    item.DateCollected = "";
                                    item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                    result.Add(item);
                                }
                                else
                                {
                                    int day_val = int.Parse(dr["Value"].ToString());
                                    DateTime date1 = Convert.ToDateTime(currentDate.ToString());
                                    DateTime date2 = Convert.ToDateTime(dr["ReleasingDate"].ToString()).AddDays(day_val);
                                    TimeSpan difference = date2 - date1;
                                    int dayDifference = difference.Days;
                                    if (dayDifference == 0)
                                    {

                                        double interestrate = double.Parse(dr["InterestRate"].ToString());
                                        double bal = double.Parse(dr["LoanPrincipal"].ToString() == "" ? "0" : dr["LoanPrincipal"].ToString());
                                        double col = 0;
                                        double total_bal = 0;
                                        double pastdueamount = 0;
                                        double interest_amount = 0;
                                        if (Convert.ToDateTime(currentDate.ToString()) > Convert.ToDateTime(dr["DueDate"].ToString()))
                                        {
                                            total_bal = double.Parse(dr["AmountDue"].ToString()) + (double.Parse(dr["AmountDue"].ToString()) * interestrate);
                                            interest_amount = double.Parse(dr["AmountDue"].ToString()) * interestrate;
                                        }
                                        else
                                        {
                                            total_bal = double.Parse(dr["AmountDue"].ToString());
                                            interest_amount = 0;
                                        }
                                        var item = new CollectionVM();
                                        item.Fname = dr["Fname"].ToString();
                                        item.Mname = dr["Mname"].ToString();
                                        item.Lname = dr["Lname"].ToString();
                                        item.Suffix = dr["Suffix"].ToString();
                                        item.Remarks = dr["Remarks"].ToString();
                                        item.Penalty = dr["Penalty"].ToString();
                                        item.DateCreated = datec;

                                        item.MemId = dr["MemId"].ToString();
                                        item.Cno = dr["Cno"].ToString();
                                        item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                        item.NAID = dr["NAID"].ToString();
                                        item.Co_Fname = dr["Co_Fname"].ToString();
                                        item.Co_Mname = dr["Co_Mname"].ToString();
                                        item.Co_Lname = dr["Co_Lname"].ToString();
                                        item.Co_Suffix = dr["Co_Suffix"].ToString();
                                        item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                        item.Co_Cno = dr["Co_Cno"].ToString();
                                        item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                        item.AmountDue = total_bal.ToString();
                                        item.DueDate = dr["DueDate"].ToString();
                                        item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                        item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                        item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                        item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                        item.ReleasingDate = dr["ReleasingDate"].ToString();
                                        item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                        item.CollectedAmount = "0";
                                        item.LapsePayment = "0";
                                        item.AdvancePayment = "0";
                                        item.PastDue = interest_amount.ToString();
                                        item.Payment_Status_Id = "0";
                                        item.Payment_Status = "PENDING";
                                        item.Collection_Status = "NO PAYMENT";
                                        item.Collection_Status_Id = "0";//
                                        item.Payment_Method = "NO PAYMENT";
                                        item.AreaName = dr_area["Area"].ToString();
                                        item.AreaID = dr_area["AreaID"].ToString();
                                        item.FieldExpenses = FieldExpenses;
                                        item.Area_RefNo = "PENDING"; ;//
                                        item.Collection_RefNo = "PENDING"; ;//
                                        item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                        item.FOID = dr_area["FOID"].ToString();
                                        item.FilePath = dr["FilePath"].ToString();
                                        item.DateCollected = dr["DateCollected"].ToString();
                                        item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                        result.Add(item);

                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        string sql_applicationdetails = $@"
                
                    select 
                    tbl_Member_Model.Fname,
                    tbl_Member_Model.Mname,
                    tbl_Member_Model.Lname,
                    tbl_Member_Model.Suffix,
                    tbl_Member_Model.Cno,
                    tbl_Member_Model.MemId,
                    tbl_Application_Model.NAID,
                    tbl_CoMaker_Model.Fname as Co_Fname,
                    tbl_CoMaker_Model.Mname as Co_Mname,
                    tbl_CoMaker_Model.lnam  as Co_Lname,
                    tbl_CoMaker_Model.Suffi as Co_Suffix,
                    tbl_CoMaker_Model.Cno as Co_Cno,
                    tbl_Application_Model.Remarks ,

                    tbl_LoanHistory_Model.Penalty,
                    Case when tbl_LoanDetails_Model.ApprovedDailyAmountDue  IS NULL then 0 
                    else tbl_LoanDetails_Model.ApprovedDailyAmountDue end
                    as DailyCollectibles,
                    case when tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0
                    ELSE tbl_LoanHistory_Model.OutstandingBalance end as AmountDue ,
                    CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DueDate END AS DueDate,
                    case when tbl_LoanHistory_Model.DateOfFullPayment  IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS  DateOfFullPayment,
                    CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL THEN 0 
                    ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount,
                    CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0 
                    ELSE tbl_Collection_AreaMember_Model.AdvancePayment  END AS ApprovedAdvancePayment,
                    tbl_LoanDetails_Model.LoanAmount as LoanPrincipal,
                    tbl_Application_Model.ReleasingDate , 
                    tbl_TermsTypeOfCollection_Model.TypeOfCollection,
                    CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount,
                    CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment,
                    CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment,
                    tbl_Collection_AreaMember_Model.Payment_Status as Payment_Status_Id,
                        CASE
                            WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING'
                            ELSE tbl_CollectionStatus_Model.[Status]
                        END as Payment_Status,


                CASE 
                WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT'
                WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT'
                ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method,




                CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                ELSE tbl_Collection_AreaMember_Model.DateCollected  END AS DateCollected,

                CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND'
                ELSE file_.FilePath END AS FilePath,

                tbl_LoanDetails_Model.ModeOfRelease,
                tbl_LoanDetails_Model.ModeOfReleaseReference,
                tbl_TermsOfPayment_Model.LoanInsuranceAmount,
                tbl_TermsOfPayment_Model.InterestRate,
                tbl_TermsOfPayment_Model.LifeInsuranceAmount, tbl_TermsTypeOfCollection_Model.Value
                from
                tbl_Application_Model left join 
                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_Member_Model on tbl_LoanDetails_Model.MemId  = tbl_Member_Model.MemId left join 
                tbl_LoanHistory_Model on tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId left JOIN 
                tbl_Collection_AreaMember_Model on tbl_Collection_AreaMember_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId left JOIN
                tbl_MemberSavings_Model on tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId left JOIN
                tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId left JOIN
                tbl_TermsTypeOfCollection_Model on tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId left JOIN
                tbl_CollectionStatus_Model on tbl_CollectionStatus_Model.Id = tbl_Collection_AreaMember_Model.Payment_Status left join 
                (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId
                where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "' and tbl_Application_Model.Status = 14 " + r_column + "";
                        DataTable tbl_application_details = db.SelectDb(sql_applicationdetails).Tables[0];

                        if (tbl_application_details.Rows.Count != 0)
                        {

                            foreach (DataRow dr in tbl_application_details.Rows)
                            {
                                if (dr["TypeOfCollection"].ToString() == "Daily")
                                {

                                    double interestrate = double.Parse(dr["InterestRate"].ToString());
                                    double bal = double.Parse(dr["LoanPrincipal"].ToString() == "" ? "0" : dr["LoanPrincipal"].ToString());
                                    double col = 0;
                                    double total_bal = 0;
                                    double pastdueamount = 0;
                                    double interest_amount = 0;
                                    if (Convert.ToDateTime(currentDate.ToString()) > Convert.ToDateTime(dr["DueDate"].ToString()))
                                    {
                                        total_bal = double.Parse(dr["AmountDue"].ToString()) + (double.Parse(dr["AmountDue"].ToString()) * interestrate);
                                        interest_amount = double.Parse(dr["AmountDue"].ToString()) * interestrate;
                                    }
                                    else
                                    {
                                        total_bal = double.Parse(dr["AmountDue"].ToString());
                                        interest_amount = 0;
                                    }
                                    if (Collection_Status == "Collected")
                                    {

                                        if (double.Parse(dr["AmountDue"].ToString()) != 0)
                                        {
                                            var item = new CollectionVM();
                                            item.Fname = dr["Fname"].ToString();
                                            item.Mname = dr["Mname"].ToString();
                                            item.Lname = dr["Lname"].ToString();
                                            item.Suffix = dr["Suffix"].ToString();
                                            item.Remarks = dr["Remarks"].ToString();
                                            item.Penalty = dr["Penalty"].ToString();
                                            item.DateCreated = datec;
                                            item.PastDue = interest_amount.ToString();
                                            item.MemId = dr["MemId"].ToString();
                                            item.Cno = dr["Cno"].ToString();
                                            item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                            item.NAID = dr["NAID"].ToString();
                                            item.Co_Fname = dr["Co_Fname"].ToString();
                                            item.Co_Mname = dr["Co_Mname"].ToString();
                                            item.Co_Lname = dr["Co_Lname"].ToString();
                                            item.Co_Suffix = dr["Co_Suffix"].ToString();
                                            item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                            item.Co_Cno = dr["Co_Cno"].ToString();
                                            item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                            item.AmountDue = total_bal.ToString();
                                            item.DueDate = dr["DueDate"].ToString();
                                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                            item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                            item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                            item.ReleasingDate = dr["ReleasingDate"].ToString();
                                            item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                            item.CollectedAmount = dr["CollectedAmount"].ToString();
                                            item.LapsePayment = dr["LapsePayment"].ToString();
                                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                            item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                            item.Payment_Status = dr["Payment_Status"].ToString();
                                            item.Collection_Status = Collection_Status;//
                                            item.Collection_Status_Id = Collection_Status_Id;//
                                            item.Payment_Method = dr["Payment_Method"].ToString();
                                            item.AreaName = dr_area["Area"].ToString();
                                            item.AreaID = dr_area["AreaID"].ToString();
                                            item.FieldExpenses = FieldExpenses;
                                            item.Area_RefNo = Area_RefNo;//
                                            item.Collection_RefNo = Collection_RefNo;//
                                            item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                            item.FOID = dr_area["FOID"].ToString();
                                            item.FilePath = dr["FilePath"].ToString();
                                            item.DateCollected = dr["DateCollected"].ToString();
                                            item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                            result.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        var item = new CollectionVM();
                                        item.Fname = dr["Fname"].ToString();
                                        item.Mname = dr["Mname"].ToString();
                                        item.Lname = dr["Lname"].ToString();
                                        item.Suffix = dr["Suffix"].ToString();
                                        item.Remarks = dr["Remarks"].ToString();
                                        item.Penalty = dr["Penalty"].ToString();
                                        item.DateCreated = datec;
                                        item.PastDue = interest_amount.ToString();
                                        item.MemId = dr["MemId"].ToString();
                                        item.Cno = dr["Cno"].ToString();
                                        item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                        item.NAID = dr["NAID"].ToString();
                                        item.Co_Fname = dr["Co_Fname"].ToString();
                                        item.Co_Mname = dr["Co_Mname"].ToString();
                                        item.Co_Lname = dr["Co_Lname"].ToString();
                                        item.Co_Suffix = dr["Co_Suffix"].ToString();
                                        item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                        item.Co_Cno = dr["Co_Cno"].ToString();
                                        item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                        item.AmountDue = total_bal.ToString();
                                        item.DueDate = dr["DueDate"].ToString();
                                        item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                        item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                        item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                        item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                        item.ReleasingDate = dr["ReleasingDate"].ToString();
                                        item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                        item.CollectedAmount = dr["CollectedAmount"].ToString();
                                        item.LapsePayment = dr["LapsePayment"].ToString();
                                        item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                        item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                        item.Payment_Status = dr["Payment_Status"].ToString();
                                        item.Collection_Status = Collection_Status;//
                                        item.Collection_Status_Id = Collection_Status_Id;//
                                        item.Payment_Method = dr["Payment_Method"].ToString();
                                        item.AreaName = dr_area["Area"].ToString();
                                        item.AreaID = dr_area["AreaID"].ToString();
                                        item.FieldExpenses = FieldExpenses;
                                        item.Area_RefNo = Area_RefNo;//
                                        item.Collection_RefNo = Collection_RefNo;//
                                        item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                        item.FOID = dr_area["FOID"].ToString();
                                        item.FilePath = dr["FilePath"].ToString();
                                        item.DateCollected = dr["DateCollected"].ToString();
                                        item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                        result.Add(item);
                                    }
                                }
                                else
                                {

                                    int day_val = int.Parse(dr["Value"].ToString());
                                    DateTime date1 = Convert.ToDateTime(currentDate.ToString());
                                    DateTime date2 = Convert.ToDateTime(dr["ReleasingDate"].ToString()).AddDays(day_val);
                                    TimeSpan difference = date2 - date1;
                                    int dayDifference = difference.Days;
                                    if (dayDifference == 0)
                                    {


                                        double interestrate = double.Parse(dr["InterestRate"].ToString());
                                        double bal = double.Parse(dr["LoanPrincipal"].ToString() == "" ? "0" : dr["LoanPrincipal"].ToString());
                                        double col = 0;
                                        double total_bal = 0;
                                        double pastdueamount = 0;
                                        double interest_amount = 0;
                                        if (Convert.ToDateTime(currentDate.ToString()) > Convert.ToDateTime(dr["DueDate"].ToString()))
                                        {
                                            total_bal = double.Parse(dr["AmountDue"].ToString()) + (double.Parse(dr["AmountDue"].ToString()) * interestrate);
                                            interest_amount = double.Parse(dr["AmountDue"].ToString()) * interestrate;
                                        }
                                        else
                                        {
                                            total_bal = double.Parse(dr["AmountDue"].ToString());
                                            interest_amount = 0;
                                        }
                                        if (Collection_Status == "Collected")
                                        {
                                            if (double.Parse(dr["AmountDue"].ToString()) != 0)
                                            {
                                                var item = new CollectionVM();
                                                item.Fname = dr["Fname"].ToString();
                                                item.Mname = dr["Mname"].ToString();
                                                item.Lname = dr["Lname"].ToString();
                                                item.Suffix = dr["Suffix"].ToString();
                                                item.Remarks = dr["Remarks"].ToString();
                                                item.Penalty = dr["Penalty"].ToString();
                                                item.DateCreated = datec;
                                                item.PastDue = interest_amount.ToString();
                                                item.MemId = dr["MemId"].ToString();
                                                item.Cno = dr["Cno"].ToString();
                                                item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                                item.NAID = dr["NAID"].ToString();
                                                item.Co_Fname = dr["Co_Fname"].ToString();
                                                item.Co_Mname = dr["Co_Mname"].ToString();
                                                item.Co_Lname = dr["Co_Lname"].ToString();
                                                item.Co_Suffix = dr["Co_Suffix"].ToString();
                                                item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                                item.Co_Cno = dr["Co_Cno"].ToString();
                                                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                                item.AmountDue = total_bal.ToString();
                                                item.DueDate = dr["DueDate"].ToString();
                                                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                                item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                                item.ReleasingDate = dr["ReleasingDate"].ToString();
                                                item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                                item.CollectedAmount = dr["CollectedAmount"].ToString();
                                                item.LapsePayment = dr["LapsePayment"].ToString();
                                                item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                                item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                                item.Payment_Status = dr["Payment_Status"].ToString();
                                                item.Collection_Status = Collection_Status;//
                                                item.Collection_Status_Id = Collection_Status_Id;//
                                                item.Payment_Method = dr["Payment_Method"].ToString();
                                                item.AreaName = dr_area["Area"].ToString();
                                                item.AreaID = dr_area["AreaID"].ToString();
                                                item.FieldExpenses = FieldExpenses;
                                                item.Area_RefNo = Area_RefNo;//
                                                item.Collection_RefNo = Collection_RefNo;//
                                                item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                                item.FOID = dr_area["FOID"].ToString();
                                                item.FilePath = dr["FilePath"].ToString();
                                                item.DateCollected = dr["DateCollected"].ToString();
                                                item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                                result.Add(item);

                                            }

                                        }
                                        else
                                        {
                                            var item = new CollectionVM();
                                            item.Fname = dr["Fname"].ToString();
                                            item.Mname = dr["Mname"].ToString();
                                            item.Lname = dr["Lname"].ToString();
                                            item.Suffix = dr["Suffix"].ToString();
                                            item.Remarks = dr["Remarks"].ToString();
                                            item.Penalty = dr["Penalty"].ToString();
                                            item.DateCreated = datec;
                                            item.PastDue = interest_amount.ToString();
                                            item.MemId = dr["MemId"].ToString();
                                            item.Cno = dr["Cno"].ToString();
                                            item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                            item.NAID = dr["NAID"].ToString();
                                            item.Co_Fname = dr["Co_Fname"].ToString();
                                            item.Co_Mname = dr["Co_Mname"].ToString();
                                            item.Co_Lname = dr["Co_Lname"].ToString();
                                            item.Co_Suffix = dr["Co_Suffix"].ToString();
                                            item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                            item.Co_Cno = dr["Co_Cno"].ToString();
                                            item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                            item.AmountDue = total_bal.ToString();
                                            item.DueDate = dr["DueDate"].ToString();
                                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                            item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                            item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                            item.ReleasingDate = dr["ReleasingDate"].ToString();
                                            item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                            item.CollectedAmount = dr["CollectedAmount"].ToString();
                                            item.LapsePayment = dr["LapsePayment"].ToString();
                                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                            item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                            item.Payment_Status = dr["Payment_Status"].ToString();
                                            item.Collection_Status = Collection_Status;//
                                            item.Collection_Status_Id = Collection_Status_Id;//
                                            item.Payment_Method = dr["Payment_Method"].ToString();
                                            item.AreaName = dr_area["Area"].ToString();
                                            item.AreaID = dr_area["AreaID"].ToString();
                                            item.FieldExpenses = FieldExpenses;
                                            item.Area_RefNo = Area_RefNo;//
                                            item.Collection_RefNo = Collection_RefNo;//
                                            item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                            item.FOID = dr_area["FOID"].ToString();
                                            item.FilePath = dr["FilePath"].ToString();
                                            item.DateCollected = dr["DateCollected"].ToString();
                                            item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                            result.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {


                            string sql_applicationdetails_2 = $@"
                
                    select 
                    tbl_Member_Model.Fname,
                    tbl_Member_Model.Mname,
                    tbl_Member_Model.Lname,
                    tbl_Member_Model.Suffix,
                    tbl_Member_Model.Cno,
                    tbl_Member_Model.MemId,
                    tbl_Application_Model.NAID,
                    tbl_CoMaker_Model.Fname as Co_Fname,
                    tbl_CoMaker_Model.Mname as Co_Mname,
                    tbl_CoMaker_Model.lnam  as Co_Lname,
                    tbl_CoMaker_Model.Suffi as Co_Suffix,
                    tbl_CoMaker_Model.Cno as Co_Cno,
                    tbl_Application_Model.Remarks ,

                    tbl_LoanHistory_Model.Penalty,
                    Case when tbl_LoanDetails_Model.ApprovedDailyAmountDue  IS NULL then 0 
                    else tbl_LoanDetails_Model.ApprovedDailyAmountDue end
                    as DailyCollectibles,
                    case when tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0
                    ELSE tbl_LoanHistory_Model.OutstandingBalance end as AmountDue ,
                    CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DueDate END AS DueDate,
                    case when tbl_LoanHistory_Model.DateOfFullPayment  IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS  DateOfFullPayment,
                    CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL THEN 0 
                    ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount,
              CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0 
                    ELSE tbl_Collection_AreaMember_Model.AdvancePayment  END AS ApprovedAdvancePayment,
                    tbl_LoanDetails_Model.LoanAmount as LoanPrincipal,
                    tbl_Application_Model.ReleasingDate , 
                    tbl_TermsTypeOfCollection_Model.TypeOfCollection,
                    CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount,
                    CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment,
                    CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment,
                    tbl_Collection_AreaMember_Model.Payment_Status as Payment_Status_Id,
                        CASE
                            WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING'
                            ELSE tbl_CollectionStatus_Model.[Status]
                        END as Payment_Status,


                CASE 
                WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT'
                WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT'
                ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method,




                CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                ELSE tbl_Collection_AreaMember_Model.DateCollected  END AS DateCollected,

                CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND'
                ELSE file_.FilePath END AS FilePath,

                tbl_LoanDetails_Model.ModeOfRelease,
                tbl_LoanDetails_Model.ModeOfReleaseReference,
                tbl_TermsOfPayment_Model.LoanInsuranceAmount,
                tbl_TermsOfPayment_Model.InterestRate,
                tbl_TermsOfPayment_Model.LifeInsuranceAmount, tbl_TermsTypeOfCollection_Model.Value
                from
                tbl_Application_Model left join 
                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_Member_Model on tbl_LoanDetails_Model.MemId  = tbl_Member_Model.MemId left join 
                tbl_LoanHistory_Model on tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId left JOIN 
                tbl_Collection_AreaMember_Model on tbl_Collection_AreaMember_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId left JOIN
                tbl_MemberSavings_Model on tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId left JOIN
                tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId left JOIN
                tbl_TermsTypeOfCollection_Model on tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId left JOIN
                tbl_CollectionStatus_Model on tbl_CollectionStatus_Model.Id = tbl_Collection_AreaMember_Model.Payment_Status left join 
                (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId
                where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "'  and tbl_Application_Model.Status =14";
                            DataTable tbl_application_details_2 = db.SelectDb(sql_applicationdetails_2).Tables[0];

                            if (tbl_application_details_2.Rows.Count != 0)
                            {
                                if (tbl_application_details_2.Rows[0]["TypeOfCollection"].ToString() == "Daily")
                                {

                                    foreach (DataRow dr in tbl_application_details_2.Rows)
                                    {
                                        double interestrate = double.Parse(dr["InterestRate"].ToString());
                                        double bal = double.Parse(dr["LoanPrincipal"].ToString() == "" ? "0" : dr["LoanPrincipal"].ToString());
                                        double col = 0;
                                        double total_bal = 0;
                                        double pastdueamount = 0;
                                        double interest_amount = 0;
                                        if (Convert.ToDateTime(currentDate.ToString()) > Convert.ToDateTime(dr["DueDate"].ToString()))
                                        {
                                            total_bal = double.Parse(dr["AmountDue"].ToString()) + (double.Parse(dr["AmountDue"].ToString()) * interestrate);
                                            interest_amount = double.Parse(dr["AmountDue"].ToString()) * interestrate;
                                        }
                                        else
                                        {
                                            total_bal = double.Parse(dr["AmountDue"].ToString());
                                            interest_amount = 0;
                                        }
                                        if (Collection_Status == "Collected")
                                        {
                                            if (double.Parse(dr["AmountDue"].ToString()) != 0)
                                            {
                                                var item = new CollectionVM();
                                                item.Fname = dr["Fname"].ToString();
                                                item.Mname = dr["Mname"].ToString();
                                                item.Lname = dr["Lname"].ToString();
                                                item.Suffix = dr["Suffix"].ToString();
                                                item.Remarks = dr["Remarks"].ToString();
                                                item.Penalty = dr["Penalty"].ToString();
                                                item.DateCreated = datec;
                                                item.PastDue = interest_amount.ToString();
                                                item.MemId = dr["MemId"].ToString();
                                                item.Cno = dr["Cno"].ToString();
                                                item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                                item.NAID = dr["NAID"].ToString();
                                                item.Co_Fname = dr["Co_Fname"].ToString();
                                                item.Co_Mname = dr["Co_Mname"].ToString();
                                                item.Co_Lname = dr["Co_Lname"].ToString();
                                                item.Co_Suffix = dr["Co_Suffix"].ToString();
                                                item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                                item.Co_Cno = dr["Co_Cno"].ToString();
                                                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                                item.AmountDue = total_bal.ToString();
                                                item.DueDate = dr["DueDate"].ToString();
                                                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                                item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                                item.ReleasingDate = dr["ReleasingDate"].ToString();
                                                item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                                item.CollectedAmount = dr["CollectedAmount"].ToString();
                                                item.LapsePayment = dr["LapsePayment"].ToString();
                                                item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                                item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                                item.Payment_Status = dr["Payment_Status"].ToString();
                                                item.Collection_Status = Collection_Status;//
                                                item.Collection_Status_Id = Collection_Status_Id;//
                                                item.Payment_Method = dr["Payment_Method"].ToString();
                                                item.AreaName = dr_area["Area"].ToString();
                                                item.AreaID = dr_area["AreaID"].ToString();
                                                item.FieldExpenses = FieldExpenses;
                                                item.Area_RefNo = Area_RefNo;//
                                                item.Collection_RefNo = Collection_RefNo;//
                                                item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                                item.FOID = dr_area["FOID"].ToString();
                                                item.FilePath = dr["FilePath"].ToString();
                                                item.DateCollected = dr["DateCollected"].ToString();
                                                item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                                result.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            var item = new CollectionVM();
                                            item.Fname = dr["Fname"].ToString();
                                            item.Mname = dr["Mname"].ToString();
                                            item.Lname = dr["Lname"].ToString();
                                            item.Suffix = dr["Suffix"].ToString();
                                            item.Remarks = dr["Remarks"].ToString();
                                            item.Penalty = dr["Penalty"].ToString();
                                            item.DateCreated = datec;
                                            item.PastDue = interest_amount.ToString();
                                            item.MemId = dr["MemId"].ToString();
                                            item.Cno = dr["Cno"].ToString();
                                            item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                            item.NAID = dr["NAID"].ToString();
                                            item.Co_Fname = dr["Co_Fname"].ToString();
                                            item.Co_Mname = dr["Co_Mname"].ToString();
                                            item.Co_Lname = dr["Co_Lname"].ToString();
                                            item.Co_Suffix = dr["Co_Suffix"].ToString();
                                            item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                            item.Co_Cno = dr["Co_Cno"].ToString();
                                            item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                            item.AmountDue = total_bal.ToString();
                                            item.DueDate = dr["DueDate"].ToString();
                                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                            item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                            item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                            item.ReleasingDate = dr["ReleasingDate"].ToString();
                                            item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                            item.CollectedAmount = dr["CollectedAmount"].ToString();
                                            item.LapsePayment = dr["LapsePayment"].ToString();
                                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                            item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                            item.Payment_Status = dr["Payment_Status"].ToString();
                                            item.Collection_Status = Collection_Status;//
                                            item.Collection_Status_Id = Collection_Status_Id;//
                                            item.Payment_Method = dr["Payment_Method"].ToString();
                                            item.AreaName = dr_area["Area"].ToString();
                                            item.AreaID = dr_area["AreaID"].ToString();
                                            item.FieldExpenses = FieldExpenses;
                                            item.Area_RefNo = Area_RefNo;//
                                            item.Collection_RefNo = Collection_RefNo;//
                                            item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                            item.FOID = dr_area["FOID"].ToString();
                                            item.FilePath = dr["FilePath"].ToString();
                                            item.DateCollected = dr["DateCollected"].ToString();
                                            item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                            result.Add(item);
                                        }
                                    }

                                }
                                else
                                {

                                    int day_val = int.Parse(tbl_application_details_2.Rows[0]["Value"].ToString());
                                    DateTime date1 = Convert.ToDateTime(currentDate.ToString());
                                    DateTime date2 = Convert.ToDateTime(tbl_application_details_2.Rows[0]["ReleasingDate"].ToString()).AddDays(day_val);
                                    TimeSpan difference = date2 - date1;
                                    int dayDifference = difference.Days;
                                    if (dayDifference == 0)
                                    {

                                        foreach (DataRow dr in tbl_application_details_2.Rows)
                                        {
                                            double interestrate = double.Parse(dr["InterestRate"].ToString());
                                            double bal = double.Parse(dr["LoanPrincipal"].ToString() == "" ? "0" : dr["LoanPrincipal"].ToString());
                                            double col = 0;
                                            double total_bal = 0;
                                            double pastdueamount = 0;
                                            double interest_amount = 0;
                                            if (Convert.ToDateTime(currentDate.ToString()) > Convert.ToDateTime(dr["DueDate"].ToString()))
                                            {
                                                total_bal = double.Parse(dr["AmountDue"].ToString()) + (double.Parse(dr["AmountDue"].ToString()) * interestrate);
                                                interest_amount = double.Parse(dr["AmountDue"].ToString()) * interestrate;
                                            }
                                            else
                                            {
                                                total_bal = double.Parse(dr["AmountDue"].ToString());
                                                interest_amount = 0;
                                            }
                                            if (bal != 0 && Collection_Status == "Collected")
                                            {
                                                if (double.Parse(dr["AmountDue"].ToString()) != 0)
                                                {
                                                    var item = new CollectionVM();
                                                    item.Fname = dr["Fname"].ToString();
                                                    item.Mname = dr["Mname"].ToString();
                                                    item.Lname = dr["Lname"].ToString();
                                                    item.Suffix = dr["Suffix"].ToString();
                                                    item.Remarks = dr["Remarks"].ToString();
                                                    item.Penalty = dr["Penalty"].ToString();
                                                    item.DateCreated = datec;
                                                    item.PastDue = interest_amount.ToString();
                                                    item.MemId = dr["MemId"].ToString();
                                                    item.Cno = dr["Cno"].ToString();
                                                    item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                                    item.NAID = dr["NAID"].ToString();
                                                    item.Co_Fname = dr["Co_Fname"].ToString();
                                                    item.Co_Mname = dr["Co_Mname"].ToString();
                                                    item.Co_Lname = dr["Co_Lname"].ToString();
                                                    item.Co_Suffix = dr["Co_Suffix"].ToString();
                                                    item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                                                    item.Co_Cno = dr["Co_Cno"].ToString();
                                                    item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                                    item.AmountDue = total_bal.ToString();
                                                    item.DueDate = dr["DueDate"].ToString();
                                                    item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                                    item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                                    item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                                    item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                                    item.ReleasingDate = dr["ReleasingDate"].ToString();
                                                    item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                                    item.CollectedAmount = dr["CollectedAmount"].ToString();
                                                    item.LapsePayment = dr["LapsePayment"].ToString();
                                                    item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                                    item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                                    item.Payment_Status = dr["Payment_Status"].ToString();
                                                    item.Collection_Status = Collection_Status;//
                                                    item.Collection_Status_Id = Collection_Status_Id;//
                                                    item.Payment_Method = dr["Payment_Method"].ToString();
                                                    item.AreaName = dr_area["Area"].ToString();
                                                    item.AreaID = dr_area["AreaID"].ToString();
                                                    item.FieldExpenses = FieldExpenses;
                                                    item.Area_RefNo = Area_RefNo;//
                                                    item.Collection_RefNo = Collection_RefNo;//
                                                    item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                                    item.FOID = dr_area["FOID"].ToString();
                                                    item.FilePath = dr["FilePath"].ToString();
                                                    item.DateCollected = dr["DateCollected"].ToString();
                                                    item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                                    result.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                var item = new CollectionVM();
                                                item.Fname = dr["Fname"].ToString();
                                                item.Mname = dr["Mname"].ToString();
                                                item.Lname = dr["Lname"].ToString();
                                                item.Suffix = dr["Suffix"].ToString();
                                                item.Remarks = dr["Remarks"].ToString();
                                                item.Penalty = dr["Penalty"].ToString();
                                                item.DateCreated = datec;
                                                item.PastDue = interest_amount.ToString();
                                                item.MemId = dr["MemId"].ToString();
                                                item.Cno = dr["Cno"].ToString();
                                                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                                item.NAID = dr["NAID"].ToString();
                                                item.Co_Fname = dr["Co_Fname"].ToString();
                                                item.Co_Mname = dr["Co_Mname"].ToString();
                                                item.Co_Lname = dr["Co_Lname"].ToString();
                                                item.Co_Suffix = dr["Co_Suffix"].ToString();
                                                item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                                                item.Co_Cno = dr["Co_Cno"].ToString();
                                                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                                                item.AmountDue = total_bal.ToString();
                                                item.DueDate = dr["DueDate"].ToString();
                                                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                                                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                                                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                                                item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                                                item.ReleasingDate = dr["ReleasingDate"].ToString();
                                                item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                                                item.CollectedAmount = dr["CollectedAmount"].ToString();
                                                item.LapsePayment = dr["LapsePayment"].ToString();
                                                item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                                                item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                                                item.Payment_Status = dr["Payment_Status"].ToString();
                                                item.Collection_Status = Collection_Status;//
                                                item.Collection_Status_Id = Collection_Status_Id;//
                                                item.Payment_Method = dr["Payment_Method"].ToString();
                                                item.AreaName = dr_area["Area"].ToString();
                                                item.AreaID = dr_area["AreaID"].ToString();
                                                item.FieldExpenses = FieldExpenses;
                                                item.Area_RefNo = Area_RefNo;//
                                                item.Collection_RefNo = Collection_RefNo;//
                                                item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                                                item.FOID = dr_area["FOID"].ToString();
                                                item.FilePath = dr["FilePath"].ToString();
                                                item.DateCollected = dr["DateCollected"].ToString();
                                                item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                                                result.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }






                }
            }

            return result;
        }
   

        public List<CollectionVM> getAreaLoanSummary_3()
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var result = new List<CollectionVM>();
            string datec = "";
            string Collection_Status_Id = "";
            string FieldExpenses = "0";
            string Area_RefNo = "PENDING";
            string Collection_RefNo = "PENDING";
            string Collection_Status = "NO PAYMENT";
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            foreach (DataRow dr_area in area_table.Rows)
            {

                string reference = $@"select
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status ,
                CASE 
                WHEN tbl_CollectionArea_Model.Collection_Status IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.Collection_Status END as Collection_Status_Id,
                CASE 
                WHEN tbl_CollectionArea_Model.Area_RefNo IS NULL THEN 'PENDING'
                ELSE tbl_CollectionArea_Model.Area_RefNo END AS Area_RefNo,
                CASE
                WHEN tbl_CollectionModel.RefNo IS NULL THEN 'PENDING' 
                ELSE  tbl_CollectionModel.RefNo  END as Collection_RefNo,
                CASE 
                WHEN col_stats.[Status] IS NULL THEN 'NO PAYMENT' 
                ELSE col_stats.[Status] END as Collection_Status,tbl_CollectionModel.DateCreated,
				 CASE 
                WHEN tbl_CollectionArea_Model.FieldExpenses IS NULL THEN '0'
                ELSE tbl_CollectionArea_Model.FieldExpenses END as FieldExpenses
                from 
                tbl_CollectionArea_Model left join
                tbl_CollectionModel on tbl_CollectionArea_Model.CollectionRefNo = tbl_CollectionModel.RefNo left join
                tbl_CollectionStatus_Model as col_stats on col_stats.Id = tbl_CollectionArea_Model.Collection_Status 
                where tbl_CollectionArea_Model.AreaID = '" + dr_area["AreaID"].ToString() + "' ";
                DataTable tbl_reference = db.SelectDb(reference).Tables[0];
                if (tbl_reference.Rows.Count != 0)
                {
                    datec = tbl_reference.Rows[0]["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(tbl_reference.Rows[0]["DateCreated"].ToString()).ToString("yyyy-MM-dd");//
                    Collection_Status_Id = "0";
                    Area_RefNo = tbl_reference.Rows[0]["Area_RefNo"].ToString();
                    Collection_RefNo = tbl_reference.Rows[0]["Collection_RefNo"].ToString();
                    Collection_Status = tbl_reference.Rows[0]["Collection_Status"].ToString();
                    FieldExpenses = tbl_reference.Rows[0]["FieldExpenses"].ToString();
                }

                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    string barangay = spliter[0];
                    string city = spliter[1];

                    string sql_applicationdetails = $@"
                
                    select 
                    tbl_Member_Model.Fname,
                    tbl_Member_Model.Mname,
                    tbl_Member_Model.Lname,
                    tbl_Member_Model.Suffix,
                    tbl_Member_Model.Cno,
                    tbl_Member_Model.MemId,
                    tbl_Application_Model.NAID,
                    tbl_CoMaker_Model.Fname as Co_Fname,
                    tbl_CoMaker_Model.Mname as Co_Mname,
                    tbl_CoMaker_Model.lnam  as Co_Lname,
                    tbl_CoMaker_Model.Suffi as Co_Suffix,
                    tbl_CoMaker_Model.Cno as Co_Cno,
                    tbl_Application_Model.Remarks ,

                    tbl_LoanHistory_Model.Penalty,
                    Case when tbl_LoanDetails_Model.ApprovedDailyAmountDue  IS NULL then 0 
                    else tbl_LoanDetails_Model.ApprovedDailyAmountDue end
                    as DailyCollectibles,
                    case when tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0
                    ELSE tbl_LoanHistory_Model.OutstandingBalance end as AmountDue ,
                    CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DueDate END AS DueDate,
                    case when tbl_LoanHistory_Model.DateOfFullPayment  IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS  DateOfFullPayment,
                    CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL THEN 0 
                    ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount,
                    CASE WHEN tbl_LoanDetails_Model.ApprovedAdvancePayment IS NULL THEN 0 
                    ELSE tbl_LoanDetails_Model.ApprovedAdvancePayment  END AS ApprovedAdvancePayment,
                    tbl_LoanDetails_Model.LoanAmount as LoanPrincipal,
                    CASE WHEN tbl_Application_Model.ReleasingDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_Application_Model.ReleasingDate  END AS ReleasingDate, 
                    tbl_TermsTypeOfCollection_Model.TypeOfCollection,
                    CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount,
                    CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment,
                    CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0
                    ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment,
                    tbl_Collection_AreaMember_Model.Payment_Status as Payment_Status_Id,
                        CASE
                            WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING'
                            ELSE tbl_CollectionStatus_Model.[Status]
                        END as Payment_Status,


                CASE 
                WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT'
                WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT'
                ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method,




                CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                ELSE tbl_Collection_AreaMember_Model.DateCollected  END AS DateCollected,

                CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND'
                ELSE file_.FilePath END AS FilePath,

                tbl_LoanDetails_Model.ModeOfRelease,
                tbl_LoanDetails_Model.ModeOfReleaseReference,
                tbl_TermsOfPayment_Model.LoanInsuranceAmount,
                tbl_TermsOfPayment_Model.LifeInsuranceAmount
                from
                tbl_Application_Model left join 
                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_Member_Model on tbl_LoanDetails_Model.MemId  = tbl_Member_Model.MemId left join 
                tbl_LoanHistory_Model on tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId left JOIN 
                tbl_Collection_AreaMember_Model on tbl_Collection_AreaMember_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId left JOIN
                tbl_MemberSavings_Model on tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId left JOIN
                tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId left JOIN
                tbl_TermsTypeOfCollection_Model on tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId left JOIN
                tbl_CollectionStatus_Model on tbl_CollectionStatus_Model.Id = tbl_Collection_AreaMember_Model.Payment_Status left join 
                (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId
                where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "' and tbl_Application_Model.Status = 14" +
                " and tbl_Collection_AreaMember_Model.DateCollected = '" + currentDate + "' ";
                    DataTable tbl_application_details = db.SelectDb(sql_applicationdetails).Tables[0];
                    if (tbl_application_details.Rows.Count != 0)
                    {

                        foreach (DataRow dr in tbl_application_details.Rows)
                        {
                            double bal = double.Parse(dr["LoanPrincipal"].ToString());
                            double col = double.Parse(dr["CollectedAmount"].ToString());
                            double total_bal = Math.Abs(bal - col);
                            var item = new CollectionVM();
                            item.Fname = dr["Fname"].ToString();
                            item.Mname = dr["Mname"].ToString();
                            item.Lname = dr["Lname"].ToString();
                            item.Suffix = dr["Suffix"].ToString();
                            item.Remarks = dr["Remarks"].ToString();
                            item.Penalty = dr["Penalty"].ToString();
                            item.DateCreated = datec;

                            item.MemId = dr["MemId"].ToString();
                            item.Cno = dr["Cno"].ToString();
                            item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                            item.NAID = dr["NAID"].ToString();
                            item.Co_Fname = dr["Co_Fname"].ToString();
                            item.Co_Mname = dr["Co_Mname"].ToString();
                            item.Co_Lname = dr["Co_Lname"].ToString();
                            item.Co_Suffix = dr["Co_Suffix"].ToString();
                            item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                            item.Co_Cno = dr["Co_Cno"].ToString();
                            item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                            item.AmountDue = total_bal.ToString();
                            item.DueDate = dr["DueDate"].ToString();
                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                            item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                            item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                            item.ReleasingDate = dr["ReleasingDate"].ToString();
                            item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                            item.CollectedAmount = dr["CollectedAmount"].ToString();
                            item.LapsePayment = dr["LapsePayment"].ToString();
                            item.AdvancePayment = dr["AdvancePayment"].ToString() == "" ? "0" : dr["AdvancePayment"].ToString();
                            item.Payment_Status_Id = dr["Payment_Status_Id"].ToString();
                            item.Payment_Status = dr["Payment_Status"].ToString();
                            item.Collection_Status = Collection_Status;//
                            item.Collection_Status_Id = Collection_Status_Id;//
                            item.Payment_Method = dr["Payment_Method"].ToString();
                            item.AreaName = dr_area["Area"].ToString();
                            item.AreaID = dr_area["AreaID"].ToString();
                            item.FieldExpenses = FieldExpenses;
                            item.Area_RefNo = Area_RefNo;//
                            item.Collection_RefNo = Collection_RefNo;//
                            item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                            item.FOID = dr_area["FOID"].ToString();
                            item.FilePath = dr["FilePath"].ToString();
                            item.DateCollected = dr["DateCollected"].ToString();
                            item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                            result.Add(item);
                        }
                    }
                    else
                    {

                        string sql_applicationdetails2 = $@"
                
                     select 
                    tbl_Member_Model.Fname,
                    tbl_Member_Model.Mname,
                    tbl_Member_Model.Lname,
                    tbl_Member_Model.Suffix,
                    tbl_Member_Model.Cno,
                    tbl_Member_Model.MemId,
                    tbl_Application_Model.NAID,
                    tbl_CoMaker_Model.Fname as Co_Fname,
                    tbl_CoMaker_Model.Mname as Co_Mname,
                    tbl_CoMaker_Model.lnam  as Co_Lname,
                    tbl_CoMaker_Model.Suffi as Co_Suffix,
                    tbl_CoMaker_Model.Cno as Co_Cno,
                    tbl_Application_Model.Remarks ,

                    tbl_LoanHistory_Model.Penalty,
                    Case when tbl_LoanDetails_Model.ApprovedDailyAmountDue  IS NULL then 0 
                    else tbl_LoanDetails_Model.ApprovedDailyAmountDue end
                    as DailyCollectibles,
                    case when tbl_LoanHistory_Model.OutstandingBalance IS NULL THEN 0
                    ELSE tbl_LoanHistory_Model.OutstandingBalance end as AmountDue ,
                    CASE WHEN tbl_LoanHistory_Model.DueDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DueDate END AS DueDate,
                    case when tbl_LoanHistory_Model.DateOfFullPayment  IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_LoanHistory_Model.DateOfFullPayment END AS  DateOfFullPayment,
                    CASE WHEN tbl_MemberSavings_Model.TotalSavingsAmount IS NULL THEN 0 
                    ELSE tbl_MemberSavings_Model.TotalSavingsAmount END AS TotalSavingsAmount,
                    CASE WHEN tbl_LoanDetails_Model.ApprovedAdvancePayment IS NULL THEN 0 
                    ELSE tbl_LoanDetails_Model.ApprovedAdvancePayment  END AS ApprovedAdvancePayment,
                    tbl_LoanDetails_Model.LoanAmount as LoanPrincipal,
                    CASE WHEN tbl_Application_Model.ReleasingDate IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
                    ELSE tbl_Application_Model.ReleasingDate  END AS ReleasingDate, 
                    tbl_TermsTypeOfCollection_Model.TypeOfCollection,
                ---    CASE WHEN tbl_Collection_AreaMember_Model.CollectedAmount IS NULL THEN 0
             ----       ELSE tbl_Collection_AreaMember_Model.CollectedAmount END AS CollectedAmount,
            -- --       CASE WHEN tbl_Collection_AreaMember_Model.LapsePayment IS NULL THEN 0
            --        ELSE tbl_Collection_AreaMember_Model.LapsePayment END AS LapsePayment,
            --        CASE WHEN tbl_Collection_AreaMember_Model.AdvancePayment IS NULL THEN 0
            --        ELSE tbl_Collection_AreaMember_Model.AdvancePayment END AS AdvancePayment,
             --       tbl_Collection_AreaMember_Model.Payment_Status as Payment_Status_Id,
           ---             CASE
            --                WHEN tbl_CollectionStatus_Model.[Status] IS NULL THEN 'PENDING'
             --               ELSE tbl_CollectionStatus_Model.[Status]
                 --       END as Payment_Status,


            --    CASE 
             --   WHEN tbl_Collection_AreaMember_Model.Payment_Method IS NULL THEN 'NO PAYMENT'
             --   WHEN tbl_Collection_AreaMember_Model.Payment_Method = '' THEN 'NO PAYMENT'
           --     ELSE tbl_Collection_AreaMember_Model.Payment_Method END AS Payment_Method,




              --  CASE WHEN tbl_Collection_AreaMember_Model.DateCollected IS NULL THEN CONVERT(DATETIME, '2000-01-01 00:00:00.000') 
            --    ELSE tbl_Collection_AreaMember_Model.DateCollected  END AS DateCollected,

                CASE WHEN file_.FilePath IS NULL THEN 'NO FILE FOUND'
                ELSE file_.FilePath END AS FilePath,

                tbl_LoanDetails_Model.ModeOfRelease,
                tbl_LoanDetails_Model.ModeOfReleaseReference,
                tbl_TermsOfPayment_Model.LoanInsuranceAmount,
                tbl_TermsOfPayment_Model.LifeInsuranceAmount
                from
                tbl_Application_Model left join 
                tbl_LoanDetails_Model on tbl_LoanDetails_Model.NAID = tbl_Application_Model.NAID left join 
                tbl_Member_Model on tbl_LoanDetails_Model.MemId  = tbl_Member_Model.MemId left join 
                tbl_LoanHistory_Model on tbl_LoanDetails_Model.MemId = tbl_LoanHistory_Model.MemId left JOIN 
                tbl_CoMaker_Model on tbl_CoMaker_Model.MemId = tbl_Member_Model.MemId left JOIN
                tbl_MemberSavings_Model on tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId left JOIN
                tbl_TermsOfPayment_Model on tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId left JOIN
                tbl_TermsTypeOfCollection_Model on tbl_TermsTypeOfCollection_Model.Id = tbl_TermsOfPayment_Model.CollectionTypeId left JOIN
                (select  FilePath,MemId from tbl_fileupload_Model where tbl_fileupload_Model.[Type] = 1)  as file_ on file_.MemId = tbl_Member_Model.MemId
                where tbl_Member_Model.Barangay = '" + barangay.Trim() + "' and tbl_Member_Model.City = '" + city.Trim() + "' and tbl_Application_Model.Status = 14 " +
                "and tbl_Application_Model.ReleasingDate = '" + currentDate + "'";
                        DataTable tbl_application_details2 = db.SelectDb(sql_applicationdetails2).Tables[0];
                        foreach (DataRow dr in tbl_application_details2.Rows)
                        {
                            double bal = double.Parse(dr["LoanPrincipal"].ToString());
                            double col = 0;
                            double total_bal = Math.Abs(bal - col);
                            var item = new CollectionVM();
                            item.Fname = dr["Fname"].ToString();
                            item.Mname = dr["Mname"].ToString();
                            item.Lname = dr["Lname"].ToString();
                            item.Suffix = dr["Suffix"].ToString();
                            item.Remarks = dr["Remarks"].ToString();
                            item.Penalty = dr["Penalty"].ToString();
                            item.DateCreated = datec;

                            item.MemId = dr["MemId"].ToString();
                            item.Cno = dr["Cno"].ToString();
                            item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                            item.NAID = dr["NAID"].ToString();
                            item.Co_Fname = dr["Co_Fname"].ToString();
                            item.Co_Mname = dr["Co_Mname"].ToString();
                            item.Co_Lname = dr["Co_Lname"].ToString();
                            item.Co_Suffix = dr["Co_Suffix"].ToString();
                            item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                            item.Co_Cno = dr["Co_Cno"].ToString();
                            item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                            item.AmountDue = total_bal.ToString();
                            item.DueDate = dr["DueDate"].ToString();
                            item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                            item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                            item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                            item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                            item.ReleasingDate = dr["ReleasingDate"].ToString();
                            item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                            item.CollectedAmount = "0";
                            item.LapsePayment = "0";
                            item.AdvancePayment = "0";
                            item.Payment_Status_Id = "0";
                            item.Payment_Status = "PENDING";
                            item.Collection_Status = "NO PAYMENT";
                            item.Collection_Status_Id = "0";//
                            item.Payment_Method = "NO PAYMENT";
                            item.AreaName = dr_area["Area"].ToString();
                            item.AreaID = dr_area["AreaID"].ToString();
                            item.FieldExpenses = FieldExpenses;
                            item.Area_RefNo = "PENDING"; ;//
                            item.Collection_RefNo = "PENDING"; ;//
                            item.FieldOfficer = dr_area["Fname"].ToString() + " " + dr_area["Mname"].ToString() + " " + dr_area["Lname"].ToString() + " " + dr_area["Suffix"].ToString();
                            item.FOID = dr_area["FOID"].ToString();
                            item.FilePath = dr["FilePath"].ToString();
                            item.DateCollected = "";
                            item.LoanInsurance = dr["LoanInsuranceAmount"].ToString();

                            result.Add(item);
                        }
                    }
                }

            }

            return result;
        }
        public List<CollectionVM> getAreaLoanSummary2()
        {


            var result = new List<CollectionVM>();


            DataTable table = db.SelectDb_SP("sp_collection_arealistnull").Tables[0];
            foreach (DataRow dr in table.Rows)
            {

                //DateTime dateToCompare = DateTime.Parse(dr["DateCreated"].ToString()); // Replace with your specific date
                //DateTime currentDate = DateTime.Now;
                //var datec = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                var item = new CollectionVM();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Remarks = dr["Remarks"].ToString();
                item.Penalty = dr["Penalty"].ToString();
                //item.DateCreated = datec;

                item.MemId = dr["MemId"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.NAID = dr["NAID"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Co_Lname"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Borrower = dr["Co_Fname"].ToString() + " " + dr["Co_Mname"].ToString() + " " + dr["Co_Lname"].ToString() + " " + dr["Co_Suffix"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.DailyCollectibles = dr["DailyCollectibles"].ToString();
                item.AmountDue = dr["AmountDue"].ToString();
                item.DueDate = dr["DueDate"].ToString();
                item.DateOfFullPayment = dr["DateOfFullPayment"].ToString();
                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                item.ApprovedAdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                item.LoanPrincipal = dr["LoanPrincipal"].ToString();
                item.ReleasingDate = dr["ReleasingDate"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();
                item.CollectedAmount = "0";
                item.LapsePayment = "0";
                item.AdvancePayment = "0";
                item.Payment_Status_Id = "0";
                item.Payment_Status = "PENDING";
                item.Collection_Status = "PENDING";
                item.Collection_Status_Id = "0";
                item.Payment_Method = "NO PAYMENT";
                item.AreaName = dr["AreaName"].ToString();
                item.AreaID = dr["AreaID"].ToString();
                item.Area_RefNo = "PENDING";
                item.Collection_RefNo = "PENDING";
                item.FieldOfficer = dr["FO_Fname"].ToString() + " " + dr["FO_Mname"].ToString() + " " + dr["FO_Lname"].ToString() + " " + dr["FO_Suffix"].ToString();
                item.FOID = dr["FOID"].ToString();
                item.FilePath = dr["FilePath"].ToString();
                //item.DateCollected = dr["DateCollected"].ToString();
                result.Add(item);

            }


            return result;

        }
        public List<LoanSummaryVM> GetCollectionListbyArea(string AreaID)
        {

            var result = new List<LoanSummaryVM>();
            var param = new IDataParameter[]
            {
            new SqlParameter("@AreaID", AreaID)
            };

            DataTable table = db.SelectDb_SP("sp_CollectionDetails", param).Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                string CI_ApprovalDate = dr["CI_ApprovalDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["CI_ApprovalDate"].ToString()).ToString("yyyy-MM-dd");
                string DeclineDate = dr["DeclineDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["DeclineDate"].ToString()).ToString("yyyy-MM-dd");
                string App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_1"].ToString()).ToString("yyyy-MM-dd");
                string App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_ApprovalDate_2"].ToString()).ToString("yyyy-MM-dd");
                string App_NotedDate = dr["App_NotedDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["App_NotedDate"].ToString()).ToString("yyyy-MM-dd");
                string DateCreated = dr["DateCreated"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("yyyy-MM-dd");
                string DateSubmitted = dr["DateSubmitted"].ToString() == "" ? "" : Convert.ToDateTime(dr["DateSubmitted"].ToString()).ToString("yyyy-MM-dd");
                string ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd");

                var item = new LoanSummaryVM();
                item.Fname = dr["Fname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Suffix = dr["Suffix"].ToString();
                item.Cno = dr["Cno"].ToString();
                item.Borrower = dr["Fname"].ToString() + " " + dr["Mname"].ToString() + " " + dr["Lname"].ToString() + " " + dr["Suffix"].ToString();
                item.AreaName = dr["AreaName"].ToString();
                item.AreaID = dr["AreaID"].ToString();
                item.City = dr["City"].ToString();
                //computation of duedate
                string approved_date = "";
                if (App_ApprovalDate_1 == "")
                {
                    approved_date = App_ApprovalDate_2;
                }
                else
                {
                    approved_date = App_ApprovalDate_1;
                }

                //    dr["InterestRate"].ToString(), );
                var computation_res = GetMultipleStrings(approved_date, dr["Days"].ToString(), dr["MemId"].ToString(), dr["InterestRate"].ToString(), dr["LoanAmount"].ToString()
                                    , dr["Loan_amount_GreaterEqual_Value"].ToString(), dr["Loan_amount_GreaterEqual"].ToString(), dr["LAGEF_Type"].ToString(), dr["Loan_amount_Lessthan_Value"].ToString()
                                    , dr["Loan_amount_Lessthan"].ToString(), dr["LALV_Type"].ToString(), dr["LoanInsurance"].ToString(), dr["LoanI_Type"].ToString(), dr["Formula"].ToString(), dr["APFID"].ToString()
                                    , dr["InterestType"].ToString(), dr["OldFormula"].ToString(), dr["NoAdvancePayment"].ToString(), dr["InterestApplied"].ToString(), dr["DeductInterest"].ToString(), dr["TypeOfCollection"].ToString()
                                    , dr["NotarialFeeOrigin"].ToString(), dr["LifeInsurance"].ToString(), dr["Savings"].ToString(), dr["Value"].ToString());
                //------------------------------------------------->
                item.Date = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                item.DueDate = Convert.ToDateTime(computation_res.add_dueday).AddDays(int.Parse(computation_res.total_days)).ToString("yyyy-MM-dd");
                item.NAID = dr["NAID"].ToString();
                item.MemId = dr["MemId"].ToString();
                item.Days = dr["Days"].ToString();
                item.PrincipalLoan = dr["LoanAmount"].ToString();
                item.LoanAmount = String.Format("{0:0.00}", computation_res.total_loan_amount);
                item.Total_InterestAmount = String.Format("{0:0.00}", computation_res.total);
                item.NotarialFee = String.Format("{0:0.00}", computation_res.total_notarial);
                item.InterestRate = dr["InterestRate"].ToString();
                item.Savings = dr["Savings"].ToString();
                item.TotalSavingsAmount = dr["TotalSavingsAmount"].ToString();
                //item.InterestType = dr["InterestType"].ToString();
                item.LoanInsurance = String.Format("{0:0.00}", computation_res.total_loaninsurance);
                //item.LoanInsurance =  dr["LoanInsurance"].ToString();
                item.LoanI_Type = dr["LoanI_Type"].ToString();
                item.Total_LoanReceivable = String.Format("{0:0.00}", computation_res.loanreceivable);
                item.DailyCollectibles = computation_res.daily_collectibles;
                item.AdvancePayment = computation_res.AdvancePayment;
                item.LifeInsurance = computation_res.LifeInsurance;
                //item.LifeInsuranceType = dr["LifeInsuranceType"].ToString();
                item.CI_ApprovedBy = dr["CI_ApprovedBy"].ToString();
                item.CI_ApprovedBy_UserId = dr["CI_ApprovedBy_UserId"].ToString();
                item.CI_ApprovalDate = dr["CI_ApprovalDate"].ToString();
                item.DeclinedBy = dr["DeclinedBy"].ToString();
                item.DeclinedBy_UserId = dr["DeclinedBy_UserId"].ToString();
                item.DeclineDate = dr["DeclineDate"].ToString();
                item.App_ApprovedBy_1 = dr["App_ApprovedBy_1"].ToString();
                item.App_ApprovedBy_1_UserId = dr["App_ApprovedBy_1_UserId"].ToString();
                item.App_ApprovalDate_1 = dr["App_ApprovalDate_1"].ToString();
                item.App_ApprovedBy_2 = dr["App_ApprovedBy_2"].ToString();
                item.App_ApprovedBy_2_UserId = dr["App_ApprovedBy_2_UserId"].ToString();
                item.App_ApprovalDate_2 = dr["App_ApprovalDate_2"].ToString();
                item.App_Notedby = dr["App_Notedby"].ToString();
                item.App_Notedby_UserId = dr["App_Notedby_UserId"].ToString();
                item.App_NotedDate = dr["App_NotedDate"].ToString();
                item.CreatedBy = dr["CreatedBy"].ToString();
                item.CreatedBy_UserId = dr["CreatedBy_UserId"].ToString();
                item.DateCreated = dr["DateCreated"].ToString();
                item.SubmittedBy = dr["SubmittedBy"].ToString();
                item.SubmittedBy_UserId = dr["SubmittedBy_UserId"].ToString();
                item.DateSubmitted = dr["DateSubmitted"].ToString();
                item.ReleasedBy = dr["ReleasedBy"].ToString();
                item.ReleasedBy_UserId = dr["ReleasedBy_UserId"].ToString();
                item.ReleasingDate = dr["ReleasingDate"].ToString();
                item.APFID = dr["APFID"].ToString();
                item.Co_Fname = dr["Co_Fname"].ToString();
                item.Co_Mname = dr["Co_Mname"].ToString();
                item.Co_Lname = dr["Co_Lname"].ToString();
                item.Co_Suffix = dr["Co_Suffix"].ToString();
                item.Co_Cno = dr["Co_Cno"].ToString();
                item.NoAdvancePayment = dr["NoAdvancePayment"].ToString();
                item.TypeOfCollection = dr["TypeOfCollection"].ToString();

                result.Add(item);

            }


            return result;
        }
        static double EvaluateExpression(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return (double)row["expression"];
        }

        public List<AreaDetailsVM> GetAreasCollection()
        {



            var areas = getAreaLoanSummary().GroupBy(a => new { a.AreaID, a.AreaName }).ToList();
            var list = getAreaLoanSummary().ToList();
            var res = new List<AreaDetailsVM>();

            foreach (var group in areas)
            {
                var dailyCollectiblesSum = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.DailyCollectibles)).Sum();
                var advance_payment = getAreaLoanSummary().Where(a => a.AreaID == group.Key.AreaID).Select(a => double.Parse(a.AdvancePayment)).Sum();

                var items = new AreaDetailsVM();
                items.AreaName = group.Key.AreaName;
                items.AreaID = group.Key.AreaID;
                items.ExpectedCollection = Math.Ceiling(double.Parse(dailyCollectiblesSum.ToString()));
                items.AdvancePayment = advance_payment;
                res.Add(items);
            }

            return res;
        }
        public List<Reports_Release> GetReport_ReleaseList()
        {
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            var result = new List<Reports_Release>();
            foreach (DataRow dr_area in area_table.Rows)
            {
                string barangay = "";
                string city = "";
                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    barangay = spliter[0];
                    city = spliter[1];

                    string applicationfilter = $@"SELECT        tbl_Status_Model.Name AS ApplicationStatus, tbl_Application_Model.NAID, tbl_Status_Model.Id AS ApplicationStatusId, tbl_Member_Model.Fname, tbl_Member_Model.Lname, tbl_Member_Model.Mname, 
                         tbl_Member_Model.Suffix, tbl_CoMaker_Model.Fname AS Co_Fname, tbl_CoMaker_Model.Mname AS Co_Mname, tbl_CoMaker_Model.Suffi AS Co_Suffix, tbl_CoMaker_Model.Lnam AS Co_Lname, 
                         tbl_LoanDetails_Model.LoanAmount, tbl_LoanType_Model.LoanTypeName, tbl_LoanDetails_Model.ApprovedAdvancePayment, tbl_Member_Model.MemId, tbl_Member_Model.Barangay, tbl_Member_Model.City, 
                         tbl_Application_Model.ReleasingDate, tbl_LoanHistory_Model.DueDate, tbl_TermsOfPayment_Model.NameOfTerms
                         FROM            tbl_Status_Model INNER JOIN
                         tbl_Application_Model ON tbl_Status_Model.Id = tbl_Application_Model.Status INNER JOIN
                         tbl_Member_Model ON tbl_Application_Model.MemId = tbl_Member_Model.MemId INNER JOIN
                         tbl_CoMaker_Model ON tbl_Member_Model.MemId = tbl_CoMaker_Model.MemId INNER JOIN
                         tbl_LoanDetails_Model ON tbl_Application_Model.NAID = tbl_LoanDetails_Model.NAID INNER JOIN
                         tbl_LoanType_Model ON tbl_LoanDetails_Model.LoanTypeID = tbl_LoanType_Model.LoanTypeID INNER JOIN
                         tbl_LoanHistory_Model ON tbl_Application_Model.MemId = tbl_LoanHistory_Model.MemId INNER JOIN
                         tbl_TermsOfPayment_Model ON tbl_LoanDetails_Model.TermsOfPayment = tbl_TermsOfPayment_Model.TopId
                        WHERE        (tbl_Application_Model.Status = 14) and tbl_Member_Model.Barangay='" + barangay.Trim() + "' and tbl_Member_Model.City ='" + city.Trim() + "'";
                    DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];

                    foreach (DataRow dr in tbl_applicationfilter.Rows)
                    {
                        var item = new Reports_Release();
                        item.DueDate = dr["DueDate"].ToString();
                        item.TermofPayment = dr["NameOfTerms"].ToString();
                        item.NAID = dr["NAID"].ToString();
                        item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Suffix"].ToString();
                        item.Co_Borrower = dr["Co_Lname"].ToString() + ", " + dr["Co_Fname"].ToString() + ", " + dr["Co_Mname"].ToString() + ", " + dr["Co_Suffix"].ToString();
                        item.Area = dr_area["Area"].ToString();
                        item.Status = dr["ApplicationStatus"].ToString();
                        item.LoanType = dr["LoanTypeName"].ToString();
                        item.LoanAmount = dr["LoanAmount"].ToString();
                        item.AdvancePayment = dr["ApprovedAdvancePayment"].ToString();
                        item.ReleasingDate = dr["ReleasingDate"].ToString() == "" ? "" : Convert.ToDateTime(dr["ReleasingDate"].ToString()).ToString("yyyy-MM-dd");
                        item.MemId = dr["MemId"].ToString();

                        result.Add(item);
                    }
                }

            }

            return result;

        }

        public List<Reports_Collection> GetReport_CollectionList()
        {
            var areas = Collection_PrintedResult().GroupBy(a => new { a.AreaID, a.AreaName, a.FieldOfficer, a.DateCollected }).ToList();
            var list = Collection_PrintedResult().ToList();
            bool containsNow = list.Any(dt => dt.ToString() == DateTime.Parse(Convert.ToDateTime(list[0].DateCreated).ToString("yyyy-MM-dd")).ToString());
            var res = new List<Reports_Collection>();
            foreach (var group in areas)
            {

                var advance_payment = list.Where(a => a.AreaID == group.Key.AreaID && a.AdvancePayment != null && a.AdvancePayment != "").Select(a => double.Parse(a.AdvancePayment)).Sum();

                var dailyCollectiblesSum = list.Where(a => a.AreaID == group.Key.AreaID && a.ApprovedDailyAmountDue != "").Select(a => double.Parse(a.ApprovedDailyAmountDue)).Sum();
                var savings = list.Where(a => a.AreaID == group.Key.AreaID && a.Savings != "").Select(a => double.Parse(a.Savings)).Sum();
                var balance = list.Where(a => a.AreaID == group.Key.AreaID && a.OutstandingBalance != "").Select(a => double.Parse(a.OutstandingBalance)).Sum();
                var advance = list.Where(a => a.AreaID == group.Key.AreaID && a.ApprovedAdvancePayment != "").Select(a => double.Parse(a.ApprovedAdvancePayment)).Sum();
                var lapses = list.Where(a => a.AreaID == group.Key.AreaID && a.LapsePayment != "").Select(a => double.Parse(a.LapsePayment)).Sum();
                var collectedamount = list.Where(a => a.AreaID == group.Key.AreaID && a.CollectedAmount != "").Select(a => double.Parse(a.CollectedAmount)).Sum();
                var fieldexpenses = list.Where(a => a.AreaID == group.Key.AreaID && a.FieldExpenses != "").Select(a => double.Parse(a.FieldExpenses)).Sum();
                var totalnp = list.Where(a => a.AreaID == group.Key.AreaID && a.Collection_Status == "NO PAYMENT").ToList();
                var items = new Reports_Collection();
                items.AreaName = group.Key.AreaName;
                items.FieldOfficer = group.Key.FieldOfficer;
                items.DateCollected = group.Key.DateCollected;
                items.TotalCollection = Math.Ceiling(dailyCollectiblesSum).ToString();
                items.TotalSavings = Math.Ceiling(double.Parse(savings.ToString())).ToString();
                items.TotalLapses = Math.Ceiling(lapses).ToString();
                items.CashRemit = Math.Ceiling(collectedamount).ToString();
                items.TotalNP = totalnp.Count.ToString();
                items.TotalAdvance = Math.Ceiling(double.Parse(advance.ToString())).ToString();
                res.Add(items);
            }


            return res;

        }
        public List<Reports_Savings> GetReport_MemberSavingsList()
        {
            string areafilter = $@"SELECT   tbl_Area_Model.Id, tbl_Area_Model.Area, tbl_Area_Model.City, tbl_Area_Model.FOID, tbl_Area_Model.Status, tbl_Area_Model.DateCreated, tbl_Area_Model.DateUpdated, tbl_Area_Model.AreaID, 
                         tbl_FieldOfficer_Model.Fname, tbl_FieldOfficer_Model.Mname, tbl_FieldOfficer_Model.Lname, tbl_FieldOfficer_Model.Suffix
FROM            tbl_Area_Model INNER JOIN
                         tbl_FieldOfficer_Model ON tbl_Area_Model.FOID = tbl_FieldOfficer_Model.FOID";
            DataTable area_table = db.SelectDb(areafilter).Tables[0];
            var result = new List<Reports_Savings>();
            foreach (DataRow dr_area in area_table.Rows)
            {
                string barangay = "";
                string city = "";
                var area_city = dr_area["City"].ToString().ToLower().Split("|").ToList();
                for (int x = 0; x < area_city.Count; x++)
                {
                    var spliter = area_city[x].Split(",");
                    barangay = spliter[0];
                    city = spliter[1];

                    string applicationfilter = $@"SELECT        
                    tbl_Member_Model.Fname, 
                    tbl_Member_Model.Lname, 
                    tbl_Member_Model.Mname, 
                    tbl_Member_Model.Suffix, 
                    tbl_Member_Model.Barangay, 
                    tbl_Member_Model.City, 
                    tbl_MemberSavings_Model.TotalSavingsAmount,
                    sum(tbl_MemberSavings_Model.TotalSavingsAmount) as TotalCollection
                    FROM            
                    tbl_Member_Model INNER JOIN
                    tbl_MemberSavings_Model ON tbl_Member_Model.MemId = tbl_MemberSavings_Model.MemId
                    WHERE   tbl_Member_Model.Barangay='" + barangay.Trim() + "' and tbl_Member_Model.City ='" + city.Trim() + "' group by " +
                  "  tbl_Member_Model.Fname, " +
                   "  tbl_Member_Model.Lname, " +
                    " tbl_Member_Model.Mname, " +
                    " tbl_Member_Model.Suffix, " +
                    " tbl_Member_Model.Barangay, " +
                    " tbl_Member_Model.City, " +
                    " tbl_MemberSavings_Model.TotalSavingsAmount";
                    DataTable tbl_applicationfilter = db.SelectDb(applicationfilter).Tables[0];

                    foreach (DataRow dr in tbl_applicationfilter.Rows)
                    {
                        var item = new Reports_Savings();
                        item.Borrower = dr["Lname"].ToString() + ", " + dr["Fname"].ToString() + ", " + dr["Mname"].ToString() + ", " + dr["Suffix"].ToString();
                        item.AreaName = dr_area["Area"].ToString();
                        item.TotalSavings = dr["TotalSavingsAmount"].ToString();
                        item.TotalCollection = dr["TotalCollection"].ToString();


                        result.Add(item);
                    }
                }

            }

            return result;

        }
        public List<Reports_PastDue> GetReport_MemberPastDueList()
        {

            var result = new List<Reports_PastDue>();

            var item = new Reports_PastDue();
            item.Borrower = "Mario Sakay";
            item.LoanAmount = "700.00";
            item.DateReleased = "2023-12-04";
            item.DueDate = "2024-02-04";
            item.TotalNP = "3";
            item.TotalPastDueDay = "4";
            item.TotalCollection = "700.00";


            return result;
        }

    }
}

