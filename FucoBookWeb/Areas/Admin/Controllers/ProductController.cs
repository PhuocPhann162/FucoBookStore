using FucoBook_DataAccess.Data;
using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model;
using FucoBook_Model.Models;
using FucoBook_Model.ViewModels;
using FucoBook_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FucoBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> lstProduct = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
  
            return View(lstProduct);
        }

        public IActionResult Upsert(int? id) // Combine UpdateInsert
        {
            // Lấy dữ liệu ds category đề làm combobox
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                // Create 
                return View(productVM);
            }
            else
            {
                // Update 
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id != 0)
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"\images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath + productPath);

                        if(!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = productPath+ @"\" + fileName,
                            ProductId = productVM.Product.Id
                        };

                        if(productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);
                    }
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Product created/updated successfully";

                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            //var oldImagePath = Path.Combine(wwwRootPath + productToBeDeleted.ImageUrl.TrimStart('\\'));

            //if (System.IO.File.Exists(oldImagePath))
            //{
            //    System.IO.File.Delete(oldImagePath);
            //}
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product deleted successfully" });
        }

        #endregion
    }
}
