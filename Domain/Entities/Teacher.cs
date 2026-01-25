namespace Domain.Entities
{
    public class Teacher : UserBase
    {
        // Add Curriculum specialization
        public ICollection<Curriculum> SpecializedCurricula { get; set; } = new HashSet<Curriculum>();
        public ICollection<TeacherClass> TeacherClasses { get; set; } = new HashSet<TeacherClass>();
        public ICollection<TeacherSubject>? TeacherSubjects { get; set; } = new HashSet<TeacherSubject>();
        public ICollection<Exam>? Exams { get; set; } = new HashSet<Exam>();
        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        public ICollection<ClassAppointment> ClassAppointments { get; set; } = new HashSet<ClassAppointment>();
    }
}
