using Microsoft.AspNetCore.Mvc;
using PayFlow.Core.Models;
using PayFlow.Core.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PayFlow.Core.Data;

namespace PayFlow.Controllers;

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
        if (request == null || request.amount <= 0 || string.IsNullOrWhiteSpace(request.currency))
        {
            return BadRequest(new { error = "Erro ao processar o pagamento." });
        }

        try
        {
            var response = await _paymentService.ProcessPaymentAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { error = "Erro ao processar o pagamento.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }
}
