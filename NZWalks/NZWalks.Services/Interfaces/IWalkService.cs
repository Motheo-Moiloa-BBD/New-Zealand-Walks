using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Services.Interfaces
{
    public interface IWalkService
    {
        Task<IEnumerable<WalkDTO>> GetAllWalks(string? filterOn = null, string? filterQuery = null, string? sortBy = null, string? sortOrder = null, int? pageNumber = 1, int? pageSize = 5);
        Task<WalkDTO> GetWalkById(Guid id);
        Task<WalkDTO> CreateWalk(AddWalkDTO addWalkDTO);
        Task<WalkDTO> UpdateWalk(Guid id, UpdateWalkDTO updatedWalkDTO);
        Task<WalkDTO> DeleteWalk(Guid id);

    }
}
