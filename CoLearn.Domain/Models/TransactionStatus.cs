using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class TransactionStatus
{
    public int TransactionStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
