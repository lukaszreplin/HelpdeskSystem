using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.MailModels
{
    public class AccountCreatedViewModel
    {
        public string Firstname { get; set; }
        public string SiteUrl { get; set; }
        public string CallbackUrl { get; set; }
    }
}