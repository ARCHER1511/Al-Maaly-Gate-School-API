namespace Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string ClassYear { get; set; } = string.Empty;
        public List<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
        public List<TeacherSubjectExam> TeacherSubjectExam { get; set; } = new List<TeacherSubjectExam>();
    }
}