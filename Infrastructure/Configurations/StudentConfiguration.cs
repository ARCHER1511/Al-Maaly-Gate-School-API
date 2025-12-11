using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            new UserBaseConfiguration<Student>().Configure(builder);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ClassYear)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.Age)
                   .IsRequired();

            builder.Property(s => s.ClassId)
                   .HasMaxLength(450);

            builder.HasOne(s => s.Class)
                   .WithMany(c => c.Students)
                   .HasForeignKey(s => s.ClassId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            // Add Curriculum relationship
            builder.HasOne(s => s.Curriculum)
                   .WithMany(c => c.Students)
                   .HasForeignKey(s => s.CurriculumId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.ToTable("Students", "Academics");
        }
    }
}
