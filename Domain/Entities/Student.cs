namespace Domain.Entities
{
    public class Student : UserBase
    {
        public string ClassYear { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string IqamaNumber { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string? ClassId { get; set; }
        public Class? Class { get; set; }
        public List<ParentStudent> Parents { get; set; } = new ();
        public List<Degree> Degrees { get; set; } = new();
        public List<Certificate> Certificates { get; set; } = new();

    }
}
