using AutoMapper;
using Moq;
using NZWalks.Core.Interfaces;
using NZWalks.Core.Mappings;
using NZWalks.Core.Models.Domain;
using NZWalks.Core.Models.DTO;
using NZWalks.Services;
using NZWalks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.API.Tests.Services
{
    public class RegionServiceTests
    {
        private readonly Mock<IRegionRepository> regionRepositoryMock = new();
        private readonly Mock<IUnitOfWork> unitOfWorkMock = new();

        private readonly RegionService regionService;
        private readonly IMapper mapper;

        public RegionServiceTests()
        {
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile());
            });

            mapper = config.CreateMapper();

            unitOfWorkMock.Setup(unitOfWork => unitOfWork.Regions).Returns(regionRepositoryMock.Object);
            regionService = new RegionService(unitOfWorkMock.Object, mapper);
        }

        [Fact]
        public async Task GetAllRegions_Success()
        {
            //Arrange
            var regions = new List<Region>()
            {
                new Region()
                {
                    Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529cdbe"),
                    Code = "EMB",
                    Name = "Embalenhle",
                    RegionImageUrl = "embalenhle-image.png"
                },
                new Region()
                {
                    Id = Guid.Parse("9710f419-cc01-4489-adc0-ad43d529abcd"),
                    Code = "SEC",
                    Name = "Secunda",
                    RegionImageUrl = "secunda-image.png"
                }
            };

            regionRepositoryMock.Setup(regionRepository => regionRepository.GetAll()).ReturnsAsync(regions);
            
            //Act
            var returnedList = await regionService.GetAllRegions();
            //Assert
            Assert.Collection(returnedList, region =>
            {
                Assert.Equal(regions[0].Id, region.Id);
                Assert.Equal(regions[0].Code, region.Code);
                Assert.Equal(regions[0].Name, region.Name);
                Assert.Equal(regions[0].RegionImageUrl, region.RegionImageUrl);
            }, region =>
            {
                Assert.Equal(regions[1].Id, region.Id);
                Assert.Equal(regions[1].Code, region.Code);
                Assert.Equal(regions[1].Name, region.Name);
                Assert.Equal(regions[1].RegionImageUrl, region.RegionImageUrl);
            });
        }
    }
}
