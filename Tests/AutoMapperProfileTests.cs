using Application.Mappings;
using AutoMapper;
using Xunit;

namespace Tests
{
    public class AutoMapperProfileTests
    {
        [Fact]
        public void ValidateAutoMapperConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // Add all your profiles here
            });

            config.AssertConfigurationIsValid(); // 💥 Will throw with detailed info
        }
    }
}
