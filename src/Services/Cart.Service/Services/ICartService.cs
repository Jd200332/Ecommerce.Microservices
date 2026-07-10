using Cart.Service.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Cart.Service.DTOs;

namespace Cart.Service.Services
{
   
    public interface ICartService
    {
        Task <List<GetCartResponse>> GetCart(int userId, int page = 1, int pagesize = 10);
        
        Task<CartData> Addtocart(AddToCartRequest requ, int userId);

        Task UpdateItemQuantity(int userId, int productId, int quantity);

        Task RemoveItem(int userId, int productId);

        Task ClearCart(GetCartResponse response, int UserId);

        Task<decimal> GetTotal(int userId);

    }

    public interface IAdminAccess
    {

        Task<List<GetCartResponse>> GetCartforadmin(int page = 1, int pagesize = 10 );
    }
}



