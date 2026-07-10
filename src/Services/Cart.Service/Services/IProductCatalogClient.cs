namespace Cart.Service.Services
{
    public interface IProductCatalogClient
    {
        Task<ProductCatalogItem?> GetProductAsync(int productId, CancellationToken cancellationToken = default);
    }

    public sealed class ProductCatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}
