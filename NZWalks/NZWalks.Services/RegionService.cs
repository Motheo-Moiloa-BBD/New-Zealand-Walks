using NZWalks.Core.Interfaces;
using NZWalks.Core.Models.Domain;
using NZWalks.Core.Models.DTO;
using NZWalks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork unitOfWork;
        public RegionService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RegionDTO>> GetAllRegions()
        {
            var existingRegions = await unitOfWork.Regions.GetAll();
            
            var regionsDto = new List<RegionDTO>();
            foreach (var region in existingRegions)
            {
                regionsDto.Add(new RegionDTO()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl,
                });
            }

            return regionsDto;
        }
        public async Task<RegionDTO> GetRegionById(Guid id)
        {
            var existingRegion = await unitOfWork.Regions.GetById(id);
            
            if (existingRegion == null)
            {
                return null;
            }

            var regionDto = new RegionDTO()
            {
                Id = existingRegion.Id,
                Code = existingRegion.Code,
                Name = existingRegion.Name,
                RegionImageUrl = existingRegion.RegionImageUrl,
            };

            return regionDto;
        }
        public async Task<RegionDTO> CreateRegion(AddRegionDTO addRegionDTO)
        {
            var newRegion = new Region
            {
                Code = addRegionDTO.Code,
                Name = addRegionDTO.Name,
                RegionImageUrl = addRegionDTO.RegionImageUrl,
            };

            await unitOfWork.Regions.Add(newRegion);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var regionDto = new RegionDTO()
                {
                    Id = newRegion.Id,
                    Code = newRegion.Code,
                    Name = newRegion.Name,
                    RegionImageUrl = newRegion.RegionImageUrl,
                };

                return regionDto;
            }else
            {
                return null;
            }
        }
        public async Task<RegionDTO> UpdateRegion(Guid id,UpdateRegionDTO updateRegionDTO)
        {
            var exisitingRegion = await unitOfWork.Regions.GetById(id);

            if (exisitingRegion == null)
            {
                return null;
            }

            exisitingRegion.Code = updateRegionDTO.Code;
            exisitingRegion.Name = updateRegionDTO.Name;
            exisitingRegion.RegionImageUrl = updateRegionDTO.RegionImageUrl;

            unitOfWork.Regions.Update(exisitingRegion);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var updatedRegion = new RegionDTO
                {
                    Id = exisitingRegion.Id,
                    Code = exisitingRegion.Code,
                    Name = exisitingRegion.Name,
                    RegionImageUrl = exisitingRegion.RegionImageUrl,
                };

                return updatedRegion;
            }
            else
            {
                return null;
            }
        }
        public async Task<RegionDTO> DeleteRegion(Guid id)
        {
            var region = await unitOfWork.Regions.GetById(id);

            if (region == null)
            {
                return null;
            }

            unitOfWork.Regions.Delete(region);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var deletedRegion = new RegionDTO
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl,
                };

                return deletedRegion;
            }
            else
            {
                return null;
            }
        }

    }
}
