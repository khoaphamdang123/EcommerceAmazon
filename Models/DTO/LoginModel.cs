using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class LoginModel
{
    [Required]
   public string UserName{get;set;}
   
   [Required]
    public string Password{get;set;}

    public bool RememberMe{get;set;}
}