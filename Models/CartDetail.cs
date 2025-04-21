using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class CartDetail
{
    public int Id { get; set; }

    public int Productid { get; set; }

    public int Cartid { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public int? Discount { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
