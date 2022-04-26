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
    public class OrderDetailDAO
    {
        private static OrderDetailDAO instance = null;
        private static readonly object instanceLock = new object();
        public static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }
        //------------------------------------------------------------

        public List<OrderDetail> GetOrderDetailByOrderId(int id)
        {
            var orders = new List<OrderDetail>();
            try
            {
                using var context = new AssignmentPRN211DBContext();
                orders = context.OrderDetails.Include(o => o.Product).Include(o => o.Order).Where(o => o.OrderId == id).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orders;
        }

        public void AddNewOrderDetail(OrderDetail order)
        {
            try
            {
                if (order != null)
                {
                    using var context = new AssignmentPRN211DBContext();
                    context.OrderDetails.Add(order);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Something went wrong, please try again later!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
