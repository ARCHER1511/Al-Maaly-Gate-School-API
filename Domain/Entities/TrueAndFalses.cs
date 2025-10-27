
namespace Domain.Entities
{
    public class TrueAndFalses : BaseEntity
    {
        public bool IsTrue { get; set; }
        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;
    }
}
