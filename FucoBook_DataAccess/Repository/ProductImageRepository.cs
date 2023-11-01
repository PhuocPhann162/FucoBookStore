using FucoBook_DataAccess.Data;
using FucoBook_DataAccess.Repository.IRepository;
using FucoBook_Model;
using FucoBook_Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FucoBook_DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDBContext _db;
        public ProductImageRepository(ApplicationDBContext db) : base(db) 
        {
            _db = db;   
        }

        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
