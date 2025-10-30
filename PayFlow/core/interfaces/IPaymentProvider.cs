using PayFlow.Core.Models;

namespace PayFlow.Core.Interfaces;

public interface IPaymentProvider
{
    string ProviderName { get; }
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
}
