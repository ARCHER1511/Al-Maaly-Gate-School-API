namespace Domain.Entities
{
    public class TeacherSubjectExam
    {
        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = new Teacher();
        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = new Subject();
        public string ExamId { get; set; } = string.Empty;
        public Exam Exam { get; set; } = new Exam();

    }
}