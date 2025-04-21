using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Size
{
    public int Id { get; set; }

    public string Sizename { get; set; } = null!;

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();
}
