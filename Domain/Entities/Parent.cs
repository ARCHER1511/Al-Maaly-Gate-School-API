namespace Domain.Entities
{
    public class Parent : UserBase
    {
        public string? Relation { get; set; }

        public ICollection<ParentStudent> ParentStudent { get; set; } = new List<ParentStudent>();
    }
}
