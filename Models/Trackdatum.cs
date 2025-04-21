using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Trackdatum
{
    public int Id { get; set; }

    public string Trackname { get; set; } = null!;

    public int? Totalcount { get; set; }
}
