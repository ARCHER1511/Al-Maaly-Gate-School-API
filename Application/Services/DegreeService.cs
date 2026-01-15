using Application.DTOs.DegreesDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DegreeService : IDegreeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDegreeComponentTypeService _componentTypeService;

        public DegreeService(IUnitOfWork unitOfWork, IDegreeComponentTypeService componentTypeService)
        {
            _unitOfWork = unitOfWork;
            _componentTypeService = componentTypeService;
        }

        public async Task<ServiceResult<string>> AddDegreesAsync(string studentId, List<DegreeInput> degreeInputs)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByIdAsync(studentId);
                if (student == null)
                    return ServiceResult<string>.Fail("Student not found");

                var validationErrors = new List<string>();
                var degreesToSave = new List<Degree>();

                foreach (var input in degreeInputs)
                {
                    // Validate input
                    var validationResult = ValidateDegreeInput(input);
                    if (!validationResult.IsValid)
                    {
                        validationErrors.Add($"Subject {input.SubjectId}: {validationResult.ErrorMessage}");
                        continue;
                    }

                    // Check if degree already exists
                    var existingDegree = await _unitOfWork.Degrees.FirstOrDefaultAsync(
                        d => d.StudentId == studentId &&
                             d.SubjectId == input.SubjectId &&
                             d.DegreeType == input.DegreeType,
                        include: q => q.Include(d => d.Components)
                    );

                    var degree = existingDegree ?? new Degree
                    {
                        StudentId = studentId,
                        SubjectId = input.SubjectId,
                        DegreeType = input.DegreeType,
                        Components = new List<DegreeComponent>()
                    };

                    // Clear existing components if updating
                    if (existingDegree != null)
                    {
                        await ClearExistingComponents(degree);
                    }

                    // Process based on input type
                    if (input.Components != null && input.Components.Any())
                    {
                        // Process with components
                        await ProcessWithComponents(degree, input);
                    }
                    else
                    {
                        // Process as simple total score
                        degree.Score = input.Score!.Value;
                        degree.MaxScore = input.MaxScore!.Value;
                        degree.Components.Clear(); // Ensure no components
                    }

                    // Get subject name
                    await UpdateSubjectName(degree);

                    if (existingDegree == null)
                    {
                        degreesToSave.Add(degree);
                    }
                }

                if (validationErrors.Any())
                {
                    return ServiceResult<string>.Fail($"Validation errors: {string.Join("; ", validationErrors)}");
                }

                // Save all degrees
                foreach (var degree in degreesToSave)
                {
                    await _unitOfWork.Degrees.AddAsync(degree);
                }

                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<string>.Ok("Degrees saved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail($"Error saving degrees: {ex.Message}");
            }
        }

        private (bool IsValid, string? ErrorMessage) ValidateDegreeInput(DegreeInput input)
        {
            // Either components OR total scores should be provided, not both
            bool hasComponents = input.Components != null && input.Components.Any();
            bool hasTotalScores = input.Score.HasValue && input.MaxScore.HasValue;

            if (!hasComponents && !hasTotalScores)
                return (false, "Either total scores or component scores must be provided");

            if (hasComponents && hasTotalScores)
                return (false, "Cannot provide both total scores and component scores. Choose one method.");

            // Validate components if provided
            if (hasComponents)
            {
                foreach (var component in input.Components!)
                {
                    if (component.Score < 0)
                        return (false, $"Component {component.ComponentName} has invalid negative score");

                    if (component.MaxScore.HasValue && component.MaxScore <= 0)
                        return (false, $"Component {component.ComponentName} has invalid max score");
                }
            }

            return (true, null);
        }

        private async Task ClearExistingComponents(Degree degree)
        {
            if (degree.Components.Any())
            {
                foreach (var component in degree.Components.ToList())
                {
                    await _unitOfWork.DegreesComponent.DeleteAsync(component);
                }
                degree.Components.Clear();
            }
        }

        private async Task ProcessWithComponents(Degree degree, DegreeInput input)
        {
            foreach (var componentInput in input.Components!)
            {
                // Verify component type exists and belongs to this subject
                var componentType = await _unitOfWork.DegreeComponentTypes.GetByIdAsync(componentInput.ComponentTypeId);
                if (componentType == null || componentType.SubjectId != input.SubjectId)
                {
                    throw new Exception($"Invalid component type for subject {input.SubjectId}");
                }

                var degreeComponent = new DegreeComponent
                {
                    ComponentTypeId = componentInput.ComponentTypeId,
                    ComponentName = componentInput.ComponentName,
                    Score = componentInput.Score,
                    MaxScore = componentInput.MaxScore ?? componentType.MaxScore
                };

                degree.Components.Add(degreeComponent);
            }

            // Calculate total from components
            degree.CalculateTotalScore();
        }

        private async Task UpdateSubjectName(Degree degree)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(degree.SubjectId);
            if (subject != null)
            {
                degree.SubjectName = subject.SubjectName;
            }
        }

        public async Task<ServiceResult<StudentDegreesDto>> GetStudentDegreesAsync(string studentId)
        {
            try
            {
                var student = await _unitOfWork.Students.FirstOrDefaultAsync(
                    s => s.Id == studentId,
                    include: q => q
                        .Include(s => s.Class)
                        .Include(s => s.Degrees)
                            .ThenInclude(d => d.Subject)
                        .Include(s => s.Degrees)
                            .ThenInclude(d => d.Components)
                );

                if (student == null)
                    return ServiceResult<StudentDegreesDto>.Fail("Student not found");

                var dto = new StudentDegreesDto
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    ClassId = student.ClassId ?? "",
                    ClassName = student.Class?.ClassName ?? "",
                    Degrees = student.Degrees.Select(d => new DegreeItemDto
                    {
                        DegreeId = d.Id,
                        SubjectId = d.SubjectId,
                        SubjectName = d.Subject.SubjectName,
                        DegreeType = d.DegreeType.ToString(),
                        Score = d.Score,
                        MaxScore = d.MaxScore,
                        Components = d.Components.Select(c => new DegreeComponentDto
                        {
                            Id = c.Id,
                            ComponentTypeId = c.ComponentTypeId,
                            ComponentName = c.ComponentName,
                            Score = c.Score,
                            MaxScore = c.MaxScore
                        }).ToList()
                    }).ToList()
                };

                return ServiceResult<StudentDegreesDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<StudentDegreesDto>.Fail($"Error loading student degrees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<StudentDegreesDto>>> GetAllStudentsDegreesAsync()
        {
            try
            {
                var students = await _unitOfWork.Students.FindAllAsync(
                    predicate: s => s.Degrees.Any(),
                    include: q => q
                        .Include(s => s.Class)
                        .Include(s => s.Degrees)
                            .ThenInclude(d => d.Subject)
                        .Include(s => s.Degrees)
                            .ThenInclude(d => d.Components)
                );

                var result = students.Select(student => new StudentDegreesDto
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    ClassId = student.ClassId ?? "",
                    ClassName = student.Class?.ClassName ?? "",
                    Degrees = student.Degrees.Select(d => new DegreeItemDto
                    {
                        DegreeId = d.Id,
                        SubjectId = d.SubjectId,
                        SubjectName = d.Subject.SubjectName,
                        DegreeType = d.DegreeType.ToString(),
                        Score = d.Score,
                        MaxScore = d.MaxScore,
                        Components = d.Components.Select(c => new DegreeComponentDto
                        {
                            Id = c.Id,
                            ComponentTypeId = c.ComponentTypeId,
                            ComponentName = c.ComponentName,
                            Score = c.Score,
                            MaxScore = c.MaxScore
                        }).ToList()
                    }).ToList()
                }).ToList();

                return ServiceResult<List<StudentDegreesDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<StudentDegreesDto>>.Fail($"Error loading all degrees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DegreeComponentTypeDto>>> GetSubjectComponentTypesAsync(string subjectId)
        {
            return await _componentTypeService.GetComponentTypesBySubjectAsync(subjectId);
        }

        public async Task<ServiceResult<string>> UpdateDegreeAsync(string degreeId, DegreeInput input)
        {
            try
            {
                var degree = await _unitOfWork.Degrees.FirstOrDefaultAsync(
                    d => d.Id == degreeId,
                    include: q => q.Include(d => d.Components)
                );

                if (degree == null)
                    return ServiceResult<string>.Fail("Degree not found");

                // Validate input
                var validationResult = ValidateDegreeInput(input);
                if (!validationResult.IsValid)
                    return ServiceResult<string>.Fail(validationResult.ErrorMessage!);

                // Clear existing components
                if (degree.Components.Any())
                {
                    foreach (var component in degree.Components.ToList())
                    {
                        await _unitOfWork.DegreesComponent.DeleteAsync(component);
                    }
                    degree.Components.Clear();
                }

                // Process based on input type
                if (input.Components != null && input.Components.Any())
                {
                    // Process with components
                    foreach (var componentInput in input.Components!)
                    {
                        var componentType = await _unitOfWork.DegreeComponentTypes.GetByIdAsync(componentInput.ComponentTypeId);
                        if (componentType == null || componentType.SubjectId != input.SubjectId)
                        {
                            return ServiceResult<string>.Fail($"Invalid component type for subject {input.SubjectId}");
                        }

                        var degreeComponent = new DegreeComponent
                        {
                            ComponentTypeId = componentInput.ComponentTypeId,
                            ComponentName = componentInput.ComponentName,
                            Score = componentInput.Score,
                            MaxScore = componentInput.MaxScore ?? componentType.MaxScore
                        };

                        degree.Components.Add(degreeComponent);
                    }

                    // Calculate total from components
                    degree.CalculateTotalScore();
                }
                else
                {
                    // Process as simple total score
                    degree.Score = input.Score!.Value;
                    degree.MaxScore = input.MaxScore!.Value;
                }

                degree.DegreeType = input.DegreeType;
                degree.SubjectId = input.SubjectId;

                await _unitOfWork.Degrees.UpdateAsync(degree);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<string>.Ok("Degree updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail($"Error updating degree: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> ConvertToComponentsAsync(string degreeId, List<DegreeComponentInput> components)
        {
            try
            {
                var degree = await _unitOfWork.Degrees.FirstOrDefaultAsync(
                    d => d.Id == degreeId,
                    include: q => q.Include(d => d.Components)
                );

                if (degree == null)
                    return ServiceResult<string>.Fail("Degree not found");

                // Clear existing components
                if (degree.Components.Any())
                {
                    foreach (var component in degree.Components.ToList())
                    {
                        await _unitOfWork.DegreesComponent.DeleteAsync(component);
                    }
                    degree.Components.Clear();
                }

                // Add new components
                foreach (var componentInput in components)
                {
                    var componentType = await _unitOfWork.DegreeComponentTypes.GetByIdAsync(componentInput.ComponentTypeId);

                    if (componentType == null || componentType.SubjectId != degree.SubjectId)
                    {
                        return ServiceResult<string>.Fail($"Invalid component type for subject {degree.SubjectId}");
                    }

                    var degreeComponent = new DegreeComponent
                    {
                        ComponentTypeId = componentInput.ComponentTypeId,
                        ComponentName = componentInput.ComponentName,
                        Score = componentInput.Score,
                        MaxScore = componentInput.MaxScore ?? componentType.MaxScore
                    };

                    degree.Components.Add(degreeComponent);
                }

                // Calculate total from components
                degree.CalculateTotalScore();

                await _unitOfWork.Degrees.UpdateAsync(degree);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<string>.Ok("Degree converted to component-based successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail($"Error converting degree: {ex.Message}");
            }
        }
    }
}