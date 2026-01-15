using Application.DTOs.SubjectDTOs;

namespace Application.DTOs.GradeDTOs
{
    public class AddSubjectToGradeDto
    {
        public string GradeId { get; set; } = string.Empty;
        public SubjectCreateDto Subject { get; set; } = null!;
    }
}
