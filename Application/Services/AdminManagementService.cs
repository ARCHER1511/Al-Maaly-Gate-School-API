using Application.DTOs.ClassDTOs;
using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AdminManagementService : IAdminManagementService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IParentRepository _parentRepository;
        private readonly IParentStudentRepository _parentStudentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdminManagementService(
            IAdminRepository adminRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IParentRepository parentRepository,
            IParentStudentRepository parentStudentRepository,
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
            _parentRepository = parentRepository;
            _parentStudentRepository = parentStudentRepository;
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
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeahcerInfo(
            string subjectName
        )
        {
            var teachers = await _teacherRepository.FindAllAsync(
                t => t.TeacherSubjects!.Any(ts => ts.Subject.SubjectName == subjectName),
                q =>
                    q.Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                        .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
                        .Include(t => t.AppUser)
            );

            if (!teachers.Any())
                return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Fail(
                    "No teachers found for this subject."
                );

            var dto = teachers.Select(t => new TeacherAdminViewDto
            {
                Id = t.Id,
                FullName = t.AppUser.FullName,
                Email = t.Email,
                Subjects = t.TeacherSubjects!.Select(ts => ts.Subject.SubjectName).ToList(),
                ClassNames = t.TeacherClasses.Select(tc => tc.Class.ClassName).ToList(),
                ProfileStatus = t.AccountStatus.ToString(),
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(
                dto,
                "Teachers retrieved successfully."
            );
        }

        public async Task<ServiceResult<bool>> ApproveUserAsync(string accountId, string adminId, string role)
        {
            object? entity = null;
            AppUser? appUser = null;
            dynamic? repository = null;

            switch (role.ToLower())
            {
                case "teacher":
                    var teacherRepo = _unitOfWork.Repository<Teacher>();
                    var teacher = await teacherRepo.FirstOrDefaultAsync(
                        t => t.Id == accountId,
                        q => q.Include(t => t.AppUser)
                    );
                    entity = teacher;
                    appUser = teacher?.AppUser;
                    repository = teacherRepo;
                    break;

                case "student":
                    var studentRepo = _unitOfWork.Repository<Student>();
                    var student = await studentRepo.FirstOrDefaultAsync(
                        s => s.Id == accountId,
                        q => q.Include(s => s.AppUser)
                    );
                    entity = student;
                    appUser = student?.AppUser;
                    repository = studentRepo;
                    break;

                case "parent":
                    var parentRepo = _unitOfWork.Repository<Parent>();
                    var parent = await parentRepo.FirstOrDefaultAsync(
                        p => p.Id == accountId,
                        q => q.Include(p => p.AppUser)
                    );
                    entity = parent;
                    appUser = parent?.AppUser;
                    repository = parentRepo;
                    break;

                default:
                    return ServiceResult<bool>.Fail($"Invalid role: {role}");
            }

            if (entity == null)
                return ServiceResult<bool>.Fail($"{role} not found.");

            if (appUser == null)
                return ServiceResult<bool>.Fail($"AppUser for {role} not found.");

            // Update status - using dynamic to avoid reflection
            dynamic dynamicEntity = entity;
            dynamicEntity.AccountStatus = AccountStatus.Active;
            appUser.AccountStatus = AccountStatus.Active;

            repository.Update(dynamicEntity);

            var identityRepo = _unitOfWork.AppUsers;
            identityRepo.Update(appUser);

            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Approved",
                message: $"Dear {appUser.FullName}, your account has been approved by the admin.",
                type: "Approval",
                creatorUserId: adminId,
                targetUserIds: new[] { appUser.Id },
                role: role
            );

            return ServiceResult<bool>.Ok(true, $"{role} approved successfully.");
        }

        // Reject User
        public async Task<ServiceResult<bool>> RejectUserAsync(
            string accountId,
            string adminId,
            string role
        )
        {
            object? entity = null;
            AppUser? appUser = null;
            dynamic? repository = null;

            switch (role.ToLower())
            {
                case "teacher":
                    var teacherRepo = _unitOfWork.Repository<Teacher>();
                    var teacher = await teacherRepo.FirstOrDefaultAsync(
                        t => t.Id == accountId,
                        q => q.Include(t => t.AppUser)
                    );
                    entity = teacher;
                    appUser = teacher?.AppUser;
                    repository = teacherRepo;
                    break;

                case "student":
                    var studentRepo = _unitOfWork.Repository<Student>();
                    var student = await studentRepo.FirstOrDefaultAsync(
                        s => s.Id == accountId,
                        q => q.Include(s => s.AppUser)
                    );
                    entity = student;
                    appUser = student?.AppUser;
                    repository = studentRepo;
                    break;

                case "parent":
                    var parentRepo = _unitOfWork.Repository<Parent>();
                    var parent = await parentRepo.FirstOrDefaultAsync(
                        p => p.Id == accountId,
                        q => q.Include(p => p.AppUser)
                    );
                    entity = parent;
                    appUser = parent?.AppUser;
                    repository = parentRepo;
                    break;

                default:
                    return ServiceResult<bool>.Fail($"Invalid role: {role}");
            }

            if (entity == null)
                return ServiceResult<bool>.Fail($"{role} not found.");

            if (appUser == null)
                return ServiceResult<bool>.Fail($"AppUser for {role} not found.");

            // Update status to Rejected
            dynamic dynamicEntity = entity;
            dynamicEntity.AccountStatus = AccountStatus.Rejected;
            appUser.AccountStatus = AccountStatus.Pending; // Or AccountStatus.Rejected based on your needs

            repository.Update(dynamicEntity);
            _unitOfWork.AppUsers.Update(appUser);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Rejected",
                message: $"Dear {appUser.FullName}, your profile has been rejected by the admin. Please review and resubmit.",
                type: "Rejection",
                creatorUserId: adminId,
                targetUserIds: new[] { appUser.Id },
                role: role
            );

            return ServiceResult<bool>.Ok(true, $"{role} rejected successfully.");
        }

        // Block User for all roles
        public async Task<ServiceResult<bool>> BlockUserAsync(
            string accountId,
            string adminId,
            string role
        )
        {
            object? entity = null;
            AppUser? appUser = null;
            dynamic? repository = null;

            switch (role.ToLower())
            {
                case "teacher":
                    var teacherRepo = _unitOfWork.Repository<Teacher>();
                    var teacher = await teacherRepo.FirstOrDefaultAsync(
                        t => t.Id == accountId,
                        q => q.Include(t => t.AppUser)
                    );
                    entity = teacher;
                    appUser = teacher?.AppUser;
                    repository = teacherRepo;
                    break;

                case "student":
                    var studentRepo = _unitOfWork.Repository<Student>();
                    var student = await studentRepo.FirstOrDefaultAsync(
                        s => s.Id == accountId,
                        q => q.Include(s => s.AppUser)
                    );
                    entity = student;
                    appUser = student?.AppUser;
                    repository = studentRepo;
                    break;

                case "parent":
                    var parentRepo = _unitOfWork.Repository<Parent>();
                    var parent = await parentRepo.FirstOrDefaultAsync(
                        p => p.Id == accountId,
                        q => q.Include(p => p.AppUser)
                    );
                    entity = parent;
                    appUser = parent?.AppUser;
                    repository = parentRepo;
                    break;

                default:
                    return ServiceResult<bool>.Fail($"Invalid role: {role}");
            }

            if (entity == null)
                return ServiceResult<bool>.Fail($"{role} not found.");

            if (appUser == null)
                return ServiceResult<bool>.Fail($"AppUser for {role} not found.");

            // Update status to Blocked
            dynamic dynamicEntity = entity;
            dynamicEntity.AccountStatus = AccountStatus.Blocked;
            appUser.AccountStatus = AccountStatus.Blocked;

            repository.Update(dynamicEntity);
            _unitOfWork.AppUsers.Update(appUser);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Blocked",
                message: $"Dear {appUser.FullName}, your account has been blocked. Please contact support for more information.",
                type: "Account Status",
                creatorUserId: adminId,
                targetUserIds: new[] { appUser.Id },
                role: role
            );

            return ServiceResult<bool>.Ok(true, $"{role} blocked successfully.");
        }

        // Unblock User for all roles
        public async Task<ServiceResult<bool>> UnblockUserAsync(
            string accountId,
            string adminId,
            string role
        )
        {
            object? entity = null;
            AppUser? appUser = null;
            dynamic? repository = null;

            switch (role.ToLower())
            {
                case "teacher":
                    var teacherRepo = _unitOfWork.Repository<Teacher>();
                    var teacher = await teacherRepo.FirstOrDefaultAsync(
                        t => t.Id == accountId,
                        q => q.Include(t => t.AppUser)
                    );
                    entity = teacher;
                    appUser = teacher?.AppUser;
                    repository = teacherRepo;
                    break;

                case "student":
                    var studentRepo = _unitOfWork.Repository<Student>();
                    var student = await studentRepo.FirstOrDefaultAsync(
                        s => s.Id == accountId,
                        q => q.Include(s => s.AppUser)
                    );
                    entity = student;
                    appUser = student?.AppUser;
                    repository = studentRepo;
                    break;

                case "parent":
                    var parentRepo = _unitOfWork.Repository<Parent>();
                    var parent = await parentRepo.FirstOrDefaultAsync(
                        p => p.Id == accountId,
                        q => q.Include(p => p.AppUser)
                    );
                    entity = parent;
                    appUser = parent?.AppUser;
                    repository = parentRepo;
                    break;

                default:
                    return ServiceResult<bool>.Fail($"Invalid role: {role}");
            }

            if (entity == null)
                return ServiceResult<bool>.Fail($"{role} not found.");

            if (appUser == null)
                return ServiceResult<bool>.Fail($"AppUser for {role} not found.");

            // Update status to Active
            dynamic dynamicEntity = entity;
            dynamicEntity.AccountStatus = AccountStatus.Active;
            appUser.AccountStatus = AccountStatus.Active;

            repository.Update(dynamicEntity);
            _unitOfWork.AppUsers.Update(appUser);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Unblocked",
                message: $"Dear {appUser.FullName}, your account has been unblocked. You can now access all features.",
                type: "Account Status",
                creatorUserId: adminId,
                targetUserIds: new[] { appUser.Id },
                role: role
            );

            return ServiceResult<bool>.Ok(true, $"{role} unblocked successfully.");
        }

        //Teachers by Class
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeachersByClassAsync(
            string classId
        )
        {
            var teachers = await _unitOfWork
                .Repository<Teacher>()
                .FindAllAsync(
                    t => t.TeacherClasses.Any(tc => tc.ClassId == classId),
                    q =>
                        q.Include(t => t.AppUser)
                            .Include(t => t.TeacherSubjects)!
                            .ThenInclude(ts => ts.Subject)
                            .Include(t => t.TeacherClasses)
                            .ThenInclude(tc => tc.Class)
                );

            var dto = teachers.Select(t => new TeacherAdminViewDto
            {
                Id = t.Id,
                FullName = t.AppUser.FullName,
                Email = t.Email,
                ContactInfo = t.ContactInfo,
                Subjects = t.TeacherSubjects!.Select(ts => ts.Subject.SubjectName).ToList(),
                ClassNames = t.TeacherClasses.Select(tc => tc.Class.ClassName).ToList(),
                ProfileStatus = t.AccountStatus.ToString(),
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(dto);
        }

        // Students by Class
        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByClassAsync(string classId)
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var students = await studentRepo.FindAllAsync(
                s => s.ClassId == classId, // DIRECT class relationship
                q => q.Include(s => s.AppUser)
                      .Include(s => s.Class)! // Include the class
            );

            var dto = students.Select(s => new StudentViewDto
            {
                Id = s.Id,
                FullName = s.AppUser.FullName,
                Email = s.Email,
                ClassName = s.Class?.ClassName ?? "N/A",
                AccountStatus = s.AccountStatus.ToString(),
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
        public async Task<ServiceResult<bool>> MoveTeacherToAnotherClassAsync(
            string teacherId,
            string? newClassId,
            string adminUserId
        )
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacherClassRepo = _unitOfWork.Repository<TeacherClass>();

            // Load teacher with AppUser for notification text
            var teacher = await teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                q => q.Include(t => t.AppUser)
            );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            // Fetch all current TeacherClass relations for this teacher
            var existingAssignments = (
                await teacherClassRepo.FindAllAsync(tc => tc.TeacherId == teacherId)
            ).ToList();

            // Remove existing assignments
            foreach (var assign in existingAssignments)
            {
                teacherClassRepo.Delete(assign);
            }

            // If a new class is provided, add the new relationship
            if (!string.IsNullOrEmpty(newClassId))
            {
                var newAssignment = new TeacherClass
                {
                    TeacherId = teacherId,
                    ClassId = newClassId,
                };

                await teacherClassRepo.AddAsync(newAssignment);
            }

            // Persist changes
            await _unitOfWork.SaveChangesAsync();

            // Prepare notification message (use AppUser.FullName if available)
            var teacherName = teacher.AppUser?.FullName ?? "Teacher";
            string message =
                newClassId == null
                    ? $"Dear {teacherName}, you have been unassigned from all your classes."
                    : $"Dear {teacherName}, you have been reassigned to a new class successfully.";

            await _notificationService.CreateNotificationAsync(
                title: "Class Assignment Updated",
                message: message,
                type: "Class Change",
                creatorUserId: adminUserId,
                targetUserIds: new[] { teacher.AppUserId },
                role: "Teacher"
            );

            return ServiceResult<bool>.Ok(true, "Teacher moved successfully.");
        }

        //Assign Teacher to Class
        public async Task<ServiceResult<bool>> AssignTeacherToClassAsync(
    string teacherId,
    string classId
    )
        {
            var teacherClassRepo = _unitOfWork.Repository<TeacherClass>();
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var classRepo = _unitOfWork.Repository<Class>();

            // Validate teacher exists
            var teacher = await teacherRepo.GetByIdAsync(teacherId);
            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            // Validate class exists
            var classEntity = await classRepo.GetByIdAsync(classId);
            if (classEntity == null)
                return ServiceResult<bool>.Fail("Class not found.");

            // Check if assignment already exists
            var existing = await teacherClassRepo.FirstOrDefaultAsync(tc =>
                tc.TeacherId == teacherId && tc.ClassId == classId
            );

            if (existing != null)
                return ServiceResult<bool>.Fail("Teacher already assigned to this class.");

            // Create new relationship entry
            var teacherClass = new TeacherClass { TeacherId = teacherId, ClassId = classId };

            await teacherClassRepo.AddAsync(teacherClass);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher assigned to class successfully.");
        }

        // Assign Teacher to Subject
        public async Task<ServiceResult<bool>> AssignTeacherToSubjectAsync(
            string teacherId,
            string subjectId
        )
        {
            var teacher = await _unitOfWork
                .Repository<Teacher>()
                .FirstOrDefaultAsync(
                    t => t.Id == teacherId,
                    q => q.Include(t => t.TeacherSubjects!)
                );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            if (teacher.TeacherSubjects!.Any(ts => ts.SubjectId == subjectId))
                return ServiceResult<bool>.Fail("Teacher already assigned to this subject.");

            teacher.TeacherSubjects!.Add(
                new TeacherSubject { TeacherId = teacherId, SubjectId = subjectId }
            );

            _unitOfWork.Repository<Teacher>().Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher assigned to subject successfully.");
        }

        // Unassign Teacher
        public async Task<ServiceResult<bool>> UnassignTeacherAsync(
            string teacherId,
            string adminUserId
        )
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                q => q.Include(t => t.ClassAppointments)
            );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ClassAppointments.Clear();
            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Class Unassignment",
                message: $"Dear {teacher.AppUser.FullName}, you have been unassigned from your class responsibilities.",
                type: "Class Change",
                creatorUserId: adminUserId,
                targetUserIds: new[] { teacher.AppUserId },
                role: "Teacher"
            );

            return ServiceResult<bool>.Ok(true, "Teacher unassigned successfully.");
        }

        //Detect Duplicate Teacher Assignments
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetDuplicateTeacherAssignmentsAsync()
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();

            // Get teachers who are assigned to the same class multiple times through TeacherClasses
            var teacherClasses = await _unitOfWork.Repository<TeacherClass>()
                .AsQueryable()
                .GroupBy(tc => new { tc.TeacherId, tc.ClassId })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key.TeacherId)
                .ToListAsync();

            var duplicates = await teacherRepo
                .FindAllAsync(
                    t => teacherClasses.Contains(t.Id),
                    q => q.Include(t => t.AppUser)
                          .Include(t => t.TeacherClasses)
                          .ThenInclude(tc => tc.Class)
                );

            var dto = duplicates.Select(t => new TeacherAdminViewDto
            {
                Id = t.Id,
                FullName = t.AppUser.FullName,
                Email = t.Email,
                ClassNames = t.TeacherClasses.Select(tc => tc.Class.ClassName).ToList(),
                ProfileStatus = t.AccountStatus.ToString(),
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(dto);
        }

        //Get Class Results
        public async Task<ServiceResult<IEnumerable<ClassResultDto>>> GetClassResultsAsync(string classId)
        {
            var examRepo = _unitOfWork.StudentExamAnswers;

            // Fetch all student exam answers for students belonging to the target class
            var results = await examRepo.FindAllAsync(
                e => e.Student.ClassId == classId, // DIRECT class relationship
                q => q.Include(e => e.Student)
                      .ThenInclude(s => s.Class) // Include class
                      .Include(e => e.Exam)
            );

            // Group results by class and compute aggregates
            var grouped = results
                .GroupBy(e => e.Student.Class)
                .Select(g => new ClassResultDto
                {
                    ClassId = g.Key!.Id,
                    ClassName = g.Key.ClassName,
                    StudentCount = g.Select(e => e.StudentId).Distinct().Count(),
                    ExamCount = g.Select(e => e.ExamId).Distinct().Count(),
                    AverageMark = g.Any() ? (double)g.Average(e => e.Exam?.MinMark ?? 0) : 0,
                });

            return ServiceResult<IEnumerable<ClassResultDto>>.Ok(grouped);
        }

        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetPendingTeachersAsync()
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();

            var pendingTeachers = await teacherRepo.FindAllAsync(
                predicate: t => t.AccountStatus == AccountStatus.Pending,
                include: q => q.Include(t => t.AppUser)
            );

            if (!pendingTeachers.Any())
            {
                return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(
                    Enumerable.Empty<TeacherAdminViewDto>(),
                    "No pending teachers found."
                );
            }
            var result = _mapper.Map<IEnumerable<TeacherAdminViewDto>>(pendingTeachers);

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(
                result,
                "Pending teachers retrieved successfully."
            );
        }

        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetPendingStudentsAsync()
        {
            var studentRepo = _unitOfWork.Repository<Student>();

            var pendingStudents = await studentRepo.FindAllAsync(
                predicate: s => s.AccountStatus == AccountStatus.Pending,
                include: q => q.Include(s => s.AppUser)
                               .Include(s => s.Class) // Include class
                               .ThenInclude(c => c.Grade) // Include grade for grade name
            );

            if (!pendingStudents.Any())
                return ServiceResult<IEnumerable<StudentViewDto>>.Ok(
                    Enumerable.Empty<StudentViewDto>(),
                    "No pending students found."
                );

            var result = pendingStudents.Select(s => new StudentViewDto
            {
                Id = s.Id,
                FullName = s.AppUser.FullName,
                Email = s.Email,
                ClassName = s.Class?.ClassName ?? "N/A",
                GradeName = s.Class?.Grade?.GradeName ?? "N/A", // Add grade name
                AccountStatus = s.AccountStatus.ToString(),
            });

            return ServiceResult<IEnumerable<StudentViewDto>>.Ok(
                result,
                "Pending students retrieved successfully."
            );
        }

        public async Task<ServiceResult<bool>> MoveStudentToAnotherClassAsync(
            string studentId,
            string? newClassId,
            string adminUserId
)
        {
            Console.WriteLine($"Moving student {studentId} to class {newClassId ?? "NULL"}");

            var studentRepo = _unitOfWork.Repository<Student>();
            var classRepo = _unitOfWork.Repository<Class>();

            // Check if student exists
            var student = await studentRepo.FirstOrDefaultAsync(
                s => s.Id == studentId,
                q => q.Include(s => s.Class).Include(s => s.AppUser)
            );

            if (student == null)
            {
                Console.WriteLine($"Student with ID {studentId} not found");
                return ServiceResult<bool>.Fail("Student not found.");
            }

            Console.WriteLine($"Found student: {student.AppUser.FullName}");

            string? oldClassName = student.Class?.ClassName;

            // If newClassId is null or empty → unassign the student
            if (string.IsNullOrEmpty(newClassId))
            {
                Console.WriteLine("Unassigning student from class");
                student.ClassId = null!;
            }
            else
            {
                Console.WriteLine($"Looking for class with ID: {newClassId}");
                var newClass = await classRepo.FirstOrDefaultAsync(c => c.Id == newClassId);

                if (newClass == null)
                {
                    Console.WriteLine($"Class with ID {newClassId} not found in database");

                    // Let's check what classes actually exist
                    var allClasses = await classRepo.GetAllAsync();
                    Console.WriteLine($"Available classes: {string.Join(", ", allClasses.Select(c => $"{c.Id}: {c.ClassName}"))}");

                    return ServiceResult<bool>.Fail("New class not found.");
                }

                Console.WriteLine($"Found class: {newClass.ClassName}");
                student.ClassId = newClassId;
                student.Class = newClass;
            }

            studentRepo.Update(student);
            await _unitOfWork.SaveChangesAsync();
            Console.WriteLine("Student moved successfully");
            await _unitOfWork.SaveChangesAsync();

            // Build notification message
            string message;
            if (string.IsNullOrEmpty(newClassId))
                message = $"Dear {student.AppUser.FullName}, you have been unassigned from your previous class ({oldClassName ?? "N/A"}).";
            else
                message = $"Dear {student.AppUser.FullName}, you have been moved to class {student.Class?.ClassName ?? "N/A"} successfully.";

            await _notificationService.CreateNotificationAsync(
                title: "Class Assignment Updated",
                message: message,
                type: "Class Change",
                creatorUserId: adminUserId,
                targetUserIds: new[] { student.AppUserId },
                role: "Student"
            );

            return ServiceResult<bool>.Ok(true, "Student moved successfully.");
        }

        public async Task<ServiceResult<IEnumerable<ParentViewWithChildrenDto>>> GetPendingParentsAsync()
        {
            var parentRepo = _unitOfWork.Repository<Parent>();

            var pendingParents = await parentRepo.FindAllAsync(
                predicate: p => p.AccountStatus == AccountStatus.Pending,
                include: q => q.Include(p => p.AppUser)
            );

            if (!pendingParents.Any())
                return ServiceResult<IEnumerable<ParentViewWithChildrenDto>>.Ok(
                    Enumerable.Empty<ParentViewWithChildrenDto>(),
                    "No pending parents found."
                );

            var result = pendingParents.Select(p => new ParentViewWithChildrenDto
            {
                Id = p.Id,
                FullName = p.AppUser.FullName,
                Email = p.Email,
            });

            return ServiceResult<IEnumerable<ParentViewWithChildrenDto>>.Ok(
                result,
                "Pending parents retrieved successfully."
            );
        }

        public async Task<ServiceResult<IEnumerable<Subject>>> GetSubjectsByGradeAsync(string gradeId)
        {
            var subjectRepo = _unitOfWork.Repository<Subject>();
            var subjects = await subjectRepo.FindAllAsync(
                s => s.GradeId == gradeId,
                q => q.Include(s => s.Grade)
            );

            return ServiceResult<IEnumerable<Subject>>.Ok(subjects);
        }

        // Unassign Teacher from Class
        public async Task<ServiceResult<bool>> UnassignTeacherFromClassAsync(string teacherId, string classId)
        {
            var teacherClassRepo = _unitOfWork.Repository<TeacherClass>();

            // Find the specific teacher-class assignment
            var assignment = await teacherClassRepo.FirstOrDefaultAsync(tc =>
                tc.TeacherId == teacherId && tc.ClassId == classId
            );

            if (assignment == null)
                return ServiceResult<bool>.Fail("Teacher is not assigned to this class.");

            // Remove the assignment
            teacherClassRepo.Delete(assignment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher unassigned from class successfully.");
        }

        // Bulk Assign Teachers
        // Bulk Assign Teachers

        public async Task<ServiceResult<bool>> BulkAssignTeachersAsync(BulkAssignTeachersDto dto)
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacherClassRepo = _unitOfWork.Repository<TeacherClass>();
            var classRepo = _unitOfWork.Repository<Class>();

            // Validate input
            if (!dto.TeacherIds.Any() || !dto.ClassIds.Any())
                return ServiceResult<bool>.Fail("Both TeacherIds and ClassIds are required.");

            // Validate all teachers exist
            var teachers = await teacherRepo.FindAllAsync(t => dto.TeacherIds.Contains(t.Id));
            if (teachers.Count() != dto.TeacherIds.Count())
                return ServiceResult<bool>.Fail("One or more teachers not found.");

            // Validate all classes exist
            var classes = await classRepo.FindAllAsync(c => dto.ClassIds.Contains(c.Id));
            if (classes.Count() != dto.ClassIds.Count())
                return ServiceResult<bool>.Fail("One or more classes not found.");

            // Get existing assignments to avoid duplicates
            var existingAssignments = await teacherClassRepo.FindAllAsync(tc =>
                dto.TeacherIds.Contains(tc.TeacherId) && dto.ClassIds.Contains(tc.ClassId)
            );

            var existingAssignmentKeys = existingAssignments
                .Select(ea => (ea.TeacherId, ea.ClassId))
                .ToHashSet();

            // Create new assignments (all combinations of teachers and classes)
            var newAssignmentsCount = 0;

            foreach (var teacherId in dto.TeacherIds)
            {
                foreach (var classId in dto.ClassIds)
                {
                    // Skip if assignment already exists
                    if (existingAssignmentKeys.Contains((teacherId, classId)))
                        continue;

                    // Create and add new assignment
                    var newAssignment = new TeacherClass
                    {
                        TeacherId = teacherId,
                        ClassId = classId
                    };

                    await teacherClassRepo.AddAsync(newAssignment);
                    newAssignmentsCount++;
                }
            }

            if (newAssignmentsCount == 0)
                return ServiceResult<bool>.Fail("All teacher-class assignments already exist.");

            // Save all changes
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true,
                $"Successfully created {newAssignmentsCount} teacher-class assignments.");
        }

        public async Task<ServiceResult<bool>> ApproveParentWithStudent(RelationParentWithStudentRequest relationRequest)
        {
            try
            {
                var parent = await _parentRepository.GetByIdAsync(relationRequest.ParentId);
                if (parent == null)
                {
                    return ServiceResult<bool>.Fail("Parent not found");
                }

                var student = await _studentRepository.GetByIdAsync(relationRequest.StudentId);
                if (student == null)
                {
                    return ServiceResult<bool>.Fail("Student not found");
                }

                if (parent.AccountStatus == AccountStatus.Active)
                {
                    return await AddStudentToExistingParent(relationRequest.ParentId, relationRequest.StudentId);
                }

                var existingRelationship = await _parentStudentRepository
                    .FirstOrDefaultAsync(ps => ps.ParentId == relationRequest.ParentId && ps.StudentId == relationRequest.StudentId);

                if (existingRelationship == null)
                {
                    var parentStudent = new ParentStudent
                    {
                        ParentId = relationRequest.ParentId,
                        StudentId = relationRequest.StudentId,
                        Relation = relationRequest.Relation ?? parent.Relation
                    };

                    await _parentStudentRepository.AddAsync(parentStudent);
                }
                else
                {
                    if (!string.IsNullOrEmpty(relationRequest.Relation))
                    {
                        existingRelationship.Relation = relationRequest.Relation;
                        _parentStudentRepository.Update(existingRelationship);
                    }
                }

                _parentRepository.Update(parent);

                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true, "Parent approved successfully");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred while approving parent");
            }
        }
        public async Task<ServiceResult<bool>> ApproveParentBulk(ParentApprovalBulkDto bulkDto)
        {

            try
            {
                var parent = await _parentRepository.GetByIdAsync(bulkDto.ParentId);
                if (parent == null)
                {
                    return ServiceResult<bool>.Fail("Parent not found");
                }

                var studentIds = bulkDto.StudentApprovals.Select(s => s.StudentId).ToList();

                var existingStudents = await _studentRepository
                    .FindAllAsync(s => studentIds.Contains(s.Id));

                var existingStudentIds = existingStudents.Select(s => s.Id).ToList();
                var missingStudentIds = studentIds.Except(existingStudentIds).ToList();

                if (missingStudentIds.Any())
                {
                    return ServiceResult<bool>.Fail($"Students not found: {string.Join(", ", missingStudentIds)}");
                }

                var existingRelationships = await _parentStudentRepository
                    .FindAllAsync(ps => ps.ParentId == bulkDto.ParentId);

                foreach (var approval in bulkDto.StudentApprovals)
                {
                    var existingRelationship = existingRelationships
                        .FirstOrDefault(ps => ps.StudentId == approval.StudentId);

                    if (existingRelationship == null)
                    {
                        var parentStudent = new ParentStudent
                        {
                            ParentId = bulkDto.ParentId,
                            StudentId = approval.StudentId,
                            Relation = approval.Relation ?? parent.Relation
                        };

                        await _parentStudentRepository.AddAsync(parentStudent);
                    }
                    else if (!string.IsNullOrEmpty(approval.Relation))
                    {
                        existingRelationship.Relation = approval.Relation;
                        _parentStudentRepository.Update(existingRelationship);
                    }
                }

                parent.AccountStatus = AccountStatus.Active;
                _parentRepository.Update(parent);

                await _unitOfWork.SaveChangesAsync();


                return ServiceResult<bool>.Ok(true,
                    $"Parent approved successfully with {bulkDto.StudentApprovals.Count} student(s)");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred during bulk parent approval");
            }
        }
        public async Task<ServiceResult<bool>> AddStudentToParent(string parentId, ParentStudentApprovalDto studentDto)
        {
            try
            {
                var parent = await _parentRepository
                    .FirstOrDefaultAsync(p => p.Id == parentId && p.AccountStatus == AccountStatus.Active);

                if (parent == null)
                {
                    return ServiceResult<bool>.Fail("Active parent not found");
                }

                var student = await _studentRepository.GetByIdAsync(studentDto.StudentId);
                if (student == null)
                {
                    return ServiceResult<bool>.Fail("Student not found");
                }

                var existingRelationship = await _parentStudentRepository
                    .FirstOrDefaultAsync(ps => ps.ParentId == parentId && ps.StudentId == studentDto.StudentId);

                if (existingRelationship != null)
                {
                    return ServiceResult<bool>.Fail("Student is already linked to this parent");
                }

                // Create new relationship
                var parentStudent = new ParentStudent
                {
                    ParentId = parentId,
                    StudentId = studentDto.StudentId,
                    Relation = studentDto.Relation ?? parent.Relation
                };

                await _parentStudentRepository.AddAsync(parentStudent);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true, "Student added to parent successfully");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred while adding student to parent");
            }
        }
        public async Task<ServiceResult<bool>> RemoveStudentFromParent(string parentId, string studentId)
        {
            try
            {
                var relationship = await _parentStudentRepository
                    .FirstOrDefaultAsync(ps => ps.ParentId == parentId && ps.StudentId == studentId);

                if (relationship == null)
                {
                    return ServiceResult<bool>.Fail("Relationship not found");
                }

                var parent = await _parentRepository.GetByIdAsync(parentId);
                var student = await _studentRepository.GetByIdAsync(studentId);

                _parentStudentRepository.Delete(relationship);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true, "Student removed from parent successfully");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred while removing student from parent");
            }
        }
        public async Task<ServiceResult<bool>> AddStudentToExistingParent(string parentId, string studentId)
        {
            try
            {
                var existingRelationship = await _parentStudentRepository
                    .FirstOrDefaultAsync(ps => ps.ParentId == parentId && ps.StudentId == studentId);

                if (existingRelationship != null)
                {
                    return ServiceResult<bool>.Fail("Student is already linked to this parent");
                }

                var parent = await _parentRepository.GetByIdAsync(parentId);
                var student = await _studentRepository.GetByIdAsync(studentId);

                if (parent == null || student == null)
                {
                    return ServiceResult<bool>.Fail("Parent or student not found");
                }

                var parentStudent = new ParentStudent
                {
                    ParentId = parentId,
                    StudentId = studentId,
                    Relation = parent.Relation

                };

                await _parentStudentRepository.AddAsync(parentStudent);
                await _unitOfWork.SaveChangesAsync();


                return ServiceResult<bool>.Ok(true, "Student added to existing parent successfully");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred while adding student to existing parent");
            }
        }

    }
}
