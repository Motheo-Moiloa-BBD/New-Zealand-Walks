using AutoMapper;
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
        private readonly IMapper mapper;

        public RegionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<RegionDTO>> GetAllRegions()
        {
            var existingRegions = await unitOfWork.Regions.GetAll();
            
            var regionsDto = mapper.Map<IEnumerable<Region>, IEnumerable<RegionDTO>>(existingRegions);
           
            return regionsDto;
        }
        public async Task<RegionDTO> GetRegionById(Guid id)
        {
            var existingRegion = await unitOfWork.Regions.GetById(id);
            
            if (existingRegion == null)
            {
                return null;
            }

            var regionDto = mapper.Map<Region, RegionDTO>(existingRegion);

            return regionDto;
        }
        public async Task<RegionDTO> CreateRegion(AddRegionDTO addRegionDTO)
        {
            var newRegion = mapper.Map<AddRegionDTO, Region>(addRegionDTO);

            await unitOfWork.Regions.Add(newRegion);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var regionDto = mapper.Map<Region, RegionDTO>(newRegion);

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
                var updatedRegion = mapper.Map<Region, RegionDTO>(exisitingRegion);

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
                var deletedRegion = mapper.Map<Region, RegionDTO>(region);

                return deletedRegion;
            }
            else
            {
                return null;
            }
        }

    }
}
