using Application.DTOs.DegreesDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DegreeComponentTypeService : IDegreeComponentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DegreeComponentTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<DegreeComponentTypeDto>> CreateComponentTypeAsync(CreateDegreeComponentTypeDto dto)
        {
            try
            {
                // Check if subject exists
                var subject = await _unitOfWork.Subjects.GetByIdAsync(dto.SubjectId);
                if (subject == null)
                    return ServiceResult<DegreeComponentTypeDto>.Fail("Subject not found");

                // Check if component name already exists for this subject
                var existing = await _unitOfWork.DegreeComponentTypes.FirstOrDefaultAsync(
                    c => c.SubjectId == dto.SubjectId && c.ComponentName.ToLower() == dto.ComponentName.ToLower());

                if (existing != null)
                    return ServiceResult<DegreeComponentTypeDto>.Fail($"Component '{dto.ComponentName}' already exists for this subject");

                // Get the next order number
                var maxOrder = await _unitOfWork.DegreeComponentTypes.FindAllAsync(
                    predicate: c => c.SubjectId == dto.SubjectId,
                    orderBy: q => q.OrderByDescending(c => c.Order)
                );
                var nextOrder = maxOrder.Any() ? maxOrder.First().Order + 1 : 1;

                var componentType = new DegreeComponentType
                {
                    SubjectId = dto.SubjectId,
                    ComponentName = dto.ComponentName,
                    Order = dto.Order > 0 ? dto.Order : nextOrder,
                    MaxScore = dto.MaxScore,
                    IsActive = dto.IsActive
                };

                await _unitOfWork.DegreeComponentTypes.AddAsync(componentType);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = new DegreeComponentTypeDto
                {
                    Id = componentType.Id,
                    SubjectId = componentType.SubjectId,
                    ComponentName = componentType.ComponentName,
                    Order = componentType.Order,
                    MaxScore = componentType.MaxScore,
                    IsActive = componentType.IsActive
                };

                return ServiceResult<DegreeComponentTypeDto>.Ok(resultDto, "Component type created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<DegreeComponentTypeDto>.Fail($"Error creating component type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DegreeComponentTypeDto>> UpdateComponentTypeAsync(string id, UpdateDegreeComponentTypeDto dto)
        {
            try
            {
                var componentType = await _unitOfWork.DegreeComponentTypes.GetByIdAsync(id);
                if (componentType == null)
                    return ServiceResult<DegreeComponentTypeDto>.Fail("Component type not found");

                // Check if component name already exists for this subject (excluding current one)
                var existing = await _unitOfWork.DegreeComponentTypes.FirstOrDefaultAsync(
                    c => c.SubjectId == componentType.SubjectId &&
                         c.Id != id &&
                         c.ComponentName.ToLower() == dto.ComponentName.ToLower());

                if (existing != null)
                    return ServiceResult<DegreeComponentTypeDto>.Fail($"Component '{dto.ComponentName}' already exists for this subject");

                // Update properties
                componentType.ComponentName = dto.ComponentName;
                componentType.Order = dto.Order;
                componentType.MaxScore = dto.MaxScore;
                componentType.IsActive = dto.IsActive;

                // Note: If your repository doesn't have UpdateAsync, you need to save changes through the unit of work
                await _unitOfWork.DegreeComponentTypes.UpdateAsync(componentType);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = new DegreeComponentTypeDto
                {
                    Id = componentType.Id,
                    SubjectId = componentType.SubjectId,
                    ComponentName = componentType.ComponentName,
                    Order = componentType.Order,
                    MaxScore = componentType.MaxScore,
                    IsActive = componentType.IsActive
                };

                return ServiceResult<DegreeComponentTypeDto>.Ok(resultDto, "Component type updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<DegreeComponentTypeDto>.Fail($"Error updating component type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteComponentTypeAsync(string id)
        {
            try
            {
                var componentType = await _unitOfWork.DegreeComponentTypes.GetByIdAsync(id);
                if (componentType == null)
                    return ServiceResult<bool>.Fail("Component type not found");

                // Check if this component type is used in any degrees
                var usedInDegrees = await _unitOfWork.DegreesComponent.FirstOrDefaultAsync(
                    c => c.ComponentTypeId == id);

                if (usedInDegrees != null)
                {
                    // Instead of deleting, mark as inactive
                    componentType.IsActive = false;
                    await _unitOfWork.DegreeComponentTypes.UpdateAsync(componentType);
                    await _unitOfWork.SaveChangesAsync();
                    return ServiceResult<bool>.Ok(true, "Component type marked as inactive (it is being used in existing degrees)");
                }

                await _unitOfWork.DegreeComponentTypes.DeleteAsync(componentType);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true, "Component type deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Error deleting component type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DegreeComponentTypeDto>>> GetComponentTypesBySubjectAsync(string subjectId)
        {
            try
            {
                var componentTypes = await _unitOfWork.DegreeComponentTypes.FindAllAsync(
                    predicate: c => c.SubjectId == subjectId && c.IsActive,
                    orderBy: q => q.OrderBy(c => c.Order)
                );

                var result = componentTypes.Select(c => new DegreeComponentTypeDto
                {
                    Id = c.Id,
                    SubjectId = c.SubjectId,
                    ComponentName = c.ComponentName,
                    Order = c.Order,
                    MaxScore = c.MaxScore,
                    IsActive = c.IsActive
                }).ToList();

                return ServiceResult<List<DegreeComponentTypeDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<DegreeComponentTypeDto>>.Fail($"Error loading component types: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DegreeComponentTypeDto>> GetComponentTypeByIdAsync(string id)
        {
            try
            {
                var componentType = await _unitOfWork.DegreeComponentTypes.GetByIdAsync(id);
                if (componentType == null)
                    return ServiceResult<DegreeComponentTypeDto>.Fail("Component type not found");

                var resultDto = new DegreeComponentTypeDto
                {
                    Id = componentType.Id,
                    SubjectId = componentType.SubjectId,
                    ComponentName = componentType.ComponentName,
                    Order = componentType.Order,
                    MaxScore = componentType.MaxScore,
                    IsActive = componentType.IsActive
                };

                return ServiceResult<DegreeComponentTypeDto>.Ok(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<DegreeComponentTypeDto>.Fail($"Error loading component type: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ReorderComponentTypesAsync(string subjectId, List<string> componentTypeIds)
        {
            try
            {
                var componentTypes = await _unitOfWork.DegreeComponentTypes.FindAllAsync(
                    c => c.SubjectId == subjectId && componentTypeIds.Contains(c.Id)
                );

                if (componentTypes.Count() != componentTypeIds.Count)
                    return ServiceResult<bool>.Fail("Some component types not found");

                for (int i = 0; i < componentTypeIds.Count; i++)
                {
                    var componentType = componentTypes.FirstOrDefault(c => c.Id == componentTypeIds[i]);
                    if (componentType != null)
                    {
                        componentType.Order = i + 1;
                        await _unitOfWork.DegreeComponentTypes.UpdateAsync(componentType);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<bool>.Ok(true, "Component types reordered successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Error reordering component types: {ex.Message}");
            }
        }
    }
}