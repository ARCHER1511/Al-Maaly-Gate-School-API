using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClassDTOs
{
    public class BulkMoveClassesDto
    {
        public List<string> ClassIds { get; set; } = new();
        public string NewGradeId { get; set; } = string.Empty;
    }
}
