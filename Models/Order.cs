using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Order
{
    public int Id { get; set; }

    public string Userid { get; set; } = null!;

    public string Createddate { get; set; } = null!;

    public decimal Total { get; set; }

    public string Status { get; set; } = null!;

    public string Shippingaddress { get; set; } = null!;

    public int Paymentid { get; set; }

    public string? OrderId { get; set; }

    /// <summary>
    /// Note for Order
    /// </summary>
    public string? Note { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Payment Payment { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
