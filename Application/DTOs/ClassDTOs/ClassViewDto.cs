using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Domain.Entities;


namespace Application.DTOs.ClassDTOs
{
    public class ClassViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TeacherViewDto> AssignedTeachers { get; set; } = new();
        public List<StudentViewDto>? Students { get; set; } = new List<StudentViewDto>();
        public List<ClassAssets>? ClassAssets { get; set; } = new List<ClassAssets>();
        public List<ClassAppointment>? ClassAppointments { get; set; } = new List<ClassAppointment>();
    }
}
