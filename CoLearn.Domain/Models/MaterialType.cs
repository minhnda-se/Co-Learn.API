using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class MaterialType
{
    public int MaterialTypeId { get; set; }

    public string TypeName { get; set; } = null!;
}
