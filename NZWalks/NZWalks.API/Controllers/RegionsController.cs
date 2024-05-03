using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.Services.Interfaces;
using NZWalks.Core.Models.DTO;
using NZWalks.Core.Models.Domain;
using NZWalks.Infrastructure.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService regionService;

        public RegionsController(IRegionService regionService)
        {
            this.regionService = regionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await regionService.GetAllRegions();

            return Ok(regions);
        }

        
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var region = await regionService.GetRegionById(id);

            if(region == null)
            {
                return NotFound();
            }
           
            return Ok(region);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionDTO addRegionDTO)
        {
           var createdRegion = await regionService.CreateRegion(addRegionDTO);

           if(createdRegion == null)
           {
                return BadRequest();
           }

            return CreatedAtAction(nameof(GetById), new {id = createdRegion.Id }, createdRegion);
        }

        
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDTO updateRegionRequestDTO)
        {
            var updatedRegion = await regionService.UpdateRegion(id, updateRegionRequestDTO); 

            if(updatedRegion == null)
            {
                return NotFound();
            }

            return Ok(updatedRegion);
        }
        
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedRegion = await regionService.DeleteRegion(id);

            if (deletedRegion == null)
            {
                return NotFound();
            }
            
            return Ok(deletedRegion);
        }

        
    }
}
