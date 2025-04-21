
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce_Product.Controllers;
public class ManualController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IManualRepository _manual;

 private readonly ICategoryListRepository _category;

 private readonly ISettingRepository _setting;

 private readonly IVideoRepository _video;

 private readonly ILogger<ManualController> _logger;
 

 private readonly Support_Serive.Service _sp_services;
 

public ManualController(IBannerListRepository banner,IManualRepository manual,IVideoRepository video,Support_Serive.Service sp_service,ISettingRepository setting,ICategoryListRepository category,IUserListRepository user ,ILogger<ManualController> logger):base(category,user,banner)
{
    this._banner=banner;
    this._manual=manual;
    this._video=video;
    this._category=category;
    this._setting=setting;
    this._logger=logger;
    this._sp_services=sp_service;
}

// public IActionResult HomePage()
// {
//   return View();
// }



[HttpGet]
[Route("manual/{id}")]
public async Task<IActionResult> Manual(int id)
{   
Console.WriteLine("Did come to manual");

var manuals= await this._manual.findManualByProductId(id);

var videos = await this._video.findVideoByProductId(id);

ViewBag.videos=videos;

return View("~/Views/ClientSide/Manual/Manual.cshtml",manuals);
}

}