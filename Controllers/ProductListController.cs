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

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class ProductListController : BaseAdminController
{
    private readonly ILogger<ProductListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

  private readonly IProductRepository _product;

  private readonly ICategoryListRepository _category;
  private readonly IWebHostEnvironment _webHostEnv;
  
   
   public ProductListController(IProductRepository product,IBannerListRepository banner,ICategoryListRepository category,ILogger<ProductListController> logger,IWebHostEnvironment webHostEnv):base(banner)
   {
    this._product=product;
    this._category=category;
    this._webHostEnv=webHostEnv;
    this._logger=logger;     
   }
  [Route("product_list")]
  [HttpGet]
  public async Task<IActionResult> ProductList()
  {       

  string id_user=this.HttpContext.Session.GetString("AdminId");

if(string.IsNullOrEmpty(id_user))
{
  return RedirectToAction("Index","LoginAdmin");
}
          string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
    try
    {  
        var prods=await this._product.pagingProduct(7,1);
        return View(prods);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Product List Exception:"+er.Message);
    }
    return View();
  }


//[Authorize(Roles ="Admin")]
  [Route("product_list/paging")]
   [HttpGet]
  public async Task<IActionResult> ProductListPaging([FromQuery]int page_size,[FromQuery] int page=1,string productname="",string brand="",string category="",string startdate="",string enddate="",string status="",int sub_cat=-1)
  {
    try{
         var prods=await this._product.pagingProduct(page_size,page);
         if(!string.IsNullOrEmpty(productname)||!string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(startdate) || !string.IsNullOrEmpty(enddate) || !string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(status))
         {
            FilterProduct prod=new FilterProduct(productname,startdate,enddate,category,brand,status);
            var filter_prods=await this._product.filterProduct(prod);
            prods=PageList<Product>.CreateItem(filter_prods.AsQueryable(),page,page_size);
            ViewBag.filter_obj=prod; 
            Console.WriteLine("Paging did come here");
         }
         if(sub_cat!=-1)
         {
            var filter_prods=await this._product.getProductBySubCategory(sub_cat);
            prods =PageList<Product>.CreateItem(filter_prods.AsQueryable(),page,page_size);
            ViewBag.SubCat=sub_cat;
         }
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;

          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
        
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/ProductList/ProductList.cshtml",prods);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Product List Exception:"+er.Message);
        }
    return View();
  }

//[Authorize(Roles ="Admin")]

   [Route("product_list")]
   [HttpPost]
   public async Task<IActionResult> ProductList(FilterProduct products)
   {
    try
    {   
    string startdate=products.StartDate;

    string enddate = products.EndDate;

 if(!string.IsNullOrEmpty(startdate))
 {
   string[] reformatted=startdate.Trim().Split('-');

   startdate=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];   
 }
     if(!string.IsNullOrEmpty(enddate))
{ 
   string[] reformatted=enddate.Trim().Split('-');

   enddate=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }      string select_size="7"; 
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
         var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
       var product_list=await this._product.filterProduct(products);
       var product_paging=PageList<Product>.CreateItem(product_list.AsQueryable(),1,7);
       ViewBag.filter_obj=products;  
    return View("~/Views/ProductList/ProductList.cshtml",product_paging);
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Filter Product List Exception:"+er.Message); 
    }
    return View();
   }
  
  [Route("product_list/delete")]
  [HttpGet]
  public async Task<IActionResult> DeleteProduct(int id)
  {
    try
    {
      int res=await this._product.deleteProduct(id);
      
      if(res==0)
      {
        TempData["Status_Delete"]=0;
        TempData["Message_Delete"]=$"Xóa sản phẩm mã {id} thất bại";
      }
      else{
          this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} delete product {id} successfully");

         TempData["Status_Delete"]=1;
         
         TempData["Message_Delete"]=$"Xóa sản phẩm mã {id} thành công"; 
      }
    }
    catch(Exception er)
    {
       this._logger.LogTrace("Remove Product Exception:"+er.Message); 
    }
    return RedirectToAction("ProductList","ProductList");
  }
 [Route("product_list/export")]
 [HttpGet]
  public async Task<IActionResult> ExportToExcel()
  {
    try
    {
     var content= await this._product.exportToExcelProduct();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Products.xlsx");
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Export Product Excel Exception:"+er.Message); 
    }
    return RedirectToAction("ProductList","ProductList");
  }

[Route("product_list/filter")]
[HttpGet]
public async Task<IActionResult> ProductsByName(string product_name)
{    Console.WriteLine("Product name here is:"+product_name);
     var products=await this._product.filterProductByNameAndCategory(product_name,"");
     string select_size="7";

          ViewBag.selected_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          FilterProduct prod_filter=new FilterProduct("","","","","","");
          ViewBag.filter_obj=prod_filter;         
          var cats=await this._category.getAllCategory();
          var brands=await this._category.getAllBrandList();
          ViewBag.CatList=cats;
          ViewBag.BrandList = brands;
          ViewBag.StatusList = new List<string>{"Hết hàng","Còn hàng"};
          var prods=await this._product.pagingProductByList(products.ToList().Count,1,products);
          return View("~/Views/ProductList/ProductList.cshtml",prods);
}

  [Route("prominent_product_list/sort")]
  [HttpGet]
  public async Task<IActionResult> SortProminentProductList()
  {
    try
    {
        var prods=await this._product.getAllProminentProductList();
        
        return View(prods);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Prominent Product List Exception:"+er.Message);
    }
    return View();    
  }

  [Route("prominent_product_list/sort")]
  [HttpPost]
  public async Task<JsonResult> SortProminentProduct(List<string> product_list)
  {
    Console.WriteLine("Product Prominent Json did come to here");
  try
   {

    List<Product> products=new List<Product>();

    foreach(var product_id in product_list)
    {
      var product=await this._product.findProductById(Convert.ToInt32(product_id));
      
      if(product!=null)
      {
        products.Add(product);
      }

    }

   await this._product.saveProminentProduct(products);

   }
   catch(Exception er)
   {
    this._logger.LogTrace("Sort Prominent Product Exception:"+er.Message);
    return Json(new{status=0,message=er.Message});
   }
   return Json(new{status=1,message="Sắp xếp thành công"});
  }

  [Route("product_list/sort")]
  [HttpGet]
  public async Task<IActionResult> SortProductList()
  {
    try
    {  
        var prods=await this._product.getAllProductList();

        return View(prods);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Product List Exception:"+er.Message);
    }  
    return View();            
  }

  [HttpPost]
  [Route("product_list/sort")]
  public async Task<JsonResult> SortProduct(List<string> product_list)
  {
  Console.WriteLine("Product Json did come to here");
  try
   {
    List<Product> products=new List<Product>();
    foreach(var product_id in product_list)
    {
      var product=await this._product.findProductById(Convert.ToInt32(product_id));
      if(product!=null)
      {
        products.Add(product);
      }
    }

   await this._product.SaveProduct(products);

   }
   catch(Exception er)
   {
    this._logger.LogTrace("Sort Product Exception:"+er.Message);
    return Json(new{status=0,message=er.Message});
   }
   return Json(new{status=1,message="Sắp xếp thành công"});
  }

  [Route("product_list/add")]
  [HttpGet]
  public async Task<IActionResult> AddProductList()
  { 
    var category_list=await this._category.getAllCategory();
    
    var brand_list = await this._category.getAllBrandList();

    List<SubCategory> sub_cat_list=new List<SubCategory>();

    foreach(var cat in category_list)
    {
      foreach(var sub_cat in cat.SubCategory)
      {
        sub_cat_list.Add(sub_cat);
      }
    }
    
    ViewBag.CategoryList=category_list;
    
    ViewBag.BrandList=brand_list;
    
    ViewBag.SubCatList=sub_cat_list;
    
    return View();
  }

  
  [Route("product_list/add_val")]
  [HttpPost]
  public async Task<JsonResult> GetSampleData(AddProductModel model)
{ Console.WriteLine("used to stay here:"+model.ProductName);
    
    StatusResponse response_data; 

    int created_res = await this._product.addNewProduct(model);

    if (created_res == 0)
    {  
        response_data = new StatusResponse 
        {
            Status = 0,
            Message = "Thêm sản phẩm thất bại"
        };
    }
    else if (created_res == -1)
    {
        response_data = new StatusResponse  
        {
            Status = -1,
            Message = "Sản phẩm đã tồn tại trong hệ thống"
        };
    }
    else
    {
        response_data = new StatusResponse  
        {
            Status = 1,
            Message = "Thêm sản phẩm thành công"
        };
    }
    
    return Json(response_data);
    
}


 

// [Route("product_list/add_val")]
//  [HttpPost]
//  public async Task<JsonResult> AddProductListJson(AddProductModel model)
//  {
//   try
//   {
//  int created_res=await this._product.addNewProduct(model);
//  if(created_res==0)
//  {  
//  var response_data={
//     "status":"0",
//     "created_product":"Thêm sản phẩm thất bại"
//       };
//  }
//  else if(created_res==-1)
//  {
//  var response_data={
//     "status":"-1",
//     "created_product":"Sản phẩm đã tồn tại trogng hệ thống"
//       };
//  }
//  else
//  {
//   var response_data={
//     "status":"1",
//     "created_product":"Thêm sản phẩm"
//       };
//  }
//  }
//   catch(Exception er)
//   {
//     this._logger.LogTrace("Add Product Exception:"+er.Message);
//     Console.WriteLine("Add Product List Exception:"+er.Message);
//   }
//   return Json(response_data);
//  }


//   [Route("product_list/add")]
//   [HttpPost]
//   public async Task<IActionResult> AddProductList(AddProductModel model)
//   {
//   try
//   {

//  int created_res=await this._product.addNewProduct(model);

//  if(created_res==0)
//  {  
//   ViewBag.Status=0;
//   ViewBag.Created_Product="Thêm sản phẩm thất bại";
//  }
//  else if(created_res==-1)
//  {
//   ViewBag.Status=-1;
//   TempData["Status"]="-1";
//   ViewBag.Created_Product="Sản phẩm đã tồn tại trong hệ thống";
//  }
//  else
//  {
//  ViewBag.Status=1;
//  ViewBag.Created_Product="Thêm sản phẩm thành công";
//  }
//  }
//   catch(Exception er)
//   {
//     this._logger.LogTrace("Add Product Exception:"+er.Message);
//     Console.WriteLine("Add Product List Exception:"+er.Message);
//   }
// var category_list=await this._category.getAllCategory(); 
    
//     var brand_list = await this._category.getAllBrandList();

//     List<SubCategory> sub_cat_list=new List<SubCategory>();

//     foreach(var cat in category_list)
//     {
//       foreach(var sub_cat in cat.SubCategory)
//       {
//         sub_cat_list.Add(sub_cat);
//       }
//     }
//     ViewBag.CategoryList=category_list;
//     ViewBag.BrandList=brand_list;
//     ViewBag.SubCatList=sub_cat_list;
//   return View();
//   }

  [Route("product_list/{id}/variant")]
  [HttpGet]
  public async Task<IActionResult> VariantList(int id)
  {
      string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
         var variant_list=await this._product.pagingVariant(id,7,1);
         return View(variant_list);
  }
  

 [Route("product_list/sub_category/{sub_cat}")]

 [HttpGet]

 public async Task<IActionResult> ProductListBySubCategory(int sub_cat)
 {
    var category_list=await this._category.getAllCategory(); 
    
    var brand_list = await this._category.getAllBrandList();
  
    List<SubCategory> sub_cat_list=new List<SubCategory>();
  
    foreach(var cat in category_list)
    {
      foreach(var sub_cat_ob in cat.SubCategory)
      {
        sub_cat_list.Add(sub_cat_ob);
      }
    }
    ViewBag.CategoryList=category_list;
    ViewBag.BrandList=brand_list;
    ViewBag.SubCatList=sub_cat_list;
    ViewBag.SubCat=sub_cat;    
    var product_list=await this._product.getProductBySubCategory(sub_cat);
    return View("~/Views/ProductList/ProductList.cshtml",product_list);
 }


  [Route("product_list/{id}/variant/paging")]
  [HttpGet]
  public async Task<IActionResult> VariantListPaging(int id,int page_size,int page=1)
  { 
    
    Console.WriteLine("In Paging variant");
     var variant_list=await this._product.pagingVariant(id,page_size,page);
     List<string> options=new List<string>(){"7","10","20","50"};
          
     ViewBag.options=options;
        
          
     string select_size=page_size.ToString();
          
      ViewBag.select_size=select_size;
          
      return View("~/Views/ProductList/VariantList.cshtml",variant_list);
  }

  [Route("product_list/{id}/product_info")]
  
  [HttpGet]
  public async Task<IActionResult> ProductInfo(int id)
  {
    var category_list=await this._category.getAllCategory(); 
    
    var brand_list = await this._category.getAllBrandList();

    List<SubCategory> sub_cat_list=new List<SubCategory>();

    foreach(var cat in category_list)
    {
      foreach(var sub_cat in cat.SubCategory)
      {
        sub_cat_list.Add(sub_cat);
      }
    }
    ViewBag.CategoryList=category_list;
    ViewBag.BrandList=brand_list;
    ViewBag.SubCatList=sub_cat_list;
    var product=await this._product.findProductById(id);
    return View("~/Views/ProductList/ProductInfo.cshtml",product);
  }




[HttpPost("/image/upload")]
public async Task<IActionResult> UploadImage(IFormFile file)
{  
  Console.WriteLine("did come to upload image here");
    if (file == null || file.Length == 0)
    {
        return BadRequest("No file uploaded.");
    }

    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadImages");
    
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }

    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

    var filePath = Path.Combine(uploadPath, fileName);    

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var imageUrl = $"{Request.Scheme}://{Request.Host}/UploadImages/{fileName}";

    this._logger.LogInformation("Image URL:"+imageUrl);
    
    return Ok(new { link = imageUrl });
}

[HttpPost("/video/upload")]

public async Task<IActionResult> UploadVideo(IFormFile file)
{
  Console.WriteLine("Upload Video did come to here");

    if(file==null || file.Length==0)
    {
      return BadRequest("No file uploaded");
    }

    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadVideos");
    
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }
    
    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

    var filePath = Path.Combine(uploadPath, fileName);
    
    using(var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }
    var videoUrl = $"{Request.Scheme}://{Request.Host}/UploadVideos/{fileName}";

    this._logger.LogInformation("Video URL:"+videoUrl);    
    
    return Ok(new { link = videoUrl }); 
}



[Route("product_list/product_info")]
[HttpPost]
  public async Task<JsonResult> GetSampleInfo(AddProductModel product)
{ 
  
  StatusResponse response_data; 

  Console.WriteLine("sample info:"+product.StatDescription);

  int created_res =await this._product.updateProduct(product.Id,product);

    if (created_res == 0)
    {  
        response_data = new StatusResponse 
        {
            Status = 0,
            Message = "Cập nhật sản phẩm thất bại"
        };
    }
    else
    {
        response_data = new StatusResponse  
        {
            Status = 1,
            Message = "cập nhật sản phẩm thành công"
        };
    }

    return Json(response_data);    
}




//  [Route("product_list/{id}/product_info")]
//  [HttpPost]
// public async Task<IActionResult> ProductInfo(int id,AddProductModel product)
// {
// try
// {

//   Console.WriteLine("Id for this product is:"+id);
// int update_res=await this._product.updateProduct(id,product);

//  if(update_res==0)
//  {  
//   ViewBag.Status=0;
//   ViewBag.Updated_Product="Cập nhật sản phẩm thất bại";
//  }
//  else
//  {
//  ViewBag.Status=1;
//  ViewBag.Updated_Product="Cập nhật sản phẩm thành công";
//  }
//  var product_ob=await this._product.findProductById(id);
//  return View("~/Views/ProductList/ProductInfo.cshtml",product_ob);
// }
// catch(Exception er)
// {
//   this._logger.LogTrace("Update Product Info Exception:"+er.Message);
// }
//   return View();
//  }
}
