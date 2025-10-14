using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TeacherService(ITeacherRepository teacherRepo,IUnitOfWork unitOfWork,IMapper mapper) 
        {
            _teacherRepo = teacherRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync()
        {
            var teachers = await _teacherRepo.GetAllAsync();
            if (teachers == null)
                return ServiceResult<IEnumerable<TeacherViewDto>>.Fail("No teachers found");

            var teachersDto = _mapper.Map<IEnumerable<TeacherViewDto>>(teachers);

            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(teachersDto, "Teacher retrieved successfully");
        }
    }
}
