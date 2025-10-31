using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AppointmentsDTOs
{
    public class ClassAppointmentDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Url]
        public string? Link { get; set; }
        public string Status { get; set; } = "Upcoming";

        public string ClassId { get; set; } = string.Empty;
        public Class Class { get; set; } = null!;

        public string TeacherId { get; set; } = string.Empty;
        public Teacher Teacher { get; set; } = null!;

        public string SubjectId { get; set; } = string.Empty;
        public Subject Subject { get; set; } = null!;
    }
}
