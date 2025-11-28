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
        private readonly IAdminRepository _adminRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdminManagementService(
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
            var teachers = await _teacherRepository.FindAllAsync(
                t => t.TeacherSubjects!.Any(ts => ts.Subject.SubjectName == subjectName),
                q => q.Include(t => t.TeacherSubjects!)
                      .ThenInclude(ts => ts.Subject)
                      .Include(t => t.TeacherClasses)
                      .ThenInclude(tc => tc.Class)
                      .Include(t => t.AppUser)
            );

            if (!teachers.Any())
                return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Fail("No teachers found for this subject.");

            var dto = teachers.Select(t => new TeacherAdminViewDto
            {
                Id = t.Id,
                FullName = t.AppUser.FullName,
                Email = t.Email,
                Subjects = t.TeacherSubjects!.Select(ts => ts.Subject.SubjectName).ToList(),
                ClassNames = t.TeacherClasses.Select(tc => tc.Class.ClassName).ToList(),
                ProfileStatus = t.ProfileStatus.ToString(),
            });

            return ServiceResult<IEnumerable<TeacherAdminViewDto>>.Ok(dto, "Teachers retrieved successfully.");
        }



        //New
        //Approve Teacher
        public async Task<ServiceResult<bool>> ApproveTeacherAsync(
            string teacherId,
            string creatorUserId
        )
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                q => q.Include(t => t.AppUser)
            );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ProfileStatus = ProfileStatus.Approved;
            teacher.AppUser.AccountStatus = AccountStatus.Active;

            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.CreateNotificationAsync(
                title: "Profile Approved",
                message: $"Dear {teacher.AppUser.FullName}, your profile has been approved by the admin.",
                type: "Approval",
                creatorUserId: creatorUserId, // or admin Id if available
                targetUserIds: new[] { teacher.AppUserId },
                role: "Teacher"
            );

            return ServiceResult<bool>.Ok(true, "Teacher approved successfully.");
        }

        //Reject Teacher
        public async Task<ServiceResult<bool>> RejectTeacherAsync(
            string teacherId,
            string adminUserId
        )
        {
            var teacherRepo = _unitOfWork.Repository<Teacher>();
            var teacher = await teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                q => q.Include(t => t.AppUser)
            );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            teacher.ProfileStatus = ProfileStatus.Rejected;
            teacher.AppUser.AccountStatus = AccountStatus.PendingApproval;

            teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Rejected",
                message: $"Dear {teacher.AppUser.FullName}, your profile has been rejected by the admin. Please review and resubmit.",
                type: "Rejection",
                creatorUserId: adminUserId,
                targetUserIds: new[] { teacher.AppUserId },
                role: "Teacher"
            );
            return ServiceResult<bool>.Ok(true, "Teacher rejected successfully.");
        }

        // Block User
        public async Task<ServiceResult<bool>> BlockUserAsync(
            string appUserId,
            string adminUserId,
            string role
        )
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.AccountStatus = AccountStatus.Blocked;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Blocked",
                message: $"Dear {user.FullName}, your account has been blocked. Please contact support for more information.",
                type: "Account Status",
                creatorUserId: adminUserId,
                targetUserIds: new[] { user.Id },
                role: role
            );

            return ServiceResult<bool>.Ok(true, "User blocked successfully.");
        }

        //Unblock User
        public async Task<ServiceResult<bool>> UnblockUserAsync(
            string appUserId,
            string adminUserId,
            string role
        )
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("User not found.");

            user.AccountStatus = AccountStatus.Active;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.CreateNotificationAsync(
                title: "Account Unblocked",
                message: $"Dear {user.FullName}, your account has been unblocked. You can now access all features.",
                type: "Account Status",
                creatorUserId: adminUserId,
                targetUserIds: new[] { user.Id },
                role: role
            );
            return ServiceResult<bool>.Ok(true, "User unblocked successfully.");
        }

        //Teachers by Class
        public async Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeachersByClassAsync(string classId)
        {
            var teachers = await _unitOfWork.Repository<Teacher>()
                .FindAllAsync(
                    t => t.TeacherClasses.Any(tc => tc.ClassId == classId),
                    q => q.Include(t => t.AppUser)
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
                Subjects = t.TeacherSubjects!.Select(ts => ts.Subject.SubjectName).ToList(),
                ClassNames = t.TeacherClasses.Select(tc => tc.Class.ClassName).ToList(),
                ProfileStatus = t.ProfileStatus.ToString(),
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
                      .Include(s => s.Class) // Include the class
            );

            var dto = students.Select(s => new StudentViewDto
            {
                Id = s.Id,
                FullName = s.AppUser.FullName,
                Email = s.Email,
                ClassName = s.Class?.ClassName ?? "N/A",
                ProfileStatus = s.ProfileStatus.ToString(),
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
            var existingAssignments = (await teacherClassRepo.FindAllAsync(
                tc => tc.TeacherId == teacherId
            )).ToList();

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
                    ClassId = newClassId
                };

                await teacherClassRepo.AddAsync(newAssignment);
            }

            // Persist changes
            await _unitOfWork.SaveChangesAsync();

            // Prepare notification message (use AppUser.FullName if available)
            var teacherName = teacher.AppUser?.FullName ?? "Teacher";
            string message = newClassId == null
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
            var teacherClass = new TeacherClass
            {
                TeacherId = teacherId,
                ClassId = classId
            };

            await teacherClassRepo.AddAsync(teacherClass);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "Teacher assigned to class successfully.");
        }

        // Assign Teacher to Subject
        public async Task<ServiceResult<bool>> AssignTeacherToSubjectAsync(string teacherId, string subjectId)
        {
            var teacher = await _unitOfWork.Repository<Teacher>()
                .FirstOrDefaultAsync(
                    t => t.Id == teacherId,
                    q => q.Include(t => t.TeacherSubjects!)
                );

            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found.");

            if (teacher.TeacherSubjects!.Any(ts => ts.SubjectId == subjectId))
                return ServiceResult<bool>.Fail("Teacher already assigned to this subject.");

            teacher.TeacherSubjects!.Add(new TeacherSubject
            {
                TeacherId = teacherId,
                SubjectId = subjectId
            });

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
                ProfileStatus = t.ProfileStatus.ToString(),
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
                predicate: t => t.ProfileStatus == ProfileStatus.Pending,
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

        //Student
        //Approve Student
        public async Task<ServiceResult<bool>> ApproveStudentAsync(
            string studentId,
            string adminUserId
        )
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var student = await studentRepo.FirstOrDefaultAsync(
                s => s.Id == studentId,
                q => q.Include(s => s.AppUser)
            );
            if (student == null)
                return ServiceResult<bool>.Fail("Student not found");

            student.ProfileStatus = ProfileStatus.Approved;
            student.AppUser.AccountStatus = AccountStatus.Active;

            studentRepo.Update(student);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Approved",
                message: $"Dear {student.AppUser.FullName}, your profile has been approved by the admin.",
                type: "Approval",
                creatorUserId: adminUserId,
                targetUserIds: new[] { student.AppUserId },
                role: "Student"
            );
            return ServiceResult<bool>.Ok(true, "Student approved succesfully");
        }

        public async Task<ServiceResult<bool>> RejectStudentAsync(
            string studentId,
            string adminUserId
        )
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var student = await studentRepo.FirstOrDefaultAsync(
                s => s.Id == studentId,
                q => q.Include(s => s.AppUser)
            );
            if (student == null)
                return ServiceResult<bool>.Fail("Student not found");

            student.ProfileStatus = ProfileStatus.Rejected;
            student.AppUser.AccountStatus = AccountStatus.PendingApproval;

            studentRepo.Update(student);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Rejected",
                message: $"Dear {student.AppUser.FullName}, your profile has been rejected by the admin. Please review and resubmit.",
                type: "Rejection",
                creatorUserId: adminUserId,
                targetUserIds: new[] { student.AppUserId },
                role: "Student"
            );
            return ServiceResult<bool>.Ok(true, "Student rejected successfully");
        }

        public async Task<ServiceResult<bool>> BlockStudentAsync(
            string appUserId,
            string adminUserId
        )
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("Student not found.");

            user.AccountStatus = AccountStatus.Blocked;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Blocked",
                message: $"Dear {user.FullName}, your account has been blocked. Please contact support for more information.",
                type: "Account Status",
                creatorUserId: adminUserId,
                targetUserIds: new[] { user.Id },
                role: "Student"
            );

            return ServiceResult<bool>.Ok(true, "Student blocked successfully.");
        }

        public async Task<ServiceResult<bool>> UnblockStudentAsync(
            string appUserId,
            string adminUserId
        )
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("Student not found.");

            user.AccountStatus = AccountStatus.Active;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Unblocked",
                message: $"Dear {user.FullName}, your account has been unblocked. You can now access all features.",
                type: "Account Status",
                creatorUserId: adminUserId,
                targetUserIds: new[] { user.Id },
                role: "Student"
            );

            return ServiceResult<bool>.Ok(true, "Student unblocked successfully.");
        }

        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetPendingStudentsAsync()
        {
            var studentRepo = _unitOfWork.Repository<Student>();

            var pendingStudents = await studentRepo.FindAllAsync(
                predicate: s => s.ProfileStatus == ProfileStatus.Pending,
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
                ProfileStatus = s.ProfileStatus.ToString(),
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

        //Parent
        public async Task<ServiceResult<bool>> ApproveParentAsync(
            string parentId,
            string adminUserId
        )
        {
            var parentRepo = _unitOfWork.Repository<Parent>();
            var parent = await parentRepo.FirstOrDefaultAsync(
                p => p.Id == parentId,
                q => q.Include(p => p.AppUser)
            );

            if (parent == null)
                return ServiceResult<bool>.Fail("Parent not found.");

            parent.ProfileStatus = ProfileStatus.Approved;
            parent.AppUser.AccountStatus = AccountStatus.Active;

            parentRepo.Update(parent);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Approved",
                message: $"Dear {parent.AppUser.FullName}, your profile has been approved by the admin.",
                type: "Approval",
                creatorUserId: adminUserId,
                targetUserIds: new[] { parent.AppUserId },
                role: "Parent"
            );

            return ServiceResult<bool>.Ok(true, "Parent approved successfully.");
        }

        public async Task<ServiceResult<bool>> RejectParentAsync(
            string parentId,
            string adminUserId
        )
        {
            var parentRepo = _unitOfWork.Repository<Parent>();
            var parent = await parentRepo.FirstOrDefaultAsync(
                p => p.Id == parentId,
                q => q.Include(p => p.AppUser)
            );

            if (parent == null)
                return ServiceResult<bool>.Fail("Parent not found.");

            parent.ProfileStatus = ProfileStatus.Rejected;
            parent.AppUser.AccountStatus = AccountStatus.PendingApproval;

            parentRepo.Update(parent);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Profile Rejected",
                message: $"Dear {parent.AppUser.FullName}, your profile has been rejected by the admin. Please review and resubmit.",
                type: "Rejection",
                creatorUserId: adminUserId,
                targetUserIds: new[] { parent.AppUserId },
                role: "Parent"
            );

            return ServiceResult<bool>.Ok(true, "Parent rejected successfully.");
        }

        public async Task<ServiceResult<bool>> BlockParentAsync(
            string appUserId,
            string adminUserId
        )
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("Parent not found.");

            user.AccountStatus = AccountStatus.Blocked;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Blocked",
                message: $"Dear {user.FullName}, your account has been blocked. Please contact support for more information.",
                type: "Account Status",
                creatorUserId: adminUserId,
                targetUserIds: new[] { user.Id },
                role: "Parent"
            );

            return ServiceResult<bool>.Ok(true, "Parent blocked successfully.");
        }

        public async Task<ServiceResult<bool>> UnblockParentAsync(
            string appUserId,
            string adminUserId
        )
        {
            var appUserRepo = _unitOfWork.AppUsers;
            var user = await appUserRepo.FirstOrDefaultAsync(u => u.Id == appUserId);

            if (user == null)
                return ServiceResult<bool>.Fail("Parent not found.");

            user.AccountStatus = AccountStatus.Active;
            appUserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                title: "Account Unblocked",
                message: $"Dear {user.FullName}, your account has been unblocked. You can now access all features.",
                type: "Account Status",
                creatorUserId: adminUserId,
                targetUserIds: new[] { user.Id },
                role: "Parent"
            );

            return ServiceResult<bool>.Ok(true, "Parent unblocked successfully.");
        }

        public async Task<ServiceResult<IEnumerable<ParentViewDto>>> GetPendingParentsAsync()
        {
            var parentRepo = _unitOfWork.Repository<Parent>();

            var pendingParents = await parentRepo.FindAllAsync(
                predicate: p => p.ProfileStatus == ProfileStatus.Pending,
                include: q => q.Include(p => p.AppUser)
            );

            if (!pendingParents.Any())
                return ServiceResult<IEnumerable<ParentViewDto>>.Ok(
                    Enumerable.Empty<ParentViewDto>(),
                    "No pending parents found."
                );

            var result = pendingParents.Select(p => new ParentViewDto
            {
                Id = p.Id,
                FullName = p.AppUser.FullName,
                Email = p.Email,
                ProfileStatus = p.ProfileStatus,
            });

            return ServiceResult<IEnumerable<ParentViewDto>>.Ok(
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
    }
}
