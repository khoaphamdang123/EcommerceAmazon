using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class ForgotPassword
{
    [Required]
   public string Email{get;set;}
   public string Receiver{get;set;}
}