using Abp.Application.Services.Dto;
using Abp.Application.Services;
using System.Threading.Tasks;

public interface ILookupAppService : IApplicationService
{
    Task<ListResultDto<ComboboxItemDto>> GetPeopleComboboxItems();
}