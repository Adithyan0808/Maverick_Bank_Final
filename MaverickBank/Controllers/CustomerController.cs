using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaverickBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowReactApp")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerSelfDetailsDTO>> GetCustomerDetails(int id)
        {
            try
            {
                var customer = await _service.GetCustomerDetailsAsync(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerSelfDetailsDTO>> UpdateCustomer(int id, CustomerUpdateDTO dto)
        {
            try
            {
                var updated = await _service.UpdateCustomerAsync(id, dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/accounts")]
        public async Task<ActionResult<IEnumerable<AccountDetailsDTO>>> GetAccounts(int id)
        {
            try
            {
                var accounts = await _service.GetAccountsByCustomerIdAsync(id);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }

}
