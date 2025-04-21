using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.Text;
using Ecommerce_Product.Support_Serive;


namespace Ecommerce_Product.Controllers;
[Authorize(Roles="Admin")]
[Route("admin")]
public class FirebaseController : BaseAdminController
{
   private readonly ILogger<FirebaseController> _logger;    

   private readonly FirebaseService _firebase_serive;

   private readonly ISettingRepository _setting;


   public FirebaseController(FirebaseService firebase_serivce,ISettingRepository setting,IBannerListRepository banner,ILogger<FirebaseController> logger):base(banner)
   {
  this._firebase_serive=firebase_serivce;
  this._setting=setting;
  this._logger=logger;   
   }
[Route("firebase")]

[HttpGet]
public async Task<IActionResult> Firebase()
{ 
try
{
string id_user=this.HttpContext.Session.GetString("AdminId");

if(string.IsNullOrEmpty(id_user))
{
  return RedirectToAction("Index","LoginAdmin");    
}
  var setting_obj=await this._setting.getSettingObjByName("Firebase");

  if(setting_obj!=null)
  {
    ViewBag.setting=setting_obj;
  }
}
catch(Exception er)
{
    this._logger.LogError("Get Firebase Setting Exception:"+er.Message);
}

 return View();
  }

[Route("firebase/update")]
[HttpPost]
public async Task<IActionResult> UpdateFirebaseSetting(FirebaseSettingModel setting)
{
  try
  { 
    Console.WriteLine("Firebase Mess here is:"+setting.Firebase_Mess);

    Console.WriteLine("Firebase Status:"+setting.Status);

    int updated_res=await this._setting.updateFirebaseSetting(setting);
       
    if(updated_res!=0)
    {
      ViewBag.Status=updated_res;
      ViewBag.Message="Cập nhật cấu hình cài đặt thành công";
      this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Updated Setting Successfully");
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Message="Cập nhật cấu hình cài đặt thất bại";
      this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Updated Setting Failed");
    }
   var setting_obj=await this._setting.getSettingObjByName("Firebase");

  if(setting_obj!=null)
  {
    ViewBag.setting=setting_obj;
  }
  }
  catch(Exception er)
  {
      this._logger.LogError("Update Firebase Setting Exception:"+er.Message);
  }
  return View("~/Views/Firebase/Firebase.cshtml");
    
}

}
