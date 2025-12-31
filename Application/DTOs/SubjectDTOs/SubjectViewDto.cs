using Application.DTOs.DegreesDTOs;

namespace Application.DTOs.SubjectDTOs
{
    public class SubjectViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GradeId { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public double CreditHours { get; set; }
        public int TeacherCount { get; set; }
        public int ExamCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // NEW: Component types information
        public List<DegreeComponentTypeDto> ComponentTypes { get; set; } = new();
        public bool HasComponentTypes => ComponentTypes != null && ComponentTypes.Any();
        public int ComponentTypeCount => ComponentTypes?.Count ?? 0;
    }
}
