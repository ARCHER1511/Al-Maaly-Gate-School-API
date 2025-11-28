using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GradeDTOs
{
    public class MoveClassDto
    {
        public string ClassId { get; set; } = string.Empty;
        public string NewGradeId { get; set; } = string.Empty;
    }
}
