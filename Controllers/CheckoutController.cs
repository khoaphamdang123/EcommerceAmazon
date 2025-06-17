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
using Newtonsoft.Json.Linq;




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

  private readonly IStaticFilesRepository _staticFile;
  public CheckoutController(ICartRepository cart, IProductRepository product, Support_Serive.Service sp, IBannerListRepository banner,IStaticFilesRepository staticFile,SmtpService smtpService, IOrderRepository order, IOptions<RecaptchaResponse> recaptcha_response, ISettingRepository setting, IPaymentRepository payment, IUserListRepository user, ICategoryListRepository category, IConfiguration configuration, PaypalService paypalService, ILogger<CheckoutController> logger) : base(category, user, staticFile,banner)
  {
    this._cart = cart;
    this._sp = sp;
    this._smtpService = smtpService;
    this._category = category;
    this._setting = setting;
    this._recaptcha_response = recaptcha_response.Value;
    this._product = product;
    this._order = order;
    this._logger = logger;
    this._payment = payment;
    this._configuration = configuration;
    this._paypalService = paypalService;
    this._user = user;
  }


  [Route("checkout")]
  [HttpGet]
  public IActionResult Checkout()
  {

    if (string.IsNullOrEmpty(this.HttpContext.Session.GetString("UserId")))
    {
      this.HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());
    }

    return RedirectToAction("CheckoutCart", "Checkout", new { id = this.HttpContext.Session.GetString("UserId") });

  }



  [Route("checkout/get_city")]

  [HttpPost]
  public async Task<JsonResult> GetCityByState(string country, string state)
  {
    try
    {
      string url = "https://countriesnow.space/api/v0.1/countries/state/cities";

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Clear();

        var content = new StringContent(JsonConvert.SerializeObject(new { country = country, state = state }), System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsStringAsync();

          var jObject = JObject.Parse(result);

          var cityData = jObject.SelectToken("data");

          var cityList = cityData.ToObject<List<string>>();

          Console.WriteLine("City Data here is:" + cityData.ToList());

          if (cityList != null && cityList.Count() > 0)
          {
            return Json(new { status = 1, data = cityList });
          }
        }
      }
    }
    catch (Exception er)
    {
      Console.WriteLine("Get City By State Exception:" + er.Message);
      this._logger.LogError("Get City By State Exception:" + er.Message);
      return Json(new { status = 0, data = "" });

    }
    return Json(new { status = 0, data = "" });

  }


  [Route("checkout/get_state")]
  [HttpPost]

  public async Task<JsonResult> GetStateByCountry(string country)
  {
    try
    {
      Console.WriteLine("Get City By Country did come here");
      Console.WriteLine("Country here is:" + country);
      if (string.IsNullOrEmpty(country))
      {
        return Json(new { status = 0, message = "Country is empty" });
      }

      var state = JsonConvert.DeserializeObject<Dictionary<string, List<StateInfo>>>(HttpContext.Session.GetString("STATE"));

      if (state != null && state.ContainsKey(country))
      {
        return Json(new { status = 1, data = state[country] });
      }
    }
    catch (Exception er)
    {
      Console.WriteLine("Get City By Country Exception:" + er.Message);
      this._logger.LogError("Get City By Country Exception:" + er.Message);
    }
    return Json(new { status = 0, message = "Failed to get cities" });
  }


  [Route("checkout/{id}")]
  [HttpGet]
  public async Task<IActionResult> CheckoutCart(string id)
  {
    var cart = this._cart.getCart();
    Console.WriteLine("Checkout Cart did come here");
    try
    {
      if (cart == null || cart.Count == 0)
      {
        return RedirectToAction("Cart", "Cart");
      }
      // string username = HttpContext.Session.GetString("UserName");

      //  var payment_methods=await this._payment.getAllPayment();

      // ViewBag.payment_methods=payment_methods;

      //  int setting_status=await this._setting.getStatusByName("recaptcha");

      //  if(setting_status==1)
      //  {
      //   ViewBag.SiteKey=this._recaptcha_response.SiteKey;
      //  }

      bool is_saved_account = false;
      if (string.IsNullOrEmpty(HttpContext.Session.GetString("COUNTRY")))
      {
        Console.WriteLine("Fetching countries data from API");
        using (var client = new HttpClient())
        {
          string url = "https://countriesnow.space/api/v0.1/countries/states";
          client.BaseAddress = new Uri(url);

          client.DefaultRequestHeaders.Accept.Clear();

          var response = await client.GetAsync(url);

          if (response.IsSuccessStatusCode)
          {
            Console.WriteLine("Is Success Status Code");

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Content is compiled");

            var list_data = JsonConvert.DeserializeObject<CountryInfoModel>(content);

            Console.WriteLine("List data here");

            if (list_data != null)
            {
              Console.WriteLine("Countries fetched successfully");

              Dictionary<string, string> countries = new Dictionary<string, string>();

              Dictionary<string, List<StateInfo>> state = new Dictionary<string, List<StateInfo>>();

              foreach (var country in list_data?.Data)
              {
                if (countries.Keys.Contains(country?.Name))
                {
                  continue;
                }
                countries.Add(country?.Name, country.Iso3);

                if (country?.States != null && country.States.Count > 0)
                {
                  state.Add(country.Name, country.States);
                }
              }

              HttpContext.Session.SetString("COUNTRY", JsonConvert.SerializeObject(countries));

              HttpContext.Session.SetString("STATE", JsonConvert.SerializeObject(state));

              Console.WriteLine("Countries listed here is:" + HttpContext.Session.GetString("COUNTRY"));
            }
            else
            {
              Console.WriteLine("Failed to fetch countries");

              this._logger.LogError("Failed to fetch countries");
            }
          }
        }
      }
      ViewBag.Countries = JsonConvert.DeserializeObject<Dictionary<string, string>>(HttpContext.Session.GetString("COUNTRY"));

      ViewBag.Cities = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(HttpContext.Session.GetString("STATE"));

      if (Request.Cookies["UserAccount"] != null)
      {
        is_saved_account = true;
        string account = Request.Cookies["UserAccount"];
        Console.WriteLine("Account here is:" + account);
        ViewBag.Account = account;
        ViewBag.SavedAccount = is_saved_account;
      }
      

    }
    catch (Exception er)
    {
      Console.WriteLine("Checkout Order Exception:" + er.Message);

      this._logger.LogError("Checkout Cart Exception:" + er.Message);
    }
    return View("~/Views/ClientSide/Checkout/Checkout.cshtml", cart);
  }

  [Route("checkout/partial_view")]
  [HttpPost]
  public async Task<IActionResult> UserLoginPartialView()
  {
    return PartialView("~/Views/Shared/_LoginUser.cshtml");
  }





  [Route("checkout/payment")]
  [HttpPost]
  public async Task<IActionResult> CheckoutPaymentForm(string first_name, string last_name, string email, string zip_code, string phone_number, string address1,string address2, string country, string state, string city)
  {
    try
    {
      Console.WriteLine("Checkout Payment Form did come here");
      Console.WriteLine("First name here is:" + first_name);
      Console.WriteLine("Last name here is:" + last_name);
      Console.WriteLine("Email here is:" + email);
      Console.WriteLine("Zip code here is:" + zip_code);
      Console.WriteLine("Phone number here is:" + phone_number);
      Console.WriteLine("Address here is:" + address1);
      Console.WriteLine("Address2 here is:" + address2);
      Console.WriteLine("Country here is:" + country);
      Console.WriteLine("State here is:" + state);
      Console.WriteLine("City here is:" + city);
      HttpContext.Session.SetString("UserName", first_name + " " + last_name);
      HttpContext.Session.SetString("Email", email);
      HttpContext.Session.SetString("PhoneNumber", phone_number);
      HttpContext.Session.SetString("Address", address1);
      HttpContext.Session.SetString("Address2", address2);
      HttpContext.Session.SetString("ZipCode", zip_code);
      HttpContext.Session.SetString("Country", country);
      HttpContext.Session.SetString("State", state);
      HttpContext.Session.SetString("City", city);
    }
    catch (Exception er)
    {
      Console.WriteLine("Checkout Payment Form Exception:" + er.Message);
      this._logger.LogError("Checkout Payment Form Exception:" + er.Message);
    }
    return RedirectToAction("CheckoutPayment", "Checkout", new { id = this.HttpContext.Session.GetString("UserId") });
  }

  [Route("checkout/{id}/payment")]
  [HttpGet]
  public async Task<IActionResult> CheckoutPayment(string id)
  {

    string username = HttpContext.Session.GetString("UserName");
    string email = HttpContext.Session.GetString("Email");
    string phone = HttpContext.Session.GetString("PhoneNumber");
    string address = HttpContext.Session.GetString("Address");
    string zipcode = HttpContext.Session.GetString("ZipCode");
    string country = HttpContext.Session.GetString("Country");
    string state = HttpContext.Session.GetString("State");
    string city = HttpContext.Session.GetString("City");

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(zipcode) || string.IsNullOrEmpty(country) || string.IsNullOrEmpty(state))
    {
      return RedirectToAction("CheckoutCart", "Checkout", new { id = this.HttpContext.Session.GetString("UserId") });
    }

    var cart = this._cart.getCart();

    try
    {

      if (cart == null || cart.Count == 0)
      {
        return RedirectToAction("Cart", "Cart");
      }

      var payment_methods = await this._payment.getAllPayment();

      ViewBag.payment_methods = payment_methods;
    }
    catch (Exception er)
    {
      Console.WriteLine("Checkout Payment Exception:" + er.Message);
      this._logger.LogError("Checkout Payment Exception:" + er.Message);
    }
    return View("~/Views/ClientSide/Checkout/CheckoutPayment.cshtml", cart);
  }

  [Route("checkout/review")]
  [HttpPost]
  public async Task<IActionResult> CheckoutReviewForm(string card_number, string card_owner, string mm, string yy, string cvv, string payment_method)
  {
    try
    {
      string payment_method_value = payment_method;
      Console.WriteLine("Checkout Review Form did come here");
      Console.WriteLine("Card Number here is:" + card_number);
      Console.WriteLine("Card Owner here is:" + card_owner);
      Console.WriteLine("MM here is:" + mm);
      Console.WriteLine("YY here is:" + yy);
      Console.WriteLine("CVV here is:" + cvv);


      Console.WriteLine("Payment Method here is:" + payment_method_value);

      HttpContext.Session.SetString("PaymentMethod", payment_method_value);

      if (payment_method_value == "Credit Card")
      {
        HttpContext.Session.SetString("CardNumber", card_number);
        HttpContext.Session.SetString("CardOwner", card_owner);
        HttpContext.Session.SetString("MM", mm);
        HttpContext.Session.SetString("YY", yy);
        HttpContext.Session.SetString("CVV", cvv);
      }
    }
    catch (Exception er)
    {
      Console.WriteLine("Checkout Review Form Exception:" + er.Message);

      this._logger.LogError("Checkout Review Form Exception:" + er.Message);
    }

    return RedirectToAction("CheckoutReview", "Checkout", new { id = this.HttpContext.Session.GetString("UserId") });

  }


  [Route("checkout/{id}/review")]
  [HttpGet]
  public async Task<IActionResult> CheckoutReview(string id)
  {
    string payment_method = HttpContext.Session.GetString("PaymentMethod");

    if (string.IsNullOrEmpty(payment_method))
    {
      return RedirectToAction("CheckoutPayment", "Checkout", new { id = this.HttpContext.Session.GetString("UserId") });
    }

    string username = HttpContext.Session.GetString("UserName");
    string email = HttpContext.Session.GetString("Email");
    string phone = HttpContext.Session.GetString("PhoneNumber");
    string address = HttpContext.Session.GetString("Address");
    string zipcode = HttpContext.Session.GetString("ZipCode");
    string country = HttpContext.Session.GetString("Country");
    string state = HttpContext.Session.GetString("State");
    string city = HttpContext.Session.GetString("City");

    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(zipcode) || string.IsNullOrEmpty(country) || string.IsNullOrEmpty(state))
    {
      return RedirectToAction("CheckoutCart", "Checkout", new { id = this.HttpContext.Session.GetString("UserId") });
    }


    var cart = this._cart.getCart();

    try
    {
      if (cart == null || cart.Count == 0)
      {
        return RedirectToAction("Cart", "Cart");
      }

      var payment_methods = await this._payment.getAllPayment();

      ViewBag.payment_methods = payment_methods;                  
    }
    catch (Exception er)
    {
      Console.WriteLine("Checkout Review Exception:" + er.Message);

      this._logger.LogError("Checkout Review Exception:" + er.Message);
    }
    return View("~/Views/ClientSide/Checkout/CheckoutReview.cshtml", cart);
  }

  [Route("checkout/createOrder")]
  [HttpPost]
  public async Task<IActionResult> CreateOrder([FromBody] CheckoutItemModel checkout_item)
  {
    try
    {
      Console.WriteLine("total price:" + checkout_item.Total_Price);

      var paypal_request = new OrdersCreateRequest();

      string total_price = checkout_item.Total_Price;

      CheckoutModel checkout = checkout_item.Checkout;

      checkout.Note = "Đã thanh toán thành công qua Paypal";

      Console.WriteLine("Checkout here is:" + JsonConvert.SerializeObject(checkout));

      paypal_request.Prefer("return=representation");

      string usd_value = await this._sp.convertVNDToUSD(total_price);

      paypal_request.RequestBody(new OrderRequest
      {
        CheckoutPaymentIntent = "CAPTURE",
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

      var paypal_client = this._paypalService.getClient();

      var paypal_response = await paypal_client.Execute(paypal_request);

      Console.WriteLine("Paypal response here is:" + JsonConvert.SerializeObject(paypal_response));


      var paypal_result = paypal_response.Result<PayPalCheckoutSdk.Orders.Order>();

      Console.WriteLine("Paypal Result:" + JsonConvert.SerializeObject(paypal_result));

      // var approvalLink=paypal_result.Links.FirstOrDefault(x => x.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase)).Href;

      // Console.WriteLine("Approval link here is:"+approvalLink);

      // return Redirect(approvalLink);

      HttpContext.Session.SetString(paypal_result.Id, JsonConvert.SerializeObject(checkout));

      return Json(new { orderId = paypal_result.Id });
    }
    catch (Exception er)
    {
      Console.WriteLine("Create Order Exception:" + er.Message);
      this._logger.LogError("Create Order Exception:" + er.Message);
    }

    return BadRequest(new { status = 0, message = "Tạo đơn hàng thất bại" });

  }


  [Route("checkout/submit")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutModel checkout)
  {
    Console.WriteLine("Checkout Submit did come here");

    try
    {
      Console.WriteLine("User name here is:" + checkout.UserName);

      Console.WriteLine("PHONE here is:" + checkout.PhoneNumber);

      Console.WriteLine("Payment method here is:" + checkout.PaymentMethod);

      Console.WriteLine("Address here is:" + checkout.Address1);

      Console.WriteLine("City here is:" + checkout.Address2);
      
      string username = checkout.UserName.Replace(" ", "_");

      string email = checkout.Email;

      string note = checkout.Note;

      string zip_code = checkout.ZipCode;

      string city = checkout.City;

      string state = checkout.State;

      string country = checkout.Country;

      Console.WriteLine("Email here is:" + email);

      string address1 = checkout.Address1;

      string phone = checkout.PhoneNumber;

      string payment_method = checkout.PaymentMethod;

      var check_user_exist = await this._user.checkUserExist(email, username);

      var cart = this._cart.getCart();
      //ApplicationUser user = new ApplicationUser();

      // if(check_user_exist && User.Identity.IsAuthenticated && User.IsInRole("User"))
      // {
      //   user=await this._user.findUserByName(username);
      // }

      //user = new ApplicationUser { UserName = username, Email = email, PhoneNumber = phone, Address2 = address1 };

      // string role = "Anonymous";

      // var create_role = await this._user.createRole(role);

      // user = await this._user.findUserByEmail(email);

      // var new_user = new Register { UserName = username, Email = email, Password = "123456", Address2 = address1, PhoneNumber = phone };

      // var create_user = await this._user.createUser(new_user, role);
      
      UserOrderInfo user = new UserOrderInfo
      {
        UserName = username,
        Email = email,
        PhoneNumber = phone,
        Address1 = address1,
        Address2 = checkout.Address2
      };

      var payment = await this._payment.findPaymentByName(payment_method);


      //var asp_user = await this._user.getAspUser(user.Id);

      var created_order = await this._order.createOrder(user, cart, payment, zip_code, country, state, city, note);

      if (created_order == 1)
      {
        Console.WriteLine("Order created successfully");

        Console.WriteLine("Order IDD here is:" + this.HttpContext.Session.GetString("ORDID"));

        var order = await this._order.getLatestOrderByUsername("15");

        Console.WriteLine("render view 1");        

        var render_view = new RazorViewRenderer();

        Console.WriteLine("render view");

        var company_user = await this._user.findUserByName("company");

        //string[] email_list = company_user.Email.Split('#');

        // string email_value = "";

        // if(email_list.Length > 0)
        // {
        //   email_value = email_list[0];
        // }

        // string extra_info = email_list[1];

        // string bank_name = "";

        // string account_num = "";

        // string account_name = "";

        // if (!string.IsNullOrEmpty(extra_info))
        // {
        //   string[] info_values = extra_info.Split('\n');
        //   foreach (var info in info_values)
        //   {
        //     if (info.Contains("bank_name"))
        //     {
        //       bank_name = info.Split('~')[1].Trim();
        //     }
        //     else if (info.Contains("account_name"))
        //     {
        //       account_name = info.Split('~')[1].Trim();
        //     }
        //     else if (info.Contains("account_num"))
        //     {
        //       account_num = info.Split('~')[1].Trim();
        //     }
        //   }
        // }

        //  ReceiptModel receipt=new ReceiptModel{Order=order,BankName=bank_name,AccountName=account_name,AccountNumber=account_num};

        CheckoutResultModel receipt = new CheckoutResultModel{ Order = order,Company=company_user, Cart = cart };

          string mail_path="MailTemplate/index.cshtml";

          string render_string=await render_view.RenderViewToStringAsync(mail_path,receipt);

          bool is_sent=await this._smtpService.sendEmailGeneral(2,render_string,email);

        if(is_sent)
         {
          this._logger.LogInformation("Send order checkout successfully");
         }
         else
         {
          this._logger.LogInformation("Send order checkout failed");
         }

         Console.WriteLine("Render string here is:"+render_string);

        CheckoutResultModel checkout_result = new CheckoutResultModel { Order = order, Cart = cart };

        Console.WriteLine("did come to here");

        return View("~/Views/ClientSide/Checkout/CheckoutResult.cshtml",checkout_result);
      }
    }
    catch (Exception er)
    {
      Console.WriteLine("Checkout Exception:" + er.Message);

      Console.WriteLine("Checkout Inner Exception:" + er.InnerException?.Message);

      this._logger.LogError("Checkout Exception:" + er.Message);
    }
    ViewBag.Status = 0;

    ViewBag.Message = "Đặt hàng thất bại";

    string username_value = HttpContext.Session.GetString("Username");

    var payment_methods = await this._payment.getAllPayment();

    ViewBag.payment_methods = payment_methods;

    int setting_status = await this._setting.getStatusByName("recaptcha");

    var cart_value = this._cart.getCart();

    if (setting_status == 1)
    {
      ViewBag.SiteKey = this._recaptcha_response.SiteKey;
    }

    if (string.IsNullOrEmpty(username_value))
    {
      return View("~/Views/ClientSide/Checkout/Checkout.cshtml", cart_value);
    }

    var user_value = await this._user.findUserByName(username_value);

    ViewBag.user = user_value;

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
      Console.WriteLine("Invoice Id heree is:" + token);

      var request = new OrdersCaptureRequest(token);

      Console.WriteLine("Create OrderCapture successfully");

      request.RequestBody(new OrderActionRequest());

      var paypal_client = this._paypalService.getClient();

      var response = await paypal_client.Execute(request);

      var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

      Console.WriteLine("Create result successfully");

      // if(result.Status=="COMPLETED")
      // { 
      //   return Json(new {status=1,message="Thanh toán thành công"});
      // }
      if (result.Status == "COMPLETED")
      {
        string checkout = this.HttpContext.Session.GetString(token);

        Console.WriteLine("Checkout object here is:" + checkout);

        return Json(new { status = 1, message = "Thanh toán thành công", checkout = JsonConvert.DeserializeObject<CheckoutModel>(checkout) });

      }
    }
    catch (Exception er)
    {
      this._logger.LogError("Capture Order Exception:" + er.Message);
      Console.WriteLine("Capture Order Exception:" + er.Message);
    }
    return BadRequest(new { status = 0, message = "Thanh toán thất bại" });
  }

  public IActionResult CancelOrder()
  {
    return Content("Payment cancelled.");
  }

  [Route("checkout/{id}/done")]

  [HttpGet]
  public async Task<IActionResult> CheckoutResult()
  {
    string asp_id = this.HttpContext.Session.GetString("OrderId");

    var order = await this._order.getLatestOrderByUsername(asp_id);

    var cart = this._cart.getCart();

    CheckoutResultModel checkout_result = new CheckoutResultModel { Order = order, Cart = cart };

    await this._cart.clearCart();

    return View("~/Views/ClientSide/Checkout/CheckoutResult.cshtml", checkout_result);    
    
  }
}
