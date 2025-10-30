using PayFlow.Core.Interfaces;
using PayFlow.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace PayFlow;

public class PaymentProviderFactory
{
    private readonly IEnumerable<IPaymentProvider> _providers;

    public PaymentProviderFactory(IEnumerable<IPaymentProvider> providers)
    {
        _providers = providers;
    }

    public IPaymentProvider GetProvider(PaymentRequest request)
    {
        var providersList = _providers.ToList();
        System.Console.WriteLine($"[DEBUG] Amount: {request.amount}");
        System.Console.WriteLine($"[DEBUG] Providers: {string.Join(", ", providersList.Select(p => p.ProviderName))}");

        if (request.amount < 100)
        {
            System.Console.WriteLine("[DEBUG] Selecionado: FastPay");
            return providersList.First(p => p.ProviderName == "FastPay");
        }
        else
        {
            System.Console.WriteLine("[DEBUG] Selecionado: SecurePay");
            return providersList.First(p => p.ProviderName == "SecurePay");
        }
    }
}