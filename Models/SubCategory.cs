using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class SubCategory
{
    public int Id { get; set; }

    public string? SubCategoryName { get; set; }

    public int? CategoryId { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
