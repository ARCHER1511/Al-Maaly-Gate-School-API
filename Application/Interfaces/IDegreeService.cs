using Application.DTOs.DegreesDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IDegreeService
    {
        Task<ServiceResult<string>> AddDegreesAsync(string studentId, List<DegreeInput> degreeInputs);
        Task<ServiceResult<StudentDegreesDto>> GetStudentDegreesAsync(string studentId);
        Task<ServiceResult<List<StudentDegreesDto>>> GetAllStudentsDegreesAsync();
        Task<ServiceResult<string>> UpdateDegreeAsync(string degreeId, DegreeInput input);
        Task<ServiceResult<string>> ConvertToComponentsAsync(string degreeId, List<DegreeComponentInput> components);
    }
}
