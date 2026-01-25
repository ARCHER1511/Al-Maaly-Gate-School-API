namespace Application.DTOs.DegreesDTOs
{
    public class AddDegreesDto
    {
        public string StudentId { get; set; } = string.Empty;
        public List<DegreeInput> Degrees { get; set; } = new();
    }
}
