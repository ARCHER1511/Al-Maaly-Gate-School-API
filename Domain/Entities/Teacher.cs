namespace Domain.Entities
{
    public class Teacher : UserBase
    {
        public List<Class> Classes { get; set; } = new();
        public List<TeacherSubjectExam> SubjectExams { get; set; } = new();
        public List<QuestionExamTeacher> QuestionExams { get; set; } = new();
        public List<Question> Questions { get; set; } = new();
        public List<Answer> Answers { get; set; } = new();
    }
}
