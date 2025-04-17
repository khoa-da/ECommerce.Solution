using ECommerce.Shared.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string cartId);
        Task<Cart> GetUserCartAsync(Guid userId);
        Task<Cart> GetGuestCartAsync(string guestId);
        Task<Cart> CreateCartAsync(Guid? userId = null);
        Task<Cart> CreateGuestCartAsync(string guestId);
        Task<Cart> AddToCartAsync(string cartId, CartItem item);
        Task<Cart> UpdateCartItemAsync(string cartId, CartItem item);
        Task<Cart> RemoveFromCartAsync(string cartId, Guid productId);
        Task<bool> DeleteCartAsync(string cartId);
        Task<bool> DeleteGuestCartAsync(string guestId);
        Task<Cart> MergeCartsAsync(string guestCartId, Guid userId);
    }
}
