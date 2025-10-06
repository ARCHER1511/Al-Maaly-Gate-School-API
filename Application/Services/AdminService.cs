using Domain.Interfaces.ApplicationInterfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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

        public async Task<Admin?> GetByIdAsync(object id)
        {
            return await _adminRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _adminRepository.GetAllAsync();
        }

        public async Task<Admin?> GetAsync(Expression<Func<Admin, bool>> predicate,
                                           Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null)
        {
            return await _adminRepository.GetAsync(predicate, include);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync(Expression<Func<Admin, bool>>? predicate = null,
                                                          Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
                                                          Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            return await _adminRepository.GetAllAsync(predicate, include, orderBy, skip, take);
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            await _adminRepository.AddAsync(admin);
            await _unitOfWork.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin> UpdateAsync(Admin admin)
        {
            _adminRepository.Update(admin);
            await _unitOfWork.SaveChangesAsync();
            return admin;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            var entity = await _adminRepository.GetByIdAsync(id);
            if (entity == null)
                return false;

            _adminRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
