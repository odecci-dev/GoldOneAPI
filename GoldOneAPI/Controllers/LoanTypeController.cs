using AuthSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using static GoldOneAPI.Controllers.UserRegistrationController;
using System.Text;
using static GoldOneAPI.Controllers.MemberController;
using static GoldOneAPI.Controllers.GroupController;
using static GoldOneAPI.Controllers.HolidayController;
using Microsoft.VisualBasic;
using GoldOneAPI.Manager;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GoldOneAPI.Controllers
{
    [Authorize("ApiKey")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoanTypeController : ControllerBase
    {
        string sql_ = "";
        string sql = "";
        string results = "";
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        private readonly AppSettings _appSettings;
        private readonly JwtAuthenticationManager jwtAuthenticationManager;
        private readonly IWebHostEnvironment _environment;

        public LoanTypeController(IOptions<AppSettings> appSettings, IWebHostEnvironment environment)
        {

            _appSettings = appSettings.Value;

        }
        #region Model
        public class FormulaVM
        {
            public string  Formula { get; set; }
            public string  FormulaID { get; set; }
        }
        public class CollectionTypeVM
        {
            public string Id { get; set; }
            public string TypeOfCollection { get; set; }
        }
        public class LoanHistoryVM
        {
            public string LoanAmount { get; set; }
            public string Savings { get; set; }
            public string Penalty { get; set; }
            public string OutStandingBalance { get; set; }
            public string DateReleased { get; set; }
            public string DueDate { get; set; }
            public string DateCreated { get; set; }
            public string DateOfFullPayment { get; set; }
            public string MemId { get; set; }
            public string NoPayment { get; set; }
            public string RefNo { get; set; }
            public string Fname { get; set; }
            public string Mname { get; set; }
            public string Lname { get; set; }
            public string Suffix { get; set; }
            public string Status { get; set; }
        }
        public class TermsVM
        {
            public string TermsofPayment { get; set; }
            public string TopId { get; set; }
            public string LoanTypeId { get; set; }

        }
        public class LoanTypeDetailsVM
        {
            public string LoanTypeName { get; set; }

            public string Loan_amount_Lessthan_Amount { get; set; }

            public string Loan_amount_GreaterEqual_Amount { get; set; }

       


            public string Loan_amount_GreaterEqual { get; set; }
            public string LAG_Type { get; set; }
            public string LAG_Id { get; set; }


            public string LoanInsurance { get; set; }
            public string LoanI_Type { get; set; }
            public string LoanI_Id { get; set; }

            public string LifeInsurance { get; set; }
            public string LifeI_Type { get; set; }
            public string LifeI_Id { get; set; }


            public string Loan_amount_Lessthan { get; set; }
            public string LAL_Type { get; set; }
            public string LAL_Id { get; set; }

            public string Savings { get; set; }
            public string LoanAmount_Min { get; set; }
            public string LoanAmount_Max { get; set; }
            public string LoanTypeID { get; set; }
            public string DateCreated { get; set; }
            public string DateUpdated { get; set; }
            public string Status { get; set; }

            public List<TermOfPaymentVM>? TermsofPayment { get; set; }
        }
        public class TermOfPaymentVM
        {
            public string NameOfTerms { get; set; }
            public string LoanTypeId { get; set; }
            public string TypeOfCollection { get; set; }
            public string TopId { get; set; }
            public string Terms { get; set; }
            public string InterestRate { get; set; }
            public string IR_Type { get; set; }
            public string Formula { get; set; }
            public string FormulaID { get; set; }
            public string NoAdvancePayment { get; set; }
            public string NotarialFeeOrigin { get; set; }
            public string LessThanNotarialAmount { get; set; }
            public string LALV_Type { get; set; }
            public string LALV_TypeID { get; set; }
            public string GreaterThanEqualNotarialAmount { get; set; }
            public string LAGEF_Type { get; set; }
            public string LAGEF_TypeID { get; set; }
            public string LoanInsuranceAmount { get; set; }
            public string LoanI_Type { get; set; }
            public string LoanI_TypeID { get; set; }
            public string LifeInsuranceAmount { get; set; }
            public string LifeI_Type { get; set; }
            public string LifeI_TypeID { get; set; }
            public string DeductInterest { get; set; }
            public string Days { get; set; }
            public string Status { get; set; }
            public string LoanTypeName { get; set; }
            public string? OldFormula { get; set; }
            public string? InterestApplied { get; set; }
            public string TypeOfCollectionID { get; set; }
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> GetLoanFormula()
        {
            var result = (dynamic)null;
            try
            {
                //result = dbmet.GetLoanFormulaList().ToList();

                int arrayToRemoveIndex = 1;

                 result = dbmet.GetLoanFormulaList().Where((arr, index) => index != arrayToRemoveIndex).ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        } 
        [HttpGet]
        public async Task<IActionResult> GetCollectionType()
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetTypeOfCollection().ToList();

                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveLoanType(LoanTypeModel data)
        {
            string result = "";
            try
            {
                string Insert = "";
                string Insert1 = "";

                string filePath = @"C:\data\SAVELOANTYPE.json"; // Replace with your desired file path

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));

                sql = $@"select * from tbl_LoanType_Model where LoanTypeName ='" + data.LoanTypeName + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count == 0)
                {
                    Insert = $@"INSERT INTO [dbo].[tbl_LoanType_Model]
                           ([Savings]
                           ,[LoanAmount_Min]
                           ,[LoanAmount_Max]
                           ,[Status]
                           ,[LoanTypeName]
                           ,[DateCreated])
                     VALUES
                          ('" + data.Savings + "'," +
                           "'" + data.LoanAmount_Min + "'," +
                           "'" + data.LoanAmount_Max + "'," +
                           "'1'," +
                           "'" + data.LoanTypeName + "'," +
                           "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    result = db.AUIDB_WithParam(Insert) + " Added";

                    sql = $@"select Top(1) LoanTypeID from tbl_LoanType_Model order by id desc";
                


                    DataTable table = db.SelectDb(sql).Tables[0];
                    var loantypeid = table.Rows.Count== 0 ? "LT-01" : table.Rows[0]["LoanTypeID"].ToString();

                   

                    if (data.Terms.Count != 0)
                    {
                       

                        for (int i = 0; i < data.Terms.Count; i++)
                        {
                            double interest_rate = data.Terms[i].InterestType == "1" ? double.Parse(data.Terms[i].InterestRate.ToString()) / 100 : double.Parse(data.Terms[i].InterestRate.ToString());
                            double LessThanNotarialAmount = data.Terms[i].LessThanAmountTYpe == 1 ? double.Parse(data.Terms[i].LessThanNotarialAmount.ToString()) / 100 : double.Parse(data.Terms[i].LessThanNotarialAmount.ToString());
                            double GreaterThanEqualNotarialAmount = data.Terms[i].GreaterThanEqualAmountType == 1 ? double.Parse(data.Terms[i].GreaterThanEqualNotarialAmount.ToString()) / 100 : double.Parse(data.Terms[i].GreaterThanEqualNotarialAmount.ToString());
                            double LifeInsuranceAmount = data.Terms[i].LifeInsuranceAmountType == 1 ? double.Parse(data.Terms[i].LifeInsuranceAmount.ToString()) / 100 : double.Parse(data.Terms[i].LifeInsuranceAmount.ToString());
                            double LoanInsuranceAmount = data.Terms[i].LoanInsuranceAmountType == 1 ? double.Parse(data.Terms[i].LoanInsuranceAmount.ToString()) / 100 : double.Parse(data.Terms[i].LoanInsuranceAmount.ToString());
                            Insert1 = $@"INSERT INTO [dbo].[tbl_TermsOfPayment_Model]
                                       ([NameOfTerms]
                                       ,[InterestRate]
                                       ,[InterestType]
                                       ,[Status]
                                       ,[LoanTypeId]
                                       ,[Formula]
                                       ,[InterestApplied]
                                       ,[Terms]
                                       ,[OldFormula]
                                       ,[NoAdvancePayment]
                                       ,[NotarialFeeOrigin]
                                       ,[LessThanNotarialAmount]
                                       ,[LessThanAmountTYpe]
                                       ,[GreaterThanEqualNotarialAmount]
                                       ,[GreaterThanEqualAmountType]
                                       ,[LoanInsuranceAmount]
                                       ,[LoanInsuranceAmountType]
                                       ,[LifeInsuranceAmount]
                                       ,[LifeInsuranceAmountType]
                                       ,[DeductInterest]
                                       ,[CollectionTypeId]
                                       ,[DateCreated])
                                 VALUES
                                    ('" + data.Terms[i].NameOfTerms + "'," +
                                    "'" + double.Parse(data.Terms[i].InterestRate.ToString()) / 100 + "'," +
                                    "'" + data.Terms[i].InterestType + "'," +
                                    "'1'," +
                                    "'" + loantypeid + "'," +
                                    "'" + data.Terms[i].Formula + "'," +
                                    "'" + data.Terms[i].InterestApplied + "'," +
                                    "'" + data.Terms[i].Terms + "'," +
                                    "'" + data.Terms[i].OldFormula + "'," +
                                    "'" + data.Terms[i].NoAdvancePayment + "'," +
                                    "'" + data.Terms[i].NotarialFeeOrigin + "'," +
                                    "'" + LessThanNotarialAmount + "'," +
                                    "'" + data.Terms[i].LessThanAmountTYpe + "'," +
                                    "'" + GreaterThanEqualNotarialAmount + "'," +
                                    "'" + data.Terms[i].GreaterThanEqualAmountType + "'," +
                                    "'" + LoanInsuranceAmount + "'," +
                                    "'" + data.Terms[i].LoanInsuranceAmountType + "'," +
                                    "'" + LifeInsuranceAmount + "'," +
                                    "'" + data.Terms[i].LifeInsuranceAmountType + "'," +
                                    "'" + data.Terms[i].DeductInterest + "'," +
                                    "'" + data.Terms[i].CollectionTypeId + "'," +
                                    "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            result = db.AUIDB_WithParam(Insert1) + " Added";
                        }
                    }
            
                    return Ok(result);
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
        public class LoneTypeIDVM
        {
            public string? LoanTypeID { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteLoanType(LoneTypeIDVM data)
        {
            string result = "";
            try
            {
                string Delete = "";


                sql = $@"select * from tbl_LoanType_Model where LoanTypeID ='" + data.LoanTypeID + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Delete = $@"UPDATE [dbo].[tbl_LoanType_Model] Set Status = 2 Where LoanTypeID ='" + data.LoanTypeID + "' ";


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
        public class TermsID
        {

            public string topId { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> DeleteTerms(List<TermsID> data)
        {
            string result = "";
            try
            {
                string Delete = "";

                for (int x = 0; x < data.Count; x++)
                {
                    sql = $@"select * from tbl_TermsOfPayment_Model where TopId ='" + data[x].topId + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                

                    Delete = $@"UPDATE [dbo].[tbl_TermsOfPayment_Model] Set Status = 2 Where TopId ='" + data[x].topId + "' ";


                    result = db.AUIDB_WithParam(Delete) + " Deleted";
              
                    }
            
                    else
                    {
                        return BadRequest("Invalid");
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetBaseException().ToString());

            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLoanType(LoanTypeModel data)
        {
            string result = "";
            try
            {
                string Update = "";


                sql = $@"select * from tbl_LoanType_Model where LoanTypeID ='" + data.LoanTypeID + "' ";
                DataTable table1 = db.SelectDb(sql).Tables[0];
                if (table1.Rows.Count != 0)
                {
                    Update += $@"UPDATE [dbo].[tbl_LoanType_Model]
                           SET [LoanTypeName] = '" + data.LoanTypeName + "'," +
                              "[Savings] = '" + data.Savings + "'," +
                              "[LoanAmount_Min] = '" + data.LoanAmount_Min + "'," +
                              "[LoanAmount_Max] = '" + data.LoanAmount_Max + "'," +
                              "[DateUpdated]='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                         "WHERE LoanTypeID ='" + data.LoanTypeID + "'";

                    
                    results = db.AUIDB_WithParam(Update) + " Updated";
                    //string Delete = $@"DELETE FROM [dbo].[tbl_TermsOfPayment_Model]  where LoanTypeID ='" + data.LoanTypeID + "'  ";
                    //db.AUIDB_WithParam(Delete);

                    if (data.Terms.Count != 0)
                    {
                        for (int i = 0; i < data.Terms.Count; i++)
                        {
                            double interest_rate = data.Terms[i].InterestType == "1" ? double.Parse(data.Terms[i].InterestRate.ToString()) / 100 : double.Parse(data.Terms[i].InterestRate.ToString());
                            double LessThanNotarialAmount = data.Terms[i].LessThanAmountTYpe == 1 ? double.Parse(data.Terms[i].LessThanNotarialAmount.ToString()) / 100 : double.Parse(data.Terms[i].LessThanNotarialAmount.ToString());
                            double GreaterThanEqualNotarialAmount = data.Terms[i].GreaterThanEqualAmountType == 1 ? double.Parse(data.Terms[i].GreaterThanEqualNotarialAmount.ToString()) / 100 : double.Parse(data.Terms[i].GreaterThanEqualNotarialAmount.ToString());
                            double LifeInsuranceAmount = data.Terms[i].LifeInsuranceAmountType == 1 ? double.Parse(data.Terms[i].LifeInsuranceAmount.ToString()) / 100 : double.Parse(data.Terms[i].LifeInsuranceAmount.ToString());
                            double LoanInsuranceAmount = data.Terms[i].LoanInsuranceAmountType == 1 ? double.Parse(data.Terms[i].LoanInsuranceAmount.ToString()) / 100 : double.Parse(data.Terms[i].LoanInsuranceAmount.ToString());

                            if (data.Terms[i].TopId != "")
                            {
                                string Insert1 = $@"
                                    UPDATE [dbo].[tbl_TermsOfPayment_Model]
                                       SET [NameOfTerms] =  '" + data.Terms[i].NameOfTerms + "'," +
                                        "[InterestRate] =  '" + double.Parse(data.Terms[i].InterestRate.ToString()) / 100 + "'," +
                                        "[InterestType] = '" + data.Terms[i].InterestType + "'," +
                                        "[Status] =  '1'," +
                                        "[DateUpdated] ='" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                                        "[LoanTypeId] = '" + data.Terms[i].LoanTypeID + "'," +
                                        "[Formula] =  '" + data.Terms[i].Formula + "'," +
                                        "[InterestApplied] =  '" + data.Terms[i].InterestApplied + "'," +
                                         "[Terms] = '" + data.Terms[i].Terms + "'," +
                                         "[OldFormula] = '" + data.Terms[i].OldFormula + "'," +
                                         "[NoAdvancePayment] = '" + data.Terms[i].NoAdvancePayment + "'," +
                                         "[NotarialFeeOrigin] =  '" + data.Terms[i].NotarialFeeOrigin + "'," +
                                         "[LessThanNotarialAmount] = '" + LessThanNotarialAmount + "'," +
                                         "[LessThanAmountTYpe] = '" + data.Terms[i].LessThanAmountTYpe + "'," +
                                         "[GreaterThanEqualNotarialAmount] =  '" + GreaterThanEqualNotarialAmount + "'," +
                                         "[GreaterThanEqualAmountType] =  '" + data.Terms[i].GreaterThanEqualAmountType + "'," +
                                         "[LoanInsuranceAmount] = '" + LoanInsuranceAmount + "'," +
                                         "[LoanInsuranceAmountType] =  '" + data.Terms[i].LoanInsuranceAmountType + "'," +
                                         "[LifeInsuranceAmount] =  '" + LifeInsuranceAmount + "'," +
                                         "[LifeInsuranceAmountType] = '" + data.Terms[i].LifeInsuranceAmountType + "'," +
                                         "[DeductInterest] =  '" + data.Terms[i].DeductInterest + "'," +
                                         "[CollectionTypeId] =  '" + data.Terms[i].CollectionTypeId + "'" +
                                  "WHERE TopId = '" + data.Terms[i].TopId + "'";


                                result = db.AUIDB_WithParam(Insert1) + " Added";
                            }
                            else
                            {
                                string Insert1 = $@"INSERT INTO [dbo].[tbl_TermsOfPayment_Model]
                                       ([NameOfTerms]
                                       ,[InterestRate]
                                       ,[InterestType]
                                       ,[Status]
                                       ,[LoanTypeId]
                                       ,[Formula]
                                       ,[InterestApplied]
                                       ,[Terms]
                                       ,[OldFormula]
                                       ,[NoAdvancePayment]
                                       ,[NotarialFeeOrigin]
                                       ,[LessThanNotarialAmount]
                                       ,[LessThanAmountTYpe]
                                       ,[GreaterThanEqualNotarialAmount]
                                       ,[GreaterThanEqualAmountType]
                                       ,[LoanInsuranceAmount]
                                       ,[LoanInsuranceAmountType]
                                       ,[LifeInsuranceAmount]
                                       ,[LifeInsuranceAmountType]
                                       ,[DeductInterest]
                                       ,[CollectionTypeId]
                                       ,[DateCreated])
                                 VALUES
                                    ('" + data.Terms[i].NameOfTerms + "'," +
                                   "'" + double.Parse(data.Terms[i].InterestRate.ToString()) / 100 + "'," +
                                   "'" + data.Terms[i].InterestType + "'," +
                                   "'1'," +
                                   "'" + data.Terms[i].LoanTypeID + "'," +
                                   "'" + data.Terms[i].Formula + "'," +
                                   "'" + data.Terms[i].InterestApplied + "'," +
                                   "'" + data.Terms[i].Terms + "'," +
                                   "'" + data.Terms[i].OldFormula + "'," +
                                   "'" + data.Terms[i].NoAdvancePayment + "'," +
                                   "'" + data.Terms[i].NotarialFeeOrigin + "'," +
                                   "'" + LessThanNotarialAmount + "'," +
                                   "'" + data.Terms[i].LessThanAmountTYpe + "'," +
                                   "'" + GreaterThanEqualNotarialAmount + "'," +
                                   "'" + data.Terms[i].GreaterThanEqualAmountType + "'," +
                                   "'" + LoanInsuranceAmount + "'," +
                                   "'" + data.Terms[i].LoanInsuranceAmountType + "'," +
                                   "'" + LifeInsuranceAmount + "'," +
                                   "'" + data.Terms[i].LifeInsuranceAmountType + "'," +
                                   "'" + data.Terms[i].DeductInterest + "'," +
                                   "'" + data.Terms[i].CollectionTypeId + "'," +
                                   "'" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + "')";

                                result = db.AUIDB_WithParam(Insert1) + " Added";
                            }
                           
                        }
                    }
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
        [HttpGet]
        public async Task<IActionResult> LoanTypeDetails()
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetLoanTypeDetails().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> getTermsList()
        {
            try
            {
                var result = dbmet.getTermsList().ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }   [HttpGet]
        public async Task<IActionResult> TermsFilterByLoanTypeID(string loantypeid)
        {
            try
            {
                var result = dbmet.getTermsListFilterbyLoanTypeID(loantypeid).ToList();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoanTypeDetailsFilterPaginate(string? Loantypename , int page, int pageSize)
        {
            var result = (dynamic)null;
            try
            {



                int totalItems = 0;
                int totalPages = 0;
                var items = (dynamic)null;
                if (Loantypename == null )
                {
                    var list = dbmet.GetLoanTypeDetails().ToList();


                    totalItems = list.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                    items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                }
                else
                {
                    var list = dbmet.GetLoanTypeDetails().Where(a => a.LoanTypeName.ToUpper().Contains(Loantypename.ToUpper())).ToList();


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
        public async Task<IActionResult> GetlastLoanTypeDetails()
        {
            var result = (dynamic)null;
            try
            {
                result = dbmet.GetLoanTypeDetails().ToList().LastOrDefault();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<IActionResult> LoanTypeFilter(LoneTypeIDVM data)
        {

            try
            {
                var result = dbmet.GetLoanTypeDetails().Where(a=>a.LoanTypeID == data.LoanTypeID).ToList();
                if(result.Count !=0)
                {
                    return Ok(result);
                }
                else
                {
                    var res = dbmet.GetLoanTypeDetails().ToList();
                    return Ok(res);
                }
            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
    }
}