namespace Domain.Entities
{
    public class StudentSubjectExam
    {
        public decimal? Grade { get; set; }
        public string StudentId { get; set; } = default!;
        public string SubjectId { get; set; } = default!;
        public int ExamId { get; set; }                    

        public Student Student { get; set; } = default!;
        public Subject Subject { get; set; } = default!;
        public Exam Exam { get; set; } = default!;
    }
}