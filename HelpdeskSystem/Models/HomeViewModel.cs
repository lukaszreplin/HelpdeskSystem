using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class HomeViewModel
    {
        public List<TicketsByDateViewModel> CreatedDateGroups { get; set; }
        public List<TicketsBySupportViewModel> TicketsBySupports { get; set; }
        public List<TicketsByStatusViewModel> TicketsByStatuses { get; set; }
    }
}