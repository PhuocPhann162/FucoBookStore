using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model;
using FucoBook_Model.Models;
using FucoBook_Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FucoBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product")
            };

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                double price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if(cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser userLogin = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                ApplicationUser = userLogin
            };

            foreach(var cartItem in  ShoppingCartVM.ShoppingCartList)
            {
                double price = GetPriceBasedOnQuantity(cartItem);
                ShoppingCartVM.OrderTotal += (price * cartItem.Count);
            }

            ShoppingCartVM.EstimateArrival = System.DateTime.Now;

            return View(ShoppingCartVM);
        }
    }
}
