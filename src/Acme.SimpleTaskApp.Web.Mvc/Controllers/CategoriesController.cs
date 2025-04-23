using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Acme.SimpleTaskApp.Categories.Dto;
using System.Threading.Tasks;
using Acme.SimpleTaskApp.Web.Models.Categories;
using Acme.SimpleTaskApp.Categories;
using System;
using Abp.UI;
using Abp.AspNetCore.Mvc.Authorization;
using Acme.SimpleTaskApp.Authorization;

namespace Acme.SimpleTaskApp.Web.Mvc.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Categories)]
    public class CategoriesController : AbpController
    {
        private readonly ICategoriesAppService _categoriesAppService;

        public CategoriesController(ICategoriesAppService categoriesAppService)
        {
            _categoriesAppService = categoriesAppService;
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _categoriesAppService.GetAllCategories(new PagedCategoriesDto());
            var viewModel = new IndexViewModel(categories.Items);
            return View(viewModel);
        }

        public async Task<ActionResult> EditModal(int id)
        {
           
                var category = await _categoriesAppService.GetCategories(id);
                var model = new IndexViewModel
                {
                    Category = category
                };

                return PartialView("_EditModal", model);
            
        }
    }
}


// View() là sẽ trả về 1 trang Html đầy đủ, nó sẽ tìm trong folder Views
// PartialView() là sẽ trả về 1 phần của Html ,(Modal, Popup) 
// Ok() là trả về Json , dùng khi chỉ muốn lấy dữ liệu và hiển thị thông báo 