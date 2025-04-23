using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Acme.SimpleTaskApp.Authorization;
using Acme.SimpleTaskApp.Orders;
using Acme.SimpleTaskApp.Orders.Dto;
using Acme.SimpleTaskApp.Cart;
using Acme.SimpleTaskApp.Cart.Dto;

namespace Acme.SimpleTaskApp
{
    [DependsOn(
        typeof(SimpleTaskAppCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class SimpleTaskAppApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<SimpleTaskAppAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(SimpleTaskAppApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                cfg => {
                    cfg.CreateMap<Order, OrderDto>();
                    cfg.CreateMap<OrderItem, OrderItemDto>();
                    cfg.CreateMap<Cart.Cart, CartDto>();
                    cfg.CreateMap<CartItem, CartItemDto>();
                    cfg.AddMaps(thisAssembly);
                }
            );
        }
    }
}
