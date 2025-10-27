using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            new UserBaseConfiguration<Teacher>().Configure(builder);

            builder.HasKey(t => t.Id);

            builder.HasMany(t => t.Subjects)
                   .WithOne(s => s.Teacher)
                   .HasForeignKey(s => s.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Questions)
                .WithOne(s => s.Teacher)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.ClassAppointments)
                .WithOne(s => s.Teacher)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Exams)
                 .WithOne(s => s.Teacher)
                 .HasForeignKey(s => s.TeacherId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Teachers", "Academics");
        }
    }

}
