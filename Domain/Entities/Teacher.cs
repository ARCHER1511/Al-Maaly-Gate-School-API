namespace Domain.Entities
{
    public class Teacher : BaseEntity
    {
        public string ContactInfo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public List<Class> Classes { get; set; } = new List<Class>();
        public List<TeacherSubjectExam> TeacherSubjectExam { get; set; } = new List<TeacherSubjectExam>();
    }
}
