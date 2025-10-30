using Microsoft.Extensions.Configuration;
using PayFlow.Core.Interfaces;
using PayFlow.Core.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class SecurePayProvider : IPaymentProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;
    public string ProviderName => "SecurePay";  

    public SecurePayProvider(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _endpointUrl = config["ProviderUrls:SecurePay"];
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        // Map PaymentRequest to SecurePayPayload
        var payload = new SecurePayPayload
        {
            amount_cents = (int)(request.Amount * 100),
            currency_code = request.Currency,
            client_reference = request.Reference ?? $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}"
        };

        // Enviar requisição para SecurePay API (endpoint configurável)
        var response = await _httpClient.PostAsJsonAsync(_endpointUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("O serviço SecurePay está indisponível.");
        }

        var securePayResponse = await response.Content.ReadFromJsonAsync<SecurePayResponse>();

        // Calcular taxa: 2,99% + 0,40 fixo
        var fee = decimal.Round(request.Amount * 0.0299m + 0.40m, 2);
        var netAmount = decimal.Round(request.Amount - fee, 2);

        return new PaymentResponse
        {
            ExternalId = securePayResponse?.transaction_id ?? "",
            Status = securePayResponse?.result == "success" ? "aprovado" : "recusado",
            Provider = ProviderName,
            GrossAmount = request.Amount,
            Fee = fee,
            NetAmount = netAmount,
            StatusDetail = securePayResponse?.result ?? string.Empty
        };
    }
}