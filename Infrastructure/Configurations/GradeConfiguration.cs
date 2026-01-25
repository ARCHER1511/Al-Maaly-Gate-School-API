using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("Grades", "Academics");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.GradeName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(g => g.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);

            // Add Curriculum relationship
            builder.HasOne(g => g.Curriculum)
                   .WithMany(c => c.Grades)
                   .HasForeignKey(g => g.CurriculumId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One Grade has many Classes
            builder.HasMany(g => g.Classes)
                   .WithOne(c => c.Grade)
                   .HasForeignKey(c => c.GradeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One Grade has many Subjects
            builder.HasMany(g => g.Subjects)
                   .WithOne(s => s.Grade)
                   .HasForeignKey(s => s.GradeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
