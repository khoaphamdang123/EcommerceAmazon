
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Newtonsoft.Json;

namespace Ecommerce_Product.Controllers;
public class ProductsController:BaseController
{
 
 private readonly IBannerListRepository _banner;
 private readonly IProductRepository _product;

 private readonly ICategoryListRepository _category;

 private readonly Support_Serive.Service _sp_services;

 private readonly ILogger<ProductsController> _logger;

public ProductsController(IBannerListRepository banner,Support_Serive.Service sp_services,IProductRepository product,ICategoryListRepository category,IUserListRepository user,ILogger<ProductsController> logger):base(category,user,banner)
{
    this._banner=banner;
    this._product=product;
    this._category=category;
    this._logger=logger;
    this._sp_services=sp_services;
}

[Route("collections/category/{category_name}")]
[HttpGet]
public async Task<IActionResult> ProductsByCategory(string category_name)
{    Console.WriteLine("Category name here is:"+category_name);
     var products=await this._product.getProductByCategory(category_name);
     string select_size="12";
    //  var product_list_banner=await this._banner.findBannerByName("product_list_banner");
    //  var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    // var list_product=product_list_banner.ToList();
    // var sub_list=sub_product_list_banner.ToList();
    // string product_banner=list_product[0].Image;
    // string sub_banner=sub_list[0].Image;
    //Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList());


  //  Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
  //    for(int i=5;i>=1;i--)
  //    {
  //     List<Product> prod=await this._product.getListProductRating(i);
  //     count_product_reviews.Add(i.ToString(),prod.Count);
  //    }
    //ViewBag.count_reviews=count_reviews;
    //ViewBag.count_product_reviews=count_product_reviews;

    // ViewBag.product_banner=product_banner;
    // ViewBag.sub_banner=sub_banner;

          // ViewBag.selected_size=select_size;
          // List<string> options=new List<string>(){"12","24","36","48"};
          // ViewBag.options=options;
          // FilterProduct prod_filter=new FilterProduct("","","","","","");
          // ViewBag.filter_obj=prod_filter;
    var categories=await this._category.getAllCategory();

    Dictionary<string,int> count_product_by_cats=new Dictionary<string, int>();

    foreach(var item in categories)
    {
        int count=await this._product.countProductByCategory(item.Id);

        count_product_by_cats.Add(item.CategoryName,count);
    }

    ViewBag.category_dict=count_product_by_cats;

    var colors=await this._product.getAllColor();

    Dictionary<string,int> color_dict=new Dictionary<string, int>();

    foreach(var item in colors)
    {
        int count=await this._product.getproductByColor(item.Id);

        color_dict.Add(item.Colorname,count);
    }


    ViewBag.colors=color_dict;
    
    var brands=await this._category.getAllBrandList();
    
    ViewBag.brands=brands;
    
    var prods=await this._product.pagingProductByList(30,1,products);
    
    ViewBag.products=prods;
    
    return View("~/Views/ClientSide/Products/Products.cshtml");
}


[Route("collections/brand/{brand_name}")]
[HttpGet]
public async Task<IActionResult> ProductByBrand(string brand_name)
{
   var products=await this._product.getProductByBrand(brand_name);
     string select_size="12";
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
    string sub_banner=sub_list[0].Image;
    Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList()); 

  //  Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
  //    for(int i=5;i>=1;i--)
  //    {
  //     List<Product> prod=await this._product.getListProductRating(i);
  //     count_product_reviews.Add(i.ToString(),prod.Count);
  //    }
    ViewBag.count_reviews=count_reviews;
    //ViewBag.count_product_reviews=count_product_reviews;

    ViewBag.product_banner=product_banner;
    ViewBag.sub_banner=sub_banner;

          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
var categories=await this._category.getAllCategory();

    Dictionary<string,int> count_product_by_cats=new Dictionary<string, int>();

    foreach(var item in categories)
    {
        int count=await this._product.countProductByCategory(item.Id);
        count_product_by_cats.Add(item.CategoryName,count);
    }

    ViewBag.category_dict=count_product_by_cats;

    var colors=await this._product.getAllColor();

    Dictionary<string,int> color_dict=new Dictionary<string, int>();

    foreach(var item in colors)
    {
        int count=await this._product.getproductByColor(item.Id);

        color_dict.Add(item.Colorname,count);
    }


           ViewBag.colors=color_dict;
          
          var brands=await this._category.getAllBrandList();
          ViewBag.brands=brands;
         var prods=await this._product.pagingProductByList(30,1,products);
         ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
}

[Route("collections/sub_category/{sub_cat_id}")]
[HttpGet]
public async Task<IActionResult> ProductBySubCategory(int sub_cat_id)
{
   var products=await this._product.getProductBySubCategory(sub_cat_id);
     string select_size="12";
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
    string sub_banner=sub_list[0].Image;
    Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList());   

  //  Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
  //    for(int i=5;i>=1;i--)
  //    {
  //     List<Product> prod=await this._product.getListProductRating(i);
  //     count_product_reviews.Add(i.ToString(),prod.Count);
  //    }
    ViewBag.count_reviews=count_reviews;
    //ViewBag.count_product_reviews=count_product_reviews;

    ViewBag.product_banner=product_banner;
    ViewBag.sub_banner=sub_banner;

          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.brands=brands;
         var prods=await this._product.pagingProductByList(12,1,products);
         ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
}


[Route("prominent_products/collections")]

public async Task<IActionResult> ProminentProducts()
{
   var products=await this._product.getAllProminentProductList();
     
     string select_size="12";
     
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
     
     Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList());
    //  Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
     
    //  for(int i=5;i>=1;i--)
    //  {
    //   List<Product> prod=await this._product.getListProductRating(i);
    //   count_product_reviews.Add(i.ToString(),prod.Count);
    //  }

    var list_product=product_list_banner.ToList();
    
    var sub_list=sub_product_list_banner.ToList();
    
    string product_banner=list_product[0].Image;
    
    string sub_banner=sub_list[0].Image;
    
    ViewBag.product_banner=product_banner;
    
    ViewBag.sub_banner=sub_banner;
    
    ViewBag.count_reviews=count_reviews;

    ViewBag.current_tab=0;
    
    //ViewBag.count_product_reviews=count_product_reviews;
    
    ViewBag.selected_size=select_size;
    
    List<string> options=new List<string>(){"12","24","36","48"};
    
    ViewBag.options=options;          
    
    FilterProduct prod_filter=new FilterProduct("","","","","","");
    
    ViewBag.filter_obj=prod_filter;
    
    var cats=await this._category.getAllCategory();
    
    var brands=await this._category.getAllBrandList();
    
    ViewBag.brands=brands;

    ViewBag.type=2;
    
    var prods=await this._product.pagingProminentProduct(12,1);
    
    ViewBag.products=prods;

    return View("~/Views/ClientSide/Products/Products.cshtml");   
}

[Route("collections")]
public async Task<IActionResult> Products()
{   
     var products=await this._product.getAllProductList();
     
     string select_size="12";
     
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
     
     Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList());
    //  Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
     
    //  for(int i=5;i>=1;i--)
    //  {
    //   List<Product> prod=await this._product.getListProductRating(i);
    //   count_product_reviews.Add(i.ToString(),prod.Count);
    //  }

    var list_product=product_list_banner.ToList();
    
    var sub_list=sub_product_list_banner.ToList();
    
    string product_banner=list_product[0].Image;
    
    string sub_banner=sub_list[0].Image;
    
    ViewBag.product_banner=product_banner;
    
    ViewBag.sub_banner=sub_banner;
    
    ViewBag.count_reviews=count_reviews;

    ViewBag.current_tab=0;
    
    //ViewBag.count_product_reviews=count_product_reviews;
    
    ViewBag.selected_size=select_size;
    
    List<string> options=new List<string>(){"12","24","36","48"};
    
    ViewBag.options=options;          
    
    FilterProduct prod_filter=new FilterProduct("","","","","","");
    
    ViewBag.filter_obj=prod_filter;
        
    var categories=await this._category.getAllCategory();

    Dictionary<string,int> count_product_by_cats=new Dictionary<string, int>();

    foreach(var item in categories)
    {
        int count=await this._product.countProductByCategory(item.Id);
        count_product_by_cats.Add(item.CategoryName,count);
    }

    ViewBag.category_dict=count_product_by_cats;

    var colors=await this._product.getAllColor();

    Dictionary<string,int> color_dict=new Dictionary<string, int>();

    foreach(var item in colors)
    {
        int count=await this._product.getproductByColor(item.Id);

        color_dict.Add(item.Colorname,count);
    }


    ViewBag.colors=color_dict;
    
    var prods=await this._product.pagingProduct(30,1);

    ViewBag.products=prods;

    return View("~/Views/ClientSide/Products/Products.cshtml");    
}

[Route("collections/paging")]
[HttpGet]
  public async Task<IActionResult> ProductsPaging([FromQuery]int page_size,int current_tab=0,IEnumerable<Product> products=null,[FromQuery] int page=1,int type=1)
  {
    try{ 
        PageList<Product> prods=null;
        Console.WriteLine("TYPE HERE IS:"+type);
        Console.WriteLine("Current tab here is:"+current_tab); 

        Console.WriteLine("Page size here is:"+page_size);

        if(products==null)
        {
          Console.WriteLine("Products here is null");
        }
        else
        {
          Console.WriteLine("Products here is not null");
        }

        if(products==null)
        { 
          Console.WriteLine("Paging by product normal.");

          if(type==1)
          {
          prods=await this._product.pagingProduct(page_size,page);
          }
          else
          {
            prods = await this._product.pagingProminentProduct(page_size,page); 
          }
        }
        else
        {
          Console.WriteLine("Paging by product list.");
          prods=await this._product.pagingProductByList(page_size,page,products);
        }
          string select_size=page_size.ToString();

     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();

    string product_banner=list_product[0].Image;
    //Console.WriteLine("Product banner here:"+product_banner);
    string sub_banner=sub_list[0].Image;
   // Console.WriteLine("up to this place too");
   Dictionary<string,int> count_reviews=await this._product.countAllReview(prods.item.ToList()); 

   
    //  Dictionary<string,int> count_product_reviews=new Dictionary<string, int>();
    //  for(int i=5;i>=1;i--)
    //  {
    //   List<Product> prod=await this._product.getListProductRating(i);
    //   count_product_reviews.Add(i.ToString(),prod.Count);
    //  }
    ViewBag.count_reviews=count_reviews;
    
   ViewBag.product_banner=product_banner;

   ViewBag.current_tab=current_tab;

   if(type!=1)
   {
    ViewBag.type=2;        
   }
    

          ViewBag.sub_banner=sub_banner;
          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
             
    var categories=await this._category.getAllCategory();

    Dictionary<string,int> count_product_by_cats=new Dictionary<string, int>();

    foreach(var item in categories)
    {
        int count=await this._product.countProductByCategory(item.Id);
        
        count_product_by_cats.Add(item.CategoryName,count);
    }

    ViewBag.category_dict=count_product_by_cats;

    var colors=await this._product.getAllColor();

    Dictionary<string,int> color_dict=new Dictionary<string, int>();

    foreach(var item in colors)
    {
        int count=await this._product.getproductByColor(item.Id);

        color_dict.Add(item.Colorname,count);
    }
    ViewBag.colors=color_dict;
        ViewBag.products=prods;
        //Console.WriteLine("end here");
    return View("~/Views/ClientSide/Products/Products.cshtml");
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Product List Exception:"+er.Message);
            Console.WriteLine("Error in paging product list:"+er.Message);
        }
    return View("~/Views/ClientSide/Products/Products.cshtml");
  }


 [Route("collections/filter")]
 [HttpPost]
 public async Task<IActionResult> FilterProductByNameAndCategory(string product,string category)
 {  Console.WriteLine("Product name here is:"+product);
     var products=await this._product.filterProductByNameAndCategory(product,category);     
     string select_size="12";
     var product_list_banner=await this._banner.findBannerByName("product_list_banner");
     var sub_product_list_banner=await this._banner.findBannerByName("sub_product_banner");
    var list_product=product_list_banner.ToList();
    var sub_list=sub_product_list_banner.ToList();
    string product_banner=list_product[0].Image;
   Dictionary<string,int> count_reviews=await this._product.countAllReview(products.ToList()); 

    ViewBag.count_reviews=count_reviews;

    string sub_banner=sub_list[0].Image;
    ViewBag.product_banner=product_banner;
    ViewBag.sub_banner=sub_banner;

          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"12","24","36","48"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var brands=await this._category.getAllBrandList();
          ViewBag.brands=brands;          
         var prods=await this._product.pagingProductByList(30,1,products);
         ViewBag.products=prods;
    return View("~/Views/ClientSide/Products/Products.cshtml");
 }


 [HttpPost]
 public async Task<JsonResult> RatingStar(string user_id,string product_id,string rating)
 { int rating_star=0;
  try
  {  int productId=Convert.ToInt32(product_id);
     int ratingstar= Convert.ToInt32(rating);
     rating_star=await this._product.addRatingStar(productId,user_id,ratingstar);
  }
  catch(Exception er)
  {
   this._logger.LogError("Error in rating star:"+er.Message);     
  }
    if(rating_star==1)
    {
      return Json(new {status=1,message=$"Bạn đã đánh giá sản phẩm có mã {product_id} thành công"});
    }
    else if(rating_star==-1)
    {
      return Json(new {status=rating_star,message=$"Bạn đã đánh giá sản phẩm có mã {product_id} rồi"});
    }
    else
    {
      return Json(new {status=rating_star,message=$"Đánh giá sản phẩm có mã {product_id} thất bại"});
    }
 }



  [HttpGet]
  public async Task<IActionResult> FilterProducts(int pageSize,string prices,string category,string colors)
  {
 try{

  Console.WriteLine("pagesize:"+pageSize);
  
  string pricess = prices;

  Console.WriteLine("Prices here is:"+prices);

  Console.WriteLine("Category here is:"+category);

  Console.WriteLine("Colors here is:"+colors);
  
 List<int> prices_list = new List<int>();

 List<string> category_list= new List<string>();

 List<string> color_list=new List<string>(); 

  if(!string.IsNullOrEmpty(prices) && prices!="-")
  {
  prices_list = prices.Split('-').Select(int.Parse).ToList();
  }

 if(!string.IsNullOrEmpty(category))
 {
    category_list=category.Split(',').ToList();    
 }

 if(!string.IsNullOrEmpty(colors))
 {
    color_list = colors.Split(',').ToList();
 }

 Console.WriteLine("Come to here");

 var products=await this._product.filterProductByPriceAndBrands(category_list,prices_list,color_list);
 
  Console.WriteLine("Number of products here is:"+ products.ToList().Count);

    
  Console.WriteLine("Number of products here is:"+ products.ToList().Count);  


 var prods =await this._product.pagingProductByList(pageSize,1,products);

 Console.WriteLine("Number of prods:"+prods.item.Count);

 Console.WriteLine("Number of element here is:"+pricess);  

 Console.WriteLine("gere");

 return PartialView("~/Views/ClientSide/Products/_ProductsPartial.cshtml",prods);  

}
 catch(Exception er)
 {
    Console.WriteLine(er.Message);
 }
 return PartialView("~/Views/ClientSide/Products/_ProductsPartial.cshtml");
  }


}