using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>()
            .ForMember(
                r => r.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(X => X.IsMain).Url)
            )
            .ForMember(
                r => r.Age,
                opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())
            );

            CreateMap<Photo, PhotoDTO>();
        }
    }
}