using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class StaffBasicStatisticsViewModel
    {
        public int Operated { get; set; }
        public int Available { get; set; }
        public int Closed { get; set; }

    }
}