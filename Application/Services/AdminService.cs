using System.Linq.Expressions;
using Application.DTOs.AdminDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdminService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<AdminViewDto>> GetByIdAsync(object id)
        {
            var admin = await _unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
                return ServiceResult<AdminViewDto>.Fail("Admin not found");
            var adminDto = _mapper.Map<AdminViewDto>(admin);

            return ServiceResult<AdminViewDto>.Ok(adminDto, "Admin retrieved successfully");
        }

        public async Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync()
        {
            var admins = await _unitOfWork.AdminRepository.GetAllAsync();
            if (admins == null)
                return ServiceResult<IEnumerable<AdminViewDto>>.Fail("No admins found");

            var adminDto = _mapper.Map<IEnumerable<AdminViewDto>>(admins);

            return ServiceResult<IEnumerable<AdminViewDto>>.Ok(
                adminDto,
                "Admins retrieved successfully"
            );
        }

        public async Task<ServiceResult<AdminViewDto?>> GetAsync(
            Expression<Func<Admin, bool>> predicate,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null
        )
        {
            var admin = await _unitOfWork.AdminRepository.FirstOrDefaultAsync(predicate, include);
            if (admin == null)
                return ServiceResult<AdminViewDto?>.Fail("Admin not found");

            var adminDto = _mapper?.Map<AdminViewDto>(admin);

            return ServiceResult<AdminViewDto?>.Ok(adminDto, "Admin retrieved successfully");
        }

        public async Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync(
            Expression<Func<Admin, bool>>? predicate = null,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
            Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
            int? skip = null,
            int? take = null
        )
        {
            var admins = await _unitOfWork.AdminRepository.FindAllAsync(
                predicate,
                include,
                orderBy,
                skip,
                take
            );
            if (admins == null)
                return ServiceResult<IEnumerable<AdminViewDto>>.Fail("No admins found");
            var adminDto = _mapper.Map<IEnumerable<AdminViewDto>>(admins);

            return ServiceResult<IEnumerable<AdminViewDto>>.Ok(
                adminDto,
                "Admins retrieved successfully"
            );
        }

        public async Task<ServiceResult<AdminViewDto>> CreateAsync(AdminCreateDto dto)
        {
            var admin = _mapper.Map<Admin>(dto);

            await _unitOfWork.AdminRepository.AddAsync(admin);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<AdminViewDto>(admin);
            return ServiceResult<AdminViewDto>.Ok(viewDto, "Admin created successfully");
        }

        public async Task<ServiceResult<AdminViewDto>> UpdateAsync(AdminUpdateDto dto)
        {
            var existingAdmin = await _unitOfWork.AdminRepository.GetByIdAsync(dto.Id);
            if (existingAdmin == null)
                return ServiceResult<AdminViewDto>.Fail("Admin not found");

            // Apply updates from DTO to the tracked entity
            _mapper.Map(dto, existingAdmin);

            _unitOfWork.AdminRepository.Update(existingAdmin); // Optional if tracked
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<AdminViewDto>(existingAdmin);
            return ServiceResult<AdminViewDto>.Ok(viewDto, "Admin updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var entity = await _unitOfWork.AdminRepository.GetByIdAsync(id);
            if (entity == null)
                return ServiceResult<bool>.Fail("Admin not found");

            _unitOfWork.AdminRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Admin deleted successfully");
        }
    }
}
