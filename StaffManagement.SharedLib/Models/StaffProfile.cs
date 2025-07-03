using AutoMapper;
using StaffManagement.SharedLib.Models;

namespace StaffManagement.SharedLib.Profiles
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            // Staff to StaffDto mapping
            CreateMap<Staff, StaffDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // CreateStaffDto to Staff mapping
            CreateMap<CreateStaffDto, Staff>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // UpdateStaffDto to Staff mapping
            CreateMap<UpdateStaffDto, Staff>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
