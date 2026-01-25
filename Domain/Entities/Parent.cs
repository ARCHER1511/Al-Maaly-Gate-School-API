namespace Domain.Entities
{
    public class Parent : UserBase
    {
        public ICollection<ParentStudent> ParentStudent { get; set; } = new HashSet<ParentStudent>();
    }
}
