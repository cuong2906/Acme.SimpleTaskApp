using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Acme.SimpleTaskApp.Categories.Dto;
using System.Threading.Tasks;

namespace Acme.SimpleTaskApp.Categories
{
    public interface ICategoriesAppService : IApplicationService
    {
        Task<CategoriesDto> GetCategories(int id);

        Task<PagedResultDto<CategoriesDto>> GetAllCategories(PagedCategoriesDto input);

        Task Create(CategoriesDto input);

        Task Update(CreateCategoriesDto input);

        Task Delete(int id);



        //PagedResultDto<CategoriesDto> GetAll(PagedCategoriesDto input);
        //Task<PagedResultDto<CategoriesDto>> GetListAsync(PagedCategoriesDto input);
    }
}
