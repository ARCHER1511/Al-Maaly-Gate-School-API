using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configurations
{
    public class ChoicesConfiguration : IEntityTypeConfiguration<Choices>
    {
        public void Configure(EntityTypeBuilder<Choices> builder)
        {
            builder.ToTable("Choices", "Academics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Text)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(c => c.IsCorrect)
                   .IsRequired();

            builder.HasOne(c => c.Question)
                   .WithMany(q => q.Choices)
                   .HasForeignKey(c => c.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
