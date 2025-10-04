using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassAssetsConfiguration : IEntityTypeConfiguration<ClassAssets>
    {
        public void Configure(EntityTypeBuilder<ClassAssets> builder) 
        {

        }
    }
}
