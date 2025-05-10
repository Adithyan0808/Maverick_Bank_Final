using AutoMapper;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;

namespace MaverickBank.Misc
{
    public class BranchMapper : Profile
    {
        public BranchMapper()
        {
            CreateMap<Branch, BranchDto>();
        }
    }
}
