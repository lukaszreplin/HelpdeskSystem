using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HelpdeskSystem.Models;
using HelpdeskSystem.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpdeskSystem.DataAccess
{
    public class HelpdeskInitializer : CreateDatabaseIfNotExists<HelpdeskContext>
    {
        protected override void Seed(HelpdeskContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user1 = new ApplicationUser { UserName = "admin@gmail.com", Email = "admin@gmail.com" };
            var user2 = new ApplicationUser { UserName = "user@gmail.com", Email = "user@gmail.com" };
            userManager.Create(user1, "Admin@123");
            userManager.Create(user2, "User@123");
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Staff"));
            roleManager.Create(new IdentityRole("Client"));

            userManager.AddToRole(user1.Id, "Admin");
            userManager.AddToRole(user2.Id, "Staff");
            var roles = new List<Role>
            {
                new Role { Name = "Admin", PolishName = "Admin"},
                new Role { Name = "Staff", PolishName = "Obsługa"},
                new Role { Name = "Client", PolishName = "Klient"}
            };
            roles.ForEach(r => context.Roles.Add(r));
            context.SaveChanges();
            var profiles = new List<Profile>
            {
                new Profile
                {
                    Username = "admin@gmail.com",
                    Firstname = "Adam",
                    Lastname = "Kowalski",
                    RoleId = 1,
                    RegisteredDate = DateTime.Now
                },
                new Profile
                {
                    Username = "user@gmail.com",
                    Firstname = "Jan",
                    Lastname = "Kowalski",
                    RoleId = 2,
                    RegisteredDate = DateTime.Now
                }

            };
            var statuses = new List<Status>
            {
                new Status {Id = 1, Name = "Nowe", LabelStyle = "label-warning"},
                new Status {Id = 2, Name = "Otwarte", LabelStyle = "label-success"},
                new Status {Id = 3, Name = "Zamknięte", LabelStyle = "label-danger"}
            };
            statuses.ForEach(s => context.Statuses.Add(s));
            profiles.ForEach(p => context.Profiles.Add(p));
            context.SaveChanges();
        }
    }
}