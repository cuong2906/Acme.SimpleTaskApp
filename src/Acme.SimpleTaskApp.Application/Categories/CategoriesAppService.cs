using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Acme.SimpleTaskApp.Categories.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Acme.SimpleTaskApp.Entities.Categories;
using Acme.SimpleTaskApp.Entities.Products;
using Acme.SimpleTaskApp.Categories;


namespace Acme.SimpleTaskApp.Categories
{
    public class CategoriesAppService : SimpleTaskAppAppServiceBase, ICategoriesAppService
    {

        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;

        public CategoriesAppService(IRepository<Category> categoryRepository, IRepository<Product> productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task Create(CategoriesDto input)
        {
            var category = ObjectMapper.Map<Category>(input);
            await _categoryRepository.InsertAsync(category);
        }

        public async Task Delete(int id)
        {
            var category = await _categoryRepository.GetAsync(id);

            // Kiểm tra xem danh mục có sản phẩm con không
            var hasProducts = await _productRepository.GetAll().AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                throw new UserFriendlyException("Không thể xóa vì danh mục này đang chứa sản phẩm!");
            }

            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<CategoriesDto>> GetAllCategories(PagedCategoriesDto input)
        {
            var categories = _categoryRepository.GetAll();
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                categories = categories.Where(p => p.NameCategory.Contains(input.Keyword));
            }

            var count = await categories.CountAsync();

            input.Sorting = "CreationTime DESC";

            var items = await categories.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<CategoriesDto> { TotalCount = count, Items = ObjectMapper.Map<List<CategoriesDto>>(items) };
        }

        public async Task<CategoriesDto> GetCategories(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            return ObjectMapper.Map<CategoriesDto>(category);
        }

        public async Task Update(CreateCategoriesDto input)
        {
            var category = await _categoryRepository.GetAsync(input.Id);
            category.NameCategory = input.NameCategory;
            await _categoryRepository.UpdateAsync(category);
        }
    }
}
