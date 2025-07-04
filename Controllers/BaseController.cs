using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Ecommerce_Product.Models;
using Ecommerce_Product.Support_Serive;

public class BaseController : Controller
{
    private readonly ICategoryListRepository _category;

    private readonly IUserListRepository _user;

    private readonly IBannerListRepository _banner;

    private readonly IStaticFilesRepository _staticFile;

    public BaseController(ICategoryListRepository category, IUserListRepository user, IStaticFilesRepository staticFile, IBannerListRepository banner)
    {
        this._category = category;
        this._user = user;
        this._banner = banner;
        this._staticFile = staticFile;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var categories = await this._category.getAllCategory();
        
        var user = await this._user.findUserByName("company");
        
        var staticFiles = await this._staticFile.getAllStaticFile();
    
        var logo = await this._banner.findBannerByName("logo");

        Environment.SetEnvironmentVariable("Logo", logo.ToList()[0].Image);
        
        ViewBag.Categories = categories;
        
        ViewBag.Company=user;
    
    var cart_json = this.HttpContext.Session.GetString("cart");
    
    var cart= cart_json != null ? JsonConvert.DeserializeObject<List<CartModel>>(cart_json) : new List<CartModel>();

    ViewBag.static_file = staticFiles.ToList();
    
    ViewBag.cart = cart;
        
    await next();
    }
}