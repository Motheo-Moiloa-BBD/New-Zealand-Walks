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
        Task<IEnumerable<WalkDTO>> GetAllWalks();
        Task<WalkDTO> GetWalkById(Guid id);
        Task<WalkDTO> CreateWalk(AddWalkDTO addWalkDTO);
        Task<WalkDTO> UpdateWalk(Guid id, UpdateWalkDTO updatedWalkDTO);
        Task<WalkDTO> DeleteWalk(Guid id);

    }
}
