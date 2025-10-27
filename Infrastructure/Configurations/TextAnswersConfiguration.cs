using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public class TextAnswersConfiguration : IEntityTypeConfiguration<TextAnswers>
    {
        public void Configure(EntityTypeBuilder<TextAnswers> builder)
        {
            builder.ToTable("TextAnswers", "Academics");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Content)
                   .HasMaxLength(1000);

            builder.HasOne(t => t.Question)
                   .WithOne(q => q.TextAnswer)
                   .HasForeignKey<TextAnswers>(t => t.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
