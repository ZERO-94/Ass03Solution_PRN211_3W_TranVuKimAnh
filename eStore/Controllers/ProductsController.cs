using BusinessObject;
using BusinessObject.Models;
using DataAccess.repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eStore.Controllers
{
    public class ProductsController : Controller
    {
        IProductRepository productRepository = null;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(IWebHostEnvironment webHostEnvironment)
        {
            productRepository = new ProductRepository();
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductsController
        [Route("/products", Name = "products")]
        public ActionResult Index(string searchString)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            List<Product> productList = productRepository.GetAllProducts();
            if (!String.IsNullOrEmpty(searchString))
            {
                productList = productList.Where(p => p.ProductName.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) >= 0
                                                            || p.UnitPrice.ToString().Contains(searchString)).ToList();
            }
            else
            {
                ViewBag.Message = "Search field cannot be empty!";
            }
            return View(productList);
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            Product product = productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            return View();
        }

        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (product.ProductName == null || product.Weight == null)
            {
                ViewBag.Message = "All fields must be filled to create a new product!";
                return View(product);
            }
            else if (product.UnitPrice <= 0)
            {
                ViewBag.Message = "Unit price must be positive!";
                return View(product);
            } else if (product.UnitsInStock < 0)
			{
                ViewBag.Message = "Unit in stock cannot be negative!";
                return View(product);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    productRepository.CreateProduct(product);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(product);
            }
        }

        // GET: ProductsController/Edit/5
        public ActionResult Edit(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            if (product.ProductName == null || product.Weight == null)
            {
                ViewBag.Message = "All fields must be filled to create a new product!";
                return View(product);
            }
            else if (product.UnitPrice <= 0)
            {
                ViewBag.Message = "Unit price must be positive!";
                return View(product);
            }
            else if (product.UnitsInStock < 0)
            {
                ViewBag.Message = "Unit in stock cannot be negative!";
                return View(product);
            }

            try
            {
                if (id != product.ProductId)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    productRepository.UpdateProduct(product.ProductId, product);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: ProductsController/Delete/5
        public ActionResult Delete(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                productRepository.DeleteProduct(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
    }
}
