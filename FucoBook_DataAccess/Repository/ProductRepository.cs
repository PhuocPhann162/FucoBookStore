using FucoBook_DataAccess.Data;
using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FucoBook_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDBContext _db;
        public ProductRepository(ApplicationDBContext db) : base(db) 
        {
            _db = db;   
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Author = obj.Author;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.ProductImages = obj.ProductImages;
                //if(objFromDb.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
