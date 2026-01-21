using Application.DTOs.ClassDTOs;
using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AdminManagementService : IAdminManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdminManagementService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        //Teachers

        // TeacherCount
        public async Task<ServiceResult<int>> GetTeacherCount()
        {
            var teachers = await _unitOfWork.TeacherRepository.GetAllAsync();

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
            var teachers = await _unitOfWork.TeacherRepository.FindAllAsync(
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

        public async Task<ServiceResult<bool>> ApproveUserAsync(
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
                ContactInfo = t.ContactInfo!,
                Subjects = t.TeacherSubjects!.Select(ts => ts.Subject.SubjectName).ToList(),
                ClassNames = t.TeacherClasses.Select(tc => tc.Class.ClassName).ToList(),
                ProfileStatus = t.AccountStatus.ToString(),
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(dto);
        }

        // Students by Class
        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByClassAsync(
            string classId
        )
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var students = await studentRepo.FindAllAsync(
                s => s.ClassId == classId, // DIRECT class relationship
                q => q.Include(s => s.AppUser).Include(s => s.Class)! // Include the class
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
            var classRepo = _unitOfWork.Repository<Class>();

            // Load teacher with AppUser and SpecializedCurricula
            var teacher = await teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                q => q.Include(t => t.AppUser).Include(t => t.SpecializedCurricula) // Add this
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

            Curriculum? newCurriculum = null;

            // If a new class is provided, add the new relationship
            if (!string.IsNullOrEmpty(newClassId))
            {
                var newClass = await classRepo.FirstOrDefaultAsync(
                    c => c.Id == newClassId,
                    q => q.Include(c => c.Grade).ThenInclude(g => g.Curriculum) // Include curriculum
                );

                if (newClass == null)
                    return ServiceResult<bool>.Fail("New class not found.");

                var newAssignment = new TeacherClass
                {
                    TeacherId = teacherId,
                    ClassId = newClassId,
                };

                await teacherClassRepo.AddAsync(newAssignment);

                // NEW: Update teacher's curriculum specialization
                if (newClass.Grade?.Curriculum != null)
                {
                    teacher.SpecializedCurricula ??= new List<Curriculum>();

                    // Check if teacher is already specialized in this curriculum
                    var isAlreadySpecialized = teacher.SpecializedCurricula.Any(c =>
                        c.Id == newClass.Grade.CurriculumId
                    );

                    if (!isAlreadySpecialized)
                    {
                        teacher.SpecializedCurricula.Add(newClass.Grade.Curriculum);
                        teacherRepo.Update(teacher);
                        newCurriculum = newClass.Grade.Curriculum;
                    }
                }
            }
            else
            {
                // If unassigning from all classes, you might want to clear curricula
                // or keep them if the teacher should remain specialized
                // teacher.SpecializedCurricula?.Clear(); // Uncomment if you want to clear
            }

            // Persist changes
            await _unitOfWork.SaveChangesAsync();

            // Prepare notification message
            var teacherName = teacher.AppUser?.FullName ?? "Teacher";
            string message;

            if (newClassId == null)
            {
                message = $"Dear {teacherName}, you have been unassigned from all your classes.";
            }
            else
            {
                var curriculumName = newCurriculum?.Name ?? "the class curriculum";
                message =
                    $"Dear {teacherName}, you have been assigned to a new class and are now specialized in {curriculumName}.";
            }

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
            var teacher = await teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                q => q.Include(t => t.SpecializedCurricula) // Add this to include curricula
            );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            // Validate class exists with its curriculum
            var classEntity = await classRepo.FirstOrDefaultAsync(
                c => c.Id == classId,
                q =>
                    q.Include(g => g.Grade) // Include grade
                        .ThenInclude(c => c.Curriculum) // Include curriculum
            );

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

            // NEW: Add teacher to class's curriculum if curriculum exists
            if (classEntity.Grade?.CurriculumId != null && classEntity.Grade.Curriculum != null)
            {
                // Initialize SpecializedCurricula if null
                teacher.SpecializedCurricula ??= new List<Curriculum>();

                // Check if teacher is already specialized in this curriculum
                var isAlreadySpecialized = teacher.SpecializedCurricula.Any(c =>
                    c.Id == classEntity.Grade.CurriculumId
                );

                if (!isAlreadySpecialized)
                {
                    teacher.SpecializedCurricula.Add(classEntity.Grade.Curriculum);
                    teacherRepo.Update(teacher);
                }
            }

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

        //unassign from subject
        public async Task<ServiceResult<bool>> UnAssignTeacherFromSubjectAsync(
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

            var teacherSubject = teacher.TeacherSubjects!.FirstOrDefault(ts =>
                ts.SubjectId == subjectId
            );

            if (teacherSubject == null)
                return ServiceResult<bool>.Fail("Teacher is not assigned to this subject.");

            teacher.TeacherSubjects!.Remove(teacherSubject);

            _unitOfWork.Repository<Teacher>().Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher unassigned from subject successfully.");
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
        public async Task<
            ServiceResult<IEnumerable<TeacherAdminViewDto>>
        > GetDuplicateTeacherAssignmentsAsync()
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();

            // Get teachers who are assigned to the same class multiple times through TeacherClasses
            var teacherClasses = await _unitOfWork
                .Repository<TeacherClass>()
                .AsQueryable()
                .GroupBy(tc => new { tc.TeacherId, tc.ClassId })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key.TeacherId)
                .ToListAsync();

            var duplicates = await teacherRepo.FindAllAsync(
                t => teacherClasses.Contains(t.Id),
                q =>
                    q.Include(t => t.AppUser)
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
        public async Task<ServiceResult<IEnumerable<ClassResultDto>>> GetClassResultsAsync(
            string classId
        )
        {
            var examRepo = _unitOfWork.StudentExamAnswers;

            // Fetch all student exam answers for students belonging to the target class
            var results = await examRepo.FindAllAsync(
                e => e.Student.ClassId == classId, // DIRECT class relationship
                q =>
                    q.Include(e => e.Student)
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
                include: q =>
                    q.Include(s => s.AppUser)
                        .Include(s => s.Class) // Include class
                        .ThenInclude(c => c!.Grade) // Include grade for grade name
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
            var curriculumRepo = _unitOfWork.Repository<Curriculum>();

            // Check if student exists
            var student = await studentRepo.FirstOrDefaultAsync(
                s => s.Id == studentId,
                q =>
                    q.Include(s => s.Class)
                        .ThenInclude(c => c!.Grade)
                        .Include(c => c.Curriculum)
                        .Include(s => s.AppUser)
            );

            if (student == null)
            {
                Console.WriteLine($"Student with ID {studentId} not found");
                return ServiceResult<bool>.Fail("Student not found.");
            }

            Console.WriteLine($"Found student: {student.AppUser.FullName}");

            string? oldClassName = student.Class?.ClassName;
            Curriculum? oldCurriculum = student.Class?.Grade.Curriculum;

            // If newClassId is null or empty → unassign the student
            if (string.IsNullOrEmpty(newClassId))
            {
                Console.WriteLine("Unassigning student from class");
                student.ClassId = null!;
                student.CurriculumId = null; // Also remove curriculum
            }
            else
            {
                Console.WriteLine($"Looking for class with ID: {newClassId}");
                // Load class with its curriculum
                var newClass = await classRepo.FirstOrDefaultAsync(
                    c => c.Id == newClassId,
                    q => q.Include(g => g.Grade).ThenInclude(c => c.Curriculum)
                );

                if (newClass == null)
                {
                    Console.WriteLine($"Class with ID {newClassId} not found in database");

                    // Let's check what classes actually exist
                    var allClasses = await classRepo.GetAllAsync();
                    Console.WriteLine(
                        $"Available classes: {string.Join(", ", allClasses.Select(c => $"{c.Id}: {c.ClassName}"))}"
                    );

                    return ServiceResult<bool>.Fail("New class not found.");
                }

                Console.WriteLine($"Found class: {newClass.ClassName}");
                student.ClassId = newClassId;
                student.Class = newClass;

                // Set curriculum from the class
                if (newClass.Grade.CurriculumId != null)
                {
                    student.CurriculumId = newClass.Grade.CurriculumId;
                    Console.WriteLine($"Setting curriculum to: {newClass.Grade.CurriculumId}");
                }
                else
                {
                    student.CurriculumId = null;
                    Console.WriteLine("Warning: Class has no curriculum assigned");
                }
            }

            studentRepo.Update(student);
            await _unitOfWork.SaveChangesAsync();
            Console.WriteLine("Student moved successfully");

            // Build notification message
            string message;
            if (string.IsNullOrEmpty(newClassId))
            {
                message =
                    $"Dear {student.AppUser.FullName}, you have been unassigned from your previous class ({oldClassName ?? "N/A"}).";
            }
            else
            {
                var curriculumName = student.Class?.Grade.Curriculum?.Name ?? "N/A";
                message =
                    $"Dear {student.AppUser.FullName}, you have been moved to class {student.Class?.ClassName ?? "N/A"} with curriculum {curriculumName}.";
            }

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

        public async Task<ServiceResult<bool>> UpdateStudentCurriculumAsync(
            string studentId,
            string curriculumId,
            string adminUserId
        )
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var curriculumRepo = _unitOfWork.Repository<Curriculum>();

            var student = await studentRepo.FirstOrDefaultAsync(
                s => s.Id == studentId,
                q => q.Include(s => s.AppUser).Include(s => s.Class)!
            );

            if (student == null)
                return ServiceResult<bool>.Fail("Student not found.");

            // Validate curriculum exists
            var curriculum = await curriculumRepo.GetByIdAsync(curriculumId);
            if (curriculum == null)
                return ServiceResult<bool>.Fail("Curriculum not found.");

            student.CurriculumId = curriculumId;
            studentRepo.Update(student);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Curriculum Updated",
                message: $"Dear {student.AppUser.FullName}, your curriculum has been updated to {curriculum.Name}.",
                type: "Curriculum Change",
                creatorUserId: adminUserId,
                targetUserIds: new[] { student.AppUserId },
                role: "Student"
            );

            return ServiceResult<bool>.Ok(true, "Student curriculum updated successfully.");
        }

        public async Task<ServiceResult<bool>> BulkUpdateCurriculumForClassAsync(
            string classId,
            string curriculumId,
            string adminUserId
        )
        {
            var classRepo = _unitOfWork.Repository<Class>();
            var studentRepo = _unitOfWork.Repository<Student>();
            var curriculumRepo = _unitOfWork.Repository<Curriculum>();

            // Validate curriculum exists
            var curriculum = await curriculumRepo.GetByIdAsync(curriculumId);
            if (curriculum == null)
                return ServiceResult<bool>.Fail("Curriculum not found.");

            // Update class curriculum
            var classEntity = await classRepo.GetByIdAsync(classId);
            if (classEntity == null)
                return ServiceResult<bool>.Fail("Class not found.");

            classEntity.Grade.CurriculumId = curriculumId;
            classRepo.Update(classEntity);

            // Update all students in the class
            var students = await studentRepo.FindAllAsync(
                s => s.ClassId == classId,
                q => q.Include(s => s.AppUser)
            );

            foreach (var student in students)
            {
                student.CurriculumId = curriculumId;
                studentRepo.Update(student);
            }

            await _unitOfWork.SaveChangesAsync();

            // Send notifications
            var studentIds = students.Select(s => s.AppUserId).ToList();
            await _notificationService.CreateNotificationAsync(
                title: "Class Curriculum Updated",
                message: $"The curriculum for your class {classEntity.ClassName} has been updated to {curriculum.Name}.",
                type: "Curriculum Change",
                creatorUserId: adminUserId,
                targetUserIds: studentIds,
                role: "Student"
            );

            return ServiceResult<bool>.Ok(
                true,
                $"Updated curriculum for class {classEntity.ClassName} and {students.Count()} students."
            );
        }

        public async Task<
            ServiceResult<IEnumerable<ParentViewWithChildrenDto>>
        > GetPendingParentsAsync()
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

        public async Task<ServiceResult<IEnumerable<Subject>>> GetSubjectsByGradeAsync(
            string gradeId
        )
        {
            var subjectRepo = _unitOfWork.Repository<Subject>();
            var subjects = await subjectRepo.FindAllAsync(
                s => s.GradeId == gradeId,
                q => q.Include(s => s.Grade)
            );

            return ServiceResult<IEnumerable<Subject>>.Ok(subjects);
        }

        // Unassign Teacher from Class
        public async Task<ServiceResult<bool>> UnassignTeacherFromClassAsync(
            string teacherId,
            string classId
        )
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

            // Validate all teachers exist with curricula
            var teachers = await teacherRepo.FindAllAsync(
                t => dto.TeacherIds.Contains(t.Id),
                q => q.Include(t => t.SpecializedCurricula) // Add this
            );

            if (teachers.Count() != dto.TeacherIds.Count())
                return ServiceResult<bool>.Fail("One or more teachers not found.");

            // Validate all classes exist with their curricula
            var classes = await classRepo.FindAllAsync(
                c => dto.ClassIds.Contains(c.Id),
                q => q.Include(c => c.Grade).ThenInclude(g => g.Curriculum) // Add this
            );

            if (classes.Count() != dto.ClassIds.Count())
                return ServiceResult<bool>.Fail("One or more classes not found.");

            // Get existing assignments to avoid duplicates
            var existingAssignments = await teacherClassRepo.FindAllAsync(tc =>
                dto.TeacherIds.Contains(tc.TeacherId) && dto.ClassIds.Contains(tc.ClassId)
            );

            var existingAssignmentKeys = existingAssignments
                .Select(ea => (ea.TeacherId, ea.ClassId))
                .ToHashSet();

            // Dictionary to track which teachers need which curricula
            var teacherCurriculumUpdates = new Dictionary<string, List<Curriculum>>();

            // Create new assignments (all combinations of teachers and classes)
            var newAssignmentsCount = 0;

            foreach (var teacher in teachers)
            {
                foreach (var classEntity in classes)
                {
                    // Skip if assignment already exists
                    if (existingAssignmentKeys.Contains((teacher.Id, classEntity.Id)))
                        continue;

                    // Create and add new assignment
                    var newAssignment = new TeacherClass
                    {
                        TeacherId = teacher.Id,
                        ClassId = classEntity.Id,
                    };

                    await teacherClassRepo.AddAsync(newAssignment);
                    newAssignmentsCount++;

                    // NEW: Track curriculum assignments
                    if (classEntity.Grade?.Curriculum != null)
                    {
                        // Initialize teacher's curricula if needed
                        teacher.SpecializedCurricula ??= new List<Curriculum>();

                        // Check if teacher is already specialized in this curriculum
                        var isAlreadySpecialized = teacher.SpecializedCurricula.Any(c =>
                            c.Id == classEntity.Grade.CurriculumId
                        );

                        if (!isAlreadySpecialized)
                        {
                            teacher.SpecializedCurricula.Add(classEntity.Grade.Curriculum);

                            // Track for update
                            if (!teacherCurriculumUpdates.ContainsKey(teacher.Id))
                            {
                                teacherCurriculumUpdates[teacher.Id] = new List<Curriculum>();
                            }
                            teacherCurriculumUpdates[teacher.Id].Add(classEntity.Grade.Curriculum);
                        }
                    }
                }
            }

            if (newAssignmentsCount == 0)
                return ServiceResult<bool>.Fail("All teacher-class assignments already exist.");

            // Update teachers with new curricula
            foreach (var teacherUpdate in teacherCurriculumUpdates)
            {
                var teacher = teachers.First(t => t.Id == teacherUpdate.Key);
                teacherRepo.Update(teacher);
            }

            // Save all changes
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(
                true,
                $"Successfully created {newAssignmentsCount} teacher-class assignments "
                    + $"and updated {teacherCurriculumUpdates.Count} teachers with curriculum specializations."
            );
        }

        public async Task<ServiceResult<bool>> ApproveParentWithStudent(
            RelationParentWithStudentRequest relationRequest
        )
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.GetByIdAsync(
                    relationRequest.ParentId
                );
                if (parent == null)
                {
                    return ServiceResult<bool>.Fail("Parent not found");
                }

                var student = await _unitOfWork.StudentRepository.GetByIdAsync(
                    relationRequest.StudentId
                );
                if (student == null)
                {
                    return ServiceResult<bool>.Fail("Student not found");
                }

                if (parent.AccountStatus == AccountStatus.Active)
                {
                    return await AddStudentToExistingParent(relationRequest);
                }

                var existingRelationship =
                    await _unitOfWork.ParentStudentRepository.FirstOrDefaultAsync(ps =>
                        ps.ParentId == relationRequest.ParentId
                        && ps.StudentId == relationRequest.StudentId
                    );

                if (existingRelationship == null)
                {
                    var parentStudent = new ParentStudent
                    {
                        ParentId = relationRequest.ParentId,
                        StudentId = relationRequest.StudentId,
                        // Use the request relation, only fallback if it's empty
                        Relation = !string.IsNullOrEmpty(relationRequest.Relation)
                            ? relationRequest.Relation
                            : "No Relation",
                    };

                    await _unitOfWork.ParentStudentRepository.AddAsync(parentStudent);
                }

                _unitOfWork.ParentRepository.Update(parent);

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
                var parent = await _unitOfWork.ParentRepository.GetByIdAsync(bulkDto.ParentId);
                if (parent == null)
                {
                    return ServiceResult<bool>.Fail("Parent not found");
                }

                var studentIds = bulkDto.StudentApprovals.Select(s => s.StudentId).ToList();

                var existingStudents = await _unitOfWork.StudentRepository.FindAllAsync(s =>
                    studentIds.Contains(s.Id)
                );

                var existingStudentIds = existingStudents.Select(s => s.Id).ToList();
                var missingStudentIds = studentIds.Except(existingStudentIds).ToList();

                if (missingStudentIds.Any())
                {
                    return ServiceResult<bool>.Fail(
                        $"Students not found: {string.Join(", ", missingStudentIds)}"
                    );
                }

                var existingRelationships = await _unitOfWork.ParentStudentRepository.FindAllAsync(
                    ps => ps.ParentId == bulkDto.ParentId
                );

                foreach (var approval in bulkDto.StudentApprovals)
                {
                    var existingRelationship = existingRelationships.FirstOrDefault(ps =>
                        ps.StudentId == approval.StudentId
                    );

                    if (existingRelationship == null)
                    {
                        var parentStudent = new ParentStudent
                        {
                            ParentId = bulkDto.ParentId,
                            StudentId = approval.StudentId,
                            Relation = approval.Relation,
                        };

                        await _unitOfWork.ParentStudentRepository.AddAsync(parentStudent);
                    }
                    else if (!string.IsNullOrEmpty(approval.Relation))
                    {
                        existingRelationship.Relation = approval.Relation;
                        _unitOfWork.ParentStudentRepository.Update(existingRelationship);
                    }
                }

                parent.AccountStatus = AccountStatus.Active;
                _unitOfWork.ParentRepository.Update(parent);

                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(
                    true,
                    $"Parent approved successfully with {bulkDto.StudentApprovals.Count} student(s)"
                );
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred during bulk parent approval");
            }
        }

        public async Task<ServiceResult<bool>> AddStudentToParent(
            string parentId,
            ParentStudentApprovalDto studentDto
        )
        {
            try
            {
                var parent = await _unitOfWork.ParentRepository.FirstOrDefaultAsync(p =>
                    p.Id == parentId && p.AccountStatus == AccountStatus.Active
                );

                if (parent == null)
                {
                    return ServiceResult<bool>.Fail("Active parent not found");
                }

                var student = await _unitOfWork.StudentRepository.GetByIdAsync(
                    studentDto.StudentId
                );
                if (student == null)
                {
                    return ServiceResult<bool>.Fail("Student not found");
                }

                var existingRelationship =
                    await _unitOfWork.ParentStudentRepository.FirstOrDefaultAsync(ps =>
                        ps.ParentId == parentId && ps.StudentId == studentDto.StudentId
                    );

                if (existingRelationship != null)
                {
                    return ServiceResult<bool>.Fail("Student is already linked to this parent");
                }

                // Create new relationship
                var parentStudent = new ParentStudent
                {
                    ParentId = parentId,
                    StudentId = studentDto.StudentId,
                    Relation = studentDto.Relation,
                };

                await _unitOfWork.ParentStudentRepository.AddAsync(parentStudent);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true, "Student added to parent successfully");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail("An error occurred while adding student to parent");
            }
        }

        public async Task<ServiceResult<bool>> RemoveStudentFromParent(
            RemoveStudentFromParentRequest request
        )
        {
            try
            {
                var relationship = await _unitOfWork.ParentStudentRepository.FirstOrDefaultAsync(
                    ps => ps.ParentId == request.ParentId && ps.StudentId == request.StudentId
                );

                if (relationship == null)
                {
                    return ServiceResult<bool>.Fail("Relationship not found");
                }

                var parent = await _unitOfWork.ParentRepository.GetByIdAsync(request.ParentId);
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId);

                _unitOfWork.ParentStudentRepository.Delete(relationship);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true, "Student removed from parent successfully");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail(
                    "An error occurred while removing student from parent"
                );
            }
        }

        public async Task<ServiceResult<bool>> AddStudentToExistingParent(
            RelationParentWithStudentRequest relationParentRequest
        )
        {
            try
            {
                var existingRelationship =
                    await _unitOfWork.ParentStudentRepository.FirstOrDefaultAsync(ps =>
                        ps.ParentId == relationParentRequest.ParentId
                        && ps.StudentId == relationParentRequest.StudentId
                    );

                if (existingRelationship != null)
                {
                    return ServiceResult<bool>.Fail("Student is already linked to this parent");
                }

                var parent = await _unitOfWork.ParentRepository.GetByIdAsync(
                    relationParentRequest.ParentId
                );
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(
                    relationParentRequest.StudentId
                );

                if (parent == null || student == null)
                {
                    return ServiceResult<bool>.Fail("Parent or student not found");
                }

                var parentStudent = new ParentStudent
                {
                    ParentId = relationParentRequest.ParentId,
                    StudentId = relationParentRequest.StudentId,
                    // Use the provided relation, fallback to parent.Relation if needed
                    Relation = !string.IsNullOrEmpty(relationParentRequest.Relation)
                        ? relationParentRequest.Relation
                        : "No Relation",
                };

                await _unitOfWork.ParentStudentRepository.AddAsync(parentStudent);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(
                    true,
                    "Student added to existing parent successfully"
                );
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Fail(
                    "An error occurred while adding student to existing parent"
                );
            }
        }
    }
}
