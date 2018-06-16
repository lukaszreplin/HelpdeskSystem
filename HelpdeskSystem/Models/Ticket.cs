using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class Ticket
    {
        [DisplayName("Numer")]
        public int Id { get; set; }
        [DisplayName("Temat")]
        public string Subject { get; set; }
        [DisplayName("Treść zgłoszenia")]
        public string Content { get; set; }
        [DisplayName("Utworzono")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Zmodyfikowano")]
        public DateTime ModifiedDate { get; set; }
        [DisplayName("Status")]
        public int? StatusId { get; set; }
        [DisplayName("Utworzył")]
        public int ProfileId { get; set; }

        public virtual Status Status { get; set; }
        public virtual Profile Profile { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}