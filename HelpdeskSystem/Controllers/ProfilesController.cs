using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.MailModels;
using HelpdeskSystem.Models;
using HelpdeskSystem.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfilesController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        // GET: Profiles
        public ActionResult Index()
        {
            return View(db.Profiles.ToList());
        }

        // GET: Profiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: Profiles/Create
        public ActionResult Create()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Admin", Value = "Admin"},
                new SelectListItem { Text = "Obsługa", Value = "Staff" },
                new SelectListItem { Text = "Klient", Value = "Klient"}
            };
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Username,Firstname,Lastname,RoleId")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var user = new ApplicationUser { Email = profile.Username, UserName = profile.Username };
                    var result = userManager.Create(user);
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, "Client");
                        profile.RegisteredDate = DateTime.Now;
                        db.Profiles.Add(profile);
                        db.SaveChanges();

                        string code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code },
                            protocol: Request.Url.Scheme);
                        var model = new AccountCreatedViewModel
                        {
                            Firstname = profile.Firstname,
                            SiteUrl = Request.Url.GetLeftPart(UriPartial.Authority),
                            CallbackUrl = callbackUrl
                        };
                        var path = HttpContext.Server.MapPath("~/Views/Mail/SetPassword.cshtml");
                        var templateManager = new ResolvePathTemplateManager(new string[] { "~/Views/Mail/" });
                        var config = new TemplateServiceConfiguration
                        {
                            TemplateManager = templateManager
                        };
                        Engine.Razor = RazorEngineService.Create(config);
                        var html = Engine.Razor.RunCompile(path, null, model);
                        Mailing.SendMail(profile.Username, "Nowe konto w systemie", html);
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Wystapił błąd podczas dodawania nowego konta!";
                    TempData["ErrorDetails"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                }
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        // GET: Profiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "PolishName", profile.RoleId);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Firstname,Lastname,RegisteredDate,RoleId")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = userManager.FindByName(profile.Username);
                var rolesForUser = userManager.GetRoles(user.Id);
                if (rolesForUser.Any())
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        var result = userManager.RemoveFromRole(user.Id, item);
                    }
                }
                userManager.AddToRole(user.Id, db.Roles.Find(profile.RoleId).Name);
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profile profile = db.Profiles.Find(id);
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByName(profile.Username);
            var logins = user.Logins;
            var rolesForUser = userManager.GetRoles(user.Id);
            foreach (var login in logins.ToList())
            {
                userManager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));

            }

            if (rolesForUser.Any())
            {
                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    var result = userManager.RemoveFromRole(user.Id, item);
                }
            }

            userManager.Delete(user);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ContentResult GetFirstnameAndLastname(string email)
        {
            var profile = db.Profiles.FirstOrDefault(p => p.Username == email);
            return Content(profile.Firstname + " " + profile.Lastname);
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
