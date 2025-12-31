using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DegreeComponent : BaseEntity
    {
        public string DegreeId { get; set; } = string.Empty;
        public Degree Degree { get; set; } = null!;

        public string ComponentTypeId { get; set; } = string.Empty;
        public DegreeComponentType ComponentType { get; set; } = null!;

        public double Score { get; set; } = 0;
        public double MaxScore { get; set; } = 0; // Can override component type's max score if needed

        public string ComponentName { get; set; } = string.Empty; // For quick access
    }
}
