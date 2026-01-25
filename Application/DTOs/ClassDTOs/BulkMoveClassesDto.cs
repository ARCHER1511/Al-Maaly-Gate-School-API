namespace Application.DTOs.ClassDTOs
{
    public class BulkMoveClassesDto
    {
        public List<string> ClassIds { get; set; } = new();
        public string NewGradeId { get; set; } = string.Empty;
    }
}
