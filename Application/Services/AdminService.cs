using Application.DTOs.AdminDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdminService(
            IAdminRepository adminRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService
        )
        {
            _adminRepository = adminRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<AdminViewDto>> GetByIdAsync(object id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
                return ServiceResult<AdminViewDto>.Fail("Admin not found");
            var adminDto = _mapper.Map<AdminViewDto>(admin);

            return ServiceResult<AdminViewDto>.Ok(adminDto, "Admin retrieved successfully");
        }

        public async Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync()
        {
            var admins = await _adminRepository.GetAllAsync();
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
            var admin = await _adminRepository.FirstOrDefaultAsync(predicate, include);
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
            var admins = await _adminRepository.FindAllAsync(
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

            await _adminRepository.AddAsync(admin);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<AdminViewDto>(admin);
            return ServiceResult<AdminViewDto>.Ok(viewDto, "Admin created successfully");
        }

        public async Task<ServiceResult<AdminViewDto>> UpdateAsync(AdminUpdateDto dto)
        {
            var existingAdmin = await _adminRepository.GetByIdAsync(dto.Id);
            if (existingAdmin == null)
                return ServiceResult<AdminViewDto>.Fail("Admin not found");

            // Apply updates from DTO to the tracked entity
            _mapper.Map(dto, existingAdmin);

            _adminRepository.Update(existingAdmin); // Optional if tracked
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<AdminViewDto>(existingAdmin);
            return ServiceResult<AdminViewDto>.Ok(viewDto, "Admin updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var entity = await _adminRepository.GetByIdAsync(id);
            if (entity == null)
                return ServiceResult<bool>.Fail("Admin not found");

            _adminRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Admin deleted successfully");
        }
    }
}
