using FucoBook_Model.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FucoBook_Model.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList {  get; set; }

        public double OrderTotal {  get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public DateTime EstimateArrival { get; set; }
    }
}
