using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using Org.BouncyCastle.Math.EC.Rfc8032;
using System.ComponentModel;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class BannerListController : BaseAdminController
{
    private readonly ILogger<BannerListController> _logger;


   private readonly IBannerListRepository _banner;


   
   public BannerListController(IBannerListRepository banner,ILogger<BannerListController> logger):base(banner)
   {
  this._banner=banner;
  this._logger=logger;   
   }
  //[Authorize(Roles ="Admin")]
  [Route("banners")]
  [HttpGet]
  public async Task<IActionResult> BannerList()
  {    
    try
    {  
        var banners=await this._banner.getAllBanner();
        return View(banners);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Static File List Exception:"+er.Message);
    }
    return View();
  }

  [Route("banners/delete")]
  public async Task<IActionResult> DeleteBanner(int id)
  {
      int delete_res=await this._banner.deleteBanner(id);
      TempData["Status"]=delete_res;
      if(delete_res!=0)
      {   
          TempData["Message"]="Xóa banner thành công";
      }
      else
      {
          TempData["Message"]="Xóa banner thất bại";
      }
    var banners=await this._banner.getAllBanner();

      return View("~/Views/BannerList/BannerList.cshtml",banners);
  }

  [Route("banners/add")]
  [HttpGet]
  public async Task<IActionResult> AddBanner()
  {
  return View();
  }

 [Route("banners/add")]
  [HttpPost]
  public async Task<IActionResult> AddBanner(BannerModel banner)
  {
  try{
      int created_res = 0;
      try
      {
         created_res = await this._banner.addBanner(banner);
      }
      catch(Exception er)
      {
        Console.WriteLine("Add Banner Exception:"+er.Message);
        this._logger.LogTrace("Add Banner Exception:"+er.Message);
      }
    ViewBag.Status=created_res;
    if(created_res==0)
    {
          this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Created Banner Failed");

      ViewBag.Created_Banner="Thêm banner thất bại";
    }
    else if(created_res==-1)
    {
      ViewBag.Created_Banner="Banner này đã tồn tại trong hệ thống";
    }
    else{
      this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Created Banner Successfully");
      ViewBag.Created_Banner="Thêm banner thành công";
    }
  }
  catch(Exception er)
  { Console.WriteLine("Add banner exception:"+er.Message);
    
    this._logger.LogTrace("Add Banner Exception:"+er.Message);
  }
  return View();
  }

  [Route("banner/{id}/info")]
  [HttpGet]
  public async Task<IActionResult> BannerInfo(int id)
  {
    var banner=await this._banner.findBannerById(id);
    return View(banner);
  }

    [Route("banner/{id}/info")]
    
    [HttpPost]

    public async Task<IActionResult> BannerInfo(int id,BannerModel banner)
    {
    int update_res = 0;
    try
    {
      update_res = await this._banner.updateBanner(id, banner);
    }
    catch (Exception er)
    {
        Console.WriteLine("Update Banner Exception:" + er.Message);
        this._logger.LogTrace("Update Banner Exception:" + er.Message);
    }

        ViewBag.Status=update_res;
        if(update_res==0)
        {
            this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Updated Banner Failed");
            ViewBag.Update_Banner="Cập nhật banner thất bại";
        }
        else
        {  this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Updated Banner Successfully");
            ViewBag.Update_Banner="Cập nhật banner thành công";
        }
        var banner_ob=await this._banner.findBannerById(id);
       
        return View(banner_ob);
    }

}
