namespace Application.DTOs.ParentDTOs
{
    public class ParentProfileDto
    {
        public string Id { get; set; } = null!;
        public string RelationshipToStudent { get; set; } = null!;
        public int DocumentCount { get; set; }
    }
}
