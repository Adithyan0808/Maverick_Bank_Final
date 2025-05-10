using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaverickBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminRegisterController : ControllerBase
    {
        private readonly IAdminRegistrationService _adminService;

        public AdminRegisterController(IAdminRegistrationService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterAdminDTO dto)
        {
            try
            {
                var result = await _adminService.RegisterAdminAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
