using Swashbuckle.AspNetCore.Annotations;

namespace PayFlow.Core.Models;

public class PaymentRequest
{
    public decimal amount { get; set; }
    public string currency { get; set; }
}