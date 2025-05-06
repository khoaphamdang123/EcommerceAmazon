using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class AddProductModel
{   public int Id{get;set;}
    public string? ProductName{get;set;}

    public string? Price{get;set;}

    public int Quantity{get;set;}
    
    public int  Category{get;set;}
    
    public int Brand{get;set;}

    public string? Status{get;set;}

    public string? Description{get;set;}

    public string? Small_Description{get;set;}
    
    public int Discount{get;set;}

    public List<IFormFile>? ImageFiles{get;set;}

    public List<IFormFile>? VariantFiles{get;set;}

    public List<string>? Color{get;set;}

    public List<string>? Size{get;set;}

    public List<string>? Prices{get;set;}
}