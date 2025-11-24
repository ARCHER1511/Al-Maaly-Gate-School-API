using Application.DTOs.ExamDTOS;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ExamDetailsViewDto>> CreateExamWithQuestionsAsync(CreateExamWithQuestionsDto dto)
        {
            var examRepo = _unitOfWork.Repository<Exam>();
            var questionRepo = _unitOfWork.Repository<Question>();

            // Fetch existing questions by IDs
            var questions = await questionRepo.FindAllAsync(q => dto.QuestionIds.Contains(q.Id));

            if (!questions.Any())
                return ServiceResult<ExamDetailsViewDto>.Fail("No valid questions found for this exam.");

            // Create exam and assign questions
            var exam = new Exam
            {
                ExamName = dto.ExamName,
                SubjectId = dto.SubjectId,
                ClassId = dto.ClassId,
                TeacherId = dto.TeacherId,
                Start = dto.Start,
                End = dto.End,
                MinMark = dto.MinMark,
                FullMark = dto.FullMark,
                Status = dto.Status,
                Questions = questions.ToList()
            };


            await examRepo.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            // Reload exam with full related data
            var loadedExam = await examRepo.AsQueryable(e => e.Id == exam.Id)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices)
                .Include(e => e.Questions)
                //    .ThenInclude(q => q.TextAnswer)
                //.Include(e => e.Questions)
                //    .ThenInclude(q => q.TrueAndFalses)
                .FirstOrDefaultAsync();

            var result = _mapper.Map<ExamDetailsViewDto>(loadedExam);
            return ServiceResult<ExamDetailsViewDto>.Ok(result, "Exam created successfully with questions.");

        }

        public async Task<ServiceResult<ExamDetailsViewDto>> GetExamWithQuestionsAsync(string examId)
        {
            var examRepo = _unitOfWork.Repository<Exam>();

            var exam = await examRepo.AsQueryable(e => e.Id == examId)
                .Include(e => e.Subject)
                .Include(e => e.Class)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync();


            if (exam == null)
                return ServiceResult<ExamDetailsViewDto>.Fail("Exam not found.");

            var result = _mapper.Map<ExamDetailsViewDto>(exam);
            return ServiceResult<ExamDetailsViewDto>.Ok(result);
        }

        public async Task<ServiceResult<IEnumerable<ExamViewDto>>> GetAllAsync()
        {
            var repo = _unitOfWork.Repository<Exam>();

            var exams = await repo.AsQueryable()
                .Include(e => e.Subject)
                .Include(e => e.Class)
                .ToListAsync();

            var result = exams.Select(_mapper.Map<ExamViewDto>);
            return ServiceResult<IEnumerable<ExamViewDto>>.Ok(result);
        }


        public async Task<ServiceResult<ExamDetailsViewDto>> GetByIdAsync(string id)
        {
            var repo = _unitOfWork.Repository<Exam>();

            var exam = await repo.AsQueryable(e => e.Id == id)
                .Include(e => e.Subject)
                .Include(e => e.Class)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync();

            if (exam == null)
                return ServiceResult<ExamDetailsViewDto>.Fail("Exam not found");

            var mapped = _mapper.Map<ExamDetailsViewDto>(exam);
            return ServiceResult<ExamDetailsViewDto>.Ok(mapped);
        }


        public async Task<ServiceResult<IEnumerable<ExamViewDto>>> GetByTeacherAsync(string teacherId)
        {
            var repo = _unitOfWork.Repository<Exam>();

            var exams = await repo.AsQueryable(e => e.TeacherId == teacherId)
                .Include(e => e.Subject)
                .Include(e => e.Class)
                .ToListAsync();

            var result = exams.Select(_mapper.Map<ExamViewDto>);
            return ServiceResult<IEnumerable<ExamViewDto>>.Ok(result);
        }


        public async Task<ServiceResult<ExamViewDto>> CreateExamForTeacherAsync(string teacherId, CreateExamDto dto)
        {
            var repo = _unitOfWork.Repository<Exam>();
            var exam = _mapper.Map<Exam>(dto);
            exam.TeacherId = teacherId;

            await repo.AddAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<ExamViewDto>.Ok(_mapper.Map<ExamViewDto>(exam));
        }

        public async Task<ServiceResult<ExamViewDto>> UpdateAsync(string id, UpdateExamDto dto)
        {
            var repo = _unitOfWork.Repository<Exam>();
            var exam = await repo.GetByIdAsync(id);
            if (exam == null)
                return ServiceResult<ExamViewDto>.Fail("Exam not found");

            _mapper.Map(dto, exam);
            repo.Update(exam);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<ExamViewDto>.Ok(_mapper.Map<ExamViewDto>(exam));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var examRepo = _unitOfWork.Repository<Exam>();
            var questionRepo = _unitOfWork.Repository<Question>();

            var exam = await examRepo.GetByIdAsync(id);
            if (exam == null)
                return ServiceResult<bool>.Fail("Exam not found");

            // Get all questions related to this exam and set their ExamId to null
            var questions = await questionRepo.FindAllAsync(
                predicate: q => q.ExamId == id,
                include: null // Add include if you need related entities
            );

            foreach (var question in questions)
            {
                question.ExamId = null; // This will break the relationship
                questionRepo.Update(question);
            }

            examRepo.Delete(exam);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
    }
}
