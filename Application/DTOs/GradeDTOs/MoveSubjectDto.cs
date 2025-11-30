using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GradeDTOs
{
    public class MoveSubjectDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string NewGradeId { get; set; } = string.Empty;
    }
}
