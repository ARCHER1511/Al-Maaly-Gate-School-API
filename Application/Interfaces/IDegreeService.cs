using Application.DTOs.DegreesDTOs;
using Domain.Entities;
using Domain.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDegreeService
    {
        Task<ApiResponse<string>> AddDegreesAsync(string studentId, List<Degree> degrees);
        Task<ApiResponse<StudentDegreesDto>> GetStudentDegreesAsync(string studentId);
        Task<ApiResponse<List<StudentDegreesDto>>> GetAllStudentsDegreesAsync();
    }
}
