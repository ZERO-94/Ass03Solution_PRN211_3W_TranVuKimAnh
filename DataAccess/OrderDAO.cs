using BusinessObject;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDAO
    {
        private static OrderDAO instance;
        private static readonly object instanceLock = new object();
        private AssignmentPRN211DBContext context = new AssignmentPRN211DBContext();

        //Use singleton design pattern
        public OrderDAO()
        {

        }

        public bool CreateOrder(Order newOrder)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                try
                {
                    context.Orders.Add(newOrder);
                    foreach (OrderDetail detail in newOrder.OrderDetails)
                    {
                        Product product = context.Products.SingleOrDefault(p => p.ProductId == detail.ProductId);
                        product.UnitsInStock -= detail.Quantity;
                        context.Products.Update(product);
                        context.SaveChanges();
                    }
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

        public bool DeleteOrder(int id)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                try
                {
                    Order deleteOrder = context.Orders.Include(order => order.OrderDetails).ThenInclude(orderDetail => orderDetail.Product).SingleOrDefault(o => o.OrderId == id);
                    if (deleteOrder != null)
                    {
                        context.Orders.Remove(deleteOrder);
                        foreach (OrderDetail detail in deleteOrder.OrderDetails)
                        {
                            Product product = context.Products.SingleOrDefault(p => p.ProductId == detail.ProductId);
                            product.UnitsInStock += detail.Quantity;
                            context.Products.Update(product);
                            context.SaveChanges();
                        }
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

        public bool UpdateOrder(int id, Order updatedOrderInfo)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                try
                {
                    context.Orders.Update(updatedOrderInfo);
                    foreach (OrderDetail detail in updatedOrderInfo.OrderDetails)
                    {
                        context.Products.Update(detail.Product);
                        context.SaveChanges();
                    }
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

        public Order GetOrderById(int id)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Orders.Include(order => order.OrderDetails).ThenInclude(orderDetail => orderDetail.Product).SingleOrDefault<Order>((m) => m.OrderId == id);
            }
        }

        public List<Order> GetOrderByMemberId(int memberId)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Orders.Include(order => order.OrderDetails).ThenInclude(orderDetail => orderDetail.Product).Where<Order>((m) => m.MemberId == memberId).ToList();
            }
        }

        public List<Order> GetOrderByDateRange(DateTime startDate, DateTime endDate)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Orders.Include(order => order.OrderDetails).ThenInclude(orderDetail => orderDetail.Product).Where<Order>((m) => m.OrderDate >= startDate && m.OrderDate <= endDate).ToList();
            }
        }

        public List<Order> GetAllOrders()
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Orders.Include(order => order.OrderDetails).ThenInclude(orderDetail => orderDetail.Product).ToList<Order>();
            }
        }

        public SaleReport GetSaleReport(DateTime startDate, DateTime endDate)
        {
            using (context = new AssignmentPRN211DBContext())
            {
                SaleReport saleReport = new SaleReport();

                List<Order> soldOrder = context.Orders.Include(order => order.OrderDetails).ThenInclude(orderDetail => orderDetail.Product).Where<Order>((m) => m.OrderDate >= startDate && m.OrderDate <= endDate).ToList();

                List<ProductForReport> products = new List<ProductForReport>();

                foreach (Order order in soldOrder)
                {
                    foreach (OrderDetail detail in order.OrderDetails)
                    {
                        if (products.SingleOrDefault(p => p.ProductId == detail.ProductId) != null)
                        {
                            ProductForReport product = products.SingleOrDefault(p => p.ProductId == detail.ProductId);
                            

                            ProductForReport newProduct = new ProductForReport()
                            {
                                ProductId = detail.ProductId,
                                ProductName = detail.Product.ProductName,
                                UnitPrice = detail.UnitPrice,
                                SoldAmount = product.SoldAmount + detail.Quantity
                            };

                            products.Remove(product);
                            products.Add(newProduct);

                            saleReport.Income += detail.Quantity * detail.UnitPrice;
                        }
                        else
                        {
                            ProductForReport newProduct = new ProductForReport()
                            {
                                ProductId = detail.ProductId,
                                ProductName = detail.Product.ProductName,
                                UnitPrice = detail.UnitPrice,
                                SoldAmount = detail.Quantity
                            };

                            products.Add(newProduct);

                            saleReport.Income += detail.Quantity * detail.UnitPrice;
                        }
                    }
                }

                saleReport.soldProductList = products;

                return saleReport;
            }
        }
    }
}
