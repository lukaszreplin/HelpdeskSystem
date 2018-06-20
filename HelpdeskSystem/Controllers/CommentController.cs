using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.Controllers
{
    public class CommentController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        [ChildActionOnly]
        public ActionResult Create(int ticketId)
        {
            var newComment = new Comment();
            newComment.TicketId = ticketId;

            return PartialView(newComment);
        }

        [HttpPost]
        public ActionResult Create(Comment comment)
        {

            comment.TicketId = comment.Id;
            comment.Id = 0;
            comment.CreatedDate = DateTime.Now;
            comment.ProfileId = db.Profiles.Single(p => p.Username == User.Identity.Name).Id;
            db.Tickets.FirstOrDefault(t => t.Id == comment.TicketId).Comments.Add(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
        }
    }
}