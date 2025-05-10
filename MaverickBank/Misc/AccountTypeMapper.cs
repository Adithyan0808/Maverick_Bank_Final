using AutoMapper;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class AccountTypeMapper : Profile
    {
        public AccountTypeMapper()
        {
            CreateMap<AccountType, AccountTypeDto>();
        }
    }
}
