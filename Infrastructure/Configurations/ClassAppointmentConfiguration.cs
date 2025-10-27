using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassAppointmentConfiguration : IEntityTypeConfiguration<ClassAppointment>
    {
        public void Configure(EntityTypeBuilder<ClassAppointment> builder)
        {
            new BaseEntityConfiguration<ClassAppointment>().Configure(builder);

            builder.HasKey(a => a.Id);

            builder.Property(ca => ca.StartTime)
                   .IsRequired(true);

            builder.Property(ca => ca.EndTime)
                   .IsRequired(true);

            builder.Property(ca => ca.Link)
                   .IsRequired(true);

            builder.Property(a => a.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Upcoming");

            builder.HasOne(ca => ca.Class)
                   .WithMany(c => c.ClassAppointments)
                   .HasForeignKey(ca => ca.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Subject)
                   .WithMany(s => s.ClassAppointments)
                   .HasForeignKey(a => a.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Teacher)
                   .WithMany(t => t.ClassAppointments)
                   .HasForeignKey(a => a.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("ClassAppointments", "Academics");
        }
    }
}
