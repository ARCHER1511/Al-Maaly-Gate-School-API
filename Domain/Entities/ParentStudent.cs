
namespace Domain.Entities
{
    public class ParentStudent
    {
        public string ParentId { get; set; } = string.Empty;
        public Parent Parent { get; set; } = null!;

        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;
    }
}
