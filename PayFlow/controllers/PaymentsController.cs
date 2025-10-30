using Microsoft.AspNetCore.Mvc;
using PayFlow.Core.Models;
using PayFlow.Core.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PayFlow.Core.Data;

namespace PayFlow.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Currency))
            {
                return BadRequest(new { error = "Invalid payment request." });
            }

            try
            {
                var response = await _paymentService.ProcessPaymentAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(502, new { error = "Payment processing failed.", details = ex.Message });
            }
        }
    }
}