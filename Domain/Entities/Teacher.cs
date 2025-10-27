namespace Domain.Entities
{
    public class Teacher : UserBase
    {
        public List<Subject>? Subjects { get; set; } = new List<Subject>();
        public List<Exam>? Exams { get; set; } = new List<Exam>();
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<ClassAppointment> ClassAppointments { get; set; } = new();
    }
}
