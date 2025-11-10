//using System.Net.Http.Headers;
//using Al_Maaly_Gate_School;
//using Application.Authentication;
//using FluentAssertions;
//using Infrastructure.Interfaces;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Xunit;


//namespace Tests
//{
//    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
//    {
//        private readonly WebApplicationFactory<Program> _factory;
//        private readonly IConfiguration _config;

//        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
//        {
//            _factory = factory;
//            _config = factory.Services.GetRequiredService<IConfiguration>();
//        }

//        [Fact]
//        public async Task AuthCheck_ShouldReturn200_WhenTokenIsValid()
//        {
//            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
//            {
//                BaseAddress = new Uri("https://localhost:7002")
//            });

//            var token = await GenerateTestToken();

//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//            var response = await client.GetAsync("/api/AfterAuthentication/profile");

//            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

//            var content = await response.Content.ReadAsStringAsync();
//            content.Should().Contain("Token is valid");
//        }

//        private async Task<string> GenerateTestToken()
//        {
//            using var scope = _factory.Services.CreateScope();
//            var userRepo = scope.ServiceProvider.GetRequiredService<IAppUserRepository>();

//            var user = await userRepo.GetByEmailAsync("ramy@test.com");
//            var roles = await userRepo.GetRolesAsync(user!);
//            return JwtExtensions.GenerateJwtToken(user!, roles, _config);
//        }
//    }
//}
