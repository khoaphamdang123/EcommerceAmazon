using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Product.Models;

public class ChangePassword
{
   public string Email{get;set;}
   public string Password{get;set;}
   public string New_Password{get;set;}
 
public ChangePassword() { }

public ChangePassword(string email,string password,string new_password)
{
    this.Email=email;
    this.Password=password;
    this.New_Password=new_password;
}

}