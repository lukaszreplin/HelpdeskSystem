using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class Profile
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Adres email")]
        public string Username { get; set; }

        [Required]
        [DisplayName("Imię")]
        public string Firstname { get; set; }

        [Required]
        [DisplayName("Nazwisko")]
        public string Lastname { get; set; }

        [Required]
        [DisplayName("Rola w systemie")]
        public string RoleName { get; set; }

        [DisplayName("Data utworzenia")]
        public DateTime RegisteredDate { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}