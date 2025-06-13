
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Web;
using System.Text.RegularExpressions;

namespace Ecommerce_Product.Controllers;
public class ProductDetailController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

private readonly ICategoryListRepository _category;

private readonly ILogger<ProductDetailController> _logger;

private readonly IStaticFilesRepository _staticFile;

public ProductDetailController(IBannerListRepository banner,IProductRepository product,ICategoryListRepository category,IStaticFilesRepository staticFiles,IUserListRepository user,ILogger<ProductDetailController> logger):base(category,user,staticFiles,banner)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
}

[Route("product/details/{product_name}")]
[HttpGet]
public async Task<IActionResult> ProductDetail(string product_name)
{   
  //var banners= await this._banner.findBannerByName("Home");
 try
 {  
    product_name=Uri.UnescapeDataString(product_name);
 
    // var products = await this._product.getAllProduct();
    
    // var categories = await this._category.getAllCategory();
    
    // var brands = await this._category.getAllBrandList();


    // ViewBag.products = products;
    
    // ViewBag.categories=categories;
    
    // ViewBag.brands=brands;
    
    Dictionary<string,int> count_stars=new Dictionary<string, int>();    
    
    Console.WriteLine("Product name here is:"+product_name);

    var product= await this._product.findProductByName(product_name);
    
    if(product!=null)
    { 
      var products_image=product.ProductImages.Count;
      
      // var manual = await this._product.findManualByLanguage("English",product);

      var variants=await this._product.getProductVariant(product);

      if(variants==null)
      {
        Console.WriteLine("Variant here is null");
      }
        if (variants != null)
        {
          ViewBag.variants = variants;
        }
      //  if(manual!=null)
      //  {
      //   ViewBag.manual_link=manual.ManualLink;
      //  }
      
      List<Product> single_product = new List<Product>{product};      
      
    //   var count_reviews=await this._product.countAllReview(single_product);            
      
    //   ViewBag.count_reviews=count_reviews;
      
    //   Console.WriteLine("Product Id here is:"+product.Id);
      
    //   int rating_star=await this._product.getSingleProductRating(product.Id);      
      
    //   ViewBag.rating_star=rating_star;
    //   for(int i=1;i<=5;i++)
    // {
    //   int count_star=await this._product.countProductRatingByStar(i,product.Id);
    //   count_stars.Add(i.ToString(),count_star);
    // }

    // ViewBag.count_stars=count_stars;

    // var review_list=await this._product.getProductReviewList(product.Id);

    // ViewBag.review_list=review_list;    

    Regex reg= new Regex(@"\s*(<[^>]+>)\s*");
      
    Console.WriteLine("number of image details:"+products_image);

      if(!string.IsNullOrEmpty(product.Statdescription))
      {  
        // string stat_description=product.Statdescription;
        // stat_description=reg.Replace(stat_description,"$1");
        
        // stat_description=HttpUtility.HtmlDecode(stat_description);

        // stat_description=stat_description.Replace("<p>","").Replace("</p>","").Replace("<span style=\"white-space: normal;\">","").Replace("</span>","").Replace("<span style=\"white-space:pre;\"","").Replace(">>",">");
        product.Statdescription=HttpUtility.HtmlDecode(product.Statdescription);
      }
      if(!string.IsNullOrEmpty(product.Description))
      {
        string regular_text=Regex.Replace(product.Description, "<.*?>", "").Trim();
       
       if(regular_text!="Powered by Froala Editor")
       {
        product.Description=HttpUtility.HtmlDecode(product.Description);
       }
       else
       {
      product.Description="";
       }        
      }
      if(!string.IsNullOrEmpty(product.InboxDescription))
      { 
        string regular_text=Regex.Replace(product.InboxDescription, "<.*?>", "").Trim();
       if(regular_text!="Powered by Froala Editor")
       {
        product.InboxDescription=HttpUtility.HtmlDecode(product.InboxDescription);
       }
       else
       {
      product.InboxDescription="";
       }
      }
      if(!string.IsNullOrEmpty(product.DiscountDescription))
      { 
       string regular_text=Regex.Replace(product.DiscountDescription, "<.*?>", "").Trim();

       if(regular_text!="Powered by Froala Editor")
       {
        product.DiscountDescription=HttpUtility.HtmlDecode(product.DiscountDescription);
       }
       else
       {
        product.DiscountDescription="";        
       }
      }
    }    
  else
  {
    Console.WriteLine("Product here is null");
  }
    var product_by_category=await this._product.getProductByCategory(product.Category.CategoryName);

    ViewBag.product_by_category=product_by_category;
    return View("~/Views/ClientSide/ProductDetail/ProductDetail.cshtml",product);
 }
 catch(Exception er)
 {
    Console.WriteLine("Product Detail Exception:"+er.Message);
    this._logger.LogError("Product Detail Exception:"+er.Message);
 }          
    return View("~/Views/ClientSide/ProductDetail/ProductDetail.cshtml");    
}

[HttpPost]
public async Task<JsonResult> addProductReviews(string product_id,string user_id,string review)
{ int add_reviews_res=0;
  try
  { 
  if(string.IsNullOrEmpty(user_id))
  {
    return Json(new{status=0,message=$"Thêm đánh giá cho sản phẩm mã {product_id} thất bại"});    
  }
  if(string.IsNullOrEmpty(review))
  {
    return Json(new{status=0,message=$"Không được để trống thông tin đánh giá."});
  }

  Console.WriteLine("Review content here is:"+review);  
  
  
  int productId=Convert.ToInt32(product_id);
  
   add_reviews_res=await this._product.addReviews(productId,user_id,review);
  }
  catch(Exception er)
  { 
    Console.WriteLine("Add Product Review Exception:"+er.Message);

    this._logger.LogError("Add Product Review Exception:"+er.Message);
  }
  if(add_reviews_res==1)
  {
    return Json(new{status=1,message=$"Thêm đánh giá cho sản phẩm mã {product_id} thành công"});
  }
else
{
  return Json(new{status=0,message=$"Thêm đánh giá cho sản phẩm mã {product_id} thất bại"});  
}
}
}