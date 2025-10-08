using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            new UserBaseConfiguration<Admin>().Configure(builder);

            builder.Property(a => a.Type)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
