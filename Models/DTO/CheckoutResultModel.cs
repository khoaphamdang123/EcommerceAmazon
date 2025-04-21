namespace Ecommerce_Product.Models
{
    public class CheckoutResultModel
    {
   public Order Order{get;set;}
   public List<CartModel> Cart{get;set;}
    }
}