namespace Domain.Entities
{
    public class ChoiceAnswer : BaseEntity
    {
        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;

        public string ChoiceId { get; set; } = string.Empty;
        public Choices Choice { get; set; } = null!;
    }
}
