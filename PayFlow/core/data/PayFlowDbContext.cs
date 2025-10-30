using Microsoft.EntityFrameworkCore;
using PayFlow.Core.Models;
using PayFlow.Core.Data;

namespace PayFlow.Core.Data
{
    public class PayFlowDbContext : DbContext
    {
        public PayFlowDbContext(DbContextOptions<PayFlowDbContext> options) : base(options) { }

        public DbSet<PaymentResponse> Payments { get; set; }
            
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentResponse>().Property(p => p.GrossAmount).HasPrecision(18, 4);
            modelBuilder.Entity<PaymentResponse>().Property(p => p.Fee).HasPrecision(18, 4);
            modelBuilder.Entity<PaymentResponse>().Property(p => p.NetAmount).HasPrecision(18, 4);

            modelBuilder.Entity<PaymentResponse>().HasData(
                new PaymentResponse
                {
                    Id = 1,
                    ExternalId = "FP-0001",
                    Status = "approved",
                    Provider = "FastPay",
                    GrossAmount = 50.00m,
                    Fee = 1.75m,
                    NetAmount = 48.25m,
                    StatusDetail = "Pagamento aprovado"
                },
                new PaymentResponse
                {
                    Id = 2,
                    ExternalId = "SP-0001",
                    Status = "approved",
                    Provider = "SecurePay",
                    GrossAmount = 150.00m,
                    Fee = 4.89m,
                    NetAmount = 145.11m,
                    StatusDetail = "Pagamento aprovado"
                }
            );
        }
    }
}

