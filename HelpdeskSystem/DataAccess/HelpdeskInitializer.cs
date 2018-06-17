using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HelpdeskSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpdeskSystem.DataAccess
{
    public class HelpdeskInitializer : DropCreateDatabaseIfModelChanges<HelpdeskContext>
    {
        protected override void Seed(HelpdeskContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user1 = new ApplicationUser { UserName = "admin@gmail.com" };
            var user2 = new ApplicationUser { UserName = "user@gmail.com" };
            userManager.Create(user1, "Admin123");
            userManager.Create(user2, "User123");
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Staff"));
            roleManager.Create(new IdentityRole("Client"));

            userManager.AddToRole(user1.Id, "Admin");
            userManager.AddToRole(user2.Id, "Staff");

            var profiles = new List<Profile>
            {
                new Profile { Username = "admin@gmail.com", Firstname = "Adam", Lastname = "Kowalski", RegisteredDate = DateTime.Now},
                new Profile { Username = "user@gmail.com", Firstname = "Jan", Lastname = "Kowalski", RegisteredDate = DateTime.Now}

            };
            var statuses = new List<Status>
            {
                new Status {Name = "Otwarte", ClosingTicket = false},
                new Status {Name = "Zamknięte", ClosingTicket = true}
            };
            profiles.ForEach(p => context.Profiles.Add(p));
            context.SaveChanges();
        }
    }
}