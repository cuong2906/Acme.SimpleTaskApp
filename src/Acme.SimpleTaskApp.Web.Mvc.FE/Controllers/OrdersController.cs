using Microsoft.AspNetCore.Mvc;
using Acme.SimpleTaskApp.Controllers;
using Acme.SimpleTaskApp.Orders;
using Acme.SimpleTaskApp.Orders.Dto;
using Acme.SimpleTaskApp.Web.Models.Orders;
using System.Threading.Tasks;
using Abp.Runtime.Session;

namespace Acme.SimpleTaskApp.Web.Controllers
{
    public class OrdersController : SimpleTaskAppControllerBase
    {
        private readonly IOrderAppService _orderAppService;
        private readonly IAbpSession _abpSession;

        public OrdersController(IOrderAppService orderAppService, IAbpSession abpSession)
        {
            _orderAppService = orderAppService;
            _abpSession = abpSession;
        }

        public async Task<ActionResult> Index()
        {
            var input = new PagedOrderResultRequestDto
            {
                UserId = _abpSession.GetUserId()
            };
            var orders = await _orderAppService.GetAllOrders(input);
            var model = new IndexViewModel(orders.Items);
            return View(model);
        }

        public async Task<ActionResult> Details(long id)
        {
            var order = await _orderAppService.GetOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra xem hóa đơn có phải của người dùng hiện tại không
            if (order.UserId != _abpSession.GetUserId())
            {
                return Forbid();
            }

            return View(order);
        }

        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusInput input)
        {
            input.Status = (OrderStatus)input.Status;
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

            var model = new IndexViewModel
            {
                Order = order
            };

            return PartialView("_EditModal", model);
        }
    }
} 