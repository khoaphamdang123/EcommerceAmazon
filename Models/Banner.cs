using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string Bannername { get; set; } = null!;

    public string Image { get; set; } = null!;

    public DateTime? Createddate { get; set; }

    public string? Updateddate { get; set; }
}
