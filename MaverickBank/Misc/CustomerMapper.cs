using AutoMapper;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<Customer, CustomerSelfDetailsDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Accounts));

            CreateMap<Account, CustomerAccountDTO>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.AccountTypeName))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));

            
            CreateMap<Account, AccountDetailsDTO>()
    .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
    .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.AccountTypeName))
    .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));



        }
    }

}
