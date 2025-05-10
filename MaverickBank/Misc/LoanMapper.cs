// Misc/LoanMapper.cs
using AutoMapper;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class LoanMapper : Profile
    {
        public LoanMapper()
        {
            CreateMap<Loan, LoanApplicationResponseDTO>()
                .ForMember(dest => dest.LoanTypeName, opt => opt.MapFrom(src => src.LoanMaster.LoanTypeName))
                .ForMember(dest => dest.LoanStatus, opt => opt.MapFrom(src => src.LoanStatus.ToString()));

            CreateMap<LoanMaster, LoanMasterDto>();
                
        }
    }
}
