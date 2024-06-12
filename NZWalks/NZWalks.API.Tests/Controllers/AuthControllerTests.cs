using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NZWalks.Core.Exceptions;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Mock<IAuthService> authServiceMock = new();
        private readonly WebApplicationFactory<Program> factory;
        private HttpClient _httpClient;
        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory;

            _httpClient = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                    });
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton(authServiceMock.Object);
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Fact]
        public async Task Register_Success()
        {
            //Arrange
            var credentials = new RegisterDTO
            {
                Username = "test@nzwalks.co.za",
                Password = "test@123",
                Roles = ["Reader"]
            };

            var expectedUser = new IdentityUser
            {
                UserName = credentials.Username,
                Email = credentials.Username
            };
            
            authServiceMock.Setup(authService => authService.RegisterUser(It.IsAny<RegisterDTO>())).ReturnsAsync(expectedUser);

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", credentials);

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<string>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginSuccess()
        {
            //Arrange
            var credentials = new LoginDTO
            {
                Username = "test@nzwalks.co.za",
                Password = "test@123",
            };

            var expectedLoginResponse = new LoginResponseDTO()
            {
                JwtToken = "TestToken"
            };

            authServiceMock.Setup(authService => authService.LoginUser(It.IsAny<LoginDTO>())).ReturnsAsync(expectedLoginResponse);

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", credentials);

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedLoginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equivalent(expectedLoginResponse, returnedLoginResponse);
        }
    }
}
