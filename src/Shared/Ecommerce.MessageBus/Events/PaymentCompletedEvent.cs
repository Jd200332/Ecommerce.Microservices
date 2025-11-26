using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.MessageBus.Events
{
    public class PaymentCompletedEvent
    {
        public int orderid { get; set; }
        public string transactionid { get; set; }
        public decimal amount { get; set; } 
        public string status { get; set; }  

        public DateTime completedat { get; set; }
    }
}
