using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class Parent
{
    public int ParentId { get; set; }

    public int UserId { get; set; }

    public string? Relationship { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual User User { get; set; } = null!;
}
