using Application.DTOs.ClassDTOs;
using Application.DTOs.GradeDTOs;
using Application.DTOs.SubjectDTOs;
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
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepo;
        private readonly IClassRepository _classRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GradeService(IGradeRepository gradeRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _gradeRepo = gradeRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<GradeViewDto>>> GetAllAsync()
        {
            var grades = await _gradeRepo.GetAllAsync();
            var data = _mapper.Map<IEnumerable<GradeViewDto>>(grades);
            return ServiceResult<IEnumerable<GradeViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<GradeViewDto>> GetByIdAsync(string id)
        {
            var grade = await _gradeRepo.GetByIdAsync(id);
            if (grade == null)
                return ServiceResult<GradeViewDto>.Fail("Grade not found");

            return ServiceResult<GradeViewDto>.Ok(_mapper.Map<GradeViewDto>(grade));
        }

        public async Task<ServiceResult<GradeViewDto>> GetByNameAsync(string gradeName)
        {
            var grade = await _gradeRepo.GetByNameAsync(gradeName);
            if (grade == null)
                return ServiceResult<GradeViewDto>.Fail("Grade not found");

            return ServiceResult<GradeViewDto>.Ok(_mapper.Map<GradeViewDto>(grade));
        }

        public async Task<ServiceResult<GradeViewDto>> CreateAsync(CreateGradeDto dto)
        {
            // Check if grade name already exists
            var existingGrade = await _gradeRepo.GetByNameAsync(dto.GradeName);
            if (existingGrade != null)
                return ServiceResult<GradeViewDto>.Fail("Grade name already exists");

            var grade = _mapper.Map<Grade>(dto);
            grade.Id = Guid.NewGuid().ToString();

            await _gradeRepo.AddAsync(grade);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<GradeViewDto>(grade);
            return ServiceResult<GradeViewDto>.Ok(result, "Grade created successfully");
        }

        public async Task<ServiceResult<GradeViewDto>> UpdateAsync(string id, UpdateGradeDto dto)
        {
            var grade = await _gradeRepo.GetByIdAsync(id);
            if (grade == null)
                return ServiceResult<GradeViewDto>.Fail("Grade not found");

            // Check if new grade name conflicts with existing grade
            if (dto.GradeName != grade.GradeName)
            {
                var existingGrade = await _gradeRepo.GetByNameAsync(dto.GradeName);
                if (existingGrade != null && existingGrade.Id != id)
                    return ServiceResult<GradeViewDto>.Fail("Grade name already exists");
            }

            _mapper.Map(dto, grade);
            _gradeRepo.Update(grade);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<GradeViewDto>.Ok(_mapper.Map<GradeViewDto>(grade), "Grade updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var grade = await _gradeRepo.GetByIdWithDetailsAsync(id);
            if (grade == null)
                return ServiceResult<bool>.Fail("Grade not found");

            // Check if grade has classes or subjects
            if (grade.Classes.Any() || grade.Subjects.Any())
                return ServiceResult<bool>.Fail("Cannot delete grade that has classes or subjects. Please remove them first.");

            _gradeRepo.Delete(grade);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Grade deleted successfully");
        }

        public async Task<ServiceResult<ClassViewDto>> AddClassToGradeAsync(string gradeId, ClassDto dto)
        {
            // This should be for EXISTING classes - just update the GradeId
            var grade = await _gradeRepo.GetByIdAsync(gradeId);
            if (grade == null)
                return ServiceResult<ClassViewDto>.Fail("Grade not found");

            // Check if class already exists (since this is ClassDto with ID)
            var existingClass = await _classRepository.GetByIdAsync(dto.Id);
            if (existingClass == null)
                return ServiceResult<ClassViewDto>.Fail("Class not found");

            // Update the class's grade assignment
            existingClass.GradeId = gradeId;
            _classRepository.Update(existingClass);

            await _unitOfWork.SaveChangesAsync();

            // Reload with includes
            var updatedClass = await _classRepository.GetByIdAsync(dto.Id);
            var result = _mapper.Map<ClassViewDto>(updatedClass ?? existingClass);
            return ServiceResult<ClassViewDto>.Ok(result, "Class assigned to grade successfully");
        }

        public async Task<ServiceResult<ClassViewDto>> AddClassToGradeAsync(string gradeId, CreateClassInGradeDto dto)
        {
            // This should be for NEW classes - create from scratch
            var grade = await _gradeRepo.GetByIdAsync(gradeId);
            if (grade == null)
                return ServiceResult<ClassViewDto>.Fail("Grade not found");

            var classEntity = _mapper.Map<Class>(dto);
            classEntity.Id = Guid.NewGuid().ToString();
            classEntity.GradeId = gradeId;

            await _classRepository.AddAsync(classEntity);
            await _unitOfWork.SaveChangesAsync();

            // Reload with includes for complete data
            var createdClass = await _classRepository.GetByIdAsync(classEntity.Id);
            var result = _mapper.Map<ClassViewDto>(createdClass ?? classEntity);
            return ServiceResult<ClassViewDto>.Ok(result, "Class created in grade successfully");
        }

        public async Task<ServiceResult<SubjectViewDto>> AddSubjectToGradeAsync(string gradeId, SubjectCreateDto dto)
        {
            var grade = await _gradeRepo.GetByIdAsync(gradeId);
            if (grade == null)
                return ServiceResult<SubjectViewDto>.Fail("Grade not found");

            var subject = _mapper.Map<Subject>(dto);
            subject.Id = Guid.NewGuid().ToString();

            var success = await _gradeRepo.AddSubjectToGradeAsync(gradeId, subject);
            if (!success)
                return ServiceResult<SubjectViewDto>.Fail("Failed to add subject to grade");

            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<SubjectViewDto>(subject);
            return ServiceResult<SubjectViewDto>.Ok(result, "Subject added to grade successfully");
        }

        public async Task<ServiceResult<bool>> RemoveClassAsync(string classId)
        {
            var success = await _gradeRepo.RemoveClassFromGradeAsync(classId);
            if (!success)
                return ServiceResult<bool>.Fail("Class not found");

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Class removed successfully");
        }

        public async Task<ServiceResult<bool>> RemoveSubjectAsync(string subjectId)
        {
            var success = await _gradeRepo.RemoveSubjectFromGradeAsync(subjectId);
            if (!success)
                return ServiceResult<bool>.Fail("Subject not found");

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Subject removed successfully");
        }

        public async Task<ServiceResult<bool>> MoveClassToAnotherGradeAsync(string classId, string newGradeId)
        {
            var success = await _gradeRepo.MoveClassToAnotherGradeAsync(classId, newGradeId);
            if (!success)
                return ServiceResult<bool>.Fail("Class or new grade not found");

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Class moved to another grade successfully");
        }

        public async Task<ServiceResult<bool>> MoveSubjectToAnotherGradeAsync(string subjectId, string newGradeId)
        {
            var success = await _gradeRepo.MoveSubjectToAnotherGradeAsync(subjectId, newGradeId);
            if (!success)
                return ServiceResult<bool>.Fail("Subject or new grade not found");

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Subject moved to another grade successfully");
        }

        public async Task<ServiceResult<IEnumerable<ClassViewDto>>> GetClassesByGradeIdAsync(string gradeId)
        {
            var classes = await _gradeRepo.GetClassesByGradeIdAsync(gradeId);
            var data = _mapper.Map<IEnumerable<ClassViewDto>>(classes);
            return ServiceResult<IEnumerable<ClassViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetSubjectsByGradeIdAsync(string gradeId)
        {
            var subjects = await _gradeRepo.GetSubjectsByGradeIdAsync(gradeId);
            var data = _mapper.Map<IEnumerable<SubjectViewDto>>(subjects);
            return ServiceResult<IEnumerable<SubjectViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<GradeWithDetailsDto>> GetGradeWithDetailsAsync(string id)
        {
            var grade = await _gradeRepo.GetByIdWithDetailsAsync(id);
            if (grade == null)
                return ServiceResult<GradeWithDetailsDto>.Fail("Grade not found");

            var data = _mapper.Map<GradeWithDetailsDto>(grade);
            return ServiceResult<GradeWithDetailsDto>.Ok(data);
        }
    }
}
