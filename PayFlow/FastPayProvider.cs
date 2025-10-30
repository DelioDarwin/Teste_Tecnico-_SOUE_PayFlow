using Microsoft.Extensions.Configuration;
using PayFlow.Core.Interfaces;
using PayFlow.Core.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class FastPayProvider : IPaymentProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;
    public string ProviderName => "FastPay";

    public FastPayProvider(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _endpointUrl = config["ProviderUrls:FastPay"];
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        // Map PaymentRequest to FastPayPayload
        var payload = new FastPayPayload
        {
            transaction_amount = request.Amount,
            currency = request.Currency,
            payer = new Payer { email = request.PayerEmail ?? "cliente@teste.com" },
            installments = 1,
            description = "Compra via FastPay"
        };

        // Enviar requisição para FastPay API (substitua pelo endpoint real)
        var response = await _httpClient.PostAsJsonAsync(_endpointUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Serviço FastPay indisponível.");
        }

        var fastPayResponse = await response.Content.ReadFromJsonAsync<FastPayResponse>();

        // Calcular taxa: 3,49% do valor
        var fee = decimal.Round(request.Amount * 0.0349m, 2);
        var netAmount = decimal.Round(request.Amount - fee, 2);

        return new PaymentResponse
        {
            ExternalId = fastPayResponse?.id ?? "",
            Status = fastPayResponse?.status ?? "error",
            Provider = ProviderName,
            GrossAmount = request.Amount,
            Fee = fee,
            NetAmount = netAmount,
            StatusDetail = fastPayResponse?.status_detail ?? string.Empty
        };
    }
}