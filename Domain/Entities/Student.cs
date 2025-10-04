namespace Domain.Entities
{
    public class Student : BaseEntity
    {
        public string ClassYear { get; set; } = string.Empty;
        public int Age { get; set; }
        public string ContactInfo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }

        public List<ParentStudent> ParentStudent { get; set; } = new List<ParentStudent>();
        public List<StudentSubjectExam> StudentSubjectExam { get; set; } = new List<StudentSubjectExam>();
        public List<StudentQuestionAnswerExam> StudentQuestionAnswerExam { get; set; } = new List<StudentQuestionAnswerExam>();


    }
}
