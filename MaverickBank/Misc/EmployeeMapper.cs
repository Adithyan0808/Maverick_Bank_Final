using AutoMapper;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class EmployeeMapper : Profile
    {
        public EmployeeMapper()
        {
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.AccountTypeName));

            CreateMap<Customer, CustomerDetailsDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName)) 
                                                                                           
                 .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber)) 
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Accounts));
        }
    }
}
