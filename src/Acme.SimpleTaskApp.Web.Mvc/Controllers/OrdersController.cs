using Microsoft.AspNetCore.Mvc;
using Acme.SimpleTaskApp.Controllers;
using Acme.SimpleTaskApp.Orders;
using Acme.SimpleTaskApp.Orders.Dto;
using Acme.SimpleTaskApp.Web.Models.Orders;
using System.Threading.Tasks;

namespace Acme.SimpleTaskApp.Web.Controllers
{
    public class OrdersController : SimpleTaskAppControllerBase
    {
        private readonly IOrderAppService _orderAppService;

        public OrdersController(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        public async Task<ActionResult> Index()
        {
            var orders = await _orderAppService.GetAllOrders(new PagedOrderResultRequestDto());
            var model = new OrdersViewModel(orders.Items);
            return View(model);
        }

        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusInput input)
        {
            await _orderAppService.UpdateOrderStatus(input);
            return Ok();
        }

        public async Task<ActionResult> EditModal(long orderId)
        {
            var order = await _orderAppService.GetOrder(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrdersViewModel
            {
                Order = order
            };

            return PartialView("_EditModal", model);
        }
    }
}