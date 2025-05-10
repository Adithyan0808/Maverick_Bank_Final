// Controllers/LoanController.cs
using MaverickBank.Interfaces;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaverickBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowReactApp")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        // Endpoint to apply for a loan
        [HttpPost("apply")]
        public async Task<ActionResult<LoanApplicationResponseDTO>> ApplyForLoan([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                var loanResponse = await _loanService.ApplyForLoanAsync(loanApplicationDTO);
                return Ok(loanResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to get loans by customer ID
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponseDTO>>> GetLoansByCustomerId(int customerId)
        {
            var loans = await _loanService.GetLoansByCustomerIdAsync(customerId);
            return Ok(loans);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LoanApplicationResponseDTO>>> GetAllLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

      
        [HttpPut("{loanId}/status")]
        public async Task<ActionResult<LoanApplicationResponseDTO>> UpdateLoanStatus(int loanId, [FromBody] UpdateLoanStatusDTO dto)
        {
            var result = await _loanService.UpdateLoanStatusAsync(loanId, dto.NewStatus);
            return Ok(result);
        }

        [HttpGet("loanmasters")]
        public async Task<ActionResult<LoanDropdownDto>> GetLoanMasters()
        {
            try
            {
                var result = await _loanService.GetAllLoanMastersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }
}
