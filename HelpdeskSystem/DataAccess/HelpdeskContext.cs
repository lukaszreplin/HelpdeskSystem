using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.DataAccess
{
    public class HelpdeskContext : DbContext
    {
        public HelpdeskContext()
            : base("DefaultConnection")
        {
        }

        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
}