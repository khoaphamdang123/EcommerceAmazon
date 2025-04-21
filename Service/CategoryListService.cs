using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http.Connections;
using Newtonsoft.Json;

namespace Ecommerce_Product.Service;
public class CategoryListService:ICategoryListRepository
{
  private readonly EcommerceshopContext _context;
    private readonly IWebHostEnvironment _webHostEnv;
    private readonly Support_Serive.Service _support_service;

 // private readonly Logger<CategoryListService> _logger;
  public CategoryListService(EcommerceshopContext context,IWebHostEnvironment webHostEnv,Support_Serive.Service support_service)
  {
    this._context=context;
    this._webHostEnv=webHostEnv;
    this._support_service=support_service;
  }


public async Task<IEnumerable<Category>> getAllCategory()
{    
    try
    {
     var categories=this._context.Categories.Include(c=>c.SubCategory).ToList();
     return categories;     
    }
    catch(Exception er)
    {
        Console.WriteLine("Get All Category Exception:"+er.Message);
    
    }
    return null;
}
public async Task<IEnumerable<Category>> filterCategoryList(FilterCategory category)
{ 
  var cat_list=this._context.Categories.ToList();
  
  try
  {
   string category_name=category.Category;
   string start_date=category.StartDate;
   string end_date = category.EndDate;
   if(!string.IsNullOrEmpty(category_name))
   {
    category_name=category_name.Trim();
    cat_list= cat_list.Where(c=>c.CategoryName.ToLower()==category_name.ToLower()).ToList();
   }
   if(!string.IsNullOrEmpty(start_date) && string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    cat_list= cat_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(start_date,out var lowerDate) && startDate.Date==lowerDate.Date).ToList();
   }
   else if(string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   { 
    
    end_date=end_date.Trim();
   
    cat_list= cat_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(end_date,out var upperDate) && startDate.Date==upperDate.Date).ToList();
   }
   else if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    end_date=end_date.Trim();
    cat_list=cat_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var createdDate)&& DateTime.TryParse(start_date,out var startDate) && DateTime.TryParse(end_date,out var endDate) && createdDate>=startDate && createdDate<=endDate).ToList();
   }
  }
  catch(Exception er)
  {
    Console.WriteLine("Filter Category Exception:"+er.Message);
  }
  return cat_list;
}

public async Task<PageList<Category>> pagingCategory(int page_size,int page)
{
 
   IEnumerable<Category> all_cat= await this.getAllCategory();

   List<Category> cats=all_cat.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var cat_list=PageList<Category>.CreateItem(cats.AsQueryable(),page,page_size);
   
   return cat_list;
}


public async Task<bool> checkCategoryExist(string categoryname)
{  bool is_existed=false;
    try
    {
     var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.CategoryName==categoryname);
     if(cat!=null)
     {
        is_existed=true;
     }
    }
    catch(Exception er)
    {
    Console.WriteLine("Check Category Exist Exception:"+er.Message); 
    }
    return is_existed;
}

public async Task<int> createCategory(AddCategoryModel category)
{ int create_res=0;
    try
    {
    
    bool is_existed=await checkCategoryExist(category.CategoryName);

    if(is_existed)
    {   create_res=-1;
        return create_res;
    }
     string folder_name="UploadImageCategory";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

   if(!Directory.Exists(upload_path))
   {
    Directory.CreateDirectory(upload_path);
   }
   string avatar_url="";
  var avatar_obj=category.Avatar;
  if(avatar_obj!=null)
  {
   string file_name=Guid.NewGuid()+"_"+Path.GetFileName(avatar_obj.FileName);
  
   string file_path=Path.Combine(upload_path,file_name);

   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await avatar_obj.CopyToAsync(fileStream);
   } 
   avatar_url=file_path;
  }
     string avatar=avatar_url;
     
     string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     string updated_date =DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     var new_cat=new Category{CategoryName= category.CategoryName,CreatedDate=created_date,UpdatedDate=updated_date,Avatar=avatar};
     
    await this._context.Categories.AddAsync(new_cat);

    await saveChange();

     create_res=1;
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    }
    catch(Exception er)
    {   create_res=0;
        Console.WriteLine("Create Category Exception:"+er.Message);
    }
    return create_res;
}


public async Task<Category> findCategoryById(int id)
{
    try
    {
      var category=await this._context.Categories.Include(c=>c.SubCategory).FirstOrDefaultAsync(c=>c.Id==id);
      if(category!=null)
      {
        return category;
      }
    }
    catch(Exception er)
    {
    Console.WriteLine("Find Category By Id Exception:"+er.Message);
    }
    return null;
}


public async Task<int> deleteCategory(int id)
{
    int delete_res=0;

    try
    {
      var category=await this._context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
      string curr_avatar=category.Avatar;
      if(category!=null)
      {
        this._context.Categories.Remove(category);
        await this.saveChange();
      if(!string.IsNullOrEmpty(curr_avatar))
      {
        await this._support_service.removeFiles(curr_avatar);
      }
        delete_res=1;        
      }
    }
    catch(Exception er)
    {   delete_res=0;
        Console.WriteLine("Delete Category Exception:"+er.Message);
    }
    return delete_res;
}

public async Task<int> updateCategory(AddCategoryModel category)
{
    int update_res=0;
    try
    {
        int cat_id=category.Id;
        var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.Id==cat_id);
        if(cat!=null)
        {
           cat.CategoryName=category.CategoryName;
           string curr_avatar=cat.Avatar;
  string folder_name="UploadImageCategory";
        
   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

   if(!Directory.Exists(upload_path))
   {
    Directory.CreateDirectory(upload_path);
   }
   string avatar_url="";
  var avatar_obj=category.Avatar;
  if(avatar_obj!=null)
  {
   string file_name=Guid.NewGuid()+"_"+Path.GetFileName(avatar_obj.FileName);
  
   string file_path=Path.Combine(upload_path,file_name);

   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await avatar_obj.CopyToAsync(fileStream);
   } 
   avatar_url=file_path;
  cat.Avatar=avatar_url;
  }
           cat.UpdatedDate=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
           this._context.Categories.Update(cat);
           await this.saveChange();
           update_res=1;
          await this._support_service.removeFiles(curr_avatar);
        }
    }
    catch(Exception er)
    {
    update_res=0;
   Console.WriteLine("Update Category Exception:"+er.Message); 
    }
    return update_res;
}

public async Task<Category> findCategoryByName(string categoryname)
{
    try
    {
      var category=await this._context.Categories.FirstOrDefaultAsync(c=>c.CategoryName==categoryname);
      if(category!=null)
      {
        return category;
      }
    }
    catch(Exception er)
    {
       Console.WriteLine("Find Category By Name Exception:"+er.Message); 
    }
    return null;
}

public async Task<IEnumerable<SubCategory>> findSubCategoryByCat(string category)
{
    try
    {
       var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.CategoryName==category);
       if(cat!=null)
       {
        return cat.SubCategory;
       }
    }
    catch(Exception er)
    {
        Console.WriteLine("Find Sub Cat Exception:"+er.Message);
    }
    return null;
}

public async Task<IEnumerable<SubCategory>> findSubCategoryById(int category)
{
    try
    {
       var cat=await this._context.Categories.Include(c=>c.SubCategory).FirstOrDefaultAsync(c=>c.Id==category);
       if(cat!=null)
       {Console.WriteLine("sub cat here is:"+cat.SubCategory.Count.ToString());
        return cat.SubCategory;
       }
    }
    catch(Exception er)
    {
        Console.WriteLine("Find Sub Cat Exception:"+er.Message);
    }
    return null;
}



public async Task<SubCategory> findSingleSubcat(int sub_category)
{
    var sub_cat=await this._context.SubCategory.FirstOrDefaultAsync(c=>c.Id==sub_category);
    return sub_cat;
}

public async Task<PageList<SubCategory>> pagingSubCategory(int category,int page_size,int page)
{
   var all_sub_cat= await this.findSubCategoryById(category);

   if(all_sub_cat!=null)
   {

   var cats=all_sub_cat.OrderByDescending(u=>u.Id).ToList(); 
   //var users=this._userManager.Users;   
   var cat_list=PageList<SubCategory>.CreateItem(cats.AsQueryable(),page,page_size);

   Console.WriteLine("Nmber of cat list:"+cat_list.item.Count());
     
   return cat_list;
   }
   return null;
}

public async Task<bool> checkSubCatExist(string sub_cat)
{ bool is_exist=false;
  var sub_cat_obj=await this._context.SubCategory.FirstOrDefaultAsync(c=>c.SubCategoryName==sub_cat);
  if(sub_cat_obj!=null)
  {
    is_exist=true;
  }
  return is_exist;
}

public async Task<int> createSubCategory(string subcategoryname,int categoryid)
{
int create_res=0;
    try
    {
    
    bool is_existed=await checkSubCatExist(subcategoryname);

    if(is_existed)
    {   create_res=-1;
        return create_res;
    }
     string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     string updated_date =DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     var new_cat=new SubCategory{SubCategoryName= subcategoryname,CategoryId=categoryid,CreatedDate=created_date,UpdatedDate=updated_date};
     
    await this._context.SubCategory.AddAsync(new_cat);

    await saveChange();

     create_res=1;
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    }
    catch(Exception er)
    {   create_res=0;
        Console.WriteLine("Create Category Exception:"+er.Message);
    }
    return create_res;
}

public async Task<IEnumerable<Brand>> getAllBrandList()
{
    // var brand_list=this._context.Brands.ToList();
    var brand_list= this._context.CategoryBrandDetail.Include(c=>c.Brand).Select(x=>x.Brand).ToList();

    return brand_list;
}



public async Task<IEnumerable<CategoryBrandDetail>> findBrandById(int category)
{
    var brand_list= this._context.CategoryBrandDetail.Where(c=>c.CategoryId==category).Include(c=>c.Brand).Include(c=>c.Category);
   
    return brand_list;
 } 

public async Task<PageList<CategoryBrandDetail>> pagingBrand(int category,int page_size,int page)
{
   var all_brand= await this.findBrandById(category);
   
   var cats=all_brand.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var cat_list=PageList<CategoryBrandDetail>.CreateItem(cats.AsQueryable(),page,page_size);
   
   return cat_list;
}

public async Task<PageList<CategoryBrandDetail>> pagingAllBrand(int page_size,int page)
{
    var all_brand = this._context.CategoryBrandDetail.Include(c=>c.Brand).Include(c=>c.Category).ToList();

    var brands=all_brand.OrderByDescending(u=>u.Id).ToList();

    var brand_list = PageList<CategoryBrandDetail>.CreateItem(brands.AsQueryable(),page,page_size);
    return brand_list;
}


public async Task<bool> checkBrandExist(string brand_name,int category)
{
    bool is_exist=false;
    
    var brand=await this._context.CategoryBrandDetail.Include(c=>c.Brand).Include(c=>c.Category).FirstOrDefaultAsync(c=>c.Brand.BrandName==brand_name && c.Category.Id==category);
    
    if(brand!=null)
    {
        is_exist=true;
    }
    return is_exist;
}

public async Task<int> createBrand(int category,string brand_name,IFormFile avatar)
{
    int create_res=0;
    try
    {
    bool is_existed=await checkBrandExist(brand_name,category);

    if(is_existed)
    {   create_res=-1;
        return create_res;
    }
 string folder_name="UploadImageBrand";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

   if(!Directory.Exists(upload_path))
   {
    Directory.CreateDirectory(upload_path);
   }
  string avatar_url="";
  
  var avatar_obj=avatar;
  
  if(avatar_obj!=null)
  {Console.WriteLine("did not null hre");
   string file_name=Guid.NewGuid()+"_"+Path.GetFileName(avatar_obj.FileName);
  
   string file_path=Path.Combine(upload_path,file_name);

   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await avatar_obj.CopyToAsync(fileStream);
   } 
   avatar_url=file_path;
  }
  else{
    Console.WriteLine("did null here");
  }
    
     string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     string updated_date =DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     var new_brand=new Brand{BrandName=brand_name,CreatedDate=created_date,UpdatedDate=updated_date,Avatar=avatar_url};
     
     Console.WriteLine("Category id:"+category);

     var cat=await this.findCategoryById(category);

    var cat_brand_ob=new CategoryBrandDetail{
        Category=cat,
        Brand=new_brand
    };

    await this._context.CategoryBrandDetail.AddAsync(cat_brand_ob);
    
    await saveChange();

     create_res=1;
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    }
    catch(Exception er)
    {   create_res=0;
        Console.WriteLine("Create Brand Exception:"+er.Message);
    }
    return create_res;
}

public async Task<int> deleteBrand(int brand_category)
{  int delete_res=0;
    try
    {   
        var brand_cat_detail=await this._context.CategoryBrandDetail.FirstOrDefaultAsync(c=>c.Id==brand_category);
        string curr_avatar= brand_cat_detail.Brand?.Avatar;
      if(brand_cat_detail!=null)
      { 
        
      delete_res=1;
      
      Console.WriteLine("Brand cat here");

        this._context.CategoryBrandDetail.Attach(brand_cat_detail);
        
        this._context.CategoryBrandDetail.Remove(brand_cat_detail);
        
        // this._context.Brands.Remove(brand);

        Console.WriteLine("Did remove brand");
        
        await this.saveChange();        
       
       if(!string.IsNullOrEmpty(curr_avatar))
       {
        await this._support_service.removeFiles(curr_avatar);        
       }
      }
    }
    catch(Exception er)
    {
        Console.WriteLine("Delete Brand Exception:"+er.Message);
    }
    return delete_res;
}


public async Task<int> deleteSubCategory(int sub_category)
{int res_del=0;
    try
    {  
       var sub_cat=await this.findSingleSubcat(sub_category);              
       if(sub_cat!=null)
       { res_del=1;
         this._context.SubCategory.Remove(sub_cat);
         await this.saveChange();         
       }
    }
    catch(Exception er)
    {   res_del=0;
        Console.WriteLine("Delete Sub Category Exception");
    }
    return res_del;
}

public async Task<int> updateSubCategory(int id,SubCategory SubCategory)
{  
    int res_update=0;
    
    try
    {
   var sub_cat=await this.findSingleSubcat(id);
      
   if(sub_cat!=null)
   {
    sub_cat.SubCategoryName=SubCategory.SubCategoryName;
    sub_cat.CategoryId=SubCategory.CategoryId;
    sub_cat.UpdatedDate=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
    this._context.Update(sub_cat);
    await this.saveChange();
    res_update=1;    
   }
    }
    catch(Exception er)
    {
    Console.WriteLine("Update Sub Category Exception:"+er.InnerException??er.Message);
    }
   return res_update;
}

public async Task<MemoryStream> exportToExcelCategory()
 {
  using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("Category");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên Loại sản phẩm";
    worksheet.Cells[1,3].Value = "Ngày tạo";
    worksheet.Cells[1,4].Value="Ngày cập nhật";
    var categories=await this.getAllCategory();
    if(categories!=null)
    {
    List<Category> list_category=categories.ToList();
    for(int i=0;i<list_category.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_category[i].CategoryName;
    
    worksheet.Cells[i+2,3].Value=list_category[i].CreatedDate;
    
    worksheet.Cells[i+2,4].Value=list_category[i].UpdatedDate;
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  return stream;
  }
 }

public async Task<MemoryStream> exportToExcelSubCategory(int category)
{
 using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("Sub_Category");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên Loại sản phẩm phụ";
    worksheet.Cells[1,3].Value = "Tên loại sản phẩm";
    worksheet.Cells[1,4].Value="Ngày tạo";
    worksheet.Cells[1,5].Value="Ngày cập nhật";

    var categories=await this.findCategoryById(category);
    if(categories!=null)
    {
    List<SubCategory> list_category=categories.SubCategory.ToList();
    for(int i=0;i<list_category.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_category[i].SubCategoryName;
    
    worksheet.Cells[i+2,3].Value=categories.CategoryName;
    
    worksheet.Cells[i+2,4].Value=list_category[i].CreatedDate;
    
    worksheet.Cells[i+2,5].Value=list_category[i].UpdatedDate;
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  return stream;
  }
}

public async Task<MemoryStream> exportToExcelBrandCategory()
{
 using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("Brand_Category");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên nhãn hàng";
    worksheet.Cells[1,3].Value = "Loại sản phẩm";
    worksheet.Cells[1,4].Value="Ngày tạo";
    worksheet.Cells[1,5].Value = "Ngày cập nhật";

    List<CategoryBrandDetail> brands=this._context.CategoryBrandDetail.Include(c=>c.Category).Include(c=>c.Brand).ToList();
    if(brands!=null)
    {
    for(int i=0;i<brands.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=brands[i].Brand.BrandName;
    
    worksheet.Cells[i+2,3].Value=brands[i].Category.CategoryName;
    
    worksheet.Cells[i+2,4].Value=brands[i].Brand.CreatedDate;
    
    worksheet.Cells[i+2,5].Value=brands[i].Brand.UpdatedDate;
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  return stream;
  } 
}




public async Task saveChange()
{
 await this._context.SaveChangesAsync();
}
}