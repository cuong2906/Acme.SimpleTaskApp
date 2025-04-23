using System.Threading.Tasks;
using Abp.Application.Services;
using Acme.SimpleTaskApp.Cart.Dto;

namespace Acme.SimpleTaskApp.Cart
{
    public interface ICartAppService : IApplicationService
    {
        Task<CartDto> GetCart();
        Task AddToCart(AddToCartInput input);
        Task UpdateCartItem(UpdateCartItemInput input);
        Task RemoveFromCart(RemoveFromCartInput input);
        Task Checkout();
    }
} 