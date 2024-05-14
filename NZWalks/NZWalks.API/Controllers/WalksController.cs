using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Models.DTO;
using NZWalks.Services;
using NZWalks.Services.Interfaces;

namespace NZWalks.API.Controllers
{
    [Route("api/walks")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IWalkService walkService;
        public WalksController(IWalkService walkService)
        {
            this.walkService = walkService;
        }

        [HttpGet]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] string? sortOrder, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var walks = await walkService.GetAllWalks(filterOn, filterQuery, sortBy, sortOrder, pageNumber, pageSize);

            return Ok(walks);
        }
        
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await walkService.GetWalkById(id);

            if (walk == null)
            {
                return NotFound();
            }

            return Ok(walk);
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddWalkDTO addWalkDTO)
        {
            var createdWalk = await walkService.CreateWalk(addWalkDTO);

            if(createdWalk == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetById), new { id = createdWalk.Id }, createdWalk);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkDTO updatedWalkDto)
        {
            var updatedWalk = await walkService.UpdateWalk(id, updatedWalkDto);

            if(updatedWalk == null)
            {
                return NotFound();
            }

            return Ok(updatedWalk);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedWalk = await walkService.DeleteWalk(id);

            if (deletedWalk == null)
            {
                return NotFound();
            }

            return Ok(deletedWalk);
        }
    }
}
