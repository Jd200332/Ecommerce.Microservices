using System.Net;
using System.Net.Http.Json;
using Ecommerce.Shared.Models;

namespace Cart.Service.Services
{
    public sealed class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<ProductCatalogClient> logger;

        public ProductCatalogClient(HttpClient httpClient, ILogger<ProductCatalogClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<ProductCatalogItem?> GetProductAsync(int productId, CancellationToken cancellationToken = default)
        {
            using var response = await httpClient.GetAsync($"/api/Product/{productId}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Product service returned {StatusCode} for product {ProductId}", response.StatusCode, productId);
                throw new InvalidOperationException("Product service is unavailable");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductCatalogItem>>(cancellationToken);
            return apiResponse?.Success == true ? apiResponse.Data : null;
        }
    }
}
