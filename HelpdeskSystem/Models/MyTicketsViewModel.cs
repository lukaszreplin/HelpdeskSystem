using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class MyTicketsViewModel
    {
        public int OpenTicketCount { get; set; }
        public int ClosedTicketCount { get; set; }
        public int NewTicketCount { get; set; }
        public bool AllTicketClosed { get; set; }
    }
}