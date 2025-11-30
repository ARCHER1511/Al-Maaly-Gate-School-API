

namespace Application.DTOs.ClassDTOs
{
    public class ClassDto
    {
        public string Id { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty; // ADDED: Grade reference
    }
}

