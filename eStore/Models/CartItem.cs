using BusinessObject;
using BusinessObject.Models;

namespace eStore.Models
{
    public class CartItem
    {
        public int quantity { get; set; }
        public Product product { get; set; }
        public int discount { get; set; }
    }
}
