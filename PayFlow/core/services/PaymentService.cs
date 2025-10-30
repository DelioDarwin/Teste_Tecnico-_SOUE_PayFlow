using PayFlow.Core.Data;
using Microsoft.EntityFrameworkCore;
using PayFlow.Core.Interfaces;
using PayFlow.Core.Models;

namespace PayFlow.Core.Services;
public class PaymentService
{
    private readonly PaymentProviderFactory _providerFactory;
    private readonly PayFlowDbContext _dbContext;

    public PaymentService(PaymentProviderFactory providerFactory, PayFlowDbContext dbContext)
    {
        _providerFactory = providerFactory;
        _dbContext = dbContext;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        IPaymentProvider? provider = null;
        PaymentResponse? response = null;
        var triedProviders = new List<string>(); 
        try
        {
            provider = _providerFactory.GetProvider(request);
            triedProviders.Add(provider.ProviderName);
            response = await provider.ProcessPaymentAsync(request);
            return response;
        }
        catch (Exception)
        {
            var allProviders = _providerFactory
                .GetType()
                .GetField("_providers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(_providerFactory) as IEnumerable<IPaymentProvider>;

            if (allProviders != null)
            {   
                var alternative = allProviders.FirstOrDefault(p => !triedProviders.Contains(p.ProviderName));
                if (alternative != null)
                {
                    response = await alternative.ProcessPaymentAsync(request);
                    return response;
                }
            }

            throw; 
        }
    }

    public async Task<List<PaymentResponse>> GetAllPaymentsAsync()
    {
        return await _dbContext.Payments.ToListAsync();
    }
}