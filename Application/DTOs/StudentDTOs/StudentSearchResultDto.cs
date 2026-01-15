namespace Application.DTOs.StudentDTOs
{
    public class StudentSearchResultDto
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? IqamaNumber { get; set; }
        public string? PassportNumber { get; set; }
        public bool? IsInRelation { get; set; }
    }
}
