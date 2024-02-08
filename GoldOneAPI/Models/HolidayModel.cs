using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AuthSystem.Models
{
    public class HolidayModel
    {
        public string? HolidayName { get; set; }
        public DateTime? Date { get; set; }
        public string? Location { get; set; }
        public int? RepeatYearly { get; set; }
    }
}