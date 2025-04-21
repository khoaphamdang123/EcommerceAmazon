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
public class BlogListController : BaseAdminController
{
    private readonly ILogger<BlogListController> _logger;

   private readonly ICategoryListRepository _category;

   private readonly IBlogRepository _blog;

   public BlogListController(IBlogRepository blog,ICategoryListRepository category,IBannerListRepository banner,ILogger<BlogListController> logger):base(banner)
   {
  this._blog=blog;
  this._category=category;
  this._logger=logger;   
   }
  [Route("blog/news")]
  [HttpGet]
  public async Task<IActionResult> BlogList()
  {    
   string select_size="7";          
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {   
       var blogs=await this._blog.getAllBlog();
       var blogs_files=await this._blog.pagingBlogFiles(7,1,blogs);
        return View(blogs_files);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Manual File List Exception:"+er.Message);
    }
    return View();
  }

[Route("blog/news/paging")]
[HttpGet]
  public async Task<IActionResult> BlogListPaging([FromQuery]int page_size,[FromQuery] int page=1,IEnumerable<Blog> blog=null)
  {
    try{

      if(blog==null)
      {
        blog=await this._blog.getAllBlog();
      }
         var files=await this._blog.pagingBlogFiles(page_size,page,blog);
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                  
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/BlogList/BlogList.cshtml",files);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Blog List Exception:"+er.Message);
        }
    return View();    
  }



  
  [Route("blog/add_blog")]
  [HttpGet]
 public async Task<IActionResult> AddBlog()
 {
  var categories=await this._category.getAllCategory();

  ViewBag.categories=categories;
  
  return View();
 }

[Route("blog/new_blog")]
[HttpPost]
public async Task<JsonResult> addNewBlog(BlogModel blog)
{ int add_blog=0;
Console.WriteLine("did stay here");
 try
 { Console.WriteLine(blog.Blogname);
   add_blog=await this._blog.addBlog(blog);
 }
 catch(Exception er)
 {
  this._logger.LogError("Add New Blog Exception:"+er.Message);
 }
 if(add_blog==1)
 {
  return Json(new {status=1,message="Thêm bài mới thành công"});
 }
 else
 {
  return Json(new {status=0,message="Thêm bài mới thất bại"});
 }
}

[Route("blog/info/{id}")]
[HttpGet]

public async Task<IActionResult> BlogInfo(int id)
{ var blog=new Blog();
    try
    {
        blog=await this._blog.findBlogById(id);
    }
    catch(Exception er)
    {
        this._logger.LogError("Get Blog Info Exception:"+er.Message);
    }
    
    var categories=await this._category.getAllCategory();
    
    ViewBag.categories=categories;
    
    return View(blog);
}

[Route("blog/edit")]
[HttpPost]
public async Task<JsonResult> updateBlog(int id,BlogModel blog)
{  int update_res=0;
    try
    {
  update_res=await this._blog.updateBlog(id,blog);
    }
    catch(Exception er)
    {
        this._logger.LogError("Update Blog Exception:"+er.Message);
    }
    if(update_res==0)
    {
        return Json(new {status=0,message="Cập nhật bài viết thất bại"});
    }
    else
    {
        return Json(new {status=1,message="Cập nhật bài viết thành công"});
    }
}


[Route("blog/delete/{id}")]
[HttpGet]
public async Task<IActionResult> deleteBlog(int id)
{  int delete_res=0;
    try
    {
        delete_res=await this._blog.deleteBlog(id);
    }
    catch(Exception er)
    {
        this._logger.LogError("Delete Blog Exception:"+er.Message);
    }
     if(delete_res==1)
        {
            TempData["Status_Delete"]=1;
            TempData["Message_Delete"]="Xóa bài viết thành công";
        }
        else
        {
            TempData["Status_Delete"]=0;
            TempData["Message_Delete"]="Xóa bài viết thất bại";
        }
        return RedirectToAction("BlogList","BlogList");
}


}
