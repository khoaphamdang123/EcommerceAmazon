using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]

[Route("admin")]
public class CategoryListController : BaseAdminController
{
    private readonly ILogger<CategoryListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly ICategoryListRepository _category;
   
   public CategoryListController(ICategoryListRepository category,IBannerListRepository banner,ILogger<CategoryListController> logger):base(banner)
   {
    this._category=category;
    this._logger=logger;
   }
  [Route("category_list")]
  [HttpGet]
  public async Task<IActionResult> CategoryList()
  {       string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          FilterCategory cat_filter=new FilterCategory("","","");
          ViewBag.filter_obj=cat_filter;
    try
    {    Console.WriteLine("used to stay here");  
        var cats=await this._category.pagingCategory(7,1);
    
          return View(cats);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Category List Exception:"+er.Message);
    }
    return View();
  }
   

  [Authorize(Roles ="Admin")]
  [Route("category_list/{category}/sub_category")]
  [HttpGet]
  public async Task<IActionResult> SubCategoryList(int category)
  { 
    Console.WriteLine("category id:"+category);
    ViewBag.Category_Id=category;
    string select_size="7";
    ViewBag.select_size=select_size;
    List<string> options=new List<string>(){"7","10","20","50"};
    ViewBag.options=options;
   try
   {      
          var all_sub_cat=await this._category.pagingSubCategory(category,7,1);

          return View(all_sub_cat);          
   }
   catch(Exception er)
   {
    this._logger.LogTrace("Get Sub category Exception:"+er.Message);
   }
   return View();
  }
  [Route("category_list/{category}/brand/paging")]
  
  [HttpGet]
  public async Task<IActionResult> BrandList(int category,[FromQuery]int page_size,[FromQuery] int page=1)
  {
    try
    {
         var cats=await this._category.pagingBrand(category,page_size,page);
        
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View(cats);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Get Category List Exception:"+er.Message);
        }
    return View();
  }
  [Route("category_list/{category}/sub_category/paging")]
     public async Task<IActionResult> SubCategoryList(int category,[FromQuery]int page_size,[FromQuery] int page=1)
     { try
     {  
      Console.WriteLine("Category id:"+category);
         

         var cats=await this._category.pagingSubCategory(category,7,page);
        
          List<string> options=new List<string>{"7","10","20","50"};
          
          ViewBag.options=options;
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;

          Console.WriteLine("data of subcat here is:"+cats.item.Count);
          
          return View(cats);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Get Category List Exception:"+er.Message);            
        }
    return View();
    
     }
 [Route("brand_list")]
 
 [HttpGet]
 public async Task<IActionResult> BrandList()
 {
     string select_size="7";
    ViewBag.select_size=select_size;
    List<string> options=new List<string>(){"7","10","20","50"};
    ViewBag.options=options;
  try
  {
    var all_brand = await this._category.pagingAllBrand(7,1);
    if(all_brand!=null)
    {
    return View(all_brand);
    }
    return View();
  }
  catch(Exception er)
  {
      this._logger.LogTrace("Get Full Brand List Exception:"+er.Message);
  }
  return View();
 }

  [Route("category_list/{category}/brand")]
  [HttpGet]
  public async Task<IActionResult> BrandList(int category)
  { 
    ViewBag.Category_Id=category;
    Console.WriteLine("gere");
     string select_size="7";
    ViewBag.select_size=select_size;
    List<string> options=new List<string>(){"7","10","20","50"};
    ViewBag.options=options;
  try
   {      var all_brand=await this._category.pagingBrand(category,7,1);

          return View(all_brand);
   }
   catch(Exception er)
   {
    this._logger.LogTrace("Get Brand Exception:"+er.Message);
   }
   return View();
  }
  // [Route("category_list/{category}/brand/add")]
  // [HttpGet]
  // public async Task<IActionResult> AddBrand(int category)
  // {
  //   ViewBag.Category_Id=category;
  //   var category_options=await this._category.findCategoryById(category);
  //   ViewBag.Cat_Otions=category_options.CategoryName;
  //   return View();
  // }
  [Route("brand_list/add_brand")]
  [HttpGet]
  public async Task<IActionResult> AddBrand()
  {
    var category=await this._category.getAllCategory();
    ViewBag.Cat_Otions=category;
    return View();
  }

 [Route("brand_list/add")]
 [HttpPost]
  public async Task<IActionResult> AddBrandValue(int category,string brand_name,IFormFile avatar)
  {
    try
    { 
    Console.WriteLine("Add brand did come here");
    Console.WriteLine("Category:"+category);
    Console.WriteLine("Brand name:"+brand_name);
    int res= await this._category.createBrand(category,brand_name,avatar);
    if(res==1)
     {
      TempData["Status"]=1;
      TempData["Created_Category"]=$"Đã thêm nhãn hàng thành công cho loại sản phẩm mã:{category}";
     }
     else if(res==-1)
     {
      TempData["Status"]=-1;
      TempData["Created_Category"]="Nhãn hàng này đã tồn tại trong hệ thống.";
     }
     else
     {
      TempData["ViewBag.Status"]=0;
      TempData["Created_Category"]=$"Thêm nhãn hàng thất bãi cho loại sản phẩm mã:{category}";
     } 
    }
    catch(Exception er)
    { 
       Console.WriteLine("Add Brand Exception:"+er.Message);
        this._logger.LogTrace("Add Brand Exception:"+er.Message);
    }
    //return RedirectToAction("AddBrand",new{category=category});
    return RedirectToAction("AddBrand");

  }
  [Route("brand_list/delete")]
  [HttpGet]
  public async Task<IActionResult> DeleteBrand(int id)
  {

  Console.WriteLine("Mã hàng:"+id);
try
{
  int res_delete=await this._category.deleteBrand(id);
  
  if(res_delete==1)
   {
    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = $"Xóa nhãn hàng mã {id} thành công";
   }
   else
   {
  TempData["Status_Delete"]=0;
  TempData["Message_Delete"] = $"Xóa nhãn hàng mã {id} thất bại";
   }
}
catch(Exception er)
{
  Console.WriteLine("Delete Brand Exception:"+er.Message);
}
    return RedirectToAction("BrandList");
  }
  [Route("category_list/{category}/brand/add/delete")]
  [HttpGet]
  public async Task<IActionResult> DeleteBrand(int id,int category)
  {
    try
    {
    int res_delete=await this._category.deleteBrand(id);
  if(res_delete==1)
   {
    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = $"Xóa nhãn hàng cho category {category} thành công";
   }
   else
   {
  TempData["Status_Delete"]=0;
  TempData["Message_Delete"] = $"Xóa nhãn hàng cho category {category} thất bại";
   }
 }
    catch(Exception er)
    {
        Console.WriteLine("Delete Brand Exception:"+er.Message);
        this._logger.LogTrace("Delete Brand Exception:"+er.Message);       
    }
    return RedirectToAction("BrandList",new{category=category});
  }
 [Route("category_list/{category}/sub_category/add")]
 [HttpGet]
 public async Task<IActionResult> AddSubCategory(int category)
 {   ViewBag.Category_Id=category;
     var category_obj=await this._category.findCategoryById(category);
     if(category_obj!=null)
     {
     ViewBag.Cat_Options=category_obj.CategoryName;
     }
     return View();
 }
  [Route("category_list/{category}/sub_category/add")]
  [HttpPost]
  public async Task<IActionResult> AddSubCategory(string subcategoryname,int category)
  {
    try
    {
      Console.WriteLine("Category name:"+subcategoryname);
      Console.WriteLine("category id:"+category);
      int res= await this._category.createSubCategory(subcategoryname,category);
    if(res==1)
     {
      TempData["Status"]=1;
      TempData["Created_Category"]=$"Đã thêm loại sản phẩm phụ thành công cho loại sản phẩm id:{category}";
     }
     else if(res==-1)
     {
      TempData["Status"]=-1;
      TempData["Created_Category"]="Loại sản phẩm phụ này đã tồn tại trong hệ thống";
     }
     else
     {
      TempData["Status"]=0;
      TempData["Created_Category"]="Thêm Sub Category thất bại.";
     } 
    }
    catch(Exception er)
    {
        Console.WriteLine("Add Sub Category Exception:"+er.Message);
        this._logger.LogTrace("Add Sub Category Exception:"+er.Message);
    }
    return RedirectToAction("AddSubCategory",new {category=category});
  }
  

 

  [Route("category_list/{category}/sub_category/delete")]
  [HttpGet]
  public async Task<IActionResult> DeleteSubCat(int sub_cat,int category)
  {
    try
    {
  var res_del=await this._category.deleteSubCategory(sub_cat);
  if(res_del==1)
   {
    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = $"Xóa mã {sub_cat} cho category mã {category} thành công";
   }
   else
   {
  TempData["Status_Delete"]=0;
  TempData["Message_Delete"] = $"Xóa mã {sub_cat} cho category mã {category} thất bại";
   }
  }
    catch(Exception er)
    {
       Console.WriteLine("Delete Sub Category Exception:"+er.Message);
        this._logger.LogTrace("Delete Sub Category Exception:"+er.Message); 
    }
  return RedirectToAction("SubCategoryList",new {category=category});
  }
  [Route("category_list/{category}/sub_category/update")]

  [HttpGet]

  public async Task<IActionResult> SubCategoryInfo(string sub_cat_name,int sub_cat_id,int category)
  {
    try
    {
      TempData["Subcat_Name"]=sub_cat_name;
      var cat_options=await this._category.getAllCategory();
      TempData["Cat_Options"]=cat_options;
      ViewBag.Sub_Cat_Id=sub_cat_id;
      ViewBag.Category_Id=category;
    }
    catch(Exception er)
    {
       this._logger.LogTrace("Update Sub Category View Exception:"+er.Message); 
    }
   return View();
  }

 [Route("category_list/{category}/sub_category/update")]

  [HttpPost]

  public async Task<IActionResult> SubCategoryInfo(int id,string sub_cat_name,int category)
  {
  try
  { 
    SubCategory new_sub_cat = new SubCategory{SubCategoryName=sub_cat_name,CategoryId=category};
    int res_update=await this._category.updateSubCategory(id,new_sub_cat);
    if(res_update==1)
    {
      TempData["Update_Sub_Category"]=$"Đã cập nhật loại sản phẩm phụ mã {id} cho loại sản phẩm mã {category}";
      TempData["Status"]=1;
    }
    else{
      TempData["Update_Sub_Category"]=$"Đã cập nhật loại sản phẩm phụ mã {id} cho loại sản phẩm mã {category} thất bại";
      TempData["Status"]=0;
    }
  }
  catch(Exception er)
  {
   this._logger.LogTrace("Update Sub Category Exception:"+er.Message); 
  }
  return RedirectToAction("SubCategoryInfo",new{category=category,sub_cat_name=sub_cat_name,sub_cat_id=id});
  }

   [Route("category_list")]
   [HttpPost]
   public async Task<IActionResult> CategoryList(string categoryname,string startdate,string enddate)
   {
    try
    {   Console.WriteLine("Filter category action");
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
        FilterCategory cat_obj= new FilterCategory(categoryname,startdate,enddate);
       var category_list=await this._category.filterCategoryList(cat_obj);
       var category_paging=PageList<Category>.CreateItem(category_list.AsQueryable(),1,7);
       ViewBag.filter_obj=cat_obj;  
    return View("~/Views/CategoryList/CategoryList.cshtml",category_paging);
    }
    catch(Exception er)
    {
    this._logger.LogTrace("Filter Category List Exception:"+er.Message); 
    }
    return View();
   }

   [Route("category_list/page")]
   [HttpGet]
   public async Task<IActionResult> CategoryListPaging([FromQuery]int page_size,[FromQuery] int page=1,string categoryname="",string startdate="",string enddate="")
   {
       try
        { 
          var cats=await this._category.pagingCategory(page_size,page);

          if(!string.IsNullOrEmpty(categoryname) || !string.IsNullOrEmpty(startdate) || !string.IsNullOrEmpty(enddate))
          {
          
          FilterCategory filter_obj=new FilterCategory(categoryname,startdate,enddate);
          
          var filtered_cat_list=await this._category.filterCategoryList(filter_obj);
          
          cats=PageList<Category>.CreateItem(filtered_cat_list.AsQueryable(),page,page_size);
          
          ViewBag.filter_obj=filter_obj;
          }
        
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
          
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/CategoryList/CategoryList.cshtml",cats);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get Category List Exception:"+er.Message);
        }
    return RedirectToAction("CategoryList","CategoryList");
   }
   

  [Route("category_list/export")]
  [HttpGet]
  public async Task<IActionResult> ExportExcelCategory()
  {
    try
    {
  
  var content= await this._category.exportToExcelCategory();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Category_List.xlsx");
    }
    catch(Exception er)
    {
         this._logger.LogTrace("Export Category List Exception:"+er.Message);
    }
    return RedirectToAction("CategoryList","CategoryList");
  }

  [Route("category_list/{category}/sub_category/export")]
   [HttpGet]
   public async Task<IActionResult> ExportExcelSubCategory(int category)
   {
    try
    {
  var content= await this._category.exportToExcelSubCategory(category);
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Sub_Cat_List.xlsx");    
    }
    catch(Exception er)
    {
      this._logger.LogTrace("Export Sub Category List Exception:"+er.Message);
      }
     return RedirectToAction("SubCategoryList",new{category=category});
   }
   [Route("brand_list/export")]
   [HttpGet]
   public async Task<IActionResult> ExportExcelBrands()
   {
    try
    {
 var content= await this._category.exportToExcelBrandCategory();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Brand_Category_List.xlsx");    
    
    }
    catch(Exception er)
    {
          this._logger.LogTrace("Export Brand List Exception:"+er.Message); 
    }
       return RedirectToAction("BrandList");

   }
   

   


   [Route("category_list/add")]
   
   [HttpPost]
   public async Task<IActionResult> AddCategory(AddCategoryModel category)
   {
    try
    {  
   
        int res=await this._category.createCategory(category);

   if(res==1)
     {
      ViewBag.Status=1;
      ViewBag.Created_Category="Đã thêm category thành công";
     }
     else if(res==-1)
     {
      ViewBag.Status=-1;
      ViewBag.Created_Category="Loại sản phẩm này đã tồn tại trong hệ thống";
     }
     else
     {
      ViewBag.Status=0;
      ViewBag.Created_Category="Thêm Category thất bại.";
     }
    
    }
    catch(Exception er)
    {
   this._logger.LogTrace("Add Category List Exception:"+er.Message);
    }
    return View();
   }

 [Route("category_list/delete")]
 [HttpGet]

 public async Task<IActionResult> DeleteCategory(int id)
 {
    try{

    Console.WriteLine("Delete id:"+id); 

    int res_delete=await this._category.deleteCategory(id);
    
    if(res_delete==1)
   {
    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = "Xóa Category thành công";
   }
   else
   {
  TempData["Status_Delete"]=0;
  TempData["Message_Delete"] = "Xóa Category thất bại";
   }
 }
    catch(Exception er)
    {   this._logger.LogTrace("Delete Category List Exception:"+er.Message);

        Console.WriteLine(er.Message);
    }
    return RedirectToAction("CategoryList","CategoryList");
 }

[Route("category_list/update")]
[HttpPost]
public async Task<IActionResult> UpdateCategory(AddCategoryModel category)
{
    try
    {

    Console.WriteLine("Category name:"+category.CategoryName);
    Console.WriteLine("Category Id:"+category.Id);
    int res_update=await this._category.updateCategory(category);
    if(res_update==1)
    {
      ViewBag.Status=1;
      ViewBag.Update_Message="Cập nhật Category thành công";
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Update_Message="Cập nhật Category thất bại";
    }
    var category_after=await this._category.findCategoryById(category.Id);

    return View("~/Views/CategoryList/CategoryInfo.cshtml",category_after);
    }
    catch(Exception er)
    {
         this._logger.LogTrace("Update Category List Exception:"+er.Message);
    }
    return View();
}
[Route("category_list/add")]
[HttpGet]
public  IActionResult AddCategory()
{
    return View();
}

[Route("category_list/info")]
[HttpGet]
public async Task<IActionResult> CategoryInfo(string categoryname)
{   
 try
 {
  var category = await this._category.findCategoryByName(categoryname);
  if(category!=null)
  {
    return View("~/Views/CategoryList/CategoryInfo.cshtml",category);
  }
 }
 catch(Exception er)
 {
this._logger.LogTrace("Get  Category Info Exception:"+er.Message);  
 }
 return RedirectToAction("CategoryList","CategoryList");
}
}
