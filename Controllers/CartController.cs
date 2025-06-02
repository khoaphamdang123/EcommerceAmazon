using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.Text.RegularExpressions; 
using System.IO;




namespace Ecommerce_Product.Controllers;

public class CartController : BaseController
{
    private readonly ILogger<CartController> _logger;

    private readonly IProductRepository _product;

    private readonly ICategoryListRepository _category;

    private readonly Support_Serive.Service _sp;

    private readonly ICompositeViewEngine _viewEngine;


    private readonly IHttpContextAccessor _httpContextAccessor;


   private readonly ICartRepository _cart;
   public CartController(ICartRepository cart,IProductRepository product,IBannerListRepository banner,Support_Serive.Service sp,IUserListRepository user,ICompositeViewEngine viewEngine,ICategoryListRepository category,ILogger<CartController> logger):base(category,user,banner)
   {
  this._cart=cart;
  this._sp=sp;
  this._viewEngine=viewEngine;
  this._category=category;
  this._product=product;
  this._logger=logger;     
   }


 [Route("cart")]
 [HttpGet]

 public async Task<IActionResult> Cart()
 {  
    var cart=this._cart.getCart();
    if(string.IsNullOrEmpty(this.HttpContext.Session.GetString("UserId")))
    {
      string session_id=Guid.NewGuid().ToString();

      this.HttpContext.Session.SetString("UserId",session_id);
    }
    ViewBag.cart = cart;
    return View("~/Views/ClientSide/Cart/Cart.cshtml");
 }

  [Route("cart/{user_id}")]
  [HttpGet]
  public async Task<IActionResult> CartUser(string user_id)
  { Cart cart=null;
    try
    {   
      cart=await this._cart.getUserCart(user_id); 
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Video File List Exception:"+er.Message);
    }
    ViewBag.cart = cart;
    return View("~/View/ClientSide/Cart/Cart.cshtml");
  }

  [Route("cart/add_cart")]
  [HttpPost]
  public async Task<JsonResult> addItemToCart(string product_name,int quantity,string size="",string color="",string price="")
  { 
    
    Console.WriteLine("Did come to this add cart");
    int add_res=0;
    Console.WriteLine("Color in this cart is:"+color);

    Console.WriteLine("Size in this cart is:"+size);
    
    Console.WriteLine("Price here is:"+price);
    try
    {
    string regex_product=Regex.Replace(product_name, @"[^\w\d+,]", "-");

      var product = await this._product.findProductByName(regex_product);

      CartModel cart = new CartModel{ Product = product, Quantity = quantity, Size = size, Color = color, Price = price };

      add_res = await this._cart.addProductToCart(cart);
    }
    catch (Exception er)
    {
      this._logger.LogError("Add Item To Cart Exception:" + er.Message);
    }
       if(add_res==1 || add_res==-1)
        {   List<CartModel> cart=this._cart.getCart();
    
            return Json(new {status=1,message="Add product to cart successfully",data=cart});
        }
        else
        {
            return Json(new {status=add_res,message="Add product to cart failed.",data=""});
        }
  }

  [Route("cart/partial_view")]
  [HttpPost]
  public async Task<IActionResult> SubCartPartialView()
  {
    var cart = this._cart.getCart();

    return PartialView("~/Views/ClientSide/Cart/_SubCartPartial.cshtml", cart);
  }
  

   private string RenderPartialViewToString(string viewName, object model)
    {
        ViewData.Model = model;
        
        using(var writer = new StringWriter())
        {   
            var viewResult = this._viewEngine.FindView(ControllerContext, viewName, false);
          
            if(viewResult.View == null)
            {
                throw new FileNotFoundException($"Partial view '{viewName}' not found.");
            }

            var viewContext = new ViewContext
            (
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                writer,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext).Wait();
            return writer.GetStringBuilder().ToString();
        }
    }

  [Route("card/update_cart")]
  [HttpPost]

  public async Task<JsonResult> updateCart(List<int> product_ids,List<int> quantities)
  {
  int update_res=0;

  Console.WriteLine("product ids:"+product_ids.Count);

  Console.WriteLine("quantities:"+quantities.Count); 
  try
  {
      for (int i = 0; i < quantities.Count; i++)
      {
        if (quantities[i] < 1)
        {
          quantities[i] = 1;
        }

        int update_value = await this._cart.updateCart(product_ids[i], quantities[i]);

        if (update_value == 0)
        {
          return Json(new { status = 0, message = "Update Cart Failed." });
        }
    
    }
    update_res=1;
  }
  catch(Exception er)
  {
    this._logger.LogError("Update Cart Exception:"+er.Message);
    return Json(new {status=0,message=$"Update Cart Failed:{er.Message}"}); 
  }
  return Json(new {status=1,message="Update Cart Successfully."});  
  }
 
 [Route("cart/remove_item")]
 [HttpPost]
  public async Task<IActionResult> removeItemFromCart(int product_id,string view_name)
  {
    int remove_res=0;
    string url="";
    if(view_name=="cart")
    {
url="~/Views/ClientSide/Cart/_CartPartial.cshtml";
    }
    else
    {
  url="~/Views/ClientSide/Cart/_SubListPartial.cshtml";
    }
    try
    {
        remove_res=await this._cart.deleteProductFromCart(product_id);
    }
    catch(Exception er)
    {
        this._logger.LogError("Remove Item From Cart Exception:"+er.Message);
    }
    
    if (remove_res == 1)
    {
      var cart = this._cart.getCart();

      return PartialView(url, cart);
    }
    else
    {
      return PartialView(url);
    }
  }
}
