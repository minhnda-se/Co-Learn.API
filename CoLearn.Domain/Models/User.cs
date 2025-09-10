using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[]? PasswordHash { get; set; }

    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public int? PrimaryRoleId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AuditTrail> AuditTrails { get; set; } = new List<AuditTrail>();

    public virtual ICollection<FileUpload> FileUploads { get; set; } = new List<FileUpload>();

    public virtual Parent? Parent { get; set; }

    public virtual Role? PrimaryRole { get; set; }

    public virtual Student? Student { get; set; }

    public virtual ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();

    public virtual Teacher? Teacher { get; set; }

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
