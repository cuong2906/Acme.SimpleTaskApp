using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Acme.SimpleTaskApp.Controllers;
using Acme.SimpleTaskApp.Web.Models.Home;
using Acme.SimpleTaskApp.Orders;

namespace Acme.SimpleTaskApp.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : SimpleTaskAppControllerBase
    {
        private readonly IOrderAppService _orderAppService;

        public HomeController(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        public async Task<ActionResult> Index()
        {
            var model = new IndexViewModel
            {
                TopProducts = await _orderAppService.GetTopProductsByOrderQuantity(5)
            };

            return View(model);
        }
    }
}
