using Microsoft.AspNetCore.Mvc;
using PayFlow.Core.Models;
using PayFlow.Core.Data;
using System.Threading.Tasks;

namespace PayFlow.Controllers
{
    [ApiController]
    [Route("securepay")]
    public class SecurePayController : ControllerBase
    {
        private readonly PayFlowDbContext _dbContext;

        public SecurePayController(PayFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("payments")]
        public async Task<IActionResult> ProcessPayment([FromBody] SecurePayPayload payload)
        {
            // Simula o processamento do SecurePay e retorna uma resposta mock
            var response = new SecurePayResponse
            {
                transaction_id = $"SP-{new Random().Next(10000, 99999)}",
                result = "success"
            };

            // Calcula valores para persistência
            var grossAmount = payload.amount_cents / 100m;
            var fee = decimal.Round(grossAmount * 0.0299m + 0.40m, 2);
            var netAmount = decimal.Round(grossAmount - fee, 2);

            // Cria e salva o registro no banco
            var payment = new PaymentResponse
            {
                ExternalId = response.transaction_id,
                Status = response.result == "success" ? "aprovado" : "recusado",
                Provider = "SecurePay",
                GrossAmount = grossAmount,
                Fee = fee,
                NetAmount = netAmount,
                StatusDetail = response.result
            };

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            // Retorna exatamente o formato esperado
            return Ok(response);
        }
    }
}