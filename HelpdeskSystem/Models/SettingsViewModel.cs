using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class SettingsViewModel
    {
        [Required]
        [DisplayName("Adres serwera SMTP")]
        public string SmtpServer { get; set; }

        [Required]
        [DisplayName("Email użytkownika")]
        public string SmtpUser { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Hasło")]
        public string SmtpPassword { get; set; }

        [Required]
        [DisplayName("Nazwa nadawcy")]
        public string SenderName { get; set; }
    }
}