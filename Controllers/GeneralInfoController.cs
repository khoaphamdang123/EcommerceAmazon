using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Newtonsoft.Json;


namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class GeneralInfoController : BaseAdminController
{
    private readonly ILogger<GeneralInfoController> _logger;

   private readonly IUserListRepository _user;

   private readonly Support_Serive.Service _sp;

   public GeneralInfoController(IUserListRepository user,IBannerListRepository banner,Support_Serive.Service sp,ILogger<GeneralInfoController> logger):base(banner)
   {
    this._user=user;
    this._sp=sp;
    this._logger=logger;   
   }
  [Route("general_info")]
  [HttpGet]
  public async Task<IActionResult> GeneralInfo()
  {

    ApplicationUser user = null;


    try
    {
      user = await this._user.findUserByName("company");

      string extra_info = user.NormalizedEmail;

      Console.WriteLine("Extra Info:" + extra_info);

      var bank_list = this._sp.getListBank();


      if (bank_list != null)
      {
        var bank_data = bank_list.Data;

        ViewBag.bank_list = bank_data;
      }
    }
    catch (Exception er)
    {
      this._logger.LogError("Get Manual File List Exception:" + er.Message);
    }
    return View(user);
  }

  
  [Route("general_info/update")]
  
  [HttpPost]
  public async Task<JsonResult> updateUser(UserInfo user)
  {      Console.WriteLine("User info here is:"+JsonConvert.SerializeObject(user));

    int updated_res=0;
    try
    {
      Console.WriteLine("User info here is:"+user.ToString());

      string bank_name = user.BankName;

      string facebook = user.Facebook;
      
      string account_name=user.AccountName;

      Console.WriteLine("Bank Name:"+bank_name);
      
      Console.WriteLine("Facebook:"+facebook);
      
      Console.WriteLine("Account Name:"+account_name);
      
      Console.WriteLine("Google Key is:"+user.GoogleKey);

       updated_res =await this._user.updateUser(user);
    }
    catch(Exception ex)
    {
      this._logger.LogError("Update User Exception:"+ex.Message);
      updated_res=0;
      return Json(new {status=0,message=ex.Message});
    }
    if(updated_res==1)
    {
      return Json(new {status=1,message="Cập nhật thông tin thành công"});
    }
    else if(updated_res==-1)
    {
      return Json(new {status=-1,message="Cập nhật thông tin QR thất bại"});
    }
    else
    {
      return Json(new {status=0,message="Cập nhật thông tin thất bại"});
    }
  }
}
