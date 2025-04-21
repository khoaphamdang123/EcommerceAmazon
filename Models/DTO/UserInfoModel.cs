using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class UserInfo
{  

   public string Id{get;set;}
   
   public string Email{get;set;}
   
   public string UserName{get;set;}
   
   public string PhoneNumber{get;set;}
   
   public string Address1{get;set;}

   public string Address2{get;set;}
   
   public string Gender{get;set;}

   public string BankName{get;set;}

   public string AccountName{get;set;}

   public string AccountNum{get;set;}

   public string Facebook{get;set;}

   public string Zalo{get;set;}

   public string Youtube{get;set;}

   public string Instagram{get;set;}

   public string Telegram{get;set;}
   
   public IFormFile Avatar{get;set;}
    
}