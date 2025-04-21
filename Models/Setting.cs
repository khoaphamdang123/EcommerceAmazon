using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string Settingname { get; set; } = null!;

    public int Status { get; set; }

    public string? Createddate { get; set; }

    public string? Updateddate { get; set; }

    public string? App { get; set; }

    public string? Firebase_Mess{get;set;}

}
