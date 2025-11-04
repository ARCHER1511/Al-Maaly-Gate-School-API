using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChoiceAnswer : BaseEntity
    {
        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;

        public string ChoiceId { get; set; } = string.Empty;
        public Choices Choice { get; set; } = null!;
    }
}
