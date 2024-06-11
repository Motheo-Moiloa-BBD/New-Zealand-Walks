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
    public class WalksControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Mock<IWalkService> walkServiceMock = new();
        private readonly WebApplicationFactory<Program> factory;
        private HttpClient _httpClient;
        public WalksControllerTests(WebApplicationFactory<Program> factory)
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
                        services.AddSingleton(walkServiceMock.Object);
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
            var walks = new List<WalkDTO>()
            {
                new WalkDTO()
                {
                    Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                    Name = "W1",
                    Description = "W1",
                    Length = 5,
                    WalkImageUrl = "w1.jpeg",
                    Difficulty = new DifficultyDTO()
                    {
                        Id = Guid.Parse("6cfa68f3-71a8-496e-957b-eff5d48d5375"),
                        Name= "Hard"
                    },
                    Region = new RegionDTO()
                    {
                        Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                        Code = "TES",
                        Name = "Test Region",
                        RegionImageUrl = "test-region.jpeg"
                    }
                }
            };

            walkServiceMock.Setup(walkService => walkService.GetAllWalks(null, null, null, null, null, null)).ReturnsAsync(walks);

            //Act
            var response = await _httpClient.GetAsync("api/walks");

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedList = JsonConvert.DeserializeObject<List<WalkDTO>>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Collection(returnedList, walk =>
            {
                Assert.Equal(walks[0].Id, walk.Id);
                Assert.Equal(walks[0].Name, walk.Name);
                Assert.Equal(walks[0].Description, walk.Description);
                Assert.Equal(walks[0].Length, walk.Length);
                Assert.Equal(walks[0].WalkImageUrl, walk.WalkImageUrl);
                Assert.Equivalent(walks[0].Difficulty, walk.Difficulty, true);
                Assert.Equivalent(walks[0].Region, walk.Region, true);
            });
        }

        [Fact]
        public async Task GetById_Success()
        {
            //Arrange 
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");
            var existingWalk = new WalkDTO()
            {

                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Name = "W1",
                Description = "W1",
                Length = 5,
                WalkImageUrl = "w1.jpeg",
                Difficulty = new DifficultyDTO()
                {
                    Id = Guid.Parse("6cfa68f3-71a8-496e-957b-eff5d48d5375"),
                    Name = "Hard"
                },
                Region = new RegionDTO()
                {
                    Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                    Code = "TES",
                    Name = "Test Region",
                    RegionImageUrl = "test-region.jpeg"
                }
            };

            walkServiceMock.Setup(walkService => walkService.GetWalkById(id)).ReturnsAsync(existingWalk);

            //Act
            var response = await _httpClient.GetAsync($"api/walks/{id}");

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedWalk = JsonConvert.DeserializeObject<WalkDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equivalent(existingWalk, returnedWalk, strict: true);
        }

        [Fact]
        public async Task GetById_NotFoundException_404()
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            walkServiceMock.Setup(walkService => walkService.GetWalkById(id)).ThrowsAsync(new NotFoundException($"Walk with id {id} does not exist."));

            //Act
            var response = await _httpClient.GetAsync($"api/walks/{id}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Success()
        {
            //Arrange
            var addWalk = new AddWalkDTO
            {
                Name = "W1",
                Description = "W1",
                Length = 5,
                WalkImageUrl = "w1.jpeg",
                DifficultyId = Guid.Parse("de27797a-935f-4d21-8428-92fde6cb0fd0"),
                RegionId = Guid.Parse("dcc0bc47-a525-419d-99b3-8daeed36b59a")
            };

            var expectedWalk = new WalkDTO
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Name = "W1",
                Description = "W1",
                Length = 5,
                WalkImageUrl = "w1.jpeg",
                Difficulty = new DifficultyDTO()
                {
                    Id = Guid.Parse("de27797a-935f-4d21-8428-92fde6cb0fd0"),
                    Name = "Hard"
                },
                Region = new RegionDTO()
                {
                    Id = Guid.Parse("dcc0bc47-a525-419d-99b3-8daeed36b59a"),
                    Code = "TES",
                    Name = "Test Region",
                    RegionImageUrl = "test-region.jpeg"
                }
            };

            walkServiceMock.Setup(walkService => walkService.CreateWalk(It.IsAny<AddWalkDTO>())).ReturnsAsync(expectedWalk);

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/walks", addWalk);

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedWalk = JsonConvert.DeserializeObject<WalkDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equivalent(expectedWalk, returnedWalk, strict: true);
            Assert.Equivalent(expectedWalk.Region, returnedWalk.Region, strict: true);
            Assert.Equivalent(expectedWalk.Difficulty, returnedWalk.Difficulty, strict: true);
        }

        [Fact]
        public async Task Create_BadRequestException_400()
        {
            //Arrange
            var addWalk = new AddWalkDTO
            {
                Name = "W1",
                Description = "W1",
                Length = 5,
                WalkImageUrl = "w1.jpeg",
                DifficultyId = Guid.Parse("de27797a-935f-4d21-8428-92fde6cb0fd0"),
                RegionId = Guid.Parse("dcc0bc47-a525-419d-99b3-8daeed36b59a")
            };

            walkServiceMock.Setup(walkService => walkService.CreateWalk(It.IsAny<AddWalkDTO>())).ThrowsAsync(new BadRequestException("There was a problem when saving the walk."));

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/walks", addWalk);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            var updateWalk = new UpdateWalkDTO
            {
                Name = "W2",
                Description = "W2",
                Length = 10,
                WalkImageUrl = "w2.jpeg",
                DifficultyId = Guid.Parse("de27797b-935f-4d21-8428-92fde6cb0fd0"),
                RegionId = Guid.Parse("dcc0bc45-a525-419d-99b3-8daeed36b59a")
            };

            var expectedWalk = new WalkDTO
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Name = "W2",
                Description = "W2",
                Length = 5,
                WalkImageUrl = "w2.jpeg",
                Difficulty = new DifficultyDTO()
                {
                    Id = Guid.Parse("de27797b-935f-4d21-8428-92fde6cb0fd0"),
                    Name = "Hard"
                },
                Region = new RegionDTO()
                {
                    Id = Guid.Parse("dcc0bc45-a525-419d-99b3-8daeed36b59a"),
                    Code = "TES",
                    Name = "Test Region",
                    RegionImageUrl = "test-region.jpeg"
                }
            };

            walkServiceMock.Setup(walkService => walkService.UpdateWalk(id, It.IsAny<UpdateWalkDTO>())).ReturnsAsync(expectedWalk);

            //Act
            var response = await _httpClient.PutAsJsonAsync($"api/walks/{id}", updateWalk);

            //Assert
            //response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedWalk = JsonConvert.DeserializeObject<WalkDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equivalent(expectedWalk, returnedWalk, strict: true);
            Assert.Equivalent(expectedWalk.Region, returnedWalk.Region, strict: true);
            Assert.Equivalent(expectedWalk.Difficulty, returnedWalk.Difficulty, strict: true);
        }

        [Fact]
        public Task Update_NotFoundException_404()
        {
            return AssertThatUpdateHandlesGivenException(new NotFoundException("Walk not found."), HttpStatusCode.NotFound);
        }

        [Fact]
        public Task Update_BadRequestException_400()
        {
            return AssertThatUpdateHandlesGivenException(new BadRequestException("There was a problem when deleting the walk."), HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task Delete_Success()
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            var expectedWalk = new WalkDTO
            {
                Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                Name = "W1",
                Description = "W1",
                Length = 5,
                WalkImageUrl = "w1.jpeg",
                Difficulty = new DifficultyDTO()
                {
                    Id = Guid.Parse("de27797a-935f-4d21-8428-92fde6cb0fd0"),
                    Name = "Hard"
                },
                Region = new RegionDTO()
                {
                    Id = Guid.Parse("dcc0bc47-a525-419d-99b3-8daeed36b59a"),
                    Code = "TES",
                    Name = "Test Region",
                    RegionImageUrl = "test-region.jpeg"
                }
            };

            walkServiceMock.Setup(walkService => walkService.DeleteWalk(id)).ReturnsAsync(expectedWalk);

            //Act
            var response = await _httpClient.DeleteAsync($"api/walks/{id}");

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedWalk = JsonConvert.DeserializeObject<WalkDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equivalent(expectedWalk, returnedWalk, strict: true);
            Assert.Equivalent(expectedWalk.Region, returnedWalk.Region, strict: true);
            Assert.Equivalent(expectedWalk.Difficulty, returnedWalk.Difficulty, strict: true);
        }


        [Fact]
        public Task Delete_NotFoundException_404()
        {
            return AssertThatDeleteHandlesGivenException(new NotFoundException("Walk not found."), HttpStatusCode.NotFound);
        }

        [Fact]
        public Task Delete_BadRequestException_404()
        {
            return AssertThatDeleteHandlesGivenException(new BadRequestException("There was a problem when deleting the walk."), HttpStatusCode.BadRequest);
        }


        private async Task AssertThatUpdateHandlesGivenException(Exception givenException, HttpStatusCode resultingStatusCode)
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            var updateWalk = new UpdateWalkDTO
            {
                Name = "W2",
                Description = "W2",
                Length = 10,
                WalkImageUrl = "w2.jpeg",
                DifficultyId = Guid.Parse("de27797b-935f-4d21-8428-92fde6cb0fd0"),
                RegionId = Guid.Parse("dcc0bc45-a525-419d-99b3-8daeed36b59a")
            };
            
            walkServiceMock.Setup(walkService => walkService.UpdateWalk(id, It.IsAny<UpdateWalkDTO>())).ThrowsAsync(givenException);

            //Act
            var response = await _httpClient.PutAsJsonAsync($"api/walks/{id}", updateWalk);

            //Assert
            Assert.Equal(resultingStatusCode, response.StatusCode);
        }
        private async Task AssertThatDeleteHandlesGivenException(Exception givenException, HttpStatusCode resultingStatusCode)
        {
            //Arrange
            var id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe");

            walkServiceMock.Setup(walkService => walkService.DeleteWalk(id)).ThrowsAsync(givenException);

            //Act
            var response = await _httpClient.DeleteAsync($"api/walks/{id}");

            //Assert
            Assert.Equal(resultingStatusCode, response.StatusCode);
        }
    }
}
