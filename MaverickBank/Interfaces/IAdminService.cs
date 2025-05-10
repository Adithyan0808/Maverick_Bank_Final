using MaverickBank.Models.DTOs;

namespace MaverickBank.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<EmployeeDetailsDTO>> GetAllEmployeesAsync();
        Task<EmployeeDetailsDTO> GetEmployeeByIdAsync(int id);
        Task<EmployeeDTO> UpdateEmployeeDetailsAsync(int employeeId, EmployeeUpdateDTO updateDto);
        Task<object> DeleteEmployeeByIdAsync(int employeeId);


    }
}
