using FucoBook_DataAccess.Data;
using FucoBook_DataAccess.Repository.IRepository;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM roleVM = new RoleManagementVM() {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem {
                    Text = i.Name, 
                    Value = i.Id.ToString()
                })
            };
            roleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId))
                .GetAwaiter().GetResult().FirstOrDefault();
            
            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM RoleVM)
        {
            var oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == RoleVM.ApplicationUser.Id))
                .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == RoleVM.ApplicationUser.Id);
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
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, RoleVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != RoleVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = RoleVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }

            TempData["success"] = "Set role for this account successfully";

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();


            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

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
            var userFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { sucess = false, message = "Error while Locking/Unlocking" });
            }
            if (userFromDb.LockoutEnd == null || userFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked and we need to unlock them 
                userFromDb.LockoutEnd = DateTime.Now;
                _unitOfWork.ApplicationUser.Update(userFromDb);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Unlock this user successfully" });
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                _unitOfWork.ApplicationUser.Update(userFromDb);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Lock this user successfully" });
            }
        }

        #endregion
    }
}
