using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class Status
    {
        public int Id { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        [DefaultValue(false)]
        public bool ClosingTicket { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}