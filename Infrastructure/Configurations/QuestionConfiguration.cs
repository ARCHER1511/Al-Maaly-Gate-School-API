using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Type).HasMaxLength(100);
            builder.Property(q => q.Content).HasMaxLength(1000);
            builder.Property(q => q.CorrectAnswer).HasMaxLength(500);

            builder.HasOne(q => q.Teacher)
                   .WithMany(t => t.Questions)
                   .HasForeignKey(q => q.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Questions", "Academics");
        }
    }
}
