using Application.DTOs.ParentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ParentService : IParentService
    {
        private readonly IParentRepository _ParentRepository;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ParentService(IParentRepository parentRepository, IFileService fileService,IUnitOfWork unitOfWork, IMapper mapper)
        {
            _ParentRepository = parentRepository;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ParentViewDto>>> GetAllAsync()
        {
            var parent = await _ParentRepository.GetAllAsync();
            if (parent == null) return ServiceResult<IEnumerable<ParentViewDto>>.Fail("parent not found");

            var parentDto = _mapper.Map<IEnumerable<ParentViewDto>>(parent);
            return ServiceResult<IEnumerable<ParentViewDto>>.Ok(parentDto, "parent retrieved successfully");
        }
        public async Task<ServiceResult<ParentViewDto>> GetByIdAsync(object id)
        {
            var parent = await _ParentRepository.GetByIdAsync(id);
            if (parent == null) return ServiceResult<ParentViewDto>.Fail("parent not found");

            var parentDto = _mapper.Map<ParentViewDto>(parent);
            return ServiceResult<ParentViewDto>.Ok(parentDto, "parent retrieved successfully");
        }
        public async Task<ServiceResult<ParentCreateUpdateDto>> CreateAsync(ParentCreateUpdateDto dto)
        {
            var parent = _mapper.Map<Parent>(dto);

            await _ParentRepository.AddAsync(parent);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ParentCreateUpdateDto>(parent);
            return ServiceResult<ParentCreateUpdateDto>.Ok(viewDto, "parent created successfully");
        }
        public async Task<ServiceResult<ParentCreateUpdateDto>> UpdateAsync(ParentCreateUpdateDto dto)
        {
            var existingParent = await _ParentRepository.GetByIdAsync(dto.Id);
            if (existingParent == null)
                return ServiceResult<ParentCreateUpdateDto>.Fail("parent not found");

            _mapper.Map(dto, existingParent);

            _ParentRepository.Update(existingParent);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ParentCreateUpdateDto>(existingParent);
            return ServiceResult<ParentCreateUpdateDto>.Ok(viewDto, "parent updated successfully");
        }
        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var parent = await _ParentRepository.GetByIdAsync(id);
            if (parent == null)
                return ServiceResult<bool>.Fail("parent not found");

            _ParentRepository.Delete(parent);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "parent deleted successfully");
        }

        public async Task<ServiceResult<ParentViewWithChildrenDto>> GetParentWithChildrenAsync(string id)
        {
            var parent = await _ParentRepository
                  .AsQueryable()
                  .Where(p => p.Id == id)
                  .Include(p => p.ParentStudent)
                      .ThenInclude(ps => ps.Student)
                  .FirstOrDefaultAsync();

            if (parent == null)
                return ServiceResult<ParentViewWithChildrenDto>.Fail("Parent not found");

            var parentDto = _mapper.Map<ParentViewWithChildrenDto>(parent);

            return ServiceResult<ParentViewWithChildrenDto>.Ok(parentDto, "Parent retrieved successfully");
        }
        public async Task<ServiceResult<List<string>>> UploadParentDocs(IEnumerable<IFormFile> files, string controllerName, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<List<string>>.Fail("User is not authenticated");
            }
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return ServiceResult<List<string>>.Fail("Invalid upload context");
            }
            if (files == null || !files.Any())
            {
                return ServiceResult<List<string>>.Fail("No files provided for upload");
            }
            var uploadFiles = await _fileService.UploadFilesAsync(files,controllerName,userId);
            if (!uploadFiles.Success)
            {
                return ServiceResult<List<string>>.Fail("Error files not uploaded");
            }
            return ServiceResult<List<string>>.Ok(uploadFiles.Data!,"Files Uploaded Successfully");
        }
    }
}
