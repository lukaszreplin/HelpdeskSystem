using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.IO;
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
    [Authorize]
    public class TicketsController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        // GET: Tickets
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int status = 0)
        {
            ViewBag.Status = status;
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
                          where t.ProfileId == db.Profiles.FirstOrDefault(p => p.Username == User.Identity.Name).Id
                          select t;
            if (User.IsInRole("Admin"))
            {
                tickets = from t in db.Tickets
                          select t;
            }
            if (User.IsInRole("Staff"))
            {
                tickets = from t in db.Tickets
                          where t.OperatorId == db.Profiles.FirstOrDefault(p => p.Username == User.Identity.Name).Id || t.OperatorId == null
                          select t;
            }

            if (status != 0)
            {
                tickets = tickets.Where(t => t.StatusId == status);
            }
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
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tickets.ToPagedList(pageNumber, pageSize));
        }



        // GET: Tickets/Details/5
        public ActionResult Details(int? id, bool commentsDescending = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CommentDescendind = commentsDescending;

            Ticket ticket = db.Tickets.Find(id);
            if (commentsDescending)
            {
                var tmpComments = ticket.Comments.OrderByDescending(c => c.CreatedDate).ToList();
                ticket.Comments = tmpComments;
            }


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
                ticket.StatusId = 1;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                var model = new TicketAddedModel
                {
                    Firstname = db.Profiles.Single(p => p.Username == User.Identity.Name).Firstname,
                    Number = ticket.Id,
                    CreatedDate = ticket.CreatedDate,
                    Status = db.Statuses.FirstOrDefault(s => s.Id == ticket.StatusId)?.Name,
                    TicketUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Tickets/Details/" + ticket.Id,
                    SiteUrl = Request.Url.GetLeftPart(UriPartial.Authority)
                };

                IRazorEngineService service = Engine.Razor;
                var path = HttpContext.Server.MapPath("~/Views/Mail/TicketAdded.cshtml");
                var templateManager = new ResolvePathTemplateManager(new string[] { "~/Views/Mail/" });
                var config = new TemplateServiceConfiguration
                {
                    TemplateManager = templateManager
                };
                Engine.Razor = RazorEngineService.Create(config);
                var html = Engine.Razor.RunCompile(path, null, model);
                Mailing.SendMail(ticket.Profile.Username, "Potwierdzenie przyjęcia zgłoszenia", html);
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
            ViewBag.OperatorId = new SelectList(db.Profiles, "Id", "Username", ticket.OperatorId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject,Content,CreatedDate,ModifiedDate,StatusId,ProfileId,OperatorId")] Ticket ticket)
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


        public ActionResult Close(int id)
        {
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == id);
            ticket.StatusId = 3;
            ticket.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            var model = new StatusChanged
            {
                Firstname = ticket.Profile.Firstname,
                ModifiedDate = ticket.ModifiedDate,
                Number = ticket.Id,
                SiteUrl = Request.Url.GetLeftPart(UriPartial.Authority),
                Status = ticket.Status.Name,
                TicketUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Tickets/Details/" + ticket.Id
            };
            var path = HttpContext.Server.MapPath("~/Views/Mail/StatusChanged.cshtml");
            var templateManager = new ResolvePathTemplateManager(new string[] { "~/Views/Mail/" });
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = templateManager
            };
            Engine.Razor = RazorEngineService.Create(config);
            var html = Engine.Razor.RunCompile(path, null, model);
            Mailing.SendMail(ticket.Profile.Username, "Zmiana statusu zgłoszenia", html);
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Open(int id)
        {
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == id);
            ticket.StatusId = 2;
            ticket.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Support(int id)
        {
            var ticket = db.Tickets.First(t => t.Id == id);
            ticket.OperatorId = db.Profiles.Single(p => p.Username == User.Identity.Name).Id;
            ticket.StatusId = 2;
            ticket.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [AllowAnonymous]
        public ContentResult GetCount()
        {
            var count = db.Tickets.Count();
            return Content(count.ToString());
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
