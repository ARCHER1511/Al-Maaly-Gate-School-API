using Application.DTOs.DegreesDTOs;
using Domain.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDegreeComponentTypeService
    {
        Task<ServiceResult<DegreeComponentTypeDto>> CreateComponentTypeAsync(CreateDegreeComponentTypeDto dto);
        Task<ServiceResult<DegreeComponentTypeDto>> UpdateComponentTypeAsync(string id, UpdateDegreeComponentTypeDto dto);
        Task<ServiceResult<bool>> DeleteComponentTypeAsync(string id);
        Task<ServiceResult<List<DegreeComponentTypeDto>>> GetComponentTypesBySubjectAsync(string subjectId);
        Task<ServiceResult<DegreeComponentTypeDto>> GetComponentTypeByIdAsync(string id);
        Task<ServiceResult<bool>> ReorderComponentTypesAsync(string subjectId, List<string> componentTypeIds);
    }
}
