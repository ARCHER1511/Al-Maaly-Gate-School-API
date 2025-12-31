using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class CreateDegreeComponentTypeDto
    {
        public string SubjectId { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public int Order { get; set; } = 1;
        public double MaxScore { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
