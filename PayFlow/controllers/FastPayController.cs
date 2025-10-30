using Microsoft.AspNetCore.Mvc;
using PayFlow.Core.Models;
using PayFlow.Core.Data;
using System.Threading.Tasks;

namespace PayFlow.Controllers;

[ApiController]
[Route("fastpay")]
public class FastPayController : ControllerBase
{
    private readonly PayFlowDbContext _dbContext;

    public FastPayController(PayFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("payments")]
    public async Task<IActionResult> ProcessPayment([FromBody] FastPayPayload payload)
    {
        // Preenche valores padrão se vierem nulos
        payload.payer ??= new Payer("cliente@teste.com");
        if (payload.installments == 0) payload.installments = 1;
        payload.description ??= "Compra via FastPay";

        // Simula o processamento do FastPay e retorna uma resposta mock
        var response = new FastPayResponse
        {
            id = $"FP-{new Random().Next(100000, 999999)}",
            status = "approved",
            status_detail = "Pagamento aprovado"
        };

        // Calcula valores para persistência
        var grossAmount = payload.transaction_amount;
        var fee = decimal.Round(grossAmount * 0.0349m, 2);
        var netAmount = decimal.Round(grossAmount - fee, 2);

        // Cria e salva o registro no banco
        var payment = new PaymentResponse
        {
            ExternalId = response.id,
            Status = response.status == "approved" ? "aprovado" : "recusado",
            Provider = "FastPay",
            GrossAmount = grossAmount,
            Fee = fee,
            NetAmount = netAmount,
            StatusDetail = response.status_detail
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        return Ok(response);
    }
}
