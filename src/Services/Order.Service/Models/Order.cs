using System.ComponentModel.DataAnnotations;

namespace Order.Service.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";
        
        public string ShippingAddress { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }  
        public string ZipCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; }

        public ICollection<Orderpayment> OrderPayments { get; set; }

        public ICollection<Orderstatushistory> StatusHistories { get; set; }
    }

    public class OrderItem 
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }    
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }

        public bool Isreturned { get; set; }
    }

    public class Outboxmessage
    {
        public long Id { get; set; }
        public string EventType { get; set; }

        public string EventPayload { get; set; }
        public string Topic {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string Status {  get; set; } 

        public int RetryCount { get; set; }
    }

    public class Orderpayment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }    

        public Order Order { get; set; }    

        public string PaymentIntentId {  get; set; }

        public string PaymentMethod { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string GatewayResponse {  get; set; }
    }

    public class Orderstatushistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public string Fromstatus { get; set; }

        public string ToStatus { get; set; }

        public string ChangedBy { get; set; }

        public string Reason { get; set; }

        public DateTime ChangedAt { get; set; }

    }
}
