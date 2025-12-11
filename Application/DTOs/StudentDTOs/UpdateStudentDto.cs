using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StudentDTOs
{
    public class UpdateStudentDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ClassYear { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string IqamaNumber { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string? ClassId { get; set; }
        public string CurriculumId { get; set; } = string.Empty; // Add this
    }
}
