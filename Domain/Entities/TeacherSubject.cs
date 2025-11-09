using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TeacherSubject : BaseEntity
    {
        public string TeacherId { get; set;}
        public Teacher Teacher { get; set; }

        public string SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
