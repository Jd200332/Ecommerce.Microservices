using Cart.Service.Models;

namespace Cart.Service.DTOs
{
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }  

    }

    public class GetCartResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public CartStatus Status { get; set; } = CartStatus.Active;
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }

        public int ProductId { get; set; }

        public string? ProductNameSnapshot { get; set; }

        public decimal LockedPrice { get; set; }
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
