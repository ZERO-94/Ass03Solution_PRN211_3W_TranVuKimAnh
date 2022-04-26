using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        private static ProductDAO instance;
        private static readonly object instanceLock = new object();
        private AssignmentPRN211DBContext context = new AssignmentPRN211DBContext();

        //Use singleton design pattern
        public ProductDAO()
        {

        }

        public bool CreateProduct(Product newProduct)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                try
                {
                    context.Products.Add(newProduct);
                    context.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return false;
                }
                catch (DbUpdateException ex)
                {
                    return false;
                }
            }
        }

        public bool DeleteProduct(int id)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                try
                {
                    context.Products.Remove(context.Products.SingleOrDefault(p => p.ProductId == id));
                    context.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return false;
                }
                catch (DbUpdateException ex)
                {
                    return false;
                }
            }
        }

        public bool UpdateProduct(int id, Product updatedProductInfo)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                try
                {
                    context.Products.Update(updatedProductInfo);
                    context.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return false;
                }
                catch (DbUpdateException ex)
                {
                    return false;
                }
            }
        }

        public Product GetProductById(int id)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Products.AsNoTracking().SingleOrDefault<Product>((m) => m.ProductId == id);
            }
        }

        public List<Product> GetAllProducts()
        {
            using(context = new AssignmentPRN211DBContext())
            {
                return context.Products.ToList<Product>();
            }
                
        }

        public List<Product> SearchProduct(int? searchId, string? searchName, decimal? searchPrice, int? searchInStock)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Products.Where(product => searchId == null || product.ProductId == searchId)
                .Where(product => searchName == null || product.ProductName.Contains(searchName))
                .Where(product => searchPrice == null || product.UnitPrice == searchPrice)
                .Where(product => searchInStock == null || product.UnitsInStock == searchInStock)
                .ToList();
            }
        }
    }
}
