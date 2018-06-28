using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminStatisticsController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        [ChildActionOnly]
        public ActionResult TicketsByDate()
        {
            IQueryable<TicketsByDateViewModel> ticketsByDates = from ticket in db.Tickets
                                                                group ticket by DbFunctions.TruncateTime(ticket.CreatedDate)
                into dateGroup
                                                                select new TicketsByDateViewModel()
                                                                {
                                                                    CreatedDate = dateGroup.Key,
                                                                    TicketCount = dateGroup.Count()
                                                                };
            return PartialView(ticketsByDates.ToList());
        }

        [ChildActionOnly]
        public ActionResult TicketsByStatus()
        {
            IQueryable<TicketsByStatusViewModel> ticketsByStatuses = from ticket in db.Tickets
                                                                     group ticket by ticket.Status.Name
                into statGroup
                                                                     select new TicketsByStatusViewModel()
                                                                     {
                                                                         Status = statGroup.Key,
                                                                         Count = statGroup.Count()
                                                                     };
            return PartialView(ticketsByStatuses.ToList());
        }

        [ChildActionOnly]
        public ActionResult TicketsBySupport()
        {
            IQueryable<TicketsBySupportViewModel> ticketsBySupports = from ticket in db.Tickets
                                                                      group ticket by ticket.OperatorId
                into opeGroup
                                                                      select new TicketsBySupportViewModel()
                                                                      {
                                                                          Firstname = db.Profiles.FirstOrDefault(p => p.Id == opeGroup.Key).Firstname,
                                                                          Lastname = db.Profiles.FirstOrDefault(p => p.Id == opeGroup.Key).Lastname
                                                                                     ?? "Nie przydzielono",
                                                                          Count = opeGroup.Count()
                                                                      };
            return PartialView(ticketsBySupports.ToList());
        }
    }
}