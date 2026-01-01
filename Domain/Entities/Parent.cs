namespace Domain.Entities
{
    public class Parent : UserBase
    {
        public List<ParentStudent> ParentStudent { get; set; } = new List<ParentStudent>();
    }
}
