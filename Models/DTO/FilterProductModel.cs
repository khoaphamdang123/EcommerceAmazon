namespace Ecommerce_Product.Models;

public class FilterProduct
{
  public string? ProductName{get;set;}

  public string? StartDate{get;set;}
  
  public string? EndDate{get;set;}

  public string? Category{get;set;}

  public string? Brand{get;set;}

  public string? Status{get;set;}
   
   public FilterProduct()
   {
   }
    public FilterProduct(string productname,string start_date,string end_date,string category,string brand,string status)
    {
       this.ProductName=productname;
       this.StartDate=start_date;
       this.EndDate=end_date;
       this.Category=category;
       this.Brand=brand;
       this.Status=status;
    }
    
}