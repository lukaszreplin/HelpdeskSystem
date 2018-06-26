using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HelpdeskSystem.DataAccess;
using HelpdeskSystem.Models;
using Newtonsoft.Json;

namespace HelpdeskSystem.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (TempData["Logout"] != null && (bool)TempData["Logout"])
                ViewBag.Logout = true;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Statystyki.";
            using (var db = new HelpdeskContext())
            {
                IQueryable<CreatedDateGroup> data = from ticket in db.Tickets
                                                    group ticket by DbFunctions.TruncateTime(ticket.CreatedDate)
                    into dateGroup
                                                    select new CreatedDateGroup()
                                                    {
                                                        CreatedDate = dateGroup.Key,
                                                        TicketCount = dateGroup.Count()
                                                    };
                return View(data.ToList());
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Strona kontaktowa.";

            return View();
        }



        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose();
        //}
    }
}