using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public interface IProductRepository
    {
        public Product GetProductById(int id);

        public List<Product> GetAllProducts();

        public bool CreateProduct(Product Product);

        public bool UpdateProduct(int id, Product Product);

        public bool DeleteProduct(int id);

        public List<Product> SearchProduct(int? searchId, string? searchName, decimal? searchPrice, int? searchInStock);
    }
}
