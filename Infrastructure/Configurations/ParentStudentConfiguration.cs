using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ParentStudentConfiguration : IEntityTypeConfiguration<ParentStudent>
    {
        public void Configure(EntityTypeBuilder<ParentStudent> builder)
        {
            builder.HasKey(ps => new { ps.ParentId, ps.StudentId });

            builder.HasOne(ps => ps.Parent)
                   .WithMany(p => p.ParentStudent)
                   .HasForeignKey(ps => ps.ParentId);

            builder.HasOne(ps => ps.Student)
                   .WithMany(s => s.Parents)
                   .HasForeignKey(ps => ps.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("ParentStudents", "Academics");
        }
    }
}
