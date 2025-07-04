
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using iText.IO.Source;
using System.Globalization;

namespace Ecommerce_Product.Controllers;
public class BlogController:BaseController
{
 

  private readonly ICategoryListRepository _category;

  private readonly ILogger<BlogController> _logger;

  private readonly IStaticFilesRepository _staticFile;


  private readonly IBlogRepository _blog;

public BlogController(ICategoryListRepository category,IBlogRepository blog,IUserListRepository user,IStaticFilesRepository staticFile,IBannerListRepository banner,ILogger<BlogController> logger):base(category,user,staticFile,banner)
{
    this._blog=blog;
    this._category=category;
    this._logger=logger;
}
    [HttpGet]
    [Route("blog")]

    public async Task<IActionResult> Blog()
    {
        try
        {
            var blogs = await this._blog.getAllBlog();
            var categories = await this._category.getAllCategory();
            var blogs_sorted = blogs.OrderByDescending(x => DateTime.ParseExact(x.Createddate,"MM/dd/yyyy HH:mm:ss",CultureInfo.InvariantCulture)).ToList();
            ViewBag.categories = categories;
            ViewBag.blogs_sorted = blogs_sorted;
            return View("~/Views/ClientSide/Blog/Blog.cshtml", blogs);
        }
        catch (Exception er)
        {
            this._logger.LogTrace("Get Manual File List Exception:" + er.Message);
        }
        return View("~/Views/ClientSide/Blog/Blog.cshtml");
    }

    [HttpGet]
    [Route("blog/{category_id}/blog_list")]
    public async Task<IActionResult> BlogListByCategory(int category_id)
    {
        try
        {
         
        // if (string.IsNullOrEmpty(category))
        // {
        //     return View("~/Views/ClientSide/Blog/Blog.cshtml", blogs);
        // }

        var blog_by_cat = await this._blog.findBlogByCategory(category_id);

        if (blog_by_cat.Count() > 0)
        {
            return View("~/Views/ClientSide/Blog/Blog.cshtml", blog_by_cat);
        }
        
        }
     catch(Exception er)
        {
            this._logger.LogTrace("Get Blog List By Category Exception:" + er.Message);
        }

        return View("~/Views/ClientSide/Blog/Blog.cshtml");
    }

[Route("blog/blog_detail/{id}")]
public async Task<IActionResult> BlogDetail(int id)
{  
   var list_blog=await this._blog.getAllBlog();


    int nxt_index=id;

    Console.WriteLine("ID HERE:"+nxt_index);


   
   var blog = list_blog.FirstOrDefault(x=>x.Id==id);

  var blogs_sorted = list_blog.OrderByDescending(x => DateTime.ParseExact(x.Createddate,"MM/dd/yyyy HH:mm:ss",CultureInfo.InvariantCulture)).Take(5).ToList();

   var blog_name=blog?.Blogname;

   ViewBag.blogs_sorted=blogs_sorted;

   this._logger.LogInformation("Blog Name:"+blog_name);

   Console.WriteLine("Blog Name:"+blog_name);   

   ViewBag.blog_name=blog_name;
  
  if(blog?.CategoryId!=null)
  {
   var blog_by_cat=await this._blog.findBlogByCategory(blog.CategoryId);

   var sorted_blog = list_blog.OrderByDescending(x=>DateTime.Parse(x.Createddate)).Take(4).ToList();

   Random rand=new Random();

  var categories=await this._category.getAllCategory();
  
  ViewBag.categories=categories;


  if(blog_by_cat.Count()>0)
  {   
   List<int> blog_ids=blog_by_cat.Select(x=>x.Id).ToList();
   nxt_index=blog_ids[rand.Next(0,blog_by_cat.Count())];
   
   int retry=2;
   
   while(nxt_index==id && retry<2)
   {
    
    nxt_index=blog_ids[rand.Next(0,blog_by_cat.Count())];
    
    retry+=1;
   }
   
   Console.WriteLine("nxt_index:"+nxt_index);
   
  }

   ViewBag.blog_by_cat=blog_by_cat;

   ViewBag.list_blog=sorted_blog;
}
    
    ViewBag.nxt_index=nxt_index;




   blog.Content=HttpUtility.HtmlDecode(blog.Content);
    
   return View("~/Views/ClientSide/Blog/BlogDetail.cshtml",blog);
}

}