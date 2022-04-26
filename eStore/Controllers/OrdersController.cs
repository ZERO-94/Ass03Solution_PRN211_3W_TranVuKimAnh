using BusinessObject;
using BusinessObject.Models;
using DataAccess.repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace eStore.Controllers
{
    public class OrdersController : Controller
    {
        IOrderRepository orderRepository = null;
        IOrderDetailRepository orderDetailRepository = null;
        IMemberRepository memberRepository = null;
        public OrdersController()
        {
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
            memberRepository = new MemberRepository();
        }

        public IActionResult Index(DateTime startDate, DateTime endDate, string Search)
        {

            var session = HttpContext.Session;
            if (session.GetString("Role") == null)
            {
                return RedirectToAction("Login", "Members");
            }
            else if (session.GetString("Role") == "Member")
            {
                Member currentMem = memberRepository.GetMemberByEmail(session.GetString("Email"));
                var orderList = orderRepository.GetOrderByMemberId(currentMem.MemberId);

                if(Search != null)
                {
                    orderList = orderList.FindAll(o => o.OrderDate <= endDate && o.OrderDate >= startDate);
                }

                return View(orderList);
            }
            else if(Search != null)
            {
                

                if (startDate.Year == 1 || endDate.Year == 1)
                {
                    var orderList = orderRepository.GetAllOrders();
                    return View(orderList);
                }
                else if (DateTime.Compare(startDate, endDate) >= 0)
                {
                    ViewBag.Message = "Start Date must before End Date!";
                    var orderList = orderRepository.GetAllOrders();
                    return View(orderList);
                }
                else
                {
                    var orderList = orderRepository.GetOrderByDateRange(startDate, endDate);
                    return View(orderList);
                }
            }

            return View(orderRepository.GetAllOrders());
        }

        public IActionResult GetSaleReport(DateTime startDate, DateTime endDate)
        {

            var session = HttpContext.Session;
            if (session.GetString("Role") == null)
            {
                return RedirectToAction("Login", "Members");
            }
            else if (session.GetString("Role") == "Member")
            {
                return View("/");
            }
            else if (session.GetString("Role") == "Admin")
            {
                if (DateTime.Compare(startDate, endDate) >= 0)
                {
                    ViewBag.Message = "Start Date must before End Date!";
                    var orderList = new SaleReport();
                    return View(orderList.soldProductList);
                }
                else
                {
                    var orderList = orderRepository.GetSaleReport(startDate, endDate);
                    return View(orderList.soldProductList);
                }
            }

            return View(nameof(Index));
        }

        public ActionResult Details(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null)
            {
                return RedirectToAction("Login", "Members");
            }

            IEnumerable<OrderDetail> orderDetails;
            var order = orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var customerEmail = memberRepository.GetMemberById((int)order.MemberId).Email;
            if (session.GetString("Role") == "Member" && session.GetString("Email") != customerEmail)
            {
                return RedirectToAction("Index", "Orders");
            }

            orderDetails = orderDetailRepository.GetOrderDetailByOrderId(id);
            ViewData["OrderDetailList"] = orderDetails;

            return View(order);
        }

        public ActionResult Delete(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") == null || session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Login", "Members");
            }

            var order = orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var customerEmail = memberRepository.GetMemberById((int)order.MemberId).Email;
            if (session.GetString("Role") == "Member" && session.GetString("Email") != customerEmail)
            {
                return RedirectToAction("Index", "Orders");
            }

            var orderDetails = orderDetailRepository.GetOrderDetailByOrderId(id);
            ViewData["OrderDetailList"] = orderDetails;

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                orderRepository.DeleteOrder(id);
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
