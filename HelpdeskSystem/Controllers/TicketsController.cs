using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor;
using System.Web.Razor.Parser;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.MailModels;
using HelpdeskSystem.Models;
using HelpdeskSystem.Utils;
using PagedList;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace HelpdeskSystem.Controllers
{
    public class TicketsController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        // GET: Tickets
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "BySubjectDescending" : "";
            ViewBag.DateSortParm = sortOrder == "CreatedDate" ? "CreatedDateDescending" : "CreatedDate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tickets = from t in db.Tickets
                          select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.Subject.Contains(searchString)
                                               || t.Content.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "BySubjectDescending":
                    tickets = tickets.OrderByDescending(t => t.Subject);
                    break;
                case "CreatedDateDescending":
                    tickets = tickets.OrderBy(t => t.CreatedDate);
                    break;
                case "CreatedDate":
                    tickets = tickets.OrderByDescending(t => t.CreatedDate);
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Id);
                    break;
            }
            //tickets = db.Tickets.Include(t => t.Profile).Include(t => t.Status);
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(tickets.ToPagedList(pageNumber, pageSize));
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.ProfileId = new SelectList(db.Profiles, "Id", "Username");
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Subject,Content,StatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["attachment"];
                if (file != null && file.ContentLength > 0)
                {
                    ticket.Attachment = Guid.NewGuid() + file.FileName;
                    file.SaveAs(HttpContext.Server.MapPath("~/Attachments/") + ticket.Attachment);
                }
                ticket.CreatedDate = DateTime.Now;
                ticket.ModifiedDate = DateTime.Now;
                ticket.ProfileId = db.Profiles.Single(p => p.Username == User.Identity.Name).Id;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                var model = new TicketAddedModel
                {
                    Firstname = db.Profiles.Single(p => p.Username == User.Identity.Name).Firstname,
                    Lastname = db.Profiles.Single(p => p.Username == User.Identity.Name).Lastname
                };

                var templateManager = new ResolvePathTemplateManager(new[] { "~/Views/Mail/" });
                var config = new TemplateServiceConfiguration
                {
                    TemplateManager = templateManager
                };
                Engine.Razor = RazorEngineService.Create(config);
                var html = Engine.Razor.RunCompile("C:\\Users\\lreplin\\source\\repos\\HelpdeskSystem\\HelpdeskSystem\\Views\\Mail\\TicketAdded.cshtml", null, model);
                Mailing.SendMail("lukaszreplin@gmail.com", "TEST", html);
                return RedirectToAction("Index");
            }

            ViewBag.ProfileId = new SelectList(db.Profiles, "Id", "Username", ticket.ProfileId);
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProfileId = new SelectList(db.Profiles, "Id", "Username", ticket.ProfileId);
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject,Content,CreatedDate,ModifiedDate,StatusId,ProfileId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.ModifiedDate = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProfileId = new SelectList(db.Profiles, "Id", "Username", ticket.ProfileId);
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
