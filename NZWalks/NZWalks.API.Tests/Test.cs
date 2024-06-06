using Microsoft.AspNetCore.Mvc;
using Moq;
using NZWalks.API.Controllers;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NZWalks.Tests
{
    public class RegionsControllerTests
    {
        private readonly Mock<IRegionService> mockRegionService;
        private readonly RegionsController regionsController;

        public RegionsControllerTests()
        {
            mockRegionService = new Mock<IRegionService>();
            regionsController = new RegionsController(mockRegionService.Object);
        }

        [Fact]
        public async Task GetAll_Success()
        {
            // Arrange
            var mockRegions = new List<RegionDTO>
            {
                new() { Id = Guid.NewGuid(), Name = "Region1", Code = "R1" },
                new() { Id = Guid.NewGuid(), Name = "Region2", Code = "R2" }
            };

            mockRegionService.Setup(service => service.GetAllRegions()).ReturnsAsync(mockRegions);

            // Act
            var result = await regionsController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<RegionDTO>>(okResult.Value);
            Assert.Equal(mockRegions.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetById_Success()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var mockRegion = new RegionDTO { Id = regionId, Name = "Region1", Code = "R1" };

            mockRegionService.Setup(service => service.GetRegionById(regionId)).ReturnsAsync(mockRegion);

            // Act
            var result = await regionsController.GetById(regionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RegionDTO>(okResult.Value);
            Assert.Equal(regionId, returnValue.Id);
        }

        [Fact]
        public async Task Create_Success()
        {
            // Arrange
            var mockRegion = new RegionDTO { Id = Guid.NewGuid(), Name = "Region1", Code = "R1" };
            var addRegionDTO = new AddRegionDTO { Name = "Region1", Code = "R1" };

            mockRegionService.Setup(service => service.CreateRegion(addRegionDTO)).ReturnsAsync(mockRegion);

            // Act
            var result = await regionsController.Create(addRegionDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<RegionDTO>(createdAtActionResult.Value);
            Assert.Equal(mockRegion.Id, returnValue.Id);
        }

        [Fact]
        public async Task Update_Success()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var mockRegion = new RegionDTO { Id = regionId, Name = "UpdatedRegion", Code = "UR1" };
            var updateRegionDTO = new UpdateRegionDTO { Name = "UpdatedRegion", Code = "UR1" };

            mockRegionService.Setup(service => service.UpdateRegion(regionId, updateRegionDTO)).ReturnsAsync(mockRegion);

            // Act
            var result = await regionsController.Update(regionId, updateRegionDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RegionDTO>(okResult.Value);
            Assert.Equal(mockRegion.Id, returnValue.Id);
        }

        [Fact]
        public async Task Delete_Success()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var mockRegion = new RegionDTO { Id = regionId, Name = "Region1", Code = "R1" };

            mockRegionService.Setup(service => service.DeleteRegion(regionId)).ReturnsAsync(mockRegion);

            // Act
            var result = await regionsController.Delete(regionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RegionDTO>(okResult.Value);
            Assert.Equal(mockRegion.Id, returnValue.Id);
        }
    }
}
