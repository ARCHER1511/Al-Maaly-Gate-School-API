namespace Application.DTOs.ClassAppointmentsDTOs
{
    public class StudentClassAppointmentDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Link { get; set; }
        public string Status { get; set; } = "Upcoming";
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }
}
