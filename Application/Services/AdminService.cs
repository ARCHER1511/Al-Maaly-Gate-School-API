using System.Formats.Asn1;
using System.Linq.Expressions;
using Application.DTOs.AdminDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(
            IAdminRepository adminRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITeacherRepository teacherRepository
        )
        {
            _adminRepository = adminRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        //Teachers

        // TeacherCount
        public async Task<ServiceResult<int>> GetTeacherCount()
        {
            var teachers = await _teacherRepository.GetAllAsync();

            if (teachers == null)
                return ServiceResult<int>.Fail("No data retrieved from repository.");

            int count = teachers.Count();

            return ServiceResult<int>.Ok(count, "Counted successfully.");
        }

        // TeacherBySubject
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeahcerInfo(string subjectName)
        {
            var teacher = await _teacherRepository.FirstOrDefaultAsync(
                t => t.Subjects!.Any(s => s.FullName == subjectName),
                include: q => q.Include(t => t.Subjects!)
            );
            if(teacher == null)
                return ServiceResult< IEnumerable<TeacherAdminViewDto>>.Fail("Teacher not found for the given subject.");
            var teacherDto = _mapper.Map<IEnumerable<TeacherAdminViewDto>>(teacher);
            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(teacherDto, "Teacher retrieved successfully.");
        }
    }
}
