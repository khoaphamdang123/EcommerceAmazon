using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Variant
{
    public int Id { get; set; }

    public int? Productid { get; set; }

    public int? Colorid { get; set; }

    public int? Sizeid { get; set; }

    public int? Versionid { get; set; }

    public int? Mirrorid { get; set; }

    public decimal? Weight { get; set; }

    public string? Price { get; set; }

    public virtual Color? Color { get; set; }

    public virtual Mirror? Mirror { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product? Product { get; set; }

    public virtual Size? Size { get; set; }

    public virtual Version? Version { get; set; }    
}
