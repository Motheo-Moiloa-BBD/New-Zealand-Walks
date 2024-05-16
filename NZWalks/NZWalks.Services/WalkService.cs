using AutoMapper;
using NZWalks.Core.Exceptions;
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
    public class WalkService : IWalkService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public WalkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<WalkDTO>> GetAllWalks(string? filterOn = null, string? filterQuery = null, string? sortBy = null, string? sortOrder = null, int? pageNumber = 1, int? pageSize = 5)
        {
            var existingWalks = await unitOfWork.Walks.GetAllWalksAsync(filterOn, filterQuery, sortBy, sortOrder, pageNumber, pageSize);

            var walksDto = mapper.Map<IEnumerable<Walk>, IEnumerable <WalkDTO>> (existingWalks);

            return walksDto;
        }
        public async Task<WalkDTO> GetWalkById(Guid id)
        {
            var existingWalk = await unitOfWork.Walks.GetWalkByIdAsync(id);

            if (existingWalk == null)
            {
                throw new NotFoundException($"Walk with id {id} does not exist.");
            }

            var walkDto = mapper.Map<Walk, WalkDTO>(existingWalk);

            return walkDto;
        }
        public async Task<WalkDTO> CreateWalk(AddWalkDTO addWalkDTO)
        {
            var newWalk = mapper.Map<AddWalkDTO, Walk>(addWalkDTO);

            await unitOfWork.Walks.Add(newWalk);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var walkDto = mapper.Map<Walk, WalkDTO>(newWalk);

                return walkDto;
            }
            else
            {
                throw new BadRequestException("There was a problem when saving the walk.");
            }
        }

        public async Task<WalkDTO> UpdateWalk(Guid id, UpdateWalkDTO updatedWalkDTO)
        {
            var existingWalk = await unitOfWork.Walks.GetWalkByIdAsync(id);

            if (existingWalk == null)
            {
                throw new NotFoundException($"Walk with id {id} does not exist.");
            }
            
            existingWalk.Name = updatedWalkDTO.Name;
            existingWalk.Description = updatedWalkDTO.Description;
            existingWalk.Length = updatedWalkDTO.Length;
            existingWalk.WalkImageUrl = updatedWalkDTO.WalkImageUrl;
            existingWalk.DifficultyId = updatedWalkDTO.DifficultyId;
            existingWalk.RegionId = updatedWalkDTO.RegionId;

            unitOfWork.Walks.Update(existingWalk);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var updatedWalk = mapper.Map<Walk, WalkDTO>(existingWalk);

                return updatedWalk;
            }
            else
            {
                throw new BadRequestException("There was a problem when updating the walk.");
            }
        }

        public async Task<WalkDTO> DeleteWalk(Guid id)
        {
            var existingWalk = await unitOfWork.Walks.GetWalkByIdAsync(id);

            if (existingWalk == null)
            {
                throw new NotFoundException($"Walk with id {id} does not exist.");
            }

            unitOfWork.Walks.Delete(existingWalk);
            var result = await unitOfWork.Save();

            if (result > 0)
            {
                var deletedWalk = mapper.Map<Walk, WalkDTO>(existingWalk);

                return deletedWalk;
            }
            else
            {
                throw new BadRequestException("There was a problem when updating the walk.");
            }
        }
    }
}
