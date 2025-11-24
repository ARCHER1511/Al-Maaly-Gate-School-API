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
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.ToTable("Certificates", "Academics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.GPA).IsRequired();

            builder.Property(c => c.TemplateName)
                   .HasMaxLength(100);

            builder.HasOne(c => c.Student)
                   .WithMany(s => s.Certificates)
                   .HasForeignKey(c => c.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
