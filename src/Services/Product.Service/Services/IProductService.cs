using Product.Service.DTOs;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllProductsAsync(int page = 1, int pagesize = 20,
        string? search = null,
            int? categoryId = null,
            decimal? minPrice = null,        // Min price filter
                decimal? maxPrice = null,        // Max price filter
                 bool? inStockOnly = null,        // Only show items with stock > 0
                 string? sortBy = null,           // "name", "price", "stock"
                bool sortDescending = false);
    Task<ProductResponse> GetProductByIdAsync(int id);
    Task<List<ProductResponse>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pagesize = 20);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<bool> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}
