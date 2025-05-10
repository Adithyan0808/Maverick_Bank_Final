using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaverickBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<EmployeeDetailsDTO>>> GetAllEmployees()
        {
            var employees = await _adminService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<EmployeeDetailsDTO>> GetEmployeeById(int id)
        {
            var employee = await _adminService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }
        
        [HttpPut("employee/{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeDetails(int employeeId, EmployeeUpdateDTO updateDto)
        {
            try
            {
                var updatedEmployee = await _adminService.UpdateEmployeeDetailsAsync(employeeId, updateDto);
                return Ok(new { message = "Employee updated successfully", employee = updatedEmployee });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("employee/{id}")]
        public async Task<IActionResult> DeleteEmployeeById(int id)
        {
            try
            {
                var result = await _adminService.DeleteEmployeeByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }




    }
}
