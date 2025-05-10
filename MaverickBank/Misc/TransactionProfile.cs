using AutoMapper;
using MaverickBank.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MaverickBank.Misc
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionResponseDTO>()
                .ForMember(dest => dest.SourceAccountNumber, opt => opt.MapFrom(src => src.SourceAccount.AccountNumber))
                .ForMember(dest => dest.DestinationAccountNumber, opt => opt.MapFrom(src => src.DestinationAccount.AccountNumber))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.TransactionTypeName));
        }
    }
}
