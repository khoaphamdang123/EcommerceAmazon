namespace Ecommerce_Product.Models
{
 public class ProductAmazon
 {
  public string? Name{get;set;}

  public ProductInfor? Product_Information{get;set;}

  public string? Full_Description{get;set;}

  public string? Pricing{get;set;}
  
  public List<string>? Images{get;set;}

  public string? Small_Description{get;set;}

public Customization? Customization_Options{get;set;}

 } 

 public class ProductInfor
 {
    public string? Package_Dimensions{get;set;}

    public string? Item_Model_Number{get;set;}

    public string? Date_First_Available{get;set;}

    public string? Manufacturer{get;set;}

    public string? Asin{get;set;}    

 }

 public class Customization
 {
   public List<Size_Value>? Size{get;set;}
 }

 public class Size_Value
 {  
    public string? Asin{get;set;}
    public string? Value{get;set;}
 }
}