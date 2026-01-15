using Application.DTOs.ClassDTOs;

namespace Application.DTOs.GradeDTOs
{
    public class AddClassToGradeDto
    {
        public string GradeId { get; set; } = string.Empty;
        public ClassDto Class { get; set; } = null!;
    }

}
