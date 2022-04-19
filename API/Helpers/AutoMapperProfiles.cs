using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(
                      src =>
                      src.Photos.FirstOrDefault(
                          x => x.IsMain).Url))
                 .ForMember(dest => dest.Age, opt => opt.MapFrom(
                       src => src.DateOfBirth.CalculateAge())); // from appuser to memberDto
            CreateMap<Photo, PhotoDto>(); // from photo to photoDto

            //from update Dto to user entity
            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<RegisterDTO, AppUser>();
        }
    }
}

