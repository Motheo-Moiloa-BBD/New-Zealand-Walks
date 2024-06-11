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
        public async Task GetById_NotFoundException_404()
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            regionServiceMock.Setup(regionService => regionService.GetRegionById(id)).ThrowsAsync(new NotFoundException($"Region with id {id} does not exist."));
            
            //Act
            var response = await _httpClient.GetAsync($"api/regions/{id}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
            Assert.Equal(expectedRegion.Code, returnedRegion.Code);
            Assert.Equal(expectedRegion.Name, returnedRegion.Name);
            Assert.Equal(expectedRegion.RegionImageUrl, returnedRegion.RegionImageUrl);
        }

        [Fact]
        public async Task Create_BadRequestException_400()
        {
            //Arrange
            var addRegion = new AddRegionDTO
            {
                Code = "MAH",
                Name = "Mahikeng",
                RegionImageUrl = "mahikeng-image.png"
            };

            regionServiceMock.Setup(regionService => regionService.CreateRegion(It.IsAny<AddRegionDTO>())).ThrowsAsync(new BadRequestException("There was a problem when saving the region."));

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/regions", addRegion);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            var updateRegion = new UpdateRegionDTO
            {
                Code = "KIN",
                Name = "Kinross",
                RegionImageUrl = "kinross-image.png"
            };

            var expectedRegion = new RegionDTO
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Code = "KIN",
                Name = "Kinross",
                RegionImageUrl = "kinross-image.png"
            };

            regionServiceMock.Setup(regionService => regionService.UpdateRegion(id, It.IsAny<UpdateRegionDTO>())).ReturnsAsync(expectedRegion);

            //Act
            var response = await _httpClient.PutAsJsonAsync($"api/regions/{id}", updateRegion);

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedRegion = JsonConvert.DeserializeObject<RegionDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedRegion.Id, returnedRegion.Id);
            Assert.Equal(expectedRegion.Code, returnedRegion.Code);
            Assert.Equal(expectedRegion.Name, returnedRegion.Name);
            Assert.Equal(expectedRegion.RegionImageUrl, returnedRegion.RegionImageUrl);
        }

        [Fact]
        public Task Update_NotFoundException_404()
        {
            return AssertThatUpdateHandlesGivenException(new NotFoundException("Region not found."), HttpStatusCode.NotFound);
        }

        [Fact]
        public Task Update_BadRequestException_400()
        {
           return AssertThatUpdateHandlesGivenException(new BadRequestException("There was a problem when deleting the region."), HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Delete_Success()
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            var expectedRegion = new RegionDTO
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Code = "KIN",
                Name = "Kinross",
                RegionImageUrl = "kinross-image.png"
            };

            regionServiceMock.Setup(regionService => regionService.DeleteRegion(id)).ReturnsAsync(expectedRegion);

            //Act
            var response = await _httpClient.DeleteAsync($"api/regions/{id}");

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedRegion = JsonConvert.DeserializeObject<RegionDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedRegion.Id, returnedRegion.Id);
            Assert.Equal(expectedRegion.Code, returnedRegion.Code);
            Assert.Equal(expectedRegion.Name, returnedRegion.Name);
            Assert.Equal(expectedRegion.RegionImageUrl, returnedRegion.RegionImageUrl);
        }


        [Fact]
        public Task Delete_NotFoundException_404()
        {
            return AssertThatDeleteHandlesGivenException(new NotFoundException("Region not found."), HttpStatusCode.NotFound);
        }

        [Fact]
        public Task Delete_BadRequestException_404()
        {
           return AssertThatDeleteHandlesGivenException(new BadRequestException("There was a problem when deleting the region."), HttpStatusCode.BadRequest);
        }


        private async Task AssertThatUpdateHandlesGivenException(Exception givenException, HttpStatusCode resultingStatusCode)
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            var updateRegion = new UpdateRegionDTO
            {
                Code = "KIN",
                Name = "Kinross",
                RegionImageUrl = "kinross-image.png"
            };

            regionServiceMock.Setup(regionService => regionService.UpdateRegion(id, It.IsAny<UpdateRegionDTO>())).ThrowsAsync(givenException);

            //Act
            var response = await _httpClient.PutAsJsonAsync($"api/regions/{id}", updateRegion);

            //Assert
            Assert.Equal(resultingStatusCode, response.StatusCode);
        }
        private async Task AssertThatDeleteHandlesGivenException(Exception givenException, HttpStatusCode resultingStatusCode)
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            regionServiceMock.Setup(regionService => regionService.DeleteRegion(id)).ThrowsAsync(givenException);

            //Act
            var response = await _httpClient.DeleteAsync($"api/regions/{id}");

            //Assert
            Assert.Equal(resultingStatusCode, response.StatusCode);
        }
    }
}
