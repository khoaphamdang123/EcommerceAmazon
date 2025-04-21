using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Product.Models;

public class FilterUser
{
   public string? Email{get;set;}
    public string? UserName{get;set;}
    public string? PhoneNumber{get;set;}

    public string? DateTime{get;set;}

    public string? EndTime{get;set;}
   
   public FilterUser()
   {
    
   }
    public FilterUser(string username,string email,string phonenumber,string datetime,string endtime)
    {
        this.UserName=username;
        this.Email=email;
        this.PhoneNumber=phonenumber;
        this.DateTime=datetime;
        this.EndTime=endtime;
    }
    
}