namespace Domain.Entities
{
    public class ClassAssets : BaseEntity
    {
        
        // Pdf , word
        public string? AssetsPath { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }
    }
}