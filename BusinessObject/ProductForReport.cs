using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class ProductForReport
    {

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public int SoldAmount { get; set; }
    }
}
