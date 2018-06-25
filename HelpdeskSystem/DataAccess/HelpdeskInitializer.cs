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
            RoleTranslate translate = new RoleTranslate();
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user1 = new ApplicationUser { UserName = "admin@gmail.com" };
            var user2 = new ApplicationUser { UserName = "user@gmail.com" };
            userManager.Create(user1, "Admin@123");
            userManager.Create(user2, "User@123");
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Staff"));
            roleManager.Create(new IdentityRole("Client"));

            userManager.AddToRole(user1.Id, "Admin");
            userManager.AddToRole(user2.Id, "Staff");

            var profiles = new List<Profile>
            {
                new Profile
                {
                    Username = "admin@gmail.com",
                    Firstname = "Adam",
                    Lastname = "Kowalski",
                    RoleName = translate.GetValue(userManager.GetRoles(user1.Id).FirstOrDefault()),
                    RegisteredDate = DateTime.Now
                },
                new Profile
                {
                    Username = "user@gmail.com",
                    Firstname = "Jan",
                    Lastname = "Kowalski",
                    RoleName = translate.GetValue(userManager.GetRoles(user2.Id).FirstOrDefault()),
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