using FucoBook_DataAccess.Data;
using FucoBook_Model;
using FucoBook_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FucoBook_DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDBContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ApplicationDBContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            // migrations if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            } catch(Exception ex) {}

            // create roles if they are not created 
            if(!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                // if roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@phuocpp.com",
                    Email = "admin@phuocpp.com",
                    Name = "Ryan Denis",
                    PhoneNumber = "0899229788",
                    StreetAddress = "162 Test Ave",
                    State = "IL",
                    PostalCode = "160203",
                    City = "NewYork"
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser ad = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@phuocpp.com");
                _userManager.AddToRoleAsync(ad, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
