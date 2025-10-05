using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Configurations
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.HasOne(p => p.AppUser)
               .WithMany()
               .HasForeignKey(p => p.AppUserId);

            builder.HasMany(p => p.ParentStudent)
                   .WithOne(ps => ps.Parent)
                   .HasForeignKey(ps => ps.ParentId);
        }
    }
}
