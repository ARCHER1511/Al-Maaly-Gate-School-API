using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            // Add check constraint for GPA
            builder.ToTable("Certificates", "Academics",t => t.HasCheckConstraint("CK_Certificates_GPA", "[GPA] >= 0 AND [GPA] <= 4.0"));

            builder.HasKey(c => c.Id);

            builder.Property(c => c.GPA)
                   .IsRequired()
                   .HasPrecision(4, 2);

            builder.Property(c => c.TemplateName)
                   .HasMaxLength(100);

            builder.Property(c => c.FileName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(c => c.ContentType)
                   .HasMaxLength(100)
                   .HasDefaultValue("application/pdf");

            builder.Property(c => c.AcademicYear)
                   .HasMaxLength(20);

            builder.Property(c => c.Semester)
                   .HasMaxLength(20);

            builder.Property(c => c.CertificateNumber)
                   .HasMaxLength(50);

            builder.Property(c => c.VerifiedBy)
                   .HasMaxLength(255);

            builder.HasOne(c => c.Student)
                   .WithMany(s => s.Certificates)
                   .HasForeignKey(c => c.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Curriculum)
                   .WithMany(c => c.Certificates)
                   .HasForeignKey(c => c.CurriculumId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Grade)
                   .WithMany()
                   .HasForeignKey(c => c.GradeId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(c => c.Class)
                   .WithMany()
                   .HasForeignKey(c => c.ClassId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasIndex(c => c.StudentId);
            builder.HasIndex(c => c.CurriculumId);
            builder.HasIndex(c => c.GradeId);
            builder.HasIndex(c => c.ClassId);
            builder.HasIndex(c => c.IssuedDate);
            builder.HasIndex(c => c.CertificateNumber)
                   .IsUnique()
                   .HasFilter("[CertificateNumber] IS NOT NULL");

            // Add check constraint for GPA
            //builder.HasCheckConstraint("CK_Certificates_GPA", "[GPA] >= 0 AND [GPA] <= 4.0");
        }
    }
}