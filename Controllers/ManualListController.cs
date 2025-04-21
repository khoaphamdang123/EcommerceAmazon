using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;


namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class ManualListController : BaseAdminController
{
    private readonly ILogger<ManualListController> _logger;

    private readonly IProductRepository _product;

    private readonly Support_Serive.Service _sp;

   private readonly IManualRepository _manual;
   public ManualListController(IManualRepository manual,IProductRepository product,IBannerListRepository banner,Support_Serive.Service sp,ILogger<ManualListController> logger):base(banner)
  {
  this._manual=manual;
  this._sp=sp;
  this._product=product;
  this._logger=logger;     
   }
  [Route("manual_list")]
  [HttpGet]
  public async Task<IActionResult> ManualList()
  {       
          string select_size="7";          
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {   
       var manual=await this._manual.getAllManual();

       var manual_files=await this._manual.pagingManualFiles(7,1,manual);
      
       return View(manual_files);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Manual File List Exception:"+er.Message);
    }
    return View();
  }

  [Route("manual_list/add_manual")]
   [HttpGet]
   public async Task<IActionResult> AddManual()
   { 
   List<string> languages = this._sp.getListOfLanguage();
   
   var products=await this._product.getAllProductList();
   
   ViewBag.language_list=languages;
   
   ViewBag.products=products;
    
   return View();
   }

  [Route("{product_id}/manual_list")]
  
  [HttpGet]
  public async Task<IActionResult> ManualListByProduct(int product_id)
  {
 
 string select_size="7";          
 
 ViewBag.select_size=select_size; 
 
 List<string> options=new List<string>(){"7","10","20","50"};
 
 ViewBag.options=options;

  var manual_files=await this._manual.findManualByProductId(product_id);
 
  var manual_page_list=await this._manual.pagingManualFiles(7,1,manual_files);

  return View("~/Views/ManualList/ManualList.cshtml",manual_page_list);
  }
  [Route("manual_list/paging")]
  
  [HttpGet]
  public async Task<IActionResult> ManualListPaging([FromQuery]int page_size,[FromQuery] int page=1,IEnumerable<Manual> manual=null)
  {
    try{

      if(manual==null)
      {
        manual=await this._manual.getAllManual();
      }
         var files=await this._manual.pagingManualFiles(page_size,page,manual);
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                  
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/ManualList/ManualList.cshtml",files);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Manual List Exception:"+er.Message);
        }
    return View();    
  }

  [Route("manual_list/delete/{id}")]
  [HttpGet]
  
  public async Task<IActionResult> DeleteManual(int id)
  {
  int delete_res=0;
 try
 {
   delete_res=await this._manual.deleteManual(id);
    
   if(delete_res==0)
    {
      TempData["Status_Delete"]=0;
      TempData["Message_Delete"]="Xóa tài liệu thất bại";
    }
    else
    {
      TempData["Status_Delete"]=1;
      TempData["Message_Delete"]="Xóa tài liệu thành công";      
    }
 }
 catch(Exception er)
 {
  this._logger.LogError("Delete Manual Exception:"+er.Message);  
 }
  return RedirectToAction("ManualList","ManualList");
}
 
   [Route("manual_list/add_manual")]
   [HttpPost]
   public async Task<JsonResult> addManualList(ManualModel manual)
   { int add_res=0;
   Console.WriteLine("Come to this Manual function");
    try
    {
    add_res=await this._manual.addManual(manual,Request.Scheme.ToString(),Request.Host.ToString());
    }
     catch(Exception er)
    {      
        this._logger.LogError("Add Manual Exception:"+er.Message);
      return Json(new {status=0,message="Có lỗi xảy ra:"+er.Message});

    }
   if(add_res==0)
    {
      return Json(new {status=0,message="Thêm tài liệu thất bại"});
    }
    else{
      return Json(new {status=1,message="Thêm tài liệu thành công"});
    }  
   }


  [Route("manual_list/filter")]

  [HttpGet]

  public async Task<IActionResult> FilterManual(string product_name)
  {
      var manual_files=await this._manual.filterManualByProductName(product_name);

     var manual_page_list=await this._manual.pagingManualFiles(manual_files.ToList().Count,1,manual_files);
     
    try
    {
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                            
          ViewBag.select_size="7";

          
          return View("~/Views/ManualList/ManualList.cshtml",manual_page_list);
    }
    catch(Exception er)
    {
        this._logger.LogError("Filter Manual Exception:"+er.Message);

        Console.WriteLine("Filter Manual Exception:"+er.Message);
    }

    return View("~/Views/ManualList/ManualList.cshtml",manual_page_list);
    
  }

  
  [Route("manual_list/{id}")]
  [HttpGet]

  public async Task<IActionResult> ManualInfo(int id)
  {
    var manual=await this._manual.findManualById(id);
    List<string> languages = this._sp.getListOfLanguage();
    var products=await this._product.getAllProduct();
    ViewBag.language_list=languages;
    ViewBag.products=products;
    return View(manual);
  }

  [Route("manual_list/update_manual")]
  [HttpPost]
  public async Task<JsonResult> updateManualList(int id,ManualModel manual)
   { 
   int update_res=0;
    try
    {
    update_res=await this._manual.updateManual(id,manual,Request.Scheme.ToString(),Request.Host.ToString());    
    }
     catch(Exception er)
    {      
      this._logger.LogError("Add Manual Exception:"+er.Message);
      
      return Json(new {status=0,message="Có lỗi xảy ra:"+er.Message});
    }
   if(update_res==0)
    {
      return Json(new {status=0,message="Cập nhật tài liệu thất bại"});
    }
    else
    {
      return Json(new {status=1,message="Cập nhật tài liệu thành công"});
    }  
   }
}
