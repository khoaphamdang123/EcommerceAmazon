using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;


namespace Ecommerce_Product.Controllers;


[Authorize(Roles ="Admin")]

[Route("admin")]
public class VideoListController : BaseAdminController
{
    private readonly ILogger<VideoListController> _logger;

    private readonly IProductRepository _product;

    private readonly Support_Serive.Service _sp;

   private readonly IVideoRepository _video;
   public VideoListController(IVideoRepository video,IProductRepository product,IBannerListRepository banner,Support_Serive.Service sp,ILogger<VideoListController> logger):base(banner)
   {
  this._video=video;
  this._sp=sp;
  this._product=product;
  this._logger=logger;     
   }
  [Route("video_list")]

  [HttpGet]
  public async Task<IActionResult> VideoList()
  { Console.WriteLine("did come to this route already");     
     string select_size="7";          
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {   
        var video=await this._video.getAllVideo();
        
        var video_files=await this._video.pagingVideo(7,1,video);
        
        Console.WriteLine("Video Files:"+video_files.item.Count);
        
        return View(video_files);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Video File List Exception:"+er.Message);
    }
    return View();
  }

  [Route("video_list/add_video")]
   [HttpGet]
   public async Task<IActionResult> AddVideo()
   { 
   
   var products=await this._product.getAllProductList();
   
   
   ViewBag.products=products;
    
   return View();
   }

  [Route("{product_id}/video_list")]
  
  [HttpGet]
  public async Task<IActionResult> VideoListByProduct(int product_id)
  {
 string select_size="7";          
 ViewBag.select_size=select_size;
 List<string> options=new List<string>(){"7","10","20","50"};
  ViewBag.options=options;

  var video_files=await this._video.findVideoByProductId(product_id);
 
  var video_page_list=await this._video.pagingVideo(7,1,video_files);

  return View("~/Views/VideoList/VideoList.cshtml",video_page_list);
  }
  [Route("video_list/paging")]
  
  [HttpGet]
  public async Task<IActionResult> VideoListPaging([FromQuery]int page_size,[FromQuery] int page=1,IEnumerable<Video> video=null)
  {
    try{

         if(video==null)
         {
              video=await this._video.getAllVideo();
         }
         var files=await this._video.pagingVideo(page_size,page,video);
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                  
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/VideoList/VideoList.cshtml",files);          
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Video List Exception:"+er.Message);
        }
    return View();    
  }

  [Route("video_list/delete/{id}")]
  [HttpGet]
  
  public async Task<IActionResult> DeleteVideo(int id)
  {
  int delete_res=0;
 try
 {
   delete_res=await this._video.deleteVideo(id);
    
   if(delete_res==0)
    {
      TempData["Status_Delete"]=0;

      TempData["Message_Delete"]="Xóa video thất bại";
    }
    else
    {
      TempData["Status_Delete"]=1;

      TempData["Message_Delete"]="Xóa video thành công";
    }
 }
 catch(Exception er)
 {
  this._logger.LogError("Delete Video Exception:"+er.Message);  
 }
  return RedirectToAction("VideoList","VideoList");
}
 
   [Route("video_list/add_video")]
   [HttpPost]
   public async Task<JsonResult> addVideoList(Video video)
   { int add_res=0;
   Console.WriteLine("Come to this Manual function");
    try
    {
    add_res=await this._video.addVideo(video);
    }
     catch(Exception er)
    {      
        this._logger.LogError("Add Video Exception:"+er.Message);
      return Json(new {status=0,message="Có lỗi xảy ra:"+er.Message});

    }
   if(add_res==0)
    {
      return Json(new {status=0,message="Thêm video thất bại"});
    }
    else{
      return Json(new {status=1,message="Thêm video thành công"});
    }  
   }
  
  [Route("video_list/filter")]

  [HttpGet]

  public async Task<IActionResult> FilterVideo(string product_name)
  {
    var video_filter=await this._video.findVideoByProductName(product_name);
    
    var video_paging=await this._video.pagingVideo(video_filter.ToList().Count,1,video_filter);

    List<string> options=new List<string>(){"7","10","20","50"};
          
    ViewBag.options=options;
                  
    string select_size="7";
          
    ViewBag.select_size=select_size;
          
   return View("~/Views/VideoList/VideoList.cshtml",video_paging);  

  }

  [Route("video_list/{id}")]
  [HttpGet]

  public async Task<IActionResult> VideoInfo(int id)
  {
    var video=await this._video.findVideoById(id);
    var products=await this._product.getAllProduct();
    ViewBag.products=products;
    return View(video);    
  }

  [Route("video_list/update_video")]
  [HttpPost]
  public async Task<JsonResult> updateVideoList(int id,Video video)
   { 
   int update_res=0;
   Console.WriteLine("Id:"+id);
   Console.WriteLine("Come to this Manual function");
    try
    {
    update_res=await this._video.updateVideo(id,video);
    }
     catch(Exception er)
    {      
      this._logger.LogError("Update Video Exception:"+er.Message);

      return Json(new {status=0,message="Có lỗi xảy ra:"+er.Message});

    }
   if(update_res==0)
    {
      return Json(new {status=0,message="Cập nhật video thất bại"});
    }
    else{
      return Json(new {status=1,message="Cập nhật video thành công"});
    }  
   }
}
