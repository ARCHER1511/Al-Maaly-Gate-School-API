using Domain.Interfaces.ApplicationInterfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Domain.Wrappers;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
        {
            _adminRepository = adminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<Admin>> GetByIdAsync(object id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            if(admin == null)
                return ServiceResult<Admin>.Fail("Admin not found");

            return ServiceResult<Admin>.Ok(admin,"Admin retrieved successfully");
        }

        public async Task<ServiceResult<IEnumerable<Admin>>> GetAllAsync()
        {
            var admins = await _adminRepository.GetAllAsync();
            if(admins == null)
            return ServiceResult<IEnumerable<Admin>>.Fail("No admins found");

            return ServiceResult<IEnumerable<Admin>>.Ok(admins,"Admins retrieved successfully");
        }

        public async Task<ServiceResult<Admin?>> GetAsync(Expression<Func<Admin, bool>> predicate,
                                           Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null)
        {
            var admin = await _adminRepository.GetAsync(predicate, include);
            if(admin == null)
                return ServiceResult<Admin?>.Fail("Admin not found");

            return ServiceResult<Admin?>.Ok(admin,"Admin retrieved successfully");
        }

        public async Task<ServiceResult<IEnumerable<Admin>>> GetAllAsync(Expression<Func<Admin, bool>>? predicate = null,
                                                          Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
                                                          Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            var admins = await _adminRepository.GetAllAsync(predicate, include, orderBy, skip, take);
            if(admins == null)
                return ServiceResult<IEnumerable<Admin>>.Fail("No admins found");

            return ServiceResult<IEnumerable<Admin>>.Ok(admins, "Admins retrieved successfully");
        }

        public async Task<ServiceResult<Admin>> CreateAsync(Admin admin)
        {
            await _adminRepository.AddAsync(admin);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<Admin>.Ok(admin, "Admin created successfully");
        }

        public async Task<ServiceResult<Admin>> UpdateAsync(Admin admin)
        {
            _adminRepository.Update(admin);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<Admin>.Ok(admin, "Admin updated successfully");
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
