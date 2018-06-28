using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientStatisticsController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();

        [ChildActionOnly]
        public ActionResult MyTickets()
        {
            MyTicketsViewModel myTicketsViewModel = new MyTicketsViewModel
            {
                ClosedTicketCount = db.Tickets.Count(t => t.StatusId == 3 && t.Profile.Username == User.Identity.Name),
                OpenTicketCount = db.Tickets.Count(t => t.StatusId == 2 && t.Profile.Username == User.Identity.Name),
                NewTicketCount = db.Tickets.Count(t => t.StatusId == 1 && t.Profile.Username == User.Identity.Name),
                AllTicketClosed = db.Tickets.Count(t => t.Profile.Username == User.Identity.Name) ==
                                  db.Tickets.Count(t => t.StatusId == 3 && t.Profile.Username == User.Identity.Name)
            };
            return PartialView(myTicketsViewModel);
        }
    }
}