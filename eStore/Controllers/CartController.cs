using BusinessObject;
using BusinessObject.Models;
using DataAccess.repository;
using eStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace eStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        IMemberRepository memberRepository = null;
        IProductRepository productRepository = null;
        IOrderRepository orderRepository = null;
        IOrderDetailRepository orderDetailRepository = null;
        public CartController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            productRepository = new ProductRepository();
            memberRepository = new MemberRepository();
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            return View();
        }

        [Route("/addCart/{productId:int}", Name = "addCart")]
        public IActionResult AddToCart([FromRoute] int productId)
        {
            var product = productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }

            // Add to cart
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.product.ProductId == productId);
            if (cartItem != null)
            {
                // The item is already exist in the cart
                cartItem.quantity++;
            }
            else
            {
                // Add a new item to cart
                cart.Add(new CartItem()
                {
                    quantity = 1,
                    product = product
                });
            }

            // Save cart to session
            SaveCartSession(cart);
            return Redirect("/Products");
        }

        // Update item in cart
        [HttpPost]
        [Route("/updatecart", Name = "updateCart")]
        public IActionResult UpdateCartItem(int discount, int quantity, int productId)
        {
            if (discount < 0)
            {
                string messageStr = "Discount cannot be negative!";
                return RedirectToRoute("cart", new { message = messageStr });
            }

            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.product.ProductId == productId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    RemoveFromCart(cartItem.product.ProductId);
                    return RedirectToAction(nameof(Cart));
                }
                else if (quantity > productRepository.GetProductById(cartItem.product.ProductId).UnitsInStock)
                {
                    var name = productRepository.GetProductById(cartItem.product.ProductId).ProductName;
                    string messageStr = $"The product {name} is not enough in stock!";
                    return RedirectToRoute("cart", new { message = messageStr });
                }
                // Update quantity with user input in the form
                cartItem.quantity = quantity;
                cartItem.discount = discount;
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        // Delete item in cart
        [Route("/removeItem/{productId:int}", Name = "removeItem")]
        public IActionResult RemoveFromCart([FromRoute] int productId)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productId);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }


        // Show the cart
        [Route("/cart", Name = "cart")]
        public IActionResult Cart(string message)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            ViewData["CustomerList"] = new SelectList(memberRepository.GetAllMembers(), "MemberId", "Email");
            if (message != null)
            {
                ViewData["OutStockMess"] = message;
            }
            return View(GetCartItems());
        }

        //Delete cart
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove("cart");

        }

        [HttpPost]
        [Route("/addorder", Name = "addorder")]
        public IActionResult AddOrder([FromForm] decimal freight,
            [FromForm] DateTime requiredDate,
            [FromForm] DateTime orderDate,
            [FromForm] DateTime shippedDate,
            [FromForm] int customerId)
        {
            try
            {
                DateTime baseDate = new DateTime(1970, 1, 1);
                TimeSpan diff = DateTime.Now - baseDate;
                int orderId = (int)diff.TotalSeconds;
                // Add order table
                Order order = new Order
                {
                    OrderId = orderId,
                    MemberId = customerId,
                    Freight = freight,
                    OrderDate = orderDate,
                    RequiredDate = requiredDate,
                    ShippedDate = shippedDate
                };
                orderRepository.CreateOrder(order);

                // Add order detail
                List<CartItem> cart = GetCartItems();
                foreach (CartItem item in cart)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        Product = item.product,
                        Discount = item.discount,
                        Quantity = item.quantity
                    };
                    orderDetailRepository.AddOrderDetail(orderDetail);
                    Product productPruchased = productRepository.GetProductById(item.product.ProductId);
                    productPruchased.UnitsInStock -= item.quantity;
                    productRepository.UpdateProduct(productPruchased.ProductId, productPruchased);
                }

                ClearCart();
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction(nameof(Cart));
            }
        }

        // Get the cart
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsonCart = session.GetString("cart");
            if (jsonCart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);
            }

            return new List<CartItem>();
        }

        // Save cart items to session
        void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;
            string jsonCart = JsonConvert.SerializeObject(list);
            session.SetString("cart", jsonCart);
        }
    }
}
