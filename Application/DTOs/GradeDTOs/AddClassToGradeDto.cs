using Application.DTOs.ClassDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.GradeDTOs
{
    public class AddClassToGradeDto
    {
        public string GradeId { get; set; } = string.Empty;
        public ClassDto Class { get; set; } = null!;
    }

}
