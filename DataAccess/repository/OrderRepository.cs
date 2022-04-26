using BusinessObject;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public class OrderRepository : IOrderRepository
    {

        private OrderDAO daoInstance = new OrderDAO();

        public bool CreateOrder(Order newOrder) => daoInstance.CreateOrder(newOrder);

        public bool DeleteOrder(int id) => daoInstance.DeleteOrder(id);

        public List<Order> GetAllOrders() => daoInstance.GetAllOrders();

        public List<Order> GetOrderByDateRange(DateTime startDate, DateTime endDate) => daoInstance.GetOrderByDateRange(startDate, endDate);

        public Order GetOrderById(int id) => daoInstance.GetOrderById(id);

        public List<Order> GetOrderByMemberId(int memberId) => daoInstance.GetOrderByMemberId(memberId);

        public bool UpdateOrder(int id, Order updatedOrderInfo) => daoInstance.UpdateOrder(id, updatedOrderInfo);

        public SaleReport GetSaleReport(DateTime startDate, DateTime endDate)
        {
            return daoInstance.GetSaleReport(startDate, endDate);
        }
    }
}
