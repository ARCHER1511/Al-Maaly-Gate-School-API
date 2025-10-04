
namespace Domain.Entities
{
    public class ParentStudent
    {
        public string ParentId { get; set; } = string.Empty;
        public Parent? Parent { get; set; } 
        public string? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
