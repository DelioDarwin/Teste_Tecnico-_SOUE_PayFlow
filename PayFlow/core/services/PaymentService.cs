using PayFlow.Core.Interfaces;
using PayFlow.Core.Models;

namespace PayFlow.Core.Services
{
    public class PaymentService
    {
        private readonly PaymentProviderFactory _providerFactory;

        public PaymentService(PaymentProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            IPaymentProvider provider = null;
            PaymentResponse response = null;
            List<string> triedProviders = new();

            // Try the preferred provider first
            try
            {
                provider = _providerFactory.GetProvider(request);
                triedProviders.Add(provider.ProviderName);
                response = await provider.ProcessPaymentAsync(request);
                return response;
            }
            catch (Exception)
            {
                // If the preferred provider fails, try the alternative
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

                throw; // Rethrow if no provider is available
            }
        }
    }
}