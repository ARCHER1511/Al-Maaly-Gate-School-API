using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Configurations
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            new UserBaseConfiguration<Parent>().Configure(builder);

            builder.Property(p => p.Relation)
                   .HasMaxLength(100);

            builder.ToTable("Parents", "Academics");
        }
    }
}
