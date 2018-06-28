using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models.ReportsModels
{
    public class TopOperViewModel
    {
        [DisplayName("Imię")]
        public string Firstname { get; set; }

        [DisplayName("Nazwisko")]
        public string Lastname { get; set; }

        [DisplayName("Email")]
        public string Username { get; set; }

        [DisplayName("Liczba zamkniętych zgłoszeń")]
        public int ClosedCounter { get; set; }

        [DisplayName("Liczba wszystkich zgłoszeń")]
        public int TotalCounter { get; set; }
    }
}