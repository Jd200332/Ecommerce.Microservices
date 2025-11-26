using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.MessageBus.Events
{
    public class OrderCreatedEvent
    {
        public int orderid { get; set; }
        public string userid { get; set; }  
        public decimal totalamount { get; set; }    
        public DateTime createdat { get; set; } 
        public List<OrderItemEvent> items { get; set; }
    }

    public class OrderItemEvent
    {
        public int productid { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
