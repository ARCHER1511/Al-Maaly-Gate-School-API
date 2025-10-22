using Application.DTOs.ExamDTOS;
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
        Task<ServiceResult<IEnumerable<ExamViewDto>>> GetByTeacherAsync(string teacherId);
        Task<ServiceResult<ExamViewDto>> CreateExamForTeacherAsync(string teacherId, CreateExamDto dto);
        Task<ServiceResult<bool>> AssignQuestionsAsync(string teacherId, int examId, IEnumerable<int> questionIds);
        Task<ServiceResult<ExamViewDto>> GetByIdAsync(int examId);
        Task<ServiceResult<bool>> DeleteAsync(int examId);

    }
}
