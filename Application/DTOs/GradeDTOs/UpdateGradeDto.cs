using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GradeDTOs
{
    public class UpdateGradeDto
    {
        public string GradeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CurriculumId { get; set; } = string.Empty; // Add this
    }
}
