using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class ApplicationUser:IdentityUser
{   
    public int? Seq{get;set;}
    public string? Address1{get;set;}
    public string? Address2{get;set;}
    public string? Gender{get;set;}
    public string? Created_Date{get;set;}
    public string? Avatar{get;set;}
}