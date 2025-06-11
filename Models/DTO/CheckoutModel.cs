namespace Ecommerce_Product.Models
{
    public class CheckoutModel
{
   public string UserName{get;set;}
   
   public string Address1{get;set;}
   
   public string Address2{get;set;}

   public string Email { get; set; }
   
   public string PhoneNumber{get;set;}

   public string PaymentMethod{get;set;}
   
   public string ZipCode { get; set; }

   public string Country { get; set; }

   public string State{ get; set; }

   public string City{ get; set; }

   public string Note { get; set; }

 }

public class CheckoutItemModel
{
  public CheckoutModel Checkout{get;set;}

  public string Total_Price{get;set;}

  public string OrderId{get;set;}
}
}