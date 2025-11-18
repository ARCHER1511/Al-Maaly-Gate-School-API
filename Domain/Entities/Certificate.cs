using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Certificate : BaseEntity
    {
        public string StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public double GPA { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        public string TemplateName { get; set; } = "Default";
    }
}
