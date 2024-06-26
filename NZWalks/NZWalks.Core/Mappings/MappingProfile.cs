﻿using AutoMapper;
using NZWalks.Core.Models.Domain;
using NZWalks.Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            //Region mappings
            CreateMap<Region, RegionDTO>().ReverseMap();
            CreateMap<AddRegionDTO, Region>();

            //Walk mappings
            CreateMap<Walk, WalkDTO>().ReverseMap();  
            CreateMap<AddWalkDTO, Walk>();

            //Difficulty mappings
            CreateMap<Difficulty, DifficultyDTO>().ReverseMap();

            //Image mappings
            CreateMap<Image, ImageDTO>().ReverseMap();
  
        }
    }
}
