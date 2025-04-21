using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Ratingdetail
{
    public int Id { get; set; }

    public int Rating { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public string CreatedDate { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
