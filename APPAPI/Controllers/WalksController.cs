using APPAPI.CustomActionFilters;
using APPAPI.Data;
using APPAPI.Models.Domain;
using APPAPI.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;

        public WalksController(DataBaseContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy,
            [FromQuery] bool? ascending, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 1000)
        {   //Filtering

            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();
            if(!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            { 
                if(filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }         
            }
            //Sorting
            if(!string.IsNullOrWhiteSpace(sortBy))
            {
                var sort = ascending ?? null;
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase) && sort.HasValue)
                {
                    walks = (bool)sort ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase) && sort.HasValue)
                {
                    walks = (bool)sort ? walks.OrderBy(x => x.Length) : walks.OrderByDescending(x => x.Length);
                }
            }
            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return Ok(mapper.Map<List<WalkDto>>(walks.Skip(skipResults).Take(pageSize)));
        }
        [HttpGet]
        [Route("GetOptionsList")]
        public async Task<IActionResult> GetOptionsList()
        {   
            var regions = await dbContext.Regions.ToListAsync();
            var regionsDto = mapper.Map<List<RegionDto>>(regions);

            var diffituclites = await dbContext.Difficulties.ToListAsync();
            var difficultiesDto = mapper.Map<List<DifficultyDto>>(diffituclites);

            var optionsListDto = new OptionsListDto
            {
                Regions = regionsDto,
                Difficulties = difficultiesDto
            };
            return Ok(optionsListDto);
        }
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(w => w.Id == id);
            if (walk == null) return NotFound();
            var walkDto = mapper.Map<WalkDto>(walk);
            return Ok(walkDto);
        }
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] CreateWalkDto walkDto)
        {
            var walk = mapper.Map<Walk>(walkDto);
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            var createWalkDto = mapper.Map<WalkDto>(walk);
            return Ok(createWalkDto);
        }
        [HttpPut("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid Id, UpdateWalkDto updateWalkDto)
        {
            var walk = await dbContext.Walks.FirstOrDefaultAsync(w => w.Id == Id);
            if (walk == null) return NotFound();
            walk.Name = updateWalkDto.Name;
            walk.Description = updateWalkDto.Description;
            walk.Length = updateWalkDto.Length;
            walk.WalkImageUrl = updateWalkDto.WalkImageUrl;
            walk.RegionId = updateWalkDto.RegionId;
            walk.DifficultyId = updateWalkDto.DifficultyId;
            await dbContext.SaveChangesAsync();
            var walkDto = mapper.Map<Walk>(updateWalkDto);
            return Ok(walkDto);
        }
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = await dbContext.Walks.FirstOrDefaultAsync(w => w.Id == id);
            if (walk == null) return NotFound();
            dbContext.Walks.Remove(walk);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
