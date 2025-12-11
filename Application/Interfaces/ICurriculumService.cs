using Application.DTOs.CurriculumDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICurriculumService
    {
        Task<CurriculumDto?> GetByIdAsync(string id);
        Task<IEnumerable<CurriculumDto>> GetAllAsync();
        Task<CurriculumDto> CreateAsync(CreateCurriculumDto dto);
        Task<CurriculumDto?> UpdateAsync(string id, UpdateCurriculumDto dto);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<CurriculumDetailsDto?> GetWithDetailsAsync(string id);
        Task<bool> HasStudentsAsync(string curriculumId);
        Task<bool> HasTeachersAsync(string curriculumId);
    }
}
