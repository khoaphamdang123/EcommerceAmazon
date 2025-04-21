using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public string Avatar { get; set; } = null!;

    public int? Productid { get; set; }

    public virtual Product? Product { get; set; }
}
