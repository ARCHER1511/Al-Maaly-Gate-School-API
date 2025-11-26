using Application.DTOs.DegreesDTOs;
using Domain.Entities;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IDegreeService
    {
        Task<ServiceResult<string>> AddDegreesAsync(string studentId, List<Degree> degrees);
        Task<ServiceResult<StudentDegreesDto>> GetStudentDegreesAsync(string studentId);
        Task<ServiceResult<List<StudentDegreesDto>>> GetAllStudentsDegreesAsync();
    }
}
