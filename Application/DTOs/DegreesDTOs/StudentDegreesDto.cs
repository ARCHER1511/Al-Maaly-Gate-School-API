namespace Application.DTOs.DegreesDTOs
{
    public class StudentDegreesDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ClassId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public List<DegreeItemDto> Degrees { get; set; } = new();
    }
}
