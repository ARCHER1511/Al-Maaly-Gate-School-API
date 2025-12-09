using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
