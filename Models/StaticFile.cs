using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class StaticFile
{
    public int Id { get; set; }

    public string? Filename { get; set; } = null!;

    public string? Section { get; set; } = null!;

    public string? Content { get; set; } = null!;

    public string? Createddate { get; set; } = null!;

    public string? Updateddate { get; set; } = null!;
}
