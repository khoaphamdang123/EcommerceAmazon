using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class Register
{
   public string Email{get;set;}
    
   public string UserName{get;set;}
    
   public string PhoneNumber{get;set;}

   public string Password {get;set;}

   public string Address1{get;set;}

   public string Address2{get;set;}
   
   public string Gender{get;set;}   

   public IFormFile Avatar{get;set;}
    
}