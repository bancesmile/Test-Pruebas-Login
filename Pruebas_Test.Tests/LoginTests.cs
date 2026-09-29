using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Pruebas_Test.Tests
{
    public class LoginTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LoginTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_LoginPage_ReturnsSuccessAndRendersForm()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Account/Login");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            Assert.Contains("Iniciar Sesión", content);
            Assert.Contains("Usuario", content);
            Assert.Contains("Contraseña", content);
        }

        [Fact]
        public async Task Post_Login_WithValidCredentials_RedirectsToGoogle()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Usuario", "admin" },
                { "Password", "123" }
            });

            // Act
            var response = await client.PostAsync("/Account/Login", formContent);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("https://www.google.com", response.Headers.Location?.ToString());
        }

        [Theory]
        [InlineData("admin", "clave_incorrecta")]
        [InlineData("usuario_desconocido", "123")]
        [InlineData("usuario_desconocido", "clave_mala")]
        public async Task Post_Login_WithInvalidCredentials_ReturnsViewWithErrorMessage(string usuario, string password)
        {
            // Arrange
            var client = _factory.CreateClient();

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Usuario", usuario },
                { "Password", password }
            });

            // Act
            var response = await client.PostAsync("/Account/Login", formContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            Assert.Contains("Usuario o contraseña incorrectos.", content);
        }

        [Fact]
        public async Task Post_Login_WithEmptyFields_ReturnsValidationErrors()
        {
            // Arrange
            var client = _factory.CreateClient();

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Usuario", "" },
                { "Password", "" }
            });

            // Act
            var response = await client.PostAsync("/Account/Login", formContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            Assert.Contains("El usuario es obligatorio.", content);
            Assert.Contains("La contraseña es obligatoria.", content);
        }

        [Fact]
        public async Task Post_Login_WithEmptyUsuario_ReturnsOnlyUsuarioRequiredError()
        {
            // Arrange: Campo Usuario vacío, Contraseña completa
            var client = _factory.CreateClient();

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Usuario", "" },
                { "Password", "123" }
            });

            // Act
            var response = await client.PostAsync("/Account/Login", formContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            
            // El campo Usuario debe contener el mensaje de error y la clase de error
            Assert.Contains("field-validation-error\" data-valmsg-for=\"Usuario\"", content);
            Assert.Contains("El usuario es obligatorio.", content);

            // El campo Password debe mantenerse como válido (sin error de validación)
            Assert.Contains("field-validation-valid\" data-valmsg-for=\"Password\"", content);
        }

        [Fact]
        public async Task Post_Login_WithEmptyPassword_ReturnsOnlyPasswordRequiredError()
        {
            // Arrange: Campo Usuario completo, Contraseña vacía
            var client = _factory.CreateClient();

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Usuario", "admin" },
                { "Password", "" }
            });

            // Act
            var response = await client.PostAsync("/Account/Login", formContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

            // El campo Password debe contener el mensaje de error y la clase de error
            Assert.Contains("field-validation-error\" data-valmsg-for=\"Password\"", content);
            Assert.Contains("La contraseña es obligatoria.", content);

            // El campo Usuario debe mantenerse como válido (sin error de validación)
            Assert.Contains("field-validation-valid\" data-valmsg-for=\"Usuario\"", content);
        }

        [Fact]
        public async Task Get_RootPage_RendersLoginFormDirectly()
        {
            // Arrange: Verificar que la ruta raíz (/) abre el formulario de login directamente
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            Assert.Contains("Iniciar Sesión", content);
            Assert.Contains("Usuario", content);
            Assert.Contains("Contraseña", content);
            Assert.Contains("Credenciales de prueba", content);
        }

        [Fact]
        public async Task Post_RootPage_WithValidCredentials_RedirectsToGoogle()
        {
            // Arrange: Verificar que el login funciona enviándolo desde la ruta raíz (/)
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Usuario", "admin" },
                { "Password", "123" }
            });

            // Act
            var response = await client.PostAsync("/", formContent);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("https://www.google.com", response.Headers.Location?.ToString());
        }
    }
}
