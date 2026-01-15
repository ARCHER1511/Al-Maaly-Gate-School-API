namespace Application.DTOs.DegreesDTOs
{
    public class DegreeItemDto
    {
        public string DegreeId { get; set; } = string.Empty;
        public string SubjectId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string DegreeType { get; set; } = string.Empty;

        // Total values
        public double Score { get; set; }
        public double MaxScore { get; set; }

        // Component details
        public List<DegreeComponentDto> Components { get; set; } = new();

        // Flag to indicate if components are present
        public bool HasComponents => Components != null && Components.Any();
    }

}
