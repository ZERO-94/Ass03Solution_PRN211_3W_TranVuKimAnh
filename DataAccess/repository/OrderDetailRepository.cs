using BusinessObject;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public void AddOrderDetail(OrderDetail orderDetail) => OrderDetailDAO.Instance.AddNewOrderDetail(orderDetail);
        public List<OrderDetail> GetOrderDetailByOrderId(int id) => OrderDetailDAO.Instance.GetOrderDetailByOrderId(id);
    }
}
