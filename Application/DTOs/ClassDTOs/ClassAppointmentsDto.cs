using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ClassDTOs
{
    public class ClassAppointmentsDTo
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }
        [Url]
        public string? Link { get; set; }
        public string Status { get; set; } = "Upcoming";
    }
}
