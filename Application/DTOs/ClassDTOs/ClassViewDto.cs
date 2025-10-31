using Domain.Entities;


namespace Application.DTOs.ClassDTOs
{
    public class ClassViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public Teacher? Teacher { get; set; }
        public List<Student>? Students { get; set; } = new List<Student>();
        public List<ClassAssets>? ClassAssets { get; set; } = new List<ClassAssets>();
        public List<ClassAppointment>? ClassAppointments { get; set; } = new List<ClassAppointment>();
    }
}
