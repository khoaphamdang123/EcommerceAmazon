using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.Extensions.Options;
using Ecommerce_Product.Support_Serive;
using PayPalCheckoutSdk.Orders;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Payments;




namespace Ecommerce_Product.Controllers;

public class CheckoutController : BaseController
{
    private readonly ILogger<CheckoutController> _logger;

    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

    private readonly Support_Serive.Service _sp;

    private readonly IUserListRepository _user;
    
    private readonly IOrderRepository _order;

    private readonly RecaptchaResponse _recaptcha_response;

    private readonly ISettingRepository _setting;

    private readonly SmtpService _smtpService;


    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IPaymentRepository _payment;

    private readonly IConfiguration _configuration;

    private readonly PaypalService _paypalService;

   private readonly ICartRepository _cart;
   public CheckoutController(ICartRepository cart,IProductRepository product,Support_Serive.Service sp,IBannerListRepository banner,SmtpService smtpService,IOrderRepository order,IOptions<RecaptchaResponse> recaptcha_response,ISettingRepository setting,IPaymentRepository payment,IUserListRepository user,ICategoryListRepository category,IConfiguration configuration,PaypalService paypalService,ILogger<CheckoutController> logger):base(category,user,banner)
  {
  this._cart=cart;
  this._sp=sp;
  this._smtpService=smtpService;
  this._category=category;
  this._setting=setting;
  this._recaptcha_response=recaptcha_response.Value;
  this._product=product;
  this._order=order;
  this._logger=logger; 
  this._payment=payment;  
  this._configuration=configuration;
  this._paypalService=paypalService;  
  this._user=user;
  }


 [Route("checkout")]
 [HttpGet]
 public IActionResult Checkout()
 {  
  
   if(string.IsNullOrEmpty(this.HttpContext.Session.GetString("UserId")))
   {
    this.HttpContext.Session.SetString("UserId",Guid.NewGuid().ToString());
   }

   return RedirectToAction("CheckoutCart","Checkout",new {id=this.HttpContext.Session.GetString("UserId")});

 }


 [Route("checkout/{id}")]
 [HttpGet]
 public async Task<IActionResult> CheckoutCart(string id)
 {   var cart=this._cart.getCart();
    try
    {
     if(cart==null || cart.Count==0)
     {
       return RedirectToAction("Cart","Cart");
     }
     string username=HttpContext.Session.GetString("UserName");

     var payment_methods=await this._payment.getAllPayment();
     
    ViewBag.payment_methods=payment_methods;

   int setting_status=await this._setting.getStatusByName("recaptcha");

       if(setting_status==1)
       {
        ViewBag.SiteKey=this._recaptcha_response.SiteKey;
       }
       
        bool is_saved_account=false;
        
        if(Request.Cookies["UserAccount"]!=null)
        {
            is_saved_account=true;
            string account=Request.Cookies["UserAccount"];
            Console.WriteLine("Account here is:"+account);
            ViewBag.Account=account;
            ViewBag.SavedAccount=is_saved_account;
        }
     var company = await this._user.findUserByName("company");

        ViewBag.company=company;

     Console.WriteLine("qr here");

      Console.WriteLine("QR ENV:"+Environment.GetEnvironmentVariable("qr_code"));
     if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("qr_code")))
     {
    string[] email_list=company.Email.Split('#');
    string email=email_list[0];    
    string extra_info=email_list[1];
    string bank_name="";
    string account_num="";
    string account_name="";
    string qr_code="";    

    if(!string.IsNullOrEmpty(extra_info))
    {
      string[] info_values=extra_info.Split('\n');
      foreach(var info in info_values)
      {
        if(info.Contains("bank_name"))
        {
          bank_name=info.Split('~')[1].Trim();
        }
        else if(info.Contains("account_name"))
        {
          account_name=info.Split('~')[1].Trim();
        }
        else if(info.Contains("account_num"))
        {
       account_num=info.Split('~')[1].Trim();       
        }
      }
    }
    qr_code=this._sp.generateQRCode(bank_name,account_num,account_name);
    Console.WriteLine("QRCODE:"+qr_code);
    if(!string.IsNullOrEmpty(qr_code) && qr_code!="ERROR")
    { this._logger.LogInformation("QR Code In Checkout did come here:"+qr_code);
    Console.WriteLine("QR Code In Checkout did come here:"+qr_code);
     Environment.SetEnvironmentVariable("qr_code",qr_code);
    }
  }
    
     if(string.IsNullOrEmpty(username))
     {
        return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart);
     }

     var user=await this._user.findUserByName(username);

     ViewBag.user=user;    
    }
    catch(Exception er)
    {   
        Console.WriteLine("Checkout Order Exception:"+er.Message);

        this._logger.LogError("Checkout Cart Exception:"+er.Message);
    }
    return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart);    
 }
  [Route("checkout/partial_view")]
  [HttpPost]
  public async Task<IActionResult> UserLoginPartialView()
  {
    return PartialView("~/Views/Shared/_LoginUser.cshtml");    
  }
  

[Route("checkout/createOrder")]
[HttpPost]
public async Task<IActionResult> CreateOrder([FromBody] CheckoutItemModel checkout_item)
{ 
try
{
Console.WriteLine("total price:"+checkout_item.Total_Price);
    
    var paypal_request = new OrdersCreateRequest(); 
    
    string total_price =  checkout_item.Total_Price;

    CheckoutModel checkout = checkout_item.Checkout;

    checkout.Note="Đã thanh toán thành công qua Paypal";

    Console.WriteLine("Checkout here is:"+JsonConvert.SerializeObject(checkout));
    
    paypal_request.Prefer("return=representation");        

    string usd_value=await this._sp.convertVNDToUSD(total_price);
        
    paypal_request.RequestBody(new OrderRequest{
      CheckoutPaymentIntent="CAPTURE",
      PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = total_price
                    },
                    Description = "Order Payment"
                }
            }
            // ApplicationContext = new ApplicationContext
            // {
            //     ReturnUrl = Url.Action("CaptureOrder", "Checkout", null, Request.Scheme),
            //     CancelUrl = Url.Action("CancelOrder", "Checkout", null, Request.Scheme)
            // }
    });

    var paypal_client=this._paypalService.getClient();

    var paypal_response=await paypal_client.Execute(paypal_request);

    Console.WriteLine("Paypal response here is:"+JsonConvert.SerializeObject(paypal_response));

    
    var paypal_result=paypal_response.Result<PayPalCheckoutSdk.Orders.Order>();

    Console.WriteLine("Paypal Result:"+JsonConvert.SerializeObject(paypal_result));

    // var approvalLink=paypal_result.Links.FirstOrDefault(x => x.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase)).Href;

    // Console.WriteLine("Approval link here is:"+approvalLink);
    
    // return Redirect(approvalLink);

    HttpContext.Session.SetString(paypal_result.Id,JsonConvert.SerializeObject(checkout));

    return  Json(new {orderId=paypal_result.Id});
}
catch(Exception er)
{
    Console.WriteLine("Create Order Exception:"+er.Message);
    this._logger.LogError("Create Order Exception:"+er.Message);
}
return BadRequest(new {status=0,message="Tạo đơn hàng thất bại"});
}


 [Route("checkout/submit")]
 [ValidateAntiForgeryToken]
 [HttpPost]
 public async Task<IActionResult> CheckoutOrder(CheckoutModel checkout)
 {
  Console.WriteLine("Checkout Submit did come here");
  try
  {  
    Console.WriteLine("User name here is:"+checkout.UserName);
    
    Console.WriteLine("PHONE here is:"+checkout.PhoneNumber);
    
    Console.WriteLine("Payment method here is:"+checkout.PaymentMethod);
    
    string username=checkout.UserName.Replace(" ","_");    

    string email=checkout.Email;    

    string note = checkout.Note;

    Console.WriteLine("Email here is:"+email);

    string address1=checkout.Address1;

    string phone=checkout.PhoneNumber;

    string payment_method=checkout.PaymentMethod;

    var check_user_exist=await this._user.checkUserExist(email,username);

    var cart=this._cart.getCart();
  
    ApplicationUser user= new ApplicationUser();

    if(check_user_exist && User.Identity.IsAuthenticated && User.IsInRole("User"))
    {
      user=await this._user.findUserByName(username);
    }
    else
    {
      user=new ApplicationUser{UserName=username,Email=email,PhoneNumber=phone,Address2=address1};
      
      string role="Anonymous";
            
      var create_role=await this._user.createRole(role);
           
      var new_user=new Register{UserName=username,Email=email,Password="123456",Address2=address1,PhoneNumber=phone};
      
      var create_user=await this._user.createUser(new_user,role);
      
      user=await this._user.findUserByEmail(email);      
    }
    
    var payment=await this._payment.findPaymentByName(payment_method);

    Console.WriteLine("User Id here is:"+user.Id);
    
    var asp_user = await this._user.getAspUser(user.Id);    

    var created_order=await this._order.createOrder(asp_user,cart,payment,note);
      
    if(created_order==1)
    {  
      Console.WriteLine("Order created successfully");
      
      var order=await this._order.getLatestOrderByUsername(asp_user.Id);

      Console.WriteLine("render view1");

      var render_view = new RazorViewRenderer();

      Console.WriteLine("render view");

      var company_user = await this._user.findUserByName("company");       
      string[] email_list=company_user.Email.Split('#');
      string extra_info=email_list[1];
      string bank_name="";
      string account_num="";
      string account_name="";

    if(!string.IsNullOrEmpty(extra_info))
    {
      string[] info_values=extra_info.Split('\n');
      foreach(var info in info_values)
      {
        if(info.Contains("bank_name"))
        {
          bank_name=info.Split('~')[1].Trim();
        }
        else if(info.Contains("account_name"))
        {
          account_name=info.Split('~')[1].Trim();
        }
        else if(info.Contains("account_num"))
        {
       account_num=info.Split('~')[1].Trim();
        }
      }
    }

    ReceiptModel receipt=new ReceiptModel{Order=order,BankName=bank_name,AccountName=account_name,AccountNumber=account_num};

     string mail_path="MailTemplate/index.cshtml";

     string render_string=await render_view.RenderViewToStringAsync(mail_path,receipt);

     bool is_sent=await this._smtpService.sendEmailGeneral(2,render_string);

     if(is_sent)
     {
      this._logger.LogInformation("Send order checkout successfully");
     }
     else
     {
      this._logger.LogInformation("Send order checkout failed");
     }

     Console.WriteLine("Render string here is:"+render_string);
          
     CheckoutResultModel checkout_result=new CheckoutResultModel{Order=order,Cart=cart};
     
     await this._cart.clearCart();

     return View("~/Views/ClientSide/Checkout/CheckoutResult.cshtml",checkout_result);
    }    
  }
  catch(Exception er)
  {Console.WriteLine("Checkout Exception:"+er.Message);
    this._logger.LogError("Checkout Exception:"+er.Message);    
  }
  ViewBag.Status=0;

  ViewBag.Message="Đặt hàng thất bại";
   
  string username_value=HttpContext.Session.GetString("Username");

  var payment_methods=await this._payment.getAllPayment();
     
  ViewBag.payment_methods=payment_methods;

  int setting_status=await this._setting.getStatusByName("recaptcha");

  var cart_value=this._cart.getCart();

       if(setting_status==1)
       {
        ViewBag.SiteKey=this._recaptcha_response.SiteKey;
       }
    
     if(string.IsNullOrEmpty(username_value))
     {
        return View("~/Views/ClientSide/Checkout/Checkout.cshtml",cart_value);
     }

     var user_value=await this._user.findUserByName(username_value);

     ViewBag.user=user_value;     

     return View("~/Views/ClientSide/Checkout/Checkout.cshtml");    
 }  
 [Route("checkout/capture")]
 [HttpGet]
 public async Task<IActionResult> CaptureOrder(string token)
 {
  try
  {
    // Console.WriteLine("Did in the capture function");
    //+ Console.WriteLine("Capture Request here is:"+JsonConvert.SerializeObject(checkout));
   Console.WriteLine("Invoice Id heree is:"+token);    

   var request = new OrdersCaptureRequest(token);

   Console.WriteLine("Create OrderCapture successfully");
   
   request.RequestBody(new OrderActionRequest());
   
   var paypal_client= this._paypalService.getClient();
   
   var response=await paypal_client.Execute(request);
    
   var result= response.Result<PayPalCheckoutSdk.Orders.Order>();

   Console.WriteLine("Create result successfully");

    // if(result.Status=="COMPLETED")
    // { 
    //   return Json(new {status=1,message="Thanh toán thành công"});
    // }
    if(result.Status=="COMPLETED")
    {
    string checkout=this.HttpContext.Session.GetString(token);
    Console.WriteLine("Checkout object here is:"+checkout);
    return Json(new {status=1,message="Thanh toán thành công",checkout=JsonConvert.DeserializeObject<CheckoutModel>(checkout)});

    }  
    }
  catch(Exception er)
  {
    this._logger.LogError("Capture Order Exception:"+er.Message);
    Console.WriteLine("Capture Order Exception:"+er.Message);
  }
  return BadRequest(new {status=0,message="Thanh toán thất bại"});  
 }

   public IActionResult CancelOrder()
    {
        return Content("Payment cancelled.");
    }
   
  [Route("checkout/done")]
  [HttpGet]
  public async Task<IActionResult> CheckoutResult()
  {
    return View("~/Views/ClientSide/Checkout/CheckoutResult.cshtml");            
  }
}
