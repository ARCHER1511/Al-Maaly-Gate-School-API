namespace Domain.Entities
{
    public class Parent : UserBase
    {
        public string? Relation { get; set; }

        public List<ParentStudent> ParentStudent  = new List<ParentStudent>();
    }
}
