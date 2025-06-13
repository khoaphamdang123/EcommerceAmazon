
using Ecommerce_Product.Repository;
using Ecommerce_Product.Support_Serive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Implementation;
using Newtonsoft.Json;
namespace Ecommerce_Product.Controllers;
public class HomePageController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

 private readonly ICategoryListRepository _category;

 private readonly ISettingRepository _setting;

 private readonly IBlogRepository _blog;
 private readonly ILogger<HomePageController> _logger;
 

 private readonly Support_Serive.Service _sp_services;

 private readonly FirebaseService _firebase_service;
 
 private readonly IStaticFilesRepository _staticFile;
 


 

public HomePageController(IBannerListRepository banner, IProductRepository product, Support_Serive.Service sp_service, ISettingRepository setting, ICategoryListRepository category, IBlogRepository blog, IUserListRepository user,IStaticFilesRepository staticFile,FirebaseService firebase_service, ILogger<HomePageController> logger) : base(category, user,staticFile,banner)
  {
    this._banner = banner;
    this._product = product;
    this._blog = blog;
    this._category = category;
    this._setting = setting;
    this._logger = logger;
    this._firebase_service = firebase_service;
    this._sp_services = sp_service;
  }

[HttpGet]
[Route("")]
[Route("home")]
public async Task<IActionResult> HomePage()
{  try
{
    var banners= await this._banner.findBannerByName("Home");

    DateTime startTime=DateTime.Now;
    
    var products = await this._product.getAllProductList();
  
    var prominent_products = await this._product.getAllProminentProductList();
        
    DateTime endTime=DateTime.Now;
    
    int secons=endTime.Second-startTime.Second;
    
    Console.WriteLine("Time taken to get all products is:"+secons);
    
    startTime=DateTime.Now;
    
    var categories = await this._category.getAllCategory();
    
    endTime=DateTime.Now;
       
    secons=endTime.Second-startTime.Second;

    Console.WriteLine("Time taken to get all cat is:"+secons);
    
    startTime=DateTime.Now;
    
    var brands = await this._category.getAllBrandList();
    
    endTime=DateTime.Now;
    
    secons=endTime.Second-startTime.Second;
    
    Console.WriteLine("Time taken to get all brands is:"+secons);
 
    startTime=DateTime.Now;
  
   Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList()); 
  
   endTime=DateTime.Now;
  
   secons=endTime.Second-startTime.Second;
  
  // foreach(var item in products)
  // {
  //   item.Price=this._sp_services.convertToVND(item.Price);    
  // }
    Console.WriteLine("Time taken to get all reviews is:"+secons);
    
    var blogs= await this._blog.getAllBlog();
    
    var slider_content=await this._setting.getContentByName("homepage");

    ViewBag.slider_content=slider_content;    

    ViewBag.count_reviews=count_reviews;
    
    ViewBag.banners=banners;

    ViewBag.products = products;

    ViewBag.prominent_products=prominent_products;

    ViewBag.blogs=blogs;
        
    ViewBag.brands=brands; 

    Console.WriteLine("Banner count is:"+banners.Count().ToString());

    Console.WriteLine("Brand count is:"+brands.Count().ToString());

}  
catch(Exception er)
{
  Console.WriteLine("HomePage Exception:"+er.Message);
}

    return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
}



[HttpGet]
[Route("products/{id}s/variant")]
public async Task<JsonResult> VariantProduct(int id)
{ Console.WriteLine("used to come to this place:"+id);
  
  var variants=await this._product.getVariantByProductId(id);

  if(variants.Count>0)
  {
  string json=JsonConvert.SerializeObject(variants,new JsonSerializerSettings
    {
        ReferenceLoopHandling=ReferenceLoopHandling.Ignore
    });
  
  return Json(new{status=1,message="Get list of variant successfully",variant=json});
  }

  return Json(new{status=0,message="Get list of variant failed"});
}

[Route("product_detail/{id}")]

[HttpGet]

public async Task<JsonResult> productDetailInfo(int id)
{
  try
  {
    Console.WriteLine("Get product detail id is:"+id);

    var product = await this._product.findProductById(id);

    if(product!=null)
{
var settings = new JsonSerializerSettings
{
    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
    Formatting = Formatting.Indented
};
    var product_json=JsonConvert.SerializeObject(product,settings);
    
    Console.WriteLine("Product json is:"+product_json);
    
    return Json(new {status=1,message="Get product detail success",product=JsonConvert.SerializeObject(product,settings)});      
    }
  }
  catch(Exception er)
  {
    this._logger.LogError("Get Product detail exception:"+er.Message);
    Console.WriteLine("Product detail exception:"+er.Message);
  }
  return Json(new {status=0,message="Get product detail fail"});
}

[Route("/firebase_token")]
[HttpPost]
public async Task<JsonResult> FirebaseToken(string token)
{  
  Console.WriteLine("Token firebase is:"+token);
   var settings = await this._setting.getSettingObjByName("Firebase");
   if(settings!=null)
   {
    var setting_status = settings.Status;
    
    if(setting_status==1)
    {
      var firebase_message = settings.Firebase_Mess;
      await this._firebase_service.sendFirebaseMessage(token,"Notification",firebase_message);
      return Json(new {status=1,message="Send Firebase Message Success."}); 
    }
    
   }  
   return Json(new {status=0,message="Firebase message not active"});
}
}
