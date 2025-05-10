using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<CustomerDetailsDTO>> GetAllCustomersAsync();
        Task<CustomerDetailsDTO> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<CustomerDetailsDTO>> GetCustomersByAccountTypeAsync(string accountTypeName);
        Task<CustomerDetailsDTO> UpdateCustomerAsync(int customerId, CustomerUpdateDTO updateDto);
        //now
        Task<object> DeleteCustomerAsync(int customerId);


    }
}
