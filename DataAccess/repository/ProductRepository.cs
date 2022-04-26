using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public class ProductRepository : IProductRepository
    {

        private ProductDAO daoInstance = new ProductDAO();

        public bool CreateProduct(Product Product) => daoInstance.CreateProduct(Product);

        public bool DeleteProduct(int id) => daoInstance.DeleteProduct(id);

        public List<Product> GetAllProducts() => daoInstance.GetAllProducts();

        public Product GetProductById(int id) => daoInstance.GetProductById(id);

        public List<Product> SearchProduct(int? searchId, string? searchName, decimal? searchPrice, int? searchInStock) => daoInstance.SearchProduct(searchId, searchName, searchPrice, searchInStock );

        public bool UpdateProduct(int id, Product Product) => daoInstance.UpdateProduct(id, Product);
    }
}
