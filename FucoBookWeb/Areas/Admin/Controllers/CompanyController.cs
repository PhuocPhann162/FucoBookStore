using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model.Models;
using FucoBook_Model.ViewModels;
using FucoBook_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FucoBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> listCompany = _unitOfWork.Company.GetAll().ToList();
            return View(listCompany);
        }

        public IActionResult Upsert(int? id) // Combine UpdateInsert
        {
            if (id == null || id == 0)
            {
                // Create 
                return View(new Company());
            }
            else
            {
                // Update 
                Company obj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(obj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                if (companyObj.Id != 0)
                {
                    _unitOfWork.Company.Update(companyObj);
                    _unitOfWork.Save();
                    TempData["success"] = "Company updated successfully";
                }
                else
                {
                    _unitOfWork.Company.Add(companyObj);
                    _unitOfWork.Save();
                    TempData["success"] = "Company created successfully";
                }
                return RedirectToAction("Index", "Company");
            }
            return View(companyObj);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleted successfully" });
        }

        #endregion
    }
}
