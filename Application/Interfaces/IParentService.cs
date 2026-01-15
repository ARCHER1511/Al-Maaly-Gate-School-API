using Application.DTOs.ParentDTOs;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IParentService
    {
        Task<ServiceResult<IEnumerable<ParentViewDto>>> GetAllAsync();
        Task<ServiceResult<ParentViewDto>> GetByIdAsync(object id);
        Task<ServiceResult<ParentCreateUpdateDto>> CreateAsync(ParentCreateUpdateDto dto);
        Task<ServiceResult<ParentCreateUpdateDto>> UpdateAsync(ParentCreateUpdateDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
        Task<ServiceResult<ParentViewWithChildrenDto>> GetParentWithChildrenAsync(string id);
        Task<ServiceResult<List<string>>> UploadParentDocs(IEnumerable<IFormFile> files, string controllerName, string userId);
    }
}
