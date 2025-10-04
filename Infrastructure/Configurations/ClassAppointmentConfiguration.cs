using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassAppointmentConfiguration : IEntityTypeConfiguration<ClassAppointment>
    {
        public void Configure(EntityTypeBuilder<ClassAppointment> builder)
        {
            
        }
    }
}
