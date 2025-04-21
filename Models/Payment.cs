using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string Paymentname { get; set; } = null!;

    public string? Createddate { get; set; }

    public string? Updateddate { get; set; }

    public short? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
