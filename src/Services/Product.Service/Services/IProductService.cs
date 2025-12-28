using Product.Service.DTOs;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync(int page = 1, int pagesize = 20);
    Task<ProductResponse> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pagesize = 20);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<bool> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}
