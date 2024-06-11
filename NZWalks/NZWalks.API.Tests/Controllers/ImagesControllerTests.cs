
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NZWalks.Services.Interfaces;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NZWalks.Core.Models.DTO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Net;

namespace NZWalks.API.Tests.Controllers
{
    public class ImagesControllerTests: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Mock<IImageService> imageServiceMock = new();
        private readonly WebApplicationFactory<Program> factory;
        private HttpClient _httpClient;
        public ImagesControllerTests(WebApplicationFactory<Program> factory)
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
                        services.AddSingleton(imageServiceMock.Object);
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

        [Fact(Skip = "Needs Research")]
        public async Task Upload_Success()
        {
            //Arrange
            var fileMock = new Mock<IFormFile>();
            var physicalFile = new FileInfo("Images/Motheo.jpg");
            var fileName = physicalFile.Name;
            byte[] fileBytes = File.ReadAllBytes("Images/Motheo.jpg");

            IFormFile formFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "testFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "multipart/form-data",
                ContentDisposition = "image/jpg"
            };

            var imageUpload = new ImageUploadDTO
            {
                File = formFile,
                FileName = fileName,
                FileDescription = "Testing File"
            };

            var expectedImage = new ImageDTO
            {
                Id = Guid.Parse("14dd61ce-140a-46ed-9c92-6be487d163c9"),
                FileExtension = Path.GetExtension(imageUpload.File.FileName),
                FileSizeInBytes = imageUpload.File.Length,
                FileName = imageUpload.FileName,
                FileDescription = imageUpload.FileDescription
            };

            imageServiceMock.Setup(imageService => imageService.uploadImage(It.IsAny<ImageUploadDTO>())).ReturnsAsync(expectedImage);

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/images", imageUpload);

            //Assert
            response.EnsureSuccessStatusCode();
            var returnedJson = await response.Content.ReadAsStringAsync();
            var returnedRegion = JsonConvert.DeserializeObject<RegionDTO>(returnedJson);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
