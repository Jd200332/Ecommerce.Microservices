using Azure;
using Cart.Service.Data;
using Cart.Service.Models;
using ECommerce.MessageBus;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cart.Service.Services
{
    public class CartService : ICartService
    {
        private readonly CartDbContext context;
        private readonly IMessageBus messageBus;
        private readonly ILogger<CartService> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public CartService(CartDbContext context, IMessageBus messageBus, ILogger<CartService> logger, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            this.messageBus = messageBus;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public  async Task AddItem(int userId, int productId, int quantity, string ProductName, decimal Price)
        {
            if(quantity <= 0)
            {
                throw new ArgumentException("Quantity is less than 0");
            }

            var cart = await context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if(cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };


                context.Cart.Add(cart);
            }
            
            var item = cart.CartItems
                .FirstOrDefault(x => x.ProductId == productId);


            if(item != null)
            {
                item.Quantity += quantity;
            }

            if(ProductName == null)
            {
                throw new InvalidOperationException("Name cannot be empty");
            }

            if(Price < 0)
            {
                throw new InvalidOperationException("Enter the correct amount");
            }


            await context.SaveChangesAsync();

        }

        public async Task ClearCart(int ProductId, string ProductName, decimal Price, int quantity)
        {
            var remove = context.CartItems
                .Where(x => x.ProductId == ProductId)
                .Where(x => x.ProductName == ProductName)
                .FirstOrDefaultAsync();

            var delete = context.Cart.Remove;
                await context.SaveChangesAsync();

            if(remove != null)
            {
                context.CartItems.RemoveRange(context.CartItems.Where(x => x.ProductId == ProductId));
                await context.SaveChangesAsync();
            }


            if(remove.IsCompletedSuccessfully)
            {
                logger.LogInformation("Cart cleared successfully for ProductId: {ProductId}, ProductName: {ProductName}", ProductId, ProductName);

            }


        }

        public  Task<Models.Cart> GetCart(int userId)
        {
            throw new NotImplementedException();
        }

        public  Task<CartItem> GetCartItem(int userId)
        {
            throw new NotImplementedException();
        }

        public  Task<decimal> GetTotal(int userId)
        {
            throw new NotImplementedException();
        }

        public  Task RemoveItem(int userId, int productId)
        {
            throw new NotImplementedException();
        }

        public  Task UpdateItemQuantity(int userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
