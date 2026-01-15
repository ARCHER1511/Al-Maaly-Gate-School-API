using Application.DTOs.ClassDTOs;
using Application.DTOs.CurriculumDTOs;
using Application.DTOs.SubjectDTOs;

namespace Application.DTOs.GradeDTOs
{
    public class GradeWithDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string GradeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CurriculumId { get; set; } = string.Empty; // Add this
        public CurriculumDto Curriculum { get; set; } = null!; // Add this
        public List<ClassViewDto> Classes { get; set; } = new();
        public List<SubjectViewDto> Subjects { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
