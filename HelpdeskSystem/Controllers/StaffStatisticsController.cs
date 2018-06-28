using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffStatisticsController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        [ChildActionOnly]
        public ActionResult StaffBasicStatistics()
        {
            StaffBasicStatisticsViewModel staffBasicStatisticsViewModel = new StaffBasicStatisticsViewModel
            {
                Available = db.Tickets.Count(t => t.Operator == null),
                Closed = db.Tickets.Count(t => t.Operator.Username == User.Identity.Name && t.StatusId == 3),
                Operated = db.Tickets.Count(t => t.Operator.Username == User.Identity.Name && t.StatusId == 2)
            };
            return PartialView(staffBasicStatisticsViewModel);
        }
    }
}