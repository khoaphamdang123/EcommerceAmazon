
using Ecommerce_Product.Repository;
using Ecommerce_Product.Support_Serive;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ecommerce_Product.Models;
using Ecommerce_Product.Service;
namespace Ecommerce_Product.Controllers;
public class HomePageController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

private readonly IUserListRepository _user;


 private readonly ICategoryListRepository _category;

 private readonly ISettingRepository _setting;

 private readonly IBlogRepository _blog;

 private readonly ILogger<HomePageController> _logger;
 
 private readonly Support_Serive.Service _sp_services;

 private readonly FirebaseService _firebase_service;

  private readonly SmtpService _smtpService;

 
 private readonly IStaticFilesRepository _staticFile;
 

public HomePageController(IBannerListRepository banner, IProductRepository product, Support_Serive.Service sp_service,Support_Serive.SmtpService smtpService, ISettingRepository setting, ICategoryListRepository category, IBlogRepository blog, IUserListRepository user,IStaticFilesRepository staticFile,FirebaseService firebase_service, ILogger<HomePageController> logger) : base(category, user,staticFile,banner)
  {
    this._banner = banner;
    this._product = product;
    this._blog = blog;
    this._category = category;
    this._setting = setting;
    this._logger = logger;
    this._firebase_service = firebase_service;
    this._sp_services = sp_service;
    this._smtpService = smtpService;
    this._user = user;
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
      Console.WriteLine("Get product detail id is:" + id);

      var product = await this._product.findProductById(id);

      if (product != null)
      {
        var settings = new JsonSerializerSettings
        {
          PreserveReferencesHandling = PreserveReferencesHandling.Objects,
          Formatting = Formatting.Indented
        };
        var product_json = JsonConvert.SerializeObject(product, settings);

        Console.WriteLine("Product json is:" + product_json);

        return Json(new { status = 1, message = "Get product detail success", product = JsonConvert.SerializeObject(product, settings) });
      }
    }
    catch (Exception er)
    {
      this._logger.LogError("Get Product detail exception:" + er.Message);
      Console.WriteLine("Product detail exception:" + er.Message);
    }
    return Json(new { status = 0, message = "Get product detail fail" });
  }


  [Route("/confirm_email")]
  [HttpGet]

  public async Task<JsonResult> ConfirmEmail(string email)
  {
    try
    {
      Console.WriteLine("Email for confirm here is:" + email);

      if (string.IsNullOrEmpty(email))
      {
        return Json(new { status = 0, message = "Email is empty" });
      }


      string confirm_newsletter_url = Url.Action("Newsletter", "HomePage",new { email = email }, Request.Scheme);

      UserInfo receipt = new UserInfo { UserName = email, Email = email ,ConfirmNewsLetterUrl=confirm_newsletter_url};

      var render_view = new RazorViewRenderer();

      string mail_path = "MailTemplate/newsletter.cshtml";

      string render_string = await render_view.RenderViewToStringAsync(mail_path, receipt);

      Console.WriteLine("Render string here is:" + render_string);   

      bool is_sent = await this._smtpService.sendEmailGeneral(4, render_string,email);

      if (is_sent)
      {
        this._logger.LogInformation("Send confirm newsletter successfully");

        Console.WriteLine("Send confirm newsletter successfully");
      }
      else
      {
        this._logger.LogInformation("Send confirm newsletter failed");

        Console.WriteLine("Send confirm newsletter failed");


        return Json(new { status = 0, message = "Send confirm newsletter failed" });

      }

    }
    catch (Exception er)
    {
      this._logger.LogError("Confirm Email Exception:" + er.Message);

      Console.WriteLine("Confirm Email Exception:" + er.Message);

      return Json(new { status = 0, message = "Exception send newsletter:" + er.Message });

    }
    return Json(new { status = 1, message = "Send confirm newsletter successfully" });
  }


  [Route("/newsletter/{email}")]
  [HttpGet]
  public async Task<IActionResult> Newsletter(string email)
  {
    try
    {
      Console.WriteLine("Email for newsletter is:" + email);

      if (string.IsNullOrEmpty(email))
      {
        return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
      }
      string role = "Anonymous";

      var new_user = new Register { UserName = email, Email = email, Password = "123456", Address2 = email, PhoneNumber = "0123456789" };

      var create_user = await this._user.createUser(new_user, role);

      if (create_user == 1)
      {
        ViewBag.NewsLetterMessage = "Subscribe newsletter successfully";

        ViewBag.NewsLetterStatus = 1;
      }
      else
      {
        ViewBag.NewsLetterMessage = "Subscribe newsletter failed";

        ViewBag.NewsLetterStatus = 0;
      }
    }
    catch (Exception er)
    {
      this._logger.LogError("Newsletter Exception:" + er.Message);
    }  
   return View("~/Views/ClientSide/HomePage/HomePage.cshtml");
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
