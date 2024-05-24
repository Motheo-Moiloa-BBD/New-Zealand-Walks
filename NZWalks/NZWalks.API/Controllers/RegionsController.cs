using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.Services.Interfaces;
using NZWalks.Core.Models.DTO;
using NZWalks.Core.Models.Domain;
using NZWalks.Infrastructure.Repositories;
using NZWalks.Core.CustomAttributes;
using Microsoft.AspNetCore.Authorization;

namespace NZWalks.API.Controllers
{
    [Route("api/regions")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService regionService;

        public RegionsController(IRegionService regionService)
        {
            this.regionService = regionService;
        }

        [HttpGet]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll()
        {
            var regions = await regionService.GetAllRegions();

            return Ok(regions);
        }

        
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var region = await regionService.GetRegionById(id);

            return Ok(region);
        }

        
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionDTO addRegionDTO)
        {
           var createdRegion = await regionService.CreateRegion(addRegionDTO);

           return CreatedAtAction(nameof(GetById), new {id = createdRegion.Id }, createdRegion);
        }

        
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDTO updateRegionRequestDTO)
        {
            var updatedRegion = await regionService.UpdateRegion(id, updateRegionRequestDTO); 

            return Ok(updatedRegion);
        }
        
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedRegion = await regionService.DeleteRegion(id);

            return Ok(deletedRegion);
        }

        
    }
}
