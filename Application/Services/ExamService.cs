using Application.DTOs.ExamDTOS;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IQuestionExamTeacherRepository _qetRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(
            IExamRepository examRepo,
            IQuestionRepository questionRepo,
            IQuestionExamTeacherRepository qetRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _examRepo = examRepo;
            _questionRepo = questionRepo;
            _qetRepo = qetRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ExamViewDto>>> GetByTeacherAsync(string teacherId)
        {
            var exams = await _examRepo.GetByTeacherIdAsync(teacherId);
            var data = exams.Select(_mapper.Map<ExamViewDto>);
            return ServiceResult<IEnumerable<ExamViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<ExamViewDto>> CreateExamForTeacherAsync(string teacherId, CreateExamDto dto)
        {
            var exam = _mapper.Map<Exam>(dto);
            //exam.TeacherId = teacherId;

            await _examRepo.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            foreach (var qid in dto.QuestionIds)
            {
                var question = await _questionRepo.GetByIdAsync(qid);
                if (question == null) continue;

                var qet = new QuestionExamTeacher
                {
                    ExamId = exam.Id,
                    QuestionId = qid,
                    TeacherId = teacherId
                };
                await _qetRepo.AddAsync(qet);
            }

            await _unitOfWork.SaveChangesAsync();

            var createdExam = await _examRepo.GetByIdWithQuestionsAsync(exam.Id);
            return ServiceResult<ExamViewDto>.Ok(_mapper.Map<ExamViewDto>(createdExam!), "Exam created successfully");
        }

        public async Task<ServiceResult<bool>> AssignQuestionsAsync(string teacherId, int examId, IEnumerable<int> questionIds)
        {
            var exam = await _examRepo.GetByIdWithQuestionsAsync(examId);
            if (exam == null)
                return ServiceResult<bool>.Fail("Exam not found");

            foreach (var qid in questionIds)
            {
                if (exam.QuestionExamTeachers.Any(x => x.QuestionId == qid && x.TeacherId == teacherId))
                    continue;

                var question = await _questionRepo.GetByIdAsync(qid);
                if (question == null) continue;

                var qet = new QuestionExamTeacher
                {
                    ExamId = exam.Id,
                    QuestionId = qid,
                    TeacherId = teacherId
                };

                await _qetRepo.AddAsync(qet);
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Questions assigned successfully");
        }

        public async Task<ServiceResult<ExamViewDto>> GetByIdAsync(int examId)
        {
            var exam = await _examRepo.GetByIdWithQuestionsAsync(examId);
            if (exam == null)
                return ServiceResult<ExamViewDto>.Fail("Exam not found");

            return ServiceResult<ExamViewDto>.Ok(_mapper.Map<ExamViewDto>(exam));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int examId)
        {
            var exam = await _examRepo.GetByIdAsync(examId);
            if (exam == null)
                return ServiceResult<bool>.Fail("Exam not found");

            _examRepo.Delete(exam);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Exam deleted successfully");
        }
    }
}
