using Application.DTOs.ExamDTOS;
using Application.DTOs.QuestionDTOs;
using Domain.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExamService
    {
        Task<ServiceResult<IEnumerable<ExamViewDto>>> GetAllAsync();
        Task<ServiceResult<ExamDetailsViewDto>> GetByIdAsync(string id);
        Task<ServiceResult<IEnumerable<ExamViewDto>>> GetByTeacherAsync(string teacherId);
        Task<ServiceResult<ExamViewDto>> CreateExamForTeacherAsync(string teacherId, CreateExamDto dto);
        Task<ServiceResult<ExamViewDto>> UpdateAsync(string id, UpdateExamDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
        Task<ServiceResult<ExamDetailsViewDto>> CreateExamWithQuestionsAsync(CreateExamWithQuestionsDto dto);
        Task<ServiceResult<ExamDetailsViewDto>> GetExamWithQuestionsAsync(string examId);
    }
}
