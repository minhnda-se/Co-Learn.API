using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class Payment
{
    public long PaymentId { get; set; }

    public int? BookingId { get; set; }

    public int? EnrollmentId { get; set; }

    public int? PayerUserId { get; set; }

    public decimal Amount { get; set; }

    public int? MethodId { get; set; }

    public int StatusId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Enrollment? Enrollment { get; set; }

    public virtual PaymentMethod? Method { get; set; }

    public virtual TransactionStatus Status { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
