using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FucoBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> lstProduct = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(lstProduct);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int? id)
        {
            Product p = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"Category");
            return View(p);
        }
    }
}
