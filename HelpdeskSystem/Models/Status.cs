using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelpdeskSystem.Models
{
    public class Status
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [DisplayName("Nazwa")]
        public string Name { get; set; }

        public string LabelStyle { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}