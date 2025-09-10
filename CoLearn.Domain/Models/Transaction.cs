using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class Transaction
{
    public long TransactionId { get; set; }

    public long PaymentId { get; set; }

    public string GatewayTransactionCode { get; set; } = null!;

    public string? GatewayName { get; set; }

    public string? GatewayResponse { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
