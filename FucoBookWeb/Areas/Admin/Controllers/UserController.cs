using FucoBook_DataAccess.Data;
using FucoBook_Model;
using FucoBook_Model.ViewModels;
using FucoBook_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FucoBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDBContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId; 
            RoleManagementVM roleVM = new RoleManagementVM() {
                ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem {
                    Text = i.Name, 
                    Value = i.Id.ToString()
                })
            };

            roleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
            
            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM RoleVM)
        {
            var RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == RoleVM.ApplicationUser.Id).RoleId;
            var oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == RoleVM.ApplicationUser.Id);
            if (!(RoleVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                if (RoleVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = RoleVM.ApplicationUser.CompanyId;
                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, RoleVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (RoleVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = RoleVM.ApplicationUser.CompanyId;
                    _db.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { sucess = false, message = "Error while Locking/Unlocking" });
            }
            if (userFromDb.LockoutEnd == null || userFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked and we need to unlock them 
                userFromDb.LockoutEnd = DateTime.Now;
                _db.SaveChanges();
                return Json(new { success = true, message = "Unlock this user successfully" });
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                _db.SaveChanges();
                return Json(new { success = true, message = "Lock this user successfully" });
            }
        }

        #endregion
    }
}
