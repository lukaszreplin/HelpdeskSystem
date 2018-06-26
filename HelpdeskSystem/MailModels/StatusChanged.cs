using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.MailModels
{
    public class StatusChanged
    {
        public string Firstname { get; set; }
        public int Number { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; }
        public string TicketUrl { get; set; }
        public string SiteUrl { get; set; }
    }
}