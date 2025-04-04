﻿using AutoMapper;
using UdemyProject.Models.Domain;
using UdemyProject.Models.DTOs;

namespace UdemyProject.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<Region, UpdateRegionRequestDto>().ReverseMap();
            CreateMap<Region, AddRegionRequestDto>().ReverseMap();
        }
    }
}
