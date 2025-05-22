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

        public async Task<ActionResult> Index(string keyword = null, int? categoryId = null, 
            decimal? minPrice = null, decimal? maxPrice = null, int skipCount = 0, int maxResultCount = 9)
        {
            // Store current filter values in ViewBag for pagination
            ViewBag.CurrentKeyword = keyword;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentPage = (skipCount / maxResultCount) + 1;

            var input = new PagedProductDto
            {
                Keyword = keyword,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SkipCount = skipCount,
                MaxResultCount = maxResultCount,
                Sorting = "CreationTime DESC"
            };

            var products = await _productAppService.GetProductPaged(input);
            var categories = await _categoryAppService.GetAllCategories(new PagedCategoriesDto());
            
            var categoriesItems = categories.Items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameCategory,
                Selected = categoryId == c.Id
            }).ToList();

            var model = new IndexViewModel(products.Items, categoriesItems)
            {
                CurrentPage = (skipCount / maxResultCount) + 1,
                PageSize = maxResultCount,
                TotalCount = products.TotalCount,
                CurrentKeyword = keyword,
                CurrentMinPrice = minPrice,
                CurrentMaxPrice = maxPrice,
                CurrentCategoryId = categoryId
            };

            return View(model);
        }

        public async Task<ActionResult> Detail(int id)
        {
            var product = await _productAppService.GetProducts(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<ActionResult> Search(PagedProductDto input)
        {
            // Ensure we have valid pagination parameters
            input.MaxResultCount = input.MaxResultCount <= 0 ? 9 : input.MaxResultCount;
            input.SkipCount = input.SkipCount < 0 ? 0 : input.SkipCount;
            input.Sorting = string.IsNullOrEmpty(input.Sorting) ? "CreationTime DESC" : input.Sorting;

            ViewBag.CurrentPage = (input.SkipCount / input.MaxResultCount) + 1;

            var result = await _productAppService.GetProductPaged(input);
            return PartialView("_ProductList", result);
        }

        public async Task<ActionResult> Filter(PagedProductDto input)
        {
            // Ensure we have valid pagination parameters
            input.MaxResultCount = input.MaxResultCount <= 0 ? 9 : input.MaxResultCount;
            input.SkipCount = input.SkipCount < 0 ? 0 : input.SkipCount;
            input.Sorting = string.IsNullOrEmpty(input.Sorting) ? "CreationTime DESC" : input.Sorting;

            var result = await _productAppService.GetProductPaged(input);
            return PartialView("_ProductList", result);
        }
    }
}
