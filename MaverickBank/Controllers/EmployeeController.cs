
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace MaverickBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee,Admin")]
    [EnableCors("AllowReactApp")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<CustomerDetailsDTO>>> GetAllCustomers()
        {
            var customers = await _employeeService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("customers/{id}")]
        public async Task<ActionResult<CustomerDetailsDTO>> GetCustomerById(int id)
        {
            var customer = await _employeeService.GetCustomerByIdAsync(id);
            return Ok(customer);
        }

        
        [HttpGet("customers/by-account-type")]
        public async Task<ActionResult<IEnumerable<CustomerDetailsDTO>>> GetCustomersByAccountType([FromQuery] string accountType)
        {
            try
            {
                var customers = await _employeeService.GetCustomersByAccountTypeAsync(accountType);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("customers/{customerId}")]
        public async Task<ActionResult<CustomerDetailsDTO>> UpdateCustomer(int customerId, [FromBody] CustomerUpdateDTO updateDto)
        {
            try
            {
                var updatedCustomer = await _employeeService.UpdateCustomerAsync(customerId, updateDto);
                return Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        
        [HttpDelete("customer/{customerId}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            try
            {
                var result = await _employeeService.DeleteCustomerAsync(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }



    }
}
