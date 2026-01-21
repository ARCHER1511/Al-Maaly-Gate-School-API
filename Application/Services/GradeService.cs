using Application.DTOs.ClassDTOs;
using Application.DTOs.GradeDTOs;
using Application.DTOs.SubjectDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GradeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<GradeViewDto>>> GetAllAsync()
        {
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            var data = _mapper.Map<IEnumerable<GradeViewDto>>(grades);
            return ServiceResult<IEnumerable<GradeViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<GradeViewDto>> GetByIdAsync(string id)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdWithCurriculumAsync(id);
            if (grade == null)
                return ServiceResult<GradeViewDto>.Fail("Grade not found");

            return ServiceResult<GradeViewDto>.Ok(_mapper.Map<GradeViewDto>(grade));
        }

        public async Task<ServiceResult<GradeViewDto>> GetByNameAsync(string gradeName)
        {
            var grade = await _unitOfWork.GradeRepository.GetByNameWithCurriculumAsync(gradeName);
            if (grade == null)
                return ServiceResult<GradeViewDto>.Fail("Grade not found");

            return ServiceResult<GradeViewDto>.Ok(_mapper.Map<GradeViewDto>(grade));
        }

        public async Task<ServiceResult<GradeViewDto>> CreateAsync(CreateGradeDto dto)
        {
            // Check if curriculum exists
            var curriculum = await _unitOfWork.CurriculumRepository.GetByIdAsync(dto.CurriculumId);
            if (curriculum == null)
                return ServiceResult<GradeViewDto>.Fail("Curriculum not found");

            // Check if grade name already exists in the same curriculum
            var existingGrade = await _unitOfWork.GradeRepository.GetByNameAndCurriculumAsync(
                dto.GradeName,
                dto.CurriculumId
            );
            if (existingGrade != null)
                return ServiceResult<GradeViewDto>.Fail(
                    "Grade name already exists in this curriculum"
                );

            var grade = _mapper.Map<Grade>(dto);
            grade.Id = Guid.NewGuid().ToString();
            grade.CurriculumId = dto.CurriculumId;

            await _unitOfWork.GradeRepository.AddAsync(grade);
            await _unitOfWork.SaveChangesAsync();

            var createdGrade = await _unitOfWork.GradeRepository.GetByIdWithCurriculumAsync(
                grade.Id
            );
            var result = _mapper.Map<GradeViewDto>(createdGrade ?? grade);
            return ServiceResult<GradeViewDto>.Ok(result, "Grade created successfully");
        }

        public async Task<ServiceResult<GradeViewDto>> UpdateAsync(string id, UpdateGradeDto dto)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(id);
            if (grade == null)
                return ServiceResult<GradeViewDto>.Fail("Grade not found");

            // Check if new grade name conflicts with existing grade in the same curriculum
            if (dto.GradeName != grade.GradeName || dto.CurriculumId != grade.CurriculumId)
            {
                var existingGrade = await _unitOfWork.GradeRepository.GetByNameAndCurriculumAsync(
                    dto.GradeName,
                    dto.CurriculumId
                );
                if (existingGrade != null && existingGrade.Id != id)
                    return ServiceResult<GradeViewDto>.Fail(
                        "Grade name already exists in this curriculum"
                    );
            }

            // Check if curriculum exists
            if (dto.CurriculumId != grade.CurriculumId)
            {
                var curriculum = await _unitOfWork.CurriculumRepository.GetByIdAsync(
                    dto.CurriculumId
                );
                if (curriculum == null)
                    return ServiceResult<GradeViewDto>.Fail("Curriculum not found");
            }

            _mapper.Map(dto, grade);
            _unitOfWork.GradeRepository.Update(grade);
            await _unitOfWork.SaveChangesAsync();

            var updatedGrade = await _unitOfWork.GradeRepository.GetByIdWithCurriculumAsync(id);
            return ServiceResult<GradeViewDto>.Ok(
                _mapper.Map<GradeViewDto>(updatedGrade ?? grade),
                "Grade updated successfully"
            );
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdWithDetailsAsync(id);
            if (grade == null)
                return ServiceResult<bool>.Fail("Grade not found");

            if (grade.Classes.Any() || grade.Subjects.Any())
                return ServiceResult<bool>.Fail(
                    "Cannot delete grade that has classes or subjects. Please remove them first."
                );

            _unitOfWork.GradeRepository.Delete(grade);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Grade deleted successfully");
        }

        public async Task<ServiceResult<ClassViewDto>> AddClassToGradeAsync(
            string gradeId,
            ClassDto dto
        )
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(gradeId);
            if (grade == null)
                return ServiceResult<ClassViewDto>.Fail("Grade not found");

            var existingClass = await _unitOfWork.ClassRepository.GetByIdAsync(dto.Id);
            if (existingClass == null)
                return ServiceResult<ClassViewDto>.Fail("Class not found");

            // Check if class belongs to a different curriculum
            if (existingClass.Grade?.CurriculumId != grade.CurriculumId)
                return ServiceResult<ClassViewDto>.Fail(
                    "Class belongs to a different curriculum. Cannot move between curricula."
                );

            existingClass.GradeId = gradeId;
            _unitOfWork.ClassRepository.Update(existingClass);
            await _unitOfWork.SaveChangesAsync();

            var updatedClass = await _unitOfWork.ClassRepository.GetByIdAsync(dto.Id);
            var result = _mapper.Map<ClassViewDto>(updatedClass ?? existingClass);
            return ServiceResult<ClassViewDto>.Ok(result, "Class assigned to grade successfully");
        }

        public async Task<ServiceResult<ClassViewDto>> AddClassToGradeAsync(
            string gradeId,
            CreateClassInGradeDto dto
        )
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(gradeId);
            if (grade == null)
                return ServiceResult<ClassViewDto>.Fail("Grade not found");

            var classEntity = _mapper.Map<Class>(dto);
            classEntity.Id = Guid.NewGuid().ToString();
            classEntity.GradeId = gradeId;

            await _unitOfWork.ClassRepository.AddAsync(classEntity);
            await _unitOfWork.SaveChangesAsync();

            var createdClass = await _unitOfWork.ClassRepository.GetByIdAsync(classEntity.Id);
            var result = _mapper.Map<ClassViewDto>(createdClass ?? classEntity);
            return ServiceResult<ClassViewDto>.Ok(result, "Class created in grade successfully");
        }

        public async Task<ServiceResult<SubjectViewDto>> AddSubjectToGradeAsync(
            string gradeId,
            SubjectCreateDto dto
        )
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdAsync(gradeId);
            if (grade == null)
                return ServiceResult<SubjectViewDto>.Fail("Grade not found");

            var subject = _mapper.Map<Subject>(dto);
            subject.Id = Guid.NewGuid().ToString();
            subject.GradeId = gradeId;

            var success = await _unitOfWork.GradeRepository.AddSubjectToGradeAsync(
                gradeId,
                subject
            );
            if (!success)
                return ServiceResult<SubjectViewDto>.Fail("Failed to add subject to grade");

            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<SubjectViewDto>(subject);
            return ServiceResult<SubjectViewDto>.Ok(result, "Subject added to grade successfully");
        }

        // Missing method implementations
        public async Task<ServiceResult<bool>> RemoveClassAsync(string classId)
        {
            var success = await _unitOfWork.GradeRepository.RemoveClassFromGradeAsync(classId);
            if (!success)
                return ServiceResult<bool>.Fail("Class not found");

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Class removed successfully");
        }

        public async Task<ServiceResult<bool>> RemoveSubjectAsync(string subjectId)
        {
            var success = await _unitOfWork.GradeRepository.RemoveSubjectFromGradeAsync(subjectId);
            if (!success)
                return ServiceResult<bool>.Fail("Subject not found");

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Subject removed successfully");
        }

        public async Task<ServiceResult<bool>> MoveClassToAnotherGradeAsync(
            string classId,
            string newGradeId
        )
        {
            // Get the class
            var classEntity = await _unitOfWork.ClassRepository.GetByIdAsync(classId);
            if (classEntity == null)
                return ServiceResult<bool>.Fail("Class not found");

            // Get current grade
            var currentGrade = await _unitOfWork.GradeRepository.GetByIdAsync(classEntity.GradeId);
            if (currentGrade == null)
                return ServiceResult<bool>.Fail("Current grade not found");

            // Get new grade
            var newGrade = await _unitOfWork.GradeRepository.GetByIdAsync(newGradeId);
            if (newGrade == null)
                return ServiceResult<bool>.Fail("New grade not found");

            // Check if moving between same curriculum
            if (currentGrade.CurriculumId != newGrade.CurriculumId)
                return ServiceResult<bool>.Fail("Cannot move class between different curricula");

            // Update class grade
            classEntity.GradeId = newGradeId;
            _unitOfWork.ClassRepository.Update(classEntity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Class moved to another grade successfully");
        }

        public async Task<ServiceResult<bool>> MoveSubjectToAnotherGradeAsync(
            string subjectId,
            string newGradeId
        )
        {
            var subjectRepo = _unitOfWork.Repository<Subject>();
            var subject = await subjectRepo.GetByIdAsync(subjectId);
            if (subject == null)
                return ServiceResult<bool>.Fail("Subject not found");

            // Get current grade
            var currentGrade = await _unitOfWork.GradeRepository.GetByIdAsync(subject.GradeId);
            if (currentGrade == null)
                return ServiceResult<bool>.Fail("Current grade not found");

            // Get new grade
            var newGrade = await _unitOfWork.GradeRepository.GetByIdAsync(newGradeId);
            if (newGrade == null)
                return ServiceResult<bool>.Fail("New grade not found");

            // Check if moving between same curriculum
            if (currentGrade.CurriculumId != newGrade.CurriculumId)
                return ServiceResult<bool>.Fail("Cannot move subject between different curricula");

            // Update subject grade
            subject.GradeId = newGradeId;
            subjectRepo.Update(subject);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Subject moved to another grade successfully");
        }

        public async Task<ServiceResult<IEnumerable<ClassViewDto>>> GetClassesByGradeIdAsync(
            string gradeId
        )
        {
            var classes = await _unitOfWork.GradeRepository.GetClassesByGradeIdAsync(gradeId);
            var data = _mapper.Map<IEnumerable<ClassViewDto>>(classes);
            return ServiceResult<IEnumerable<ClassViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetSubjectsByGradeIdAsync(
            string gradeId
        )
        {
            var subjects = await _unitOfWork.GradeRepository.GetSubjectsByGradeIdAsync(gradeId);
            var data = _mapper.Map<IEnumerable<SubjectViewDto>>(subjects);
            return ServiceResult<IEnumerable<SubjectViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<GradeWithDetailsDto>> GetGradeWithDetailsAsync(string id)
        {
            var grade = await _unitOfWork.GradeRepository.GetByIdWithDetailsAsync(id);
            if (grade == null)
                return ServiceResult<GradeWithDetailsDto>.Fail("Grade not found");

            if (grade.Curriculum == null && grade.CurriculumId != null)
            {
                if (string.IsNullOrEmpty(grade.CurriculumId))
                {
                    // This shouldn't happen based on your model, but handle just in case
                    return ServiceResult<GradeWithDetailsDto>.Fail(
                        "Grade has no curriculum assigned"
                    );
                }
                var curriculum = await _unitOfWork.CurriculumRepository.GetByIdAsync(
                    grade.CurriculumId!
                );
                if (curriculum == null)
                    return ServiceResult<GradeWithDetailsDto>.Fail("Assigned curriculum not found");
                grade.Curriculum = curriculum;
            }

            var data = _mapper.Map<GradeWithDetailsDto>(grade);
            return ServiceResult<GradeWithDetailsDto>.Ok(data);
        }

        public async Task<ServiceResult<bool>> BulkMoveClassesAsync(BulkMoveClassesDto dto)
        {
            var classRepo = _unitOfWork.Repository<Class>();
            var gradeRepo = _unitOfWork.Repository<Grade>();

            if (!dto.ClassIds.Any())
                return ServiceResult<bool>.Fail("At least one class ID is required.");

            if (string.IsNullOrEmpty(dto.NewGradeId))
                return ServiceResult<bool>.Fail("New grade ID is required.");

            var newGrade = await gradeRepo.GetByIdAsync(dto.NewGradeId);
            if (newGrade == null)
                return ServiceResult<bool>.Fail("New grade not found.");

            var classes = await classRepo.FindAllAsync(c => dto.ClassIds.Contains(c.Id));

            if (classes.Count() != dto.ClassIds.Count)
            {
                var foundClassIds = classes.Select(c => c.Id).ToHashSet();
                var missingClassIds = dto.ClassIds.Except(foundClassIds).ToList();
                return ServiceResult<bool>.Fail(
                    $"The following classes were not found: {string.Join(", ", missingClassIds)}"
                );
            }

            // Check if all classes belong to the same curriculum as new grade
            foreach (var classEntity in classes)
            {
                var currentGrade = await gradeRepo.GetByIdAsync(classEntity.GradeId);
                if (currentGrade?.CurriculumId != newGrade.CurriculumId)
                    return ServiceResult<bool>.Fail(
                        $"Class {classEntity.ClassName} belongs to a different curriculum. Cannot move between curricula."
                    );
            }

            foreach (var classEntity in classes)
            {
                classEntity.GradeId = dto.NewGradeId;
                classRepo.Update(classEntity);
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(
                true,
                $"Successfully moved {classes.Count()} classes to grade {newGrade.GradeName}."
            );
        }

        public async Task<ServiceResult<IEnumerable<GradeViewDto>>> GetGradesByCurriculumAsync(
            string curriculumId
        )
        {
            var grades = await _unitOfWork.GradeRepository.GetGradesByCurriculumAsync(curriculumId);
            var data = _mapper.Map<IEnumerable<GradeViewDto>>(grades);
            return ServiceResult<IEnumerable<GradeViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<int>> GetGradeCountAsync()
        {
            try
            {
                var allGrades = await _unitOfWork.GradeRepository.GetAllAsync();
                var count = allGrades.Count();
                return ServiceResult<int>.Ok(count, $"Total grades: {count}");
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail($"Error counting grades: {ex.Message}");
            }
        }
    }
}
