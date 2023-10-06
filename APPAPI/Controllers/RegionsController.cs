using APPAPI.CustomActionFilters;
using APPAPI.Data;
using APPAPI.Models.Domain;
using APPAPI.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(DataBaseContext dbContext, IMapper mapper, ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> GetAll()
        {   
            var regions = await dbContext.Regions.ToListAsync();
            /*var regionsDto = new List<RegionDto>();
            foreach (var region in regions)
            { 
                regionsDto.Add(new RegionDto()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                });
            }*/
            var regionsDto = mapper.Map<List<RegionDto>>(regions);
            logger.LogInformation($"Regions: {JsonSerializer.Serialize(regions)}");
            return Ok(regionsDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var region = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if(region == null) return NotFound(); 

            /*var regionDto = new RegionDto()
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };*/
            var regionDto = mapper.Map<RegionDto>(region);
            return Ok(regionDto);
        }
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] CreateRegionDto addRegionDto)
        {
            var region = mapper.Map<Region>(addRegionDto);
            await dbContext.Regions.AddAsync(region);
            await dbContext.SaveChangesAsync();
            var regionDto = mapper.Map<RegionDto>(region);
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }
        [HttpPut("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionDto updateRegionDto)
        {  
            var region = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if (region == null) return NotFound();
            region.Code = updateRegionDto.Code;
            region.Name = updateRegionDto.Name;
            region.RegionImageUrl = updateRegionDto.RegionImageUrl;
            await dbContext.SaveChangesAsync();
            var regionDto = mapper.Map<RegionDto>(region);
            return Ok(regionDto);
        }
        [HttpDelete("{id:Guid}")]
       //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if (region == null)  return NotFound(); 
            dbContext.Regions.Remove(region);
            await dbContext.SaveChangesAsync();
            var regionDto = mapper.Map<RegionDto> (region);
            return Ok(regionDto);
        }
    }
}
