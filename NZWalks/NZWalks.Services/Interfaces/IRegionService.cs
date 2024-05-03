using NZWalks.Core.Models.Domain;
using NZWalks.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionDTO>> GetAllRegions();
        Task<RegionDTO> GetRegionById(Guid id);
        Task<RegionDTO> CreateRegion(AddRegionDTO addRegionDTO);
        Task<RegionDTO> UpdateRegion(Guid id, UpdateRegionDTO updateRegionDTO);
        Task<RegionDTO> DeleteRegion(Guid id);
    }
}
