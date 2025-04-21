using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using Org.BouncyCastle.Math.EC.Rfc8032;
using System.ComponentModel;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class StaticFilesController : BaseAdminController
{
   private readonly ILogger<StaticFilesController> _logger;
   private readonly IStaticFilesRepository _static_files;
   public StaticFilesController(IStaticFilesRepository static_files,IBannerListRepository banner,ILogger<StaticFilesController> logger):base(banner)
   {
  this._static_files=static_files;

  this._logger=logger;     
   }
  [Route("file_list")]
  [HttpGet]
  public async Task<IActionResult> StaticFiles()
  {       string select_size="7";          
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {  
        var static_files=await this._static_files.pagingStaticFiles(7,1);

        return View(static_files);        
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Static File List Exception:"+er.Message);
    }
    return View();
  }


  [Route("file_list/paging")]
  [HttpGet]
  public async Task<IActionResult> StaticFilesPaging([FromQuery]int page_size,[FromQuery] int page=1)
  {
    try{
         var files=await this._static_files.pagingStaticFiles(page_size,page);
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                  
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/StaticFiles/StaticFiles.cshtml",files);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Static File List Exception:"+er.Message);
        }
    return View();
  }
  [Route("file_list/add_page")]
  [HttpGet]
  public IActionResult AddStaticFiles()
  {
    return View();
  }
  [Route("file_list/add_page")]
  [HttpPost]
  public async Task<IActionResult> AddStaticFiles(StaticFile file)
  {  try{
      string file_name=file.Filename;
      string content= file.Content;
      int created_res=await this._static_files.addPage(file);
      if(created_res==0)
      {
        ViewBag.Status=0;
        ViewBag.Created_Page="Thêm trang thất bại";
      }
      else if(created_res==-1)
      {
        ViewBag.Status=-1;
        ViewBag.Created_Page="Trang này đã tồn tại trong hệ thống";
      }
      else
      {
        ViewBag.Status=1;
        ViewBag.Created_Page="Thêm trang thành công";
      }
  }
  catch(Exception er)
  {
    this._logger.LogTrace("Add Page Exception:"+er.Message);
  }
      return View();
  }
  [Route("file_list/delete")]
  [HttpGet]
  public async Task<IActionResult> DeletePage(int id)
  {
    try
    {
   int remove_res=await this._static_files.deletePage(id);

   if(remove_res==0)
   {
   TempData["Status_Delete"]=0;
   TempData["Message_Delete"]="Xóa trang thất bại";
   }
   else
   {
   TempData["Status_Delete"]=1;
   TempData["Message_Delete"]="Xóa trang thành công";
   }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Remove Page Exception:"+er.Message);
    }
    return RedirectToAction("StaticFiles","StaticFiles");
  }

  [Route("file_list/{id}/page_info")]
  [HttpGet]
  public async Task<IActionResult> StaticFilesInfo(int id)
  {
    var page=await this._static_files.findStaticFileById(id);
    return View(page);
  }

    [Route("file_list/{id}/page_info")]
  [HttpPost]
  public async Task<IActionResult> StaticFilesInfo(int id,StaticFile file)
  {  

    int updated_res=await this._static_files.updatePage(id,file);
    if(updated_res==0)
    {
        ViewBag.Status=0;
        ViewBag.Updated_Message="Cập nhật trang thất bại";
    }
    else
    {
  ViewBag.Status=1;
  ViewBag.Updated_Message="Cập nhật trang thành công";
    }
  
  var page=await this._static_files.findStaticFileById(id);
  
  return View(page);
  }


}
