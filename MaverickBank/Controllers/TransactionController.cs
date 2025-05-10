using MaverickBank.Interfaces;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaverickBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowReactApp")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TransactionResponseDTO>> MakeTransaction(TransactionRequestDTO request)
        {
            try
            {
                var result = await _transactionService.MakeTransaction(request);
                return Ok(result);
            }
            catch (MaverickBank.Exceptions.InsufficientBalanceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
         [Authorize]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetAll()
        {
            var results = await _transactionService.GetAllTransactions();
            return Ok(results);
        }

        [HttpGet("{id}")]
         [Authorize]
        public async Task<ActionResult<TransactionResponseDTO>> GetById(int id)
        {
            var result = await _transactionService.GetTransactionById(id);
            return Ok(result);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetTransactionsByCustomerId(int customerId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByCustomerId(customerId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}/filter")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetTransactionsByCustomerIdWithFilters(
    int customerId,
    [FromQuery] int? transactionTypeId,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate)
        {
            try
            {
                var transactions = await _transactionService
                    .GetTransactionsByCustomerIdWithFilters(customerId, transactionTypeId, fromDate, toDate);

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }


        
        [HttpGet("customer/{customerId}/recent")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetRecentTransactions(int customerId)
        {
            try
            {
                var transactions = await _transactionService.GetRecentTransactionsByCustomerId(customerId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }



    }
}
