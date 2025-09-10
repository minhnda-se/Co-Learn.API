using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class FileUpload
{
    public long FileId { get; set; }

    public int? OwnerUserId { get; set; }

    public string? RelatedType { get; set; }

    public int? RelatedId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string? ContentType { get; set; }

    public long? SizeBytes { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? OwnerUser { get; set; }
}
