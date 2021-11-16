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
            CreateMap<MemberUpdateDTO, AppUser>();
            CreateMap<RegisterDTO, AppUser>();

            CreateMap<Message, MessageDTO>()
            .ForMember(
                d => d.SenderPhotoUrl,
                opt => opt.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url)
            )
            .ForMember(
                d => d.RecipientPhotoUrl,
                opt => opt.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url)
            );

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}