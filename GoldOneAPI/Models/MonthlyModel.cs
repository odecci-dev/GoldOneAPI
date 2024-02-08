using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class MonthlyModel
    {
        public int? Id { get; set; }
        public string? MemId { get; set; }
        public decimal? ElectricBill { get; set; }
        public decimal? WaterBill { get; set; }
        public decimal? OtherBills { get; set; }
        public decimal? DailyExpenses { get; set; }
        public string? Status { get; set; }
    }
}
