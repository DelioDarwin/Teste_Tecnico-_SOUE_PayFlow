public class FastPayPayload
{
    public decimal transaction_amount { get; set; }
    public string currency { get; set; }
    public Payer payer { get; set; }
    public int installments { get; set; }
    public string description { get; set; }
}

public class Payer
{
    public string? email { get; set; }
}