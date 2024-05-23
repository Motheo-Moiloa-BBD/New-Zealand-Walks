
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;
using System.Net;
using System.Security.Claims;


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
        }

        [Fact]
        public async Task GetAll_Regions_Success()
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

    }
}
