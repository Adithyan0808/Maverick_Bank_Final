using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace MaverickBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowReactApp")]
    public class CustomerRegisterController : ControllerBase
    {
        private readonly ICustomerRegistrationService _registrationService;
        
        public CustomerRegisterController(ICustomerRegistrationService registrationService)
                              
        {
            _registrationService = registrationService;
            
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register(RegisterCustomerDTO dto)
        {
            try
            {
                var response = await _registrationService.RegisterCustomerAsync(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
