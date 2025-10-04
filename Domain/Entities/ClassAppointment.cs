namespace Domain.Entities
{
    public class ClassAppointment : BaseEntity
    {

        public DateTime? Appointment { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }


    }
}