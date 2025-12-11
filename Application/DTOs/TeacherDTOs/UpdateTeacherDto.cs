namespace Application.DTOs.TeacherDTOs
{
    public class UpdateTeacherDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> SpecializedCurriculumIds { get; set; } = new(); // Add this
    }
}
