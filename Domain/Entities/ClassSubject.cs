namespace Domain.Entities
{
    public class ClassSubject
    {
        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = new Class();
        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = new Subject();
    }
}