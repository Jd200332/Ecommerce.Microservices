using Azure;
using Cart.Service.Data;
using Cart.Service.DTOs;
using Cart.Service.Models;
using ECommerce.MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using Polly;
using System.Linq;

namespace Cart.Service.Services
{
    public class CartService : ICartService, IAdminAccess
    {
        private readonly CartDbContext context;
        private readonly ILogger<CartService> logger;
        private readonly IProductCatalogClient productCatalogClient;
        

        public CartService(CartDbContext context, 
            ILogger<CartService> logger,
            IProductCatalogClient productCatalogClient
            )
        {
            this.context = context;
            this.logger = logger;
            this.productCatalogClient = productCatalogClient;

        }

        public async Task<CartData> Addtocart(AddToCartRequest requ, int userId)
        {
            if (requ.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero");
            }

            var product = await productCatalogClient.GetProductAsync(requ.ProductId);

            if (product == null || !product.IsActive)
            {
                throw new KeyNotFoundException($"Product {requ.ProductId} not found");
            }

            var cart = await context.Carts.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == CartStatus.Active);


            if(cart == null)
            {
                cart = new CartData
                {
                    UserId = userId,
                    Status = CartStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                context.Carts.Add(cart);

                await context.SaveChangesAsync();
            }

            var exisitingitem = await context.CartItems.FirstOrDefaultAsync(c => c.CartId == cart.Id && c.ProductId == requ.ProductId);
            var requestedQuantity = requ.Quantity + (exisitingitem?.Quantity ?? 0);

            if (product.StockQuantity < requestedQuantity)
            {
                throw new InvalidOperationException($"Only {product.StockQuantity} units are available for {product.Name}");
            }

            if(exisitingitem != null)
            {
                exisitingitem.Quantity += requ.Quantity;
                exisitingitem.ProductNameSnapshot = product.Name;
                exisitingitem.LockedPrice = product.Price;
            }

            else
            {
                var cartitem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = requ.ProductId,
                    ProductNameSnapshot = product.Name,
                    Quantity = requ.Quantity,
                    LockedPrice = product.Price,
                    AddedAt = DateTime.UtcNow

                };

                context.CartItems.Add(cartitem);

              
            }


            cart.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            cart.SubTotal = await context.CartItems
                .Where(c => c.CartId == cart.Id)
                .SumAsync(c => c.LockedPrice * c.Quantity);
            cart.Total = cart.SubTotal - cart.DiscountAmount;

            await context.SaveChangesAsync();


            return cart;

            //var cart = await context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.Status == CartStatus.Active);


            //if(cart == null)
            //{
            //    cart = new CartData
            //    {
            //        UserId = userId,
            //        Status = CartStatus.Active,
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow

            //    };

            //    context.Carts.Add(cart);

            //    await context.SaveChangesAsync();

            //}

            //var existingitem = await context.CartItems.FirstOrDefaultAsync(c => c.CartId == cart.Id && c.ProductId == requ.ProductId);

            //if(existingitem != null)
            //{
            //    existingitem.Quantity += requ.Quantity; 
            //}

            //else
            //{
            //    var cartitem = new CartItem
            //    {
            //        CartId = cart.Id,
            //        ProductId = requ.ProductId,
            //        Quantity = requ.Quantity,
            //        ProductNameSnapshot = "Product " + requ.ProductId,
            //        LockedPrice = 0,
            //        AddedAt = DateTime.UtcNow
            //    };

            //    context.CartItems.Add(cartitem);
            //}

            //cart.UpdatedAt = DateTime.UtcNow;

            //await context.SaveChangesAsync();

            //var items = await context.CartItems
            //    .Where(ci => ci.CartId == cart.Id)
            //    .ToListAsync();

            //return new GetCartResponse
            //{
            //    Id = cart.Id,
            //    UserId = cart.UserId,
            //    SessionId = cart.SessionId,
            //    Status = cart.Status,
            //    CouponCode = cart.CouponCode,
            //    DiscountAmount = cart.DiscountAmount,
            //    SubTotal = cart.SubTotal,
            //    Total = cart.Total,
            //    CreatedAt = cart.CreatedAt,
            //    UpdatedAt = cart.UpdatedAt,
            //    CartItems = items
            //};


        }

        public async Task ClearCart(GetCartResponse response, int UserId)
        {
            var items = await (from ci in context.CartItems
                               join c in context.Carts
                               on ci.CartId equals c.Id
                               where c.UserId == UserId && c.Status == CartStatus.CheckedOut
                               select ci)
                               .ToListAsync();

            var carts = await (from c in context.Carts
                               where c.UserId == UserId && c.Status == CartStatus.CheckedOut
                               select c
                               ).ToListAsync();

            
            context.CartItems.RemoveRange(items);
            context.Carts.RemoveRange(carts);

            await context.SaveChangesAsync();
        }

        

        public async Task<List<GetCartResponse>> GetCart(int userId, int page = 1, int pagesize = 10)
        {
            var data = (
                from c in context.Carts
                join ci in context.CartItems
                on c.Id equals ci.CartId
                where c.UserId == userId
                orderby c.Id ascending
                select new GetCartResponse
                {
                    Id = c.Id,
                    UserId = c.UserId ?? 0,
                    Status = c.Status,
                    CouponCode = c.CouponCode ?? "",
                    DiscountAmount = c.DiscountAmount,
                    SubTotal = c.SubTotal,
                    Total = c.Total,
                    ProductNameSnapshot = ci.ProductNameSnapshot ?? "",
                    LockedPrice = ci.LockedPrice,
                    Quantity = ci.Quantity
                });

            if(userId == null)
            {
                throw new ArgumentNullException();
            }

            return await data
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                 .ToListAsync();

        }

        public async Task<decimal> GetTotal(int userId)
        {

            var totalamount = await context.Carts.FirstOrDefaultAsync
                (c => c.UserId == userId && c.Status == CartStatus.Active);


            if (totalamount == null)
            {
                throw new ArgumentNullException(nameof(totalamount));
                
            }

            var afterdiscount = totalamount.SubTotal - totalamount.DiscountAmount;
            return afterdiscount * 0.18m;

        }



        public Task RemoveItem(int userId, int productId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateItemQuantity(int userId, int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetCartResponse>> GetCartforadmin(int page = 1, int pagesize = 10)
        {
            return await (from c in context.Carts
                        join m in context.CartItems
                        on c.Id equals m.CartId
                        select new GetCartResponse
                        {
                            UserId = c.UserId,
                            SubTotal = c.SubTotal,
                            Total = c.Total,
                            ProductId = m.ProductId,
                            ProductNameSnapshot = m.ProductNameSnapshot,
                            Quantity = m.Quantity

                        }
                        )
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync();

        }
    }
}
