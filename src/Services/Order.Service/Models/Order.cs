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
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }    
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}
