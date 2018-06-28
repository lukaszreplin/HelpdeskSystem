using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models.ReportsModels
{
    public class MonthViewModel
    {
        public string MonthName { get; set; }
        public int CreatedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int AddedComments { get; set; }
        public int NewClients { get; set; }
        public string FirstnameClientWithMostTickets { get; set; }
        public string LastnameClientWithMostTickets { get; set; }
        public int TicketsOfClientWithMostTickets { get; set; }
        public IQueryable<TicketsBySupportViewModel> TicketsByOperator { get; set; }
        public IQueryable<TicketsByStatusViewModel> TicketsByStatus { get; set; }
    }
}