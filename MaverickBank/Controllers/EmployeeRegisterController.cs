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


    public class EmployeeRegisterController : ControllerBase
    {
        private readonly IEmployeeRegistrationService _employeeService;

        public EmployeeRegisterController(IEmployeeRegistrationService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeDTO dto)
        {
            try
            {
                var result = await _employeeService.RegisterEmployeeAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}

