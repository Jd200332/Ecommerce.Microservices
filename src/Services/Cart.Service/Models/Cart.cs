namespace Cart.Service.Models
{
    public class CartData
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public CartStatus Status { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string? ProductNameSnapshot { get; set; }
        public decimal LockedPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public enum CartStatus
    {
        Active = 0,
        CheckedOut = 1,
        Expired = 2
    }
}
