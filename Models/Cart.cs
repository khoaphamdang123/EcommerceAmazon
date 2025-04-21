using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Cart
{
    public int Id { get; set; }

    public string Userid { get; set; } = null!;

    public string? Createddate { get; set; }

    public string? Updateddate { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual AspNetUser User { get; set; } = null!;
}
