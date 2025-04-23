using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Acme.SimpleTaskApp.Categories;
using Acme.SimpleTaskApp.Categories.Dto;
using Acme.SimpleTaskApp.Controllers;
using Acme.SimpleTaskApp.Products;
using Acme.SimpleTaskApp.Products.Dto;
using Acme.SimpleTaskApp.Web.Models.Products;
using System.Linq;
using System.Threading.Tasks;
using Acme.SimpleTaskApp.Categories;
using System;

namespace Acme.SimpleTaskApp.Web.Controllers
{
    public class ProductsController : SimpleTaskAppControllerBase
    {

        private readonly IProductAppService _productAppService;
        private readonly ICategoriesAppService _categoryAppService;

        public ProductsController(IProductAppService productAppService, ICategoriesAppService categoryAppService)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
        }

        public async Task<ActionResult> Index()
        {
            var products = await _productAppService.GetProductPaged(new PagedProductDto());

            var categories = await _categoryAppService.GetAllCategories(new PagedCategoriesDto());
            var categoriesItems = categories.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory
            }).ToList();
            var model = new IndexViewModel(products.Items, categoriesItems);
            return View(model);
        }

        public async Task<IActionResult> Create(ProductListDto input)
        {
            await _productAppService.Create(input);
            return Ok();
        }

        public async Task<IActionResult> Update(UpdateProductDto input)
        {
            await _productAppService.Update(input);
            return Ok();
        }

        public async Task<ActionResult> EditModal(int productId)
        {
            var product = await _productAppService.GetProducts(productId);
            var categories = await _categoryAppService.GetAllCategories(new PagedCategoriesDto());
            var model = new IndexViewModel
            {
                Product = product,
                Categories = categories.Items.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NameCategory,
                    Selected = (x.Id == product.CategoryId)
                }).ToList()
            };

            return PartialView("_EditModal", model);
        }
    }
}
