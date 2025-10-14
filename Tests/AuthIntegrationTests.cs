using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Xunit;
using Al_Maaly_Gate_School;

namespace Tests
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AuthCheck_ShouldReturn200_WhenTokenIsValid()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost:7002")
            });

            var token = GenerateTestToken(); // Replace with your actual token or generator

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync("/api/auth/me");
            //var response = await client.GetAsync("/swagger/index.html");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Token is valid");
        }

        private string GenerateTestToken()
        {
            // You can either hardcode a valid token or use your GenerateJwtToken method here
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // Replace with a valid token
        }
    }
}
