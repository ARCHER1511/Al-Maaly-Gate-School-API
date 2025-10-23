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

            builder.Property(ca => ca.StartTime)
                   .IsRequired(true);

            builder.Property(ca => ca.EndTime)
                   .IsRequired(true);

            builder.Property(ca => ca.Link)
                   .IsRequired(true);

            builder.HasOne(ca => ca.Class)
                   .WithMany(c => c.ClassAppointments)
                   .HasForeignKey(ca => ca.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ClassAppointments", "Academics");
        }
    }
}
