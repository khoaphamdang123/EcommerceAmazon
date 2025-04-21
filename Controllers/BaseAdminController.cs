using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Ecommerce_Product.Models;
using Ecommerce_Product.Support_Serive;

public class BaseAdminController : Controller
{

    private readonly IBannerListRepository _banner;

    public BaseAdminController(IBannerListRepository banner)
    {
     
        this._banner=banner;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
     
        var logo= await this._banner.findBannerByName("logo");

        ViewBag.Logo = logo;
    
    await next();
    }
}