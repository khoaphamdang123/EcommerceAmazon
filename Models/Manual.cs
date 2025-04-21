using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Manual
{
    public int Id { get; set; }

    public string ManualName { get; set; } = null!;

    public string Language { get; set; } = null!;

    public string ManualLink { get; set; } = null!;

    public string CreatedDate { get; set; } = null!;

    public string UpdatedDate { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}
