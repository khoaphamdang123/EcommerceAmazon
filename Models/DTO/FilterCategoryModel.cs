using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class FilterCategory
{
  public string? Category{get;set;}

  public string? StartDate{get;set;}
  
  public string? EndDate{get;set;}
  
  public FilterCategory()
  {
    
  }
    public FilterCategory(string category,string start_date,string end_date)
    {
       this.Category=category;
       this.StartDate=start_date;
       this.EndDate=end_date;
    }
    
}