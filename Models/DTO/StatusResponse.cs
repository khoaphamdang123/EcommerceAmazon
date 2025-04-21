namespace Ecommerce_Product.Models;

public class StatusResponse
{
    public int Status { get; set; }
    
    public string Title {get;set;}
    public string Message { get; set; }

    public string SiteKey { get; set; }

    public ApplicationUser User{get;set;}
}