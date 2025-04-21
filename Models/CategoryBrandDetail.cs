using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class CategoryBrandDetail
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}
