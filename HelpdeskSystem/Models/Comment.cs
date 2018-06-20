using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TicketId { get; set; }
        public int ProfileId { get; set; }


        public virtual Ticket Ticket { get; set; }
        public virtual Profile Profile { get; set; }
    }
}