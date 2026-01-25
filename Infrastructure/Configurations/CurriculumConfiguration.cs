using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
    {
        public void Configure(EntityTypeBuilder<Curriculum> builder)
        {
            builder.ToTable("Curricula", "Academics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(c => c.Code)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(c => c.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);

            // One Curriculum has many Grades
            builder.HasMany(c => c.Grades)
                   .WithOne(g => g.Curriculum)
                   .HasForeignKey(g => g.CurriculumId)
                   .OnDelete(DeleteBehavior.Restrict);

            // One Curriculum has many Students
            builder.HasMany(c => c.Students)
                   .WithOne(s => s.Curriculum)
                   .HasForeignKey(s => s.CurriculumId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many with Teachers
            builder.HasMany(c => c.Teachers)
                   .WithMany(t => t.SpecializedCurricula)
                   .UsingEntity(j => j.ToTable("TeacherCurricula", "Academics"));
        }
    }
}
