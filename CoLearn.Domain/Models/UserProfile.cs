using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class UserProfile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public string? Address { get; set; }

    public string? ExtraJson { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
