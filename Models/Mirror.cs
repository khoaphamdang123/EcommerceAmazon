using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Mirror
{
    public int Id { get; set; }

    public string Mirrorname { get; set; } = null!;

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
