using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class ManualModel
{
    public int Id { get; set; }

    public string ManualName { get; set; } = null!;

    public string Language { get; set; } = null!;

    public IFormFile PdfLink { get; set; } = null!;

    public IFormFile WebLink {get;set;}=null!; 

    public string CreatedDate { get; set; } = null!;

    public string UpdatedDate { get; set; } = null!;

    public int ProductId { get; set; }
}
