namespace Ecommerce_Product.Models
{
    public class CartModel
    {
   public Product Product { get; set; }
    
   public int Quantity { get; set; }

   public string Size { get; set; }

   public string Color { get; set; }

   public string Price {get;set;} 
    }
}