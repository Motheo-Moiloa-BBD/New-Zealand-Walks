
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
using System.Text.Json;
using System.Text;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;


namespace NZWalks.API.Tests.Controllers
{
    public class RegionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Mock<IRegionService> regionServiceMock = new();
        private readonly WebApplicationFactory<Program> factory;
        private HttpClient _httpClient;

        public RegionsControllerTests(WebApplicationFactory<Program> factory)
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
                        services.AddSingleton(regionServiceMock.Object);
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                        services.AddAuthorization(options =>
                        {
                            options.AddPolicy("TestPolicy", policy =>
                            {
                                policy.RequireAuthenticatedUser();
                                policy.RequireClaim(ClaimTypes.Role, ["Writer, Reader"]);
                            });
                        });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Fact]
        public async Task GetAll_Success()
        {
            //Arrange
            var regions = new List<RegionDTO>()
            {
                new RegionDTO()
                {
                    Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                    Code = "EMB",
                    Name = "Embalenhle",
                    RegionImageUrl = "embalenhle-image.png"
                }
            };

            regionServiceMock.Setup(regionService => regionService.GetAllRegions()).ReturnsAsync(regions);

            //Act
            var response = await _httpClient.GetAsync("api/regions");

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedList = JsonConvert.DeserializeObject<List<RegionDTO>>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Collection(returnedList, region =>
            {
                Assert.Equal(regions[0].Id, region.Id);
                Assert.Equal(regions[0].Code, region.Code);
                Assert.Equal(regions[0].Name, region.Name);
                Assert.Equal(regions[0].RegionImageUrl, region.RegionImageUrl);
            });
        }

        [Fact]
        public async Task GetById_Success()
        {
            //Arrange 
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");
            var existingRegion = new RegionDTO()
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Code = "EMB",
                Name = "Embalenhle",
                RegionImageUrl = "embalenhle-image.png"
            };

            regionServiceMock.Setup(regionService => regionService.GetRegionById(id)).ReturnsAsync(existingRegion);

            //Act
            var response = await _httpClient.GetAsync($"api/regions/{id}");

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedRegion = JsonConvert.DeserializeObject<RegionDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equivalent(existingRegion, returnedRegion, strict: true);
        }

        [Fact]
        public Task GetById_NotFoundException_404()
        {
            return AssertThatGetByIdHandlesGivenException(new NotFoundException("Region not found."), HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_Success()
        {
            //Arrange
            var addRegion = new AddRegionDTO
            {
                Code = "MAH",
                Name = "Mahikeng",
                RegionImageUrl = "mahikeng-image.png"
            };

            var expectedRegion = new RegionDTO
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Code = "MAH",
                Name = "Mahikeng",
                RegionImageUrl = "mahikeng-image.png"
            };

            regionServiceMock.Setup(regionService => regionService.CreateRegion(It.IsAny<AddRegionDTO>())).ReturnsAsync(expectedRegion);

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/regions", addRegion);

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedRegion = JsonConvert.DeserializeObject<RegionDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        private async Task AssertThatGetByIdHandlesGivenException(Exception givenException, HttpStatusCode resultingStatusCode)
        {
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            regionServiceMock.Setup(regionService => regionService.GetRegionById(id)).ThrowsAsync(givenException);

            var response = await _httpClient.GetAsync($"api/regions/{id}");

            Assert.Equal(resultingStatusCode,response.StatusCode);
        }
    }
}
