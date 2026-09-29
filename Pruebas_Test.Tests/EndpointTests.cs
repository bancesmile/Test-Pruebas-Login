using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Pruebas_Test.Tests
{
    public class EndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Pruebas")]
        [InlineData("/Pruebas/Index")]
        public async Task Get_Endpoints_ReturnSuccessAndCorrectContentType(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task Get_PruebasIndex_ContainsHolaMundo()
        {
            // Act
            var response = await _client.GetAsync("/Pruebas");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hola, Mundo!", content);
        }

        [Fact]
        public async Task Get_NonExistentPage_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/RutaInexistenteQueDebeDar404");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
