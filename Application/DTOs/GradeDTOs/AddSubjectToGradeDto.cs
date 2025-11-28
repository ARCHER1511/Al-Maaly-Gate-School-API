using Application.DTOs.SubjectDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GradeDTOs
{
    public class AddSubjectToGradeDto
    {
        public string GradeId { get; set; } = string.Empty;
        public SubjectCreateDto Subject { get; set; } = null!;
    }
}
