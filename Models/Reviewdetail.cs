using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Reviewdetail
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? UserId { get; set; }

    public string CreatedDate { get; set; } = null!;

    public string ReviewText { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual AspNetUser? User { get; set; }
}
