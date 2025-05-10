using AutoMapper;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class AdminMapper : Profile
    {
        public AdminMapper()
        {
            CreateMap<Employee, EmployeeDetailsDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                //04.05.25
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                //end
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName));
                
            //now
            CreateMap<Employee, EmployeeDTO>();
        }
    }
}
