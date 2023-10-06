using APPAPI.Models.Domain;
using APPAPI.Models.Dto;
using AutoMapper;

namespace APPAPI.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<CreateRegionDto, Region>().ReverseMap();
            CreateMap<UpdateRegionDto,RegionDto>().ReverseMap();
            CreateMap<Walk, WalkDto>().ReverseMap();
            CreateMap<CreateWalkDto, Walk>().ReverseMap();
            CreateMap<Difficulty, DifficultyDto>().ReverseMap();
            CreateMap<UpdateWalkDto, Walk>().ReverseMap();
        }
    }
}
