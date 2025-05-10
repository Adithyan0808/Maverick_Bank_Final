using AutoMapper;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class TransactionTypeMapper : Profile
    {
        public TransactionTypeMapper()
        {
            CreateMap<TransactionType, TransactionTypeDto>()
                .ForMember(dest => dest.TransactionTypeId, opt => opt.MapFrom(src => src.TransactionTypeId))
                .ForMember(dest => dest.TransactionTypeName, opt => opt.MapFrom(src => src.TransactionTypeName));
        }
    }
}
