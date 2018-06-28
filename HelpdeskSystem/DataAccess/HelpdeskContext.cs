using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Profile)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProfileId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>()
                .HasOptional(t => t.Operator)
                .WithMany(o => o.OperatedTickets)
                .HasForeignKey(t => t.OperatorId)
                .WillCascadeOnDelete(false);
        }
    }
}