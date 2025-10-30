public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string? PayerEmail { get; set; } // Opcional, se quiser enviar
    public string? Reference { get; set; }  // Opcional, se quiser enviar
}