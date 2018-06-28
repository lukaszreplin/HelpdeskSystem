using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class TicketsByDateViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        public int TicketCount { get; set; }
    }
}