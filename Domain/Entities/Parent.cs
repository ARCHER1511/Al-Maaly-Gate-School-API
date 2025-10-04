namespace Domain.Entities
{
    public class Parent : BaseEntity
    {
        public string? ContactInfo { get; set; }
        public string? Relation { get; set; }

        public List<ParentStudent> ParentStudent  = new List<ParentStudent>();
    }
}
