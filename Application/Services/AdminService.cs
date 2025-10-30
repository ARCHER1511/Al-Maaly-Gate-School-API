using Application.DTOs.AdminDTOs;
using Application.DTOs.ClassDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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
                t => t.Subjects!.Any(s => s.SubjectName == subjectName),
                include: q => q.Include(t => t.Subjects!)
            );
            if(teacher == null)
                return ServiceResult< IEnumerable<TeacherAdminViewDto>>.Fail("Teacher not found for the given subject.");
            var teacherDto = _mapper.Map<IEnumerable<TeacherAdminViewDto>>(teacher);
            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(teacherDto, "Teacher retrieved successfully.");
        }
        //New
        //Approve Teacher
        public async Task<ServiceResult<bool>> ApproveTeacherAsync(string teacherId)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(t => t.Id == teacherId, q => q.Include(t => t.AppUser));

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ProfileStatus = ProfileStatus.Approved;
            teacher.AppUser.AccountStatus = AccountStatus.Active;

            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher approved successfully.");
        }
        //Reject Teacher
        public async Task<ServiceResult<bool>> RejectTeacherAsync(string teacherId)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(t => t.Id == teacherId, q => q.Include(t => t.AppUser));

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ProfileStatus = ProfileStatus.Rejected;
            teacher.AppUser.AccountStatus = AccountStatus.PendingApproval;

            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher rejected successfully.");
        }
        // Block User
        public async Task<ServiceResult<bool>> BlockUserAsync(string appUserId)
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.AccountStatus = AccountStatus.Blocked;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "User blocked successfully.");
        }
        //Unblock User
        public async Task<ServiceResult<bool>> UnblockUserAsync(string appUserId)
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.AccountStatus = AccountStatus.Active;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "User unblocked successfully.");
        }
        //Teachers by Class
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeachersByClassAsync(string classId)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teachers = await teacherRepo.FindAllAsync(
                t => t.ClassAppointments.Any(ca => ca.Id == classId),
                q => q.Include(t => t.AppUser).Include(t => t.ClassAppointments).ThenInclude(ca => ca.Class)
            );

            var dto = teachers.Select(t => new TeacherAdminViewDto
            {
                Id = t.Id,
                FullName = t.AppUser.FullName,
                Email = t.Email,
                Subjects = t.Subjects!.Select(s => s.SubjectName).ToList(),
                ClassNames = t.ClassAppointments.Select(ca => ca.Class.ClassName).ToList(),
                ProfileStatus = t.ProfileStatus.ToString()
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(dto);
        }
        // Students by Class
        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByClassAsync(string classId)
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var students = await studentRepo.FindAllAsync(
                s => s.Class!.ClassAppointments!.Any(ca => ca.ClassId == classId),
                q => q.Include(s => s.AppUser).Include(s => s.Class).ThenInclude(ca => ca!.ClassAppointments)!
            );

            var dto = students.Select(s => new StudentViewDto
            {
                Id = s.Id,
                FullName = s.AppUser.FullName,
                Email = s.Email,
                ClassName = s.Class!.ClassName ?? "N/A",
                ProfileStatus = s.ProfileStatus.ToString()
            });

            return ServiceResult<IEnumerable<StudentViewDto>>.Ok(dto);
        }
        // Student Count
        public async Task<ServiceResult<int>> GetStudentCountAsync()
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var count = await studentRepo.AsQueryable().CountAsync();
            return ServiceResult<int>.Ok(count);
        }
        // Move Teacher to Another Class
        public async Task<ServiceResult<bool>> MoveTeacherToAnotherClassAsync(string teacherId, string? newClassId)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(t => t.Id == teacherId, q => q.Include(t => t.ClassAppointments));

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ClassAppointments.Clear();

            if (newClassId != null)
            {
                teacher.ClassAppointments.Add(new ClassAppointment { ClassId = newClassId, TeacherId = teacherId });
            }

            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher moved successfully.");
        }
        //Assign Teacher to Class
        public async Task<ServiceResult<bool>> AssignTeacherToClassAsync(string teacherId, string classId)
        {
            var appointmentRepo = _unitOfWork.Repository<ClassAppointment>();

            var existing = await appointmentRepo.FirstOrDefaultAsync(a => a.TeacherId == teacherId && a.ClassId == classId);
            if (existing != null)
                return ServiceResult<bool>.Fail("Teacher already assigned to this class.");

            await appointmentRepo.AddAsync(new ClassAppointment { ClassId = classId, TeacherId = teacherId });
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher assigned to class successfully.");
        }

        // Assign Teacher to Subject
        public async Task<ServiceResult<bool>> AssignTeacherToSubjectAsync(string teacherId, string subjectId)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(t => t.Id == teacherId, q => q.Include(t => t.Subjects)!);

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            if (teacher.Subjects!.Any(s => s.Id == subjectId))
                return ServiceResult<bool>.Fail("Teacher already assigned to this subject.");

            teacher.Subjects!.Add(new Subject { Id = subjectId });
            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher assigned to subject successfully.");
        }

        // Unassign Teacher
        public async Task<ServiceResult<bool>> UnassignTeacherAsync(string teacherId)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(t => t.Id == teacherId, q => q.Include(t => t.ClassAppointments));

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ClassAppointments.Clear();
            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher unassigned successfully.");
        }
        //Detect Duplicate Teacher Assignments
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetDuplicateTeacherAssignmentsAsync()
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var duplicates = await teacherRepo.AsQueryable()
                .Where(t => t.ClassAppointments.GroupBy(ca => ca.ClassId).Any(g => g.Count() > 1))
                .Include(t => t.AppUser)
                .Include(t => t.ClassAppointments)
                .ThenInclude(ca => ca.Class)
                .ToListAsync();

            var dto = duplicates.Select(t => new TeacherAdminViewDto
            {
                Id = t.Id,
                FullName = t.AppUser.FullName,
                Email = t.Email,
                ClassNames = t.ClassAppointments.Select(ca => ca.Class.ClassName).ToList(),
                ProfileStatus = t.ProfileStatus.ToString()
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(dto);
        }

        //Get Class Results
        public async Task<ServiceResult<IEnumerable<ClassResultDto>>> GetClassResultsAsync(string classId)
        {
            var examRepo = _unitOfWork.StudentExamAnswers;

            // Fetch all student exam answers for students belonging to the target class
            var results = await examRepo.FindAllAsync(
                e => e.Student.Class!.ClassAppointments!.Any(ca => ca.ClassId == classId),
                q => q.Include(e => e.Student)
                      .ThenInclude(s => s.Class)
                      .ThenInclude(ca => ca!.ClassAppointments)
                      .Include(e => e.Exam)
            );

            // Group results by class and compute aggregates
            var grouped = results
                .GroupBy(e => e.Student.Class!.ClassAppointments!
                                .FirstOrDefault(ca => ca.ClassId == classId)?.Class)
                .Select(g => new ClassResultDto
                {
                    ClassId = g.Key!.Id,
                    ClassName = g.Key.ClassName,
                    StudentCount = g.Select(e => e.StudentId).Distinct().Count(),
                    ExamCount = g.Select(e => e.ExamId).Distinct().Count(),
                    AverageMark = (double)g.Average(e => e.Exam.MinMark)
                });

            return ServiceResult<IEnumerable<ClassResultDto>>.Ok(grouped);
        }
    }
}
