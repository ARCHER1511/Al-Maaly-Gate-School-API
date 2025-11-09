using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TeacherClass : BaseEntity
    {
        public string TeacherId { get; set; } = default!;
        public Teacher Teacher { get; set; } = default!;

        public string ClassId { get; set; } = default!;
        public Class Class { get; set; } = default!;
    }
}
