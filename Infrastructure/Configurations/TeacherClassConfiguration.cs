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
    public class TeacherClassConfiguration : IEntityTypeConfiguration<TeacherClass>
    {
        public void Configure(EntityTypeBuilder<TeacherClass> builder)
        {
            builder.ToTable("TeacherClasses", "Academics");

            builder.HasKey(tc => tc.Id);

            builder.HasIndex(tc => new { tc.TeacherId, tc.ClassId })
                   .IsUnique();

            builder.HasOne(tc => tc.Teacher)
                .WithMany(t => t.TeacherClasses)
                .HasForeignKey(tc => tc.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tc => tc.Class)
                .WithMany(c => c.TeacherClasses)
                .HasForeignKey(tc => tc.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
