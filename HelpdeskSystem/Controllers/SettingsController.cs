using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {

        public ActionResult Index()
        {
            SettingsViewModel settingsViewModel = new SettingsViewModel
            {
                SmtpServer = ConfigurationManager.AppSettings["smtpHost"] ?? "",
                SmtpUser = ConfigurationManager.AppSettings["senderAddress"] ?? "",
                SmtpPassword = "",
                SenderName = ConfigurationManager.AppSettings["sender"]
            };
            return View(settingsViewModel);
        }


        [HttpPost]
        public ActionResult Index(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                ConfigurationManager.AppSettings["smtpHost"] = model.SmtpServer;
                ConfigurationManager.AppSettings["senderAddress"] = model.SmtpUser;
                if (model.SmtpPassword != "")
                {
                    ConfigurationManager.AppSettings["password"] = model.SmtpPassword;
                }
                ConfigurationManager.AppSettings["sender"] = model.SenderName;
                ViewBag.Message = "Ustawienia zostały zapisane!";
            }

            return View(model);
        }
    }
}