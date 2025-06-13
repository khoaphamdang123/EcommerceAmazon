
using System.Text.RegularExpressions;
using System.Web;
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace Ecommerce_Product.Controllers;
public class StaticPageController:BaseController
{


 private readonly ICategoryListRepository _category;

 private readonly IStaticFilesRepository _static_files;

 private readonly IUserListRepository _user;

 private readonly ILogger<StaticPageController> _logger;


public StaticPageController(ICategoryListRepository category,IBannerListRepository banner,IStaticFilesRepository static_files,IUserListRepository user,ILogger<StaticPageController> logger):base(category,user,static_files,banner)
{
   
    this._category=category;
    this._user=user;
    this._static_files=static_files;
    this._logger=logger;
}


[HttpGet]
[Route("{page_name}")]

public async Task<IActionResult> StaticPage(string page_name)
{   
    
   var static_file=await this._static_files.findStaticFileByName(page_name);
   
   if(static_file==null)
   {
    return NotFound();    
   }
   string content=HttpUtility.HtmlDecode(static_file.Content); 
   Console.WriteLine("Content iss:"+content);
   Regex reg= new Regex(@"\s*(<[^>]+>)\s*");
    content=reg.Replace(content,"$1");
    ViewBag.content = content;    
    return View("~/Views/ClientSide/StaticPage/StaticPage.cshtml");
}

[HttpGet]
[Route("about-us")]
public async Task<IActionResult> AboutUs()
{   var categories=await this._category.getAllCategory();
    var company=await this._user.findUserByName("company");
    ViewBag.company=company;
    ViewBag.categories=categories;
    return View("~/Views/ClientSide/StaticPage/AboutPage.cshtml");
}
}