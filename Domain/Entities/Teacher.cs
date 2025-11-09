namespace Domain.Entities
{
    public class Teacher : UserBase
    {
        public List<TeacherClass> TeacherClasses { get; set; } = new();
        public List<TeacherSubject>? TeacherSubjects { get; set; } = new();
        public List<Exam>? Exams { get; set; } = new List<Exam>();
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<ClassAppointment> ClassAppointments { get; set; } = new();
    }
}
