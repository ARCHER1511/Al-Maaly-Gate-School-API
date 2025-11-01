using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _teacherRepo = teacherRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync()
        {
            var teachers = await _teacherRepo.GetAllAsync();
            var data = _mapper.Map<IEnumerable<TeacherViewDto>>(teachers);
            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<TeacherViewDto>> GetByIdAsync(string id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            return ServiceResult<TeacherViewDto>.Ok(_mapper.Map<TeacherViewDto>(teacher));
        }

        public async Task<ServiceResult<TeacherViewDto>> CreateAsync(CreateTeacherDto dto)
        {
            var teacher = _mapper.Map<Teacher>(dto);
            await _teacherRepo.AddAsync(teacher);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<TeacherViewDto>.Ok(_mapper.Map<TeacherViewDto>(teacher), "Teacher created successfully");
        }

        public async Task<ServiceResult<TeacherViewDto>> UpdateAsync(string id, UpdateTeacherDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            _mapper.Map(dto, teacher);
            _teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<TeacherViewDto>.Ok(_mapper.Map<TeacherViewDto>(teacher), "Teacher updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found");

            _teacherRepo.Delete(teacher);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Teacher deleted successfully");
        }
    }
}
