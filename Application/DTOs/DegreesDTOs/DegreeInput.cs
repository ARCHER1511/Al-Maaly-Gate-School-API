using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DegreesDTOs
{
    public class DegreeInput
    {
        public string SubjectId { get; set; } = string.Empty;
        public DegreeType DegreeType { get; set; }

        // Optional: Direct total scores
        public double? Score { get; set; }
        public double? MaxScore { get; set; }

        // Optional: Component breakdown
        public List<DegreeComponentInput>? Components { get; set; }
    }
}
