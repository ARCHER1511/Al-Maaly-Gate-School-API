namespace Domain.Entities
{
    public class Admin : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
