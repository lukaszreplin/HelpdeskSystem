using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HelpdeskSystem.DataAccess;
using HelpdeskSystem.Models;
using HelpdeskSystem.Models.ReportsModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using IronPdf;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();

        private IDictionary<int, string> monthsInPolish = new Dictionary<int, string>
        {
            {1, "Styczeń" },
            {2, "Luty" },
            {3, "Marzec" },
            {4, "Kwiecień" },
            {5, "Maj" },
            {6, "Czerwiec" },
            {7, "Lipiec" },
            {8, "Sierpień" },
            {9, "Wrzesień" },
            {10, "Październik" },
            {11, "Listopad" },
            {23, "Grudzień" }
        };

        // GET: Reports
        public ActionResult Index()
        {
            ReportsViewModel reportsViewModel = new ReportsViewModel
            {
                Reports = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string> ("TopOper","Najlepsi serwisanci"),
                    new KeyValuePair<string, string>("Month","Raport miesięczny")
                }
            };
            return View(reportsViewModel);
        }

        public ActionResult TopOper()
        {
            IQueryable<TopOperViewModel> topOpers = db.Profiles.Where(p => p.Role.Name.Equals("Staff"))
                .Select(p => new TopOperViewModel
                {
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    Username = p.Username,
                    ClosedCounter = p.Tickets.Count(t => t.StatusId == 3),
                    TotalCounter = p.Tickets.Count
                })
                .OrderByDescending(to => to.ClosedCounter);
            return View(topOpers.ToList());
        }



        public ActionResult Month()
        {
            MonthViewModel viewModel = new MonthViewModel();
            viewModel.MonthName = monthsInPolish[DateTime.Now.Month];
            viewModel.AddedComments =
                db.Comments.Count(c => c.CreatedDate.Month == DateTime.Now.Month);
            viewModel.ClosedTickets =
                db.Tickets.Count(t => t.ModifiedDate.Month == DateTime.Now.Month && t.StatusId == 3);
            viewModel.CreatedTickets =
                db.Tickets.Count(t => t.CreatedDate.Month == DateTime.Now.Month);
            viewModel.NewClients =
                db.Profiles.Count(p => p.RegisteredDate.Month == DateTime.Now.Month && p.Role.Name.Equals("Client"));
            viewModel.TicketsByStatus =
                db.Tickets.Where(t => t.CreatedDate.Month == DateTime.Now.Month).GroupBy(t => t.Status.Name)
                    .Select(tg => new TicketsByStatusViewModel
                    {
                        Status = tg.Key,
                        Count = tg.Count()
                    });
            viewModel.TicketsByOperator =
                db.Tickets.Where(t => t.CreatedDate.Month == DateTime.Now.Month).GroupBy(t => t.Operator.Username)
                    .Select(to => new TicketsBySupportViewModel
                    {
                        Firstname = db.Profiles.FirstOrDefault(p => p.Username == to.Key).Firstname,
                        Lastname = db.Profiles.FirstOrDefault(p => p.Username == to.Key).Lastname ?? "Nie przydzielono",
                        Count = to.Count()
                    });
            var ClientWithMostTickets = db.Profiles
                .OrderByDescending(p => p.Tickets.Count)
                .Take(1)
                .Select(p => new
                {
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    Count = p.Tickets.Count
                }).Single();
            viewModel.FirstnameClientWithMostTickets = ClientWithMostTickets.Firstname;
            viewModel.LastnameClientWithMostTickets = ClientWithMostTickets.Lastname;
            viewModel.TicketsOfClientWithMostTickets = ClientWithMostTickets.Count;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ToPdf(string report)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                HtmlToPdf HtmlToPdf = new HtmlToPdf();
                var PDF = HtmlToPdf.RenderHtmlAsPdf(report);
                return File(PDF.BinaryData, "application/pdf", $"Raport_{DateTime.Now}.pdf");
            }
        }
    }
}