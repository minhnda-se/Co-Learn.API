using CoLearn.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoLearn.API.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("payment/{paymentId}")]
        public async Task<IActionResult> GetTransactionByPaymentId(long paymentId)
        {
            var resutl = await _transactionService.GetByPaymentIdAsync(paymentId);
            return Ok(resutl);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTransactionByUserId(int userId)
        {
            var resutl = await _transactionService.GetByUserIdAsync(userId);
            return Ok(resutl);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var resutl = await _transactionService.GetAllTransactionsAsync();
            return Ok(resutl);
        }
    }
}
