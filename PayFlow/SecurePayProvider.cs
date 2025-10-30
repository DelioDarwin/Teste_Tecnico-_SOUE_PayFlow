using Microsoft.Extensions.Configuration;
using PayFlow.Core.Interfaces;
using PayFlow.Core.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PayFlow;

public class SecurePayProvider : IPaymentProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;
    public string ProviderName => "SecurePay";  

    public SecurePayProvider(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _endpointUrl = config["ProviderUrls:SecurePay"] ?? throw new ArgumentNullException(nameof(config), "SecurePay endpoint URL configuration is missing.");
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        // Map PaymentRequest to SecurePayPayload
        var payload = new SecurePayPayload
        {
            amount_cents = (int)(request.amount * 100),
            currency_code = request.currency,
            client_reference = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}"
        };

        // Enviar requisição para SecurePay API (endpoint configurável)
        var response = await _httpClient.PostAsJsonAsync(_endpointUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("O serviço SecurePay está indisponível.");
        }

        var securePayResponse = await response.Content.ReadFromJsonAsync<SecurePayResponse>();

        // Calcular taxa: 2,99% + 0,40 fixo
        var fee = decimal.Round(request.amount * 0.0299m + 0.40m, 2);
        var netAmount = decimal.Round(request.amount - fee, 2);
            
        return new PaymentResponse
        {
            ExternalId = securePayResponse?.transaction_id ?? "",
            Status = securePayResponse?.result == "success" ? "aprovado" : "recusado",
            Provider = ProviderName,
            GrossAmount = request.amount,
            Fee = fee,
            NetAmount = netAmount,
            StatusDetail = securePayResponse?.result ?? string.Empty
        };
    }
}