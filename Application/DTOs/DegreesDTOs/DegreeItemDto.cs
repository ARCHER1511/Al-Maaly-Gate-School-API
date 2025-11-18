using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class DegreeItemDto
    {
        public string DegreeId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public double Score { get; set; }
        public double MaxScore { get; set; }
        public string DegreeType { get; set; }
    }

}
