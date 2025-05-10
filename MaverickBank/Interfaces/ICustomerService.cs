using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerSelfDetailsDTO> GetCustomerDetailsAsync(int customerId);
        
        Task<CustomerSelfDetailsDTO> UpdateCustomerAsync(int customerId, CustomerUpdateDTO dto);
        //now
        Task<IEnumerable<AccountDetailsDTO>> GetAccountsByCustomerIdAsync(int customerId);



    }
}
