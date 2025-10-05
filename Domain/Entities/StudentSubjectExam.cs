namespace Domain.Entities
{
    public class StudentSubjectExam
    {
        public decimal? Grade { get; set; }
        public string? StudentId {  get; set; } 
        public Student? Student { get; set; }
        public string? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }



    }
}