using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int Productid { get; set; }

    public int Orderid { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int? Discount { get; set; }

    public int? VariantId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Variant? Variant { get; set; }
}
