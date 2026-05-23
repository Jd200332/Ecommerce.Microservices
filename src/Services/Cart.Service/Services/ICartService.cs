using Cart.Service.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Cart.Service.Services
{
   
    public interface ICartService
    {
        Task<Cart.Service.Models.Cart> GetCart(int userId);
        Task<CartItem> GetCartItem(int userId);

        Task AddItem(int userId, int productId, int quantity, string ProductName, decimal price);

        Task UpdateItemQuantity(int userId, int productId, int quantity);

        Task RemoveItem(int userId, int productId);

        Task ClearCart(int ProductId, string ProductName, decimal Price, int quantity);

        Task<decimal> GetTotal(int userId);
    }
}
