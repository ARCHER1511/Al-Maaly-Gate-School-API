using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class ChoiceAnswerConfiguration : IEntityTypeConfiguration<ChoiceAnswer>
    {
        public void Configure(EntityTypeBuilder<ChoiceAnswer> builder)
        {
            builder.ToTable("ChoiceAnswers", "Academics");

            builder.HasKey(ca => ca.Id);

            builder.HasOne(ca => ca.Question)
                   .WithOne(q => q.ChoiceAnswer)
                   .HasForeignKey<ChoiceAnswer>(ca => ca.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ca => ca.Choice)
                   .WithMany()
                   .HasForeignKey(ca => ca.ChoiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
