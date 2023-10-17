using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model.Models;
using FucoBook_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FucoBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Company)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Company> listCompany = _unitOfWork.Company.GetAll();
            return View(listCompany);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] 
        public IActionResult Create(Company obj)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Company.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index", "Company");
            }
            return View();
        } 

        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
