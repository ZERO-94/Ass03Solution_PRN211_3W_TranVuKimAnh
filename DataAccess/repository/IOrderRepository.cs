using BusinessObject;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public interface IOrderRepository
    {
        public bool CreateOrder(Order newOrder);

        public bool DeleteOrder(int id);

        public bool UpdateOrder(int id, Order updatedOrderInfo);

        public Order GetOrderById(int id);

        public List<Order> GetOrderByMemberId(int memberId);

        public List<Order> GetOrderByDateRange(DateTime startDate, DateTime endDate);

        public List<Order> GetAllOrders();

        public SaleReport GetSaleReport(DateTime startDate, DateTime endDate);
    }
}
