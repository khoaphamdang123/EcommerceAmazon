using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using System.Collections.Frozen;
using StackExchange.Redis;
using System.Text.Json;
using Newtonsoft.Json;
using System.Globalization;


namespace Ecommerce_Product.Service;

public class ProductService:IProductRepository
{
    private readonly EcommerceshopContext _context;

    private readonly IWebHostEnvironment _webHostEnv;

    private readonly Support_Serive.Service _sp_services;

    private readonly IConnectionMultiplexer _redis;

    private readonly IDatabase _db;
  public ProductService(EcommerceshopContext context,IWebHostEnvironment webHostEnv,IConnectionMultiplexer redis,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._webHostEnv=webHostEnv;
    this._redis=redis;
    this._db=this._redis.GetDatabase();
    this._sp_services=sp_services;
  }
  

  public async Task SaveProduct(List<Product> products)
  {
    try
    {
       int sort_id=0;
     foreach(var product in products)
     { 
      sort_id+=1;
      product.SortId=sort_id;
      this._context.Products.Update(product);
     } 
   await this.saveChanges();
    }
    catch(Exception er)
    {
      Console.WriteLine("Save Product Exception:"+er.Message);
    }
  }

  public async Task saveProductRedis(List<Product> products)
  {
  try
  {  if(await this._db.KeyExistsAsync("products"))
  { Console.WriteLine("did delete all here");
    await this._db.KeyDeleteAsync("products");
  }
    var json = JsonConvert.SerializeObject(products,new JsonSerializerSettings{
      ReferenceLoopHandling=ReferenceLoopHandling.Ignore
    });

    await this._db.StringSetAsync("products",json);

  }
  catch(Exception er)
  {
    Console.WriteLine("Redis save Exception:"+er.Message);
  }
  }
 
  
  public async Task saveProminentProduct(List<Product> products)
  {
    try
    {
     int sort_prominent_id=0;
     foreach(var product in products)
     { 
      sort_prominent_id+=1;
      product.SortProminentId=sort_prominent_id;
      this._context.Products.Update(product);
     } 
   await this.saveChanges();
    }
    catch(Exception er)
    {
      Console.WriteLine("Save prominent product list:"+er.Message);
    }
  }

  public async Task saveProminentProductRedis(List<Product> products)
  {
    try
    {
 if(await this._db.KeyExistsAsync("prominent_products"))
  { Console.WriteLine("did delete all here");
    await this._db.KeyDeleteAsync("prominent_products");
  }
    var json = JsonConvert.SerializeObject(products,new JsonSerializerSettings{
      ReferenceLoopHandling=ReferenceLoopHandling.Ignore
    });

    await this._db.StringSetAsync("prominent_products",json);
    }
  catch(Exception er)
  {
    Console.WriteLine("Redis save Exception:"+er.Message);
  }
  }

  public async Task<List<Product>> getProductRedis()
  {
    var products_json=await this._db.StringGetAsync("products");
     

    if(string.IsNullOrEmpty(products_json))
    {

      var products=await this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(c=>c.SubCat).Include(p=>p.ProductImages).ToListAsync();
      await this.saveProductRedis(products);
      return products;
    }
    
    return JsonConvert.DeserializeObject<List<Product>>(products_json);    

  }

  public async Task<List<Product>> getProminentProductRedis()
  {
    var product_json = await this._db.StringGetAsync("prominent_products");

    if(string.IsNullOrEmpty(product_json))
    {
      var products=await this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(c=>c.SubCat).Include(p=>p.ProductImages).ToListAsync();
      await this.saveProminentProductRedis(products);
      return products;    
    }
      return JsonConvert.DeserializeObject<List<Product>>(product_json);    
  }

  public async Task<IEnumerable<Product>> getProductList()
  {
     try
    {
       var products=await this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(c=>c.SubCat).Include(p=>p.ProductImages).ToListAsync();
      string json_products=await this._db.StringGetAsync("products");
      if(string.IsNullOrEmpty(json_products))
      {
 Console.WriteLine("did come to this place");
    await this.saveProductRedis(products);
    }
       return products;
    }
    catch(Exception er)
    {
        Console.WriteLine("Get all product:"+er.Message);
    }
    return null;
  }

  public async Task<int> getproductByColor(int id)
    {
   try
   {
    int colors = await this._context.Products.Include(p=>p.Variants).ThenInclude(c=>c.Color).CountAsync(c=>c.Variants.Any(x=>x.Color.Id==id));
    return colors;
   }
   catch(Exception er) 
   {
    Console.WriteLine("Get all color exception:"+er.Message);
   } 
   return -1;
    }

  public async Task<List<Ecommerce_Product.Models.Color>> getAllColor()
  {
    try
    {
    var colors=await this._context.Colors.ToListAsync();
    return colors;
    }
    catch(Exception er)
    {
      Console.WriteLine("Get all color exception:"+er.Message);
    }

    return null;

  }




    public async Task<int> countProductByCategory(int id)
    {
      try
      {
          var products = await this._context.Products.Include(c=>c.Category).CountAsync(c=>c.Category.Id==id);

          return products;
      }
      catch(Exception er)
      {
        Console.WriteLine("Count Product By Category Exception:"+er.Message);
      }

      return -1;
      
    }



  public async Task<List<Product>> getAllProductList()
  { 
    var products=new List<Product>();
    try
    {
     products=await this._context.Products.Include(p=>p.Category).Include(p=>p.ProductImages).Include(p=>p.Variants).ThenInclude(p=>p.Color).Include(p=>p.Variants).ThenInclude(p=>p.Size).OrderBy(p=>p.SortId).ToListAsync();          
    // int sort_id=0;
    // int sort_prominent_id=0; 
    // foreach(var product in products)
    // {
    //   if(product.SortId==null)
    //   {
    //     sort_id+=1;
    //     product.SortId=sort_id;
    //   }
    //   if(product.SortProminentId==null)
    //   {
    //     sort_prominent_id+=1;
    //     product.SortProminentId=sort_prominent_id;
    //   }
    //  this._context.Products.Update(product);
     
    // }
    }
    catch(Exception er)
    {
      Console.WriteLine("Get all product list exception:"+er.Message);
    }
    return products;    
  }

    public async Task<List<Product>> getAllProminentProductList()
    {
     var products=new List<Product>();

      try
      {
       products=await this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(c=>c.SubCat).Include(p=>p.ProductImages).OrderBy(p=>p.SortProminentId).ToListAsync();
      }
      catch(Exception er)
      {
        Console.WriteLine("Get all prominent product list exception:"+er.Message);        
      }
      return products;      
    }


  public async Task<IEnumerable<Product>> getAllProduct()
  {
    try
    {  
       var products=await this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(c=>c.SubCat).Include(p=>p.ProductImages).Include(c=>c.Videos).Include(c=>c.Manuals).ToListAsync();
      //  var json_products=await this._db.StringGetAsync("products");
      //  if(string.IsNullOrEmpty(json_products))
      //  { Console.WriteLine("did come to get all product redis");
      //     await this.saveProductRedis(products);
      //  }
       return products;
    }
    catch(Exception er)
    {
        Console.WriteLine("Get all product:"+er.Message);
    }
    return null;
  } 
 

  public async Task<Manual> findManualByLanguage(string language,Product product)
  { 
    var manual_ob=new Manual();

    try
    {
    var manuals=product.Manuals;
    
    manual_ob=manuals.FirstOrDefault(c=>c.Language==language);       
    }
    catch(Exception er)
    {
      Console.WriteLine("Find Manual By Language Exception:"+er.Message);
    }
    return manual_ob;
  }
 public async Task<Product> findProductById(int id)
 {
    var product=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(c=>c.ProductImages).Include(c=>c.Variants).ThenInclude(v=>v.Color).Include(c=>c.Variants).ThenInclude(v=>v.Size).Include(c=>c.Variants).ThenInclude(c=>c.Version).Include(c=>c.Variants).ThenInclude(c=>c.Mirror).Include(c=>c.Videos).Include(c=>c.Manuals).FirstOrDefaultAsync(p=>p.Id==id);
    
    return product;
 }

public string NormalizeString(string input)
{
    // Replace all special characters with a space
    return Regex.Replace(input, @"[^A-Za-z0-9]+", " ");
}

 public async Task<Product> findProductByName(string name)
 {
  //name=name.Replace("-"," ").Replace("/"," ");  
  
  Console.WriteLine("Product name here is:"+name);
  
  var product=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(i=>i.ProductImages).Include(c=>c.Variants).ThenInclude(v=>v.Color).Include(c=>c.Variants).ThenInclude(v=>v.Size).FirstOrDefaultAsync(p=>p.ProductName.Replace(" ", "-").Replace(".", "-").Replace("'","-").Replace("&","-") ==name); 
  
  return product;
 }


public async Task<PageList<Product>> pagingProminentProduct(int page_size,int page)
{  
   
  //  IEnumerable<Product> all_prod= await this.getProductList();
      IEnumerable<Product> all_prod= await getAllProminentProductList();


  //  List<Product> prods=all_prod.OrderByDescending(u=>u.Id).ToList(); 
   
   

   //var users=this._userManager.Users;   
   var prod_list=PageList<Product>.CreateItem(all_prod.AsQueryable(),page,page_size);
   
   return prod_list;
}

public async Task<PageList<Product>> pagingProduct(int page_size,int page)
{  
   
  //  IEnumerable<Product> all_prod= await this.getProductList();
      IEnumerable<Product> all_prod= await getAllProductList();

  //  List<Product> prods=all_prod.OrderByDescending(u=>u.Id).ToList(); 
   
   

   //var users=this._userManager.Users;   
   var prod_list=PageList<Product>.CreateItem(all_prod.AsQueryable(),page,page_size);
   
   return prod_list;
}

public async Task<IEnumerable<Product>>getProductBySubCategory(int sub_cat)
{
  var products=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(c=>c.SubCat).Include(c=>c.Variants).Include(c=>c.ProductImages).Where(c=>c.SubCat.Id==sub_cat).ToListAsync();
  return products;  
}

public async Task<IEnumerable<Product>> filterProductByNameAndCategory(string product,string category)
{ List<Product> products = await this.getAllProductList();
   
  if(!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(product))
{
    products= products.Where(c=>c.Category.CategoryName==category && c.ProductName.ToLower().Contains(product.ToLower())).ToList();
}
else if(string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(product))
{
      products= products.Where(c=>c.ProductName.ToLower().Contains(product.ToLower())).ToList();
}

   return products;         
}
public async Task<IEnumerable<Product>> getProductByCategory(string cat)
{ 
  cat=cat.Replace("-"," ").Replace("/"," ");
  var products=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(c=>c.SubCat).Include(c=>c.Variants).Include(c=>c.ProductImages).Where(c=>c.Category.CategoryName.Replace("-","").Replace("/","")==cat).ToListAsync();
  return products;  
}

  public async Task<IEnumerable<Product>> getProductByBrand(string brand)
  {
 var products=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(c=>c.SubCat).Include(c=>c.Variants).Include(c=>c.ProductImages).Where(c=>c.Brand.BrandName==brand).ToListAsync();
  
 return products;
  }


public async Task<IEnumerable<Product>> filterProduct(FilterProduct products)
{
   var prod_list=await this.getAllProduct(); 
try
{      
    string start_date=products.StartDate;
    string end_date = products.EndDate;
    string prod_name=products.ProductName;
    string brand = products.Brand;
    string category = products.Category;
    string status = products.Status;
    Console.WriteLine(start_date);
    Console.WriteLine("Brand name here:"+brand);
    Console.WriteLine("Category name here:"+category);
    Console.WriteLine("Status here:"+status);
   if(!string.IsNullOrEmpty(prod_name))
   {
    prod_name=prod_name.Trim();
    
    prod_list= prod_list.Where(c=>c.ProductName.ToLower()==prod_name.ToLower()).ToList();    
   }
   if(!string.IsNullOrEmpty(start_date) && string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    prod_list= prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(start_date,out var lowerDate) && startDate.Date==lowerDate.Date).ToList();
   }
   else if(string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   { 
    end_date=end_date.Trim();
   
    prod_list=prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(end_date,out var upperDate) && startDate.Date==upperDate.Date).ToList();
   }
   else if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    end_date=end_date.Trim();
    prod_list=prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var createdDate)&& DateTime.TryParse(start_date,out var startDate) && DateTime.TryParse(end_date,out var endDate) && createdDate>=startDate && createdDate<=endDate).ToList();
   }
   
   if(!string.IsNullOrEmpty(brand))
   {
   Console.WriteLine("filter brand here");   

   prod_list= prod_list.Where(c=>c.Brand.Id==Convert.ToInt32(brand)).ToList();

   }

   if(!string.IsNullOrEmpty(category))
   {
     Console.WriteLine("filter cat here");
     prod_list= prod_list.Where(c=>c.Category.Id==Convert.ToUInt32(category)).ToList();
   }
   if(!string.IsNullOrEmpty(status))
   {
       prod_list= prod_list.Where(c=>c.Status==status).ToList();
   }
}
catch(Exception er)
{
    Console.WriteLine("Filter Product Exception:"+er.Message);
}
return prod_list;
}

public IEnumerable<ProductImage> findProductImageByProductId(int id)
{
 var product_img=this._context.ProductImages.Where(p=>p.Productid==id).ToList();
 
 return product_img; 
}

public async Task<int> deleteProduct(int id)
{
 int res_del=0;
 try
 {
   var product = await this.findProductById(id);
   
   var product_img_ob=this.findProductImageByProductId(id);

   string front_file=product.Frontavatar;
   
   string back_file = product.Backavatar;
  
   List<string> avatar=new List<string>{front_file,back_file};

   if(product!=null)
   {
  this._context.Products.Remove(product);
  
  await this.saveChanges();  
  
  res_del=1;
   }

  foreach(string file in avatar)
  {
    if(!string.IsNullOrEmpty(file))
    {
      int val= await this._sp_services.removeFiles(file);
    }
  }
  
   foreach(var ob in product_img_ob)
   {
    int val =await this._sp_services.removeFiles(ob.Avatar);
   }

 }
 catch(Exception er)
 {
    Console.WriteLine("Delete Product Exception:"+er.Message);
 }
  
//   List<Product> new_list=new List<Product>();

//   var redis_list=await this.getProductRedis();

//   foreach(var product in redis_list)
//   {
//     if(product.Id!=id)
//     {
//       new_list.Add(product);
//     }
//   }
  
//  await this.saveProductRedis(new_list);

  // if(await this._db.KeyExistsAsync("products"))
  // {
  //   await this._db.KeyDeleteAsync("products");
  // }

 return res_del;
}

  public async Task<List<Variant>> getVariantByProductId(int id)
  {
    var products = await this._context.Products.Include(c=>c.Variants).ThenInclude(c=>c.Color).Include(c=>c.Variants).ThenInclude(c=>c.Size).FirstOrDefaultAsync(c=>c.Id==id);
    
    return products.Variants.ToList();
  }



    public async Task<Dictionary<string,List<string>>> getProductVariant(Product product)
    {
    Console.WriteLine("Inside here");
    
    var variants=product.Variants;

    List<string> versions=new List<string>();
    
    List<string> colors=new List<string>();
    
    List<string> sizes=new List<string>();
    
    List<string> mirrors=new List<string>();

    List<string> prices = new List<string>();
    
  try
  {
    foreach(var variant in variants)
    {
      var version=variant.Version?.Versionname;
      var color=variant.Color?.Colorname;
      var size=variant.Size?.Sizename;
      var mirror=variant.Mirror?.Mirrorname;
      var price= variant?.Price.ToString();
     
        versions.Add(version==null?"":version);
    
        colors.Add(color==null?"":color);
       
        sizes.Add(size==null?"":size);
     
        mirrors.Add(mirror==null?"":mirror);

        prices.Add(price);
    }
  }
  catch(Exception er)
  {
    Console.WriteLine("Get Product Variant Exception:"+er.Message);
  }
    Dictionary<string,List<string>> data_val=new Dictionary<string, List<string>>
    {
      {"versions",versions},
      {"colors",colors},
      {"sizes",sizes},
      {"mirrors",mirrors},
      {"prices",prices}
    };
    return data_val;
    }


  public async Task<MemoryStream> exportToExcelProduct()
  {
    using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("Products");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên sản phẩm";
    worksheet.Cells[1,3].Value = "Loại sản phẩm";
    worksheet.Cells[1,4].Value="Nhãn hàng";
    worksheet.Cells[1,5].Value="Giá";
    worksheet.Cells[1,6].Value="Trạng thái";
    worksheet.Cells[1,7].Value="Ngày tạo";
    worksheet.Cells[1,8].Value="Ngày cập nhật";


    var products=await this.getAllProduct();
    if(products!=null)
    {
    List<Product> list_product=products.ToList();
    for(int i=0;i<list_product.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_product[i].ProductName;
    
    worksheet.Cells[i+2,3].Value=list_product[i].Category;
    
    worksheet.Cells[i+2,4].Value=list_product[i].Brand;
    
    worksheet.Cells[i+2,5].Value=list_product[i].Price;
    
    worksheet.Cells[i+2,6].Value=list_product[i].Status;
    
    worksheet.Cells[i+2,7].Value=list_product[i].CreatedDate;
    
    worksheet.Cells[i+2,8].Value=list_product[i].UpdatedDate;
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  return stream;
  }
  }


public async Task<int> countProductRatingByStar(int star,int product_id)
{
  var count=await this._context.Ratingdetails.Where(c=>c.ProductId==product_id && c.Rating==star).CountAsync();
  return count;
}

private async Task<int> calculateAvgStar(Dictionary<int,int> list_star)
{  int total_star=0;

   int total_reviews=0;
   
   foreach(KeyValuePair<int,int> kvp in list_star)
   {
     int star=kvp.Key;

     int count=kvp.Value;
     
     total_star+=star*count;
     
     total_reviews+=count;
   }
   
   int avg=0;
   
   if(total_reviews!=0)
   {
    avg=total_star/total_reviews;
   }
   return avg;         
}

public async Task<int>getSingleProductRating(int product_id)
{ int value=0;
try
{
  List<int> stars=new List<int>{5,4,3,2,1};
  Dictionary<int,int> dict=new Dictionary<int, int>();
  foreach(int star in stars)
  {
    int count=await this.countProductRatingByStar(star,product_id);
    dict.Add(star,count);
  }
  int avg_rate=await this.calculateAvgStar(dict);
  return avg_rate;
}
catch(Exception er)
{
  Console.WriteLine("Get Single Product Rating Exception:"+er.Message);  
}
return value;
}
public async Task<List<Product>> getListProductRating(int star)
{ 
  List<Product> list_prod= new List<Product>();
  
  try
  {
    var products=await this.getAllProduct();
    
    List<int> stars=new List<int>{1,2,3,4,5};
    
    foreach(var product in products)
    {
    int prod_id=product.Id;
    
    Dictionary<int,int> dict= new Dictionary<int, int>();
    
    foreach(int star_val in stars)
    {
      int prod_val=await countProductRatingByStar(star_val,prod_id);
      dict.Add(star_val,prod_val);
    }
    int star_rate=await calculateAvgStar(dict);
    
    if(star_rate==star)
    {
      list_prod.Add(product);
    }

    }
  }
  catch(Exception er)
  {
    Console.WriteLine("Get List Product Rating Exception:"+er.Message);
  }
    return list_prod;
}


private async Task<int> countReviews(int product_id)
{
  var count=await this._context.Ratingdetails.Where(c=>c.ProductId==product_id).CountAsync();
  return count;
}

public async Task<Dictionary<string,int>> countAllReview(List<Product> products)
{
 Dictionary<string,int> dict = new Dictionary<string, int>();
 foreach(var product in products)
 {
  int prod_id=product.Id;
  int count_reviews=await this.countReviews(prod_id);
  string prod_name=product.ProductName;
  dict.Add(prod_name,count_reviews);
 }
 return dict; 
}

  public async Task<int> addReviews(int product_id,string user_id,string comment)
  { int res_add=0;
    try
    { 
      string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
    
      var review=new Reviewdetail{ProductId=product_id,UserId=user_id,ReviewText=comment,CreatedDate=created_date};
      await this._context.Reviewdetails.AddAsync(review);
      await this.saveChanges();
      res_add=1;
    }
    catch(Exception er)
    {
    Console.WriteLine("Add Reviews Exception:"+er.InnerException??er.Message); 
    }
    return res_add;
  }

  public async Task<List<Reviewdetail>> getProductReviewList(int product_id)
  {  var reviews=new List<Reviewdetail>();
    try
    {
      reviews=await this._context.Reviewdetails.Include(c=>c.Product).Include(c=>c.User).Where(p=>p.ProductId==product_id).ToListAsync();
    }
    catch(Exception er)
    {
      Console.WriteLine("Get Product Reviews Exception:"+er.Message);
    }
  if(reviews!=null)
  {
    foreach(var review in reviews)
    {
      DateTime datetime=DateTime.ParseExact(review.CreatedDate,"MM/dd/yyyy hh:mm:ss",null);
        string formattedDate = datetime.ToString("MMM dd, yyyy");
     review.CreatedDate=formattedDate;
    }
  }

    return reviews;
  }


public async Task<SubCategory> findCatIdBySubId(int id)
{
  var sub_cat=await this._context.SubCategory.Include(c=>c.Category).FirstOrDefaultAsync(c=>c.Id==id);
  return sub_cat;
}

public async Task<int> addRatingStar(int product_id,string user_id,int star)
{ int create_res=0;
  try
  {
    var check_rating=await this._context.Ratingdetails.FirstOrDefaultAsync(c=>c.ProductId==product_id && c.UserId==user_id);
    if(check_rating!=null)
    {
      create_res=-1;
    }
    else
    { 
      string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
      var rating=new Ratingdetail{ProductId=product_id,UserId=user_id,Rating=star,CreatedDate=created_date};
      await this._context.Ratingdetails.AddAsync(rating);
      await this.saveChanges();
      create_res=1;
    }
  }
  catch(Exception er)
  {
    Console.WriteLine("Add Rating Star Exception:"+er.InnerException??er.Message); 
  }
  return create_res;
}


public async Task<bool> checkProductExist(string product_name)
{
  var product=await this._context.Products.FirstOrDefaultAsync(c=>c.ProductName==product_name);
  if(product!=null)
  {
    return true;
  }
  return false;
}

public async Task<int> addNewProductByLink(string link,string link_background,int category)
{ 
  int created_res=0;
  try
  {

   using(HttpClient client = new HttpClient())
   {
    string api_url=Environment.GetEnvironmentVariable("SCRAPER_URL");

    string api_key=Environment.GetEnvironmentVariable("SCRAPER_API_KEY");

    string asin= this._sp_services.extractASIN(link);

    Console.WriteLine("API_URL:"+api_url);

    Console.WriteLine("API_KEY:"+api_key);

    Console.WriteLine("ASIN:"+asin);


    if(!string.IsNullOrEmpty(asin))
    {  

    string request_url=$"{api_url}?api_key={api_key}&asin={asin}&country=us&tld=com";
    Console.WriteLine("Request_url:"+request_url);
    DateTime startTime = DateTime.Now;
    var response = await client.GetAsync(request_url);
    
    if (response.IsSuccessStatusCode)
          {
            DateTime endTime = DateTime.Now;

            Console.WriteLine("Time taken to get response:" + (endTime - startTime).TotalSeconds);

            var data = await response.Content.ReadAsStringAsync();

            var product_data = JsonConvert.DeserializeObject<ProductAmazon>(data);

            if (product_data != null)
            {
              string product_name = product_data.Name;

              var check_product_exist = await this._context.Products.FirstOrDefaultAsync(c => c.ProductName == product_name);

              if (check_product_exist != null)
              {
                return -1;
              }

              string product_price = product_data.Pricing.Replace("$", "").Replace(".", ",").Trim();

              string full_description = product_data.Full_Description;

              string small_description = product_data.Small_Description;

              string back_avatar = product_data.Images[0];

              string front_avatar = link_background;

              string manufacturer = product_data.Product_Information.Manufacturer;

              string package_dimensions = product_data.Product_Information.Package_Dimensions;

              string model = product_data.Product_Information.Item_Model_Number;

              string date_first_available = product_data.Product_Information.Date_First_Available;

              List<string> sizes = new List<string>();

              var size_list = product_data.Customization_Options.Size;

              foreach (var size in size_list)
              {
                sizes.Add(size.Value);
              }

              string created_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");

              string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");

              List<string> colors = new List<string>() { "Blue", "Pink", "Black", "White" };

              if (colors.Count < sizes.Count)
              {
                for (int i = colors.Count; i < sizes.Count; i++)
                {
                  string color = "";
                  colors.Add(color);
                }
              }

              List<Variant> variant = new List<Variant>();

              var existingColors = await _context.Colors
                 .Where(c => colors.Contains(c.Colorname))
                 .ToDictionaryAsync(c => c.Colorname, c => c);
              var existingSizes = await _context.Sizes
                  .Where(s => sizes.Contains(s.Sizename))
                  .ToDictionaryAsync(s => s.Sizename, s => s);

              var newColors = new List<Models.Color>();
              var newSizes = new List<Models.Size>();
              var variants = new List<Variant>();

              foreach (var size in sizes)
              {
                if (string.IsNullOrEmpty(size)) continue;

                foreach (var color in colors)
                {
                  if (string.IsNullOrEmpty(color)) continue;

                  if (!existingColors.TryGetValue(color, out var colorObj))
                  {
                    colorObj = new Models.Color { Colorname = color };
                    newColors.Add(colorObj);
                    existingColors[color] = colorObj;
                  }

                  if (!existingSizes.TryGetValue(size, out var sizeObj))
                  {
                    sizeObj = new Models.Size { Sizename = size };
                    newSizes.Add(sizeObj);
                    existingSizes[size] = sizeObj;
                  }

                  var variant_obj = new Variant
                  {
                    Colorid = colorObj?.Id,
                    Sizeid = sizeObj?.Id,
                    Price = string.IsNullOrEmpty(product_price) ? "" : product_price
                  };

                  if (colorObj != null || sizeObj != null)
                  {
                    variants.Add(variant_obj);
                  }
                }
              }

              if (newColors.Any())
              {
                await _context.Colors.AddRangeAsync(newColors);
              }
              if (newSizes.Any())
              {
                await _context.Sizes.AddRangeAsync(newSizes);
              }

              await _context.SaveChangesAsync();

              variant.AddRange(variants);

              List<ProductImage> product_images = new List<ProductImage>();

              if (product_data.Images != null && product_data.Images.Count > 1)
              {
                for (int i = 1; i < product_data.Images.Count; i++)
                {
                  string image_url = product_data.Images[i];
                  var product_img = new ProductImage { Avatar = image_url };
                  product_images.Add(product_img);
                }
              }

              var latest_sort_id = await this._context.Products.MaxAsync(c => c.SortId) ?? 0;

              var new_sort_id = latest_sort_id + 1;              

              var product = new Product { ProductName = product_name, CategoryId = category, Price = product_price, Quantity = 100, Status = "Còn hàng", Description = full_description, Frontavatar = front_avatar, Backavatar = back_avatar, SortId = new_sort_id, SortProminentId = new_sort_id, Small_Description = small_description, Model = model, Manufacturer = manufacturer, Asin = asin, Package_Dimensions = package_dimensions, Date_First_Available = date_first_available, CreatedDate = created_date, UpdatedDate = updated_date, ProductImages = product_images, Variants = variant };

              await this._context.Products.AddAsync(product);

              await this.saveChanges();

              created_res = 1;
            }
          }
          else
          {
            Console.WriteLine("Request API Failed:" + response.StatusCode);
          }
}
  }
}
  catch(Exception er)
  {
    Console.WriteLine("Add New Product By Link Exception:"+er.Message);
  }
  return created_res;
}

public async Task<int> addNewProduct(AddProductModel model)
{ int created_res=0;
//   try
//   {
  
//   List<IFormFile> img_files=new List<IFormFile>();

//   List<IFormFile> variant_files=new List<IFormFile>();

//    string product_name=model.ProductName;

//    int discount = model.Discount;

//    Console.WriteLine("Product name:"+product_name);
   
//    int price=model.Price;

//    if(price<0)
//    {
//     price=0;
//    }

//       Console.WriteLine("Price:"+price);

   
//    int quantity = model.Quantity;

//       Console.WriteLine("QUANTITY:"+quantity);

   
//    int sub_cat=model.SubCategory;

//       Console.WriteLine("Subcat:"+sub_cat);

   
//    int brand=model.Brand;
//       Console.WriteLine("brand:"+brand);


//    int category = model.Category;

//          Console.WriteLine("category:"+category);


//    string description=model.Description;

//    string stat_description=model.StatDescription;

//          Console.WriteLine("description:"+description);

   
//    string inbox_description=model.InboxDescription;
   
//       Console.WriteLine("inbox:"+inbox_description);


//    string discount_description = model.DiscountDescription;

//     Console.WriteLine("discount:"+discount_description);


//    string folder_name="UploadImages";

//    string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

//   string front_avatar="";

//   string back_avatar="";

//   Console.WriteLine("CHECK POINT 0");

//    List<string> colors=model.Color;

//         Console.WriteLine("colors:"+colors.Count);

   
//    List<string> weights=model.Weight;
  
//       Console.WriteLine("weight:"+weights.Count);

   
//    List<string> sizes=model.Size;

//           Console.WriteLine("sizes:"+sizes.Count);

   
//    List<string> mirrors=model.Mirror;

//           Console.WriteLine("mirror:"+mirrors.Count);

   
//    List<string> versions=model.Version;
   
//    List<string> prices = model.Prices;

//           Console.WriteLine("version:"+versions.Count);

//  if(model.ImageFiles!=null)
//  {
//   img_files=model.ImageFiles;
//  }

      
//   Console.WriteLine("img_file:"+img_files.Count);

// if(model.VariantFiles!=null)
// {
// variant_files = model.VariantFiles;
// }

//   Console.WriteLine("variant file:"+variant_files.Count);

   
//    if(await checkProductExist(product_name))
//    {
//     created_res=-1;
//     return created_res;
//    }

//    if(sub_cat!=-1)
//    {
//       var sub_cat_ob=await this.findCatIdBySubId(sub_cat);
//       if(sub_cat_ob!=null)
//       {
//         category=sub_cat_ob.Category.Id;
//       } 
//    }
//   Console.WriteLine("check point 1");

//   if(!Directory.Exists(upload_path))
//   {
//     Directory.CreateDirectory(upload_path);
//   }

//  Console.WriteLine("check point 2");

//  Console.WriteLine("check point 2.5");


//  string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
 
//  string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
 
//  List<Variant> variant=new List<Variant>();
 
// Console.WriteLine("colors count:"+colors.Count);

//  for(int i=0;i<colors.Count;i++)
//  {
//   string color=colors[i];
//   string weight=weights[i];
//   string size=sizes[i];
//   string version=versions[i];
//   string mirror=mirrors[i];
//   string price_value = prices[i];
//   Regex reg=new Regex("[^0-9]");
//   if(weight!=null)
//   {
//   weight=reg.Replace(weight,"");
//   }
//  Console.WriteLine("inside here.");
//   var check_color_exist = await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);
//   var check_size_exist = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);
//   var check_version_exist = await this._context.Versions.FirstOrDefaultAsync(c=>c.Versionname==version);
//   var check_mirror_exist = await this._context.Mirrors.FirstOrDefaultAsync(c=>c.Mirrorname==mirror);
//   Console.WriteLine("throught here");
//   if(check_color_exist==null)
//   {if(!string.IsNullOrEmpty(color))
//   {
//     var new_color = new Models.Color{Colorname=color};

//     await this._context.Colors.AddAsync(new_color);
//   }
//   }
//    if(check_version_exist==null)
//   {
//    if(!string.IsNullOrEmpty(version))
//    {
//     var new_version = new Ecommerce_Product.Models.Version{Versionname=version};
//     await this._context.Versions.AddAsync(new_version);
//    }
//   }
//    if(check_size_exist==null)
//   { if(!string.IsNullOrEmpty(size))
//   {
//     var new_size = new Ecommerce_Product.Models.Size{Sizename=size};
//     await this._context.Sizes.AddAsync(new_size);
//   }
//   }
//    if(check_mirror_exist==null)
//   { if(!string.IsNullOrEmpty(mirror))
//   {
//     var new_mirror = new Ecommerce_Product.Models.Mirror{Mirrorname=mirror};
//     await this._context.Mirrors.AddAsync(new_mirror);
//   }
//   }

//   await this.saveChanges();
 
//  Console.WriteLine("out of here");
 
//   var new_color_ob=await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);

//   var new_size_ob = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);

//   var new_version_ob = await this._context.Versions.FirstOrDefaultAsync(c=>c.Versionname==version);

//   var new_mirror_ob = await this._context.Mirrors.FirstOrDefaultAsync(c=>c.Mirrorname==mirror);

//   var new_varian_ob=new Variant{Colorid=new_color_ob!=null?new_color_ob.Id:null,Sizeid=new_size_ob!=null?new_size_ob.Id:null,Weight=string.IsNullOrEmpty(weight)?-1:Convert.ToUInt32(weight),Price=string.IsNullOrEmpty(price_value)?0:Convert.ToInt32(price_value),Versionid=new_version_ob!=null?new_version_ob.Id:null,Mirrorid=new_mirror_ob!=null?new_mirror_ob.Id:null};
//  if(new_color_ob!=null || new_size_ob!=null || new_version_ob!=null || new_mirror_ob!=null)
//  {
//   variant.Add(new_varian_ob);
//  } 
//  }

//  Console.WriteLine("check point 3");

// if(img_files!=null)
// {
//  for(int i=0;i<img_files.Count;i++)
//  { 
//    var img=img_files[i];
   
//   string extension=Path.GetExtension(img.FileName);

//   string file_name=Guid.NewGuid().ToString()+extension;

//   string file_path = Path.Combine(upload_path,file_name);
//    if(i==0)
//    {
//    front_avatar=file_path;
//    }
//    else
//    {
//     back_avatar=file_path;
//    }
    
//    using(var fileStream=new FileStream(file_path,FileMode.Create))
//    {
//     await img.CopyToAsync(fileStream);
//    }
//  }
// }

// List<ProductImage> list_img=new List<ProductImage>();

// if(variant_files!=null)
// { 
// for(int i=0;i<variant_files.Count;i++)
// {
//   var img = variant_files[i];

//   string extension=Path.GetExtension(img.FileName);

//   string file_name=Guid.NewGuid().ToString()+extension;

//   string file_path = Path.Combine(upload_path,file_name);

//   var img_obj=new ProductImage{Avatar=file_path};

//   list_img.Add(img_obj);

//   using(var fileStream=new FileStream(file_path,FileMode.Create))
//   {
//     await img.CopyToAsync(fileStream);
//   }
// }
// } 
  
//  var latest_sort_id=await this._context.Products.MaxAsync(c=>c.SortId)??0;

//  var new_sort_id=latest_sort_id+1;

//  var product= new Product{ProductName=product_name,CategoryId=category,Discount=discount,SubCatId=sub_cat==-1?null:sub_cat,BrandId=brand==-1?null:brand,Price=price.ToString(),Quantity=quantity,Status="Còn hàng",Description=description,Statdescription=stat_description,InboxDescription=inbox_description,DiscountDescription=discount_description,Frontavatar=front_avatar,Backavatar=back_avatar,SortId=new_sort_id,SortProminentId=new_sort_id,CreatedDate=created_date,UpdatedDate=updated_date,ProductImages=list_img,Variants=variant};

//   await this._context.Products.AddAsync(product);

//   await this.saveChanges();

  // created_res=1;

  // Console.WriteLine("Created res here is:"+created_res);
  // }
  // catch(Exception er)
  // { created_res=0;
  //   Console.WriteLine("Add New Product Exception:"+er.Message);
  // }

  return created_res;
}

public async Task<int> updateProduct(int id,AddProductModel model)
{
 
 int updated_res=0; 

try
{
   string temp_front_avatar="";
   
   string temp_back_avatar="";
   
   List<string> temp_list_img=new List<string>();

   var product_ob=await this.findProductById(id);

   string small_description = model.Small_Description;

   string product_name=model.ProductName;

   Console.WriteLine("Product name:"+product_name);
   
   string price=model.Price;
   
   int parse_price = Convert.ToInt32(price.Replace(",","").Replace(".",""),CultureInfo.InvariantCulture);

   if(parse_price<0)
   {
    price="0";
   }

   Console.WriteLine("Price:"+price);

   
   int quantity = model.Quantity;

   
   Console.WriteLine("QUANTITY:"+quantity);
   
   int discount=model.Discount;


   int category =model.Category;


   string description=model.Description;

   Console.WriteLine("description:"+description);

   string status=model.Status;

   Console.WriteLine("Status:"+status);


  string folder_name="UploadImages";

  string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

  string front_avatar="";

  string back_avatar="";


  Console.WriteLine("CHECK POINT 0");

  List<string> colors=model?.Color;

    
  List<string> sizes=model?.Size;


  List<string> prices = model?.Prices;


  List<IFormFile> img_files=model?.ImageFiles;

      
  //  Console.WriteLine("img_file:"+img_files.Count);

   
  List<IFormFile> variant_files = model?.VariantFiles;



  //  if(sub_cat!=-1)
  //  {
  //     var sub_cat_ob=await this.findCatIdBySubId(sub_cat);
      
  //     if(sub_cat_ob!=null)
  //     {
  //       category=sub_cat_ob.Category.Id;
  //     }
  //  }
  Console.WriteLine("check point 1");

  if(!Directory.Exists(upload_path))
  {
    Directory.CreateDirectory(upload_path);
  }

 Console.WriteLine("check point 2");

 
 string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
 
 List<Variant> variant=new List<Variant>();

if(colors!=null)
{
 for(int i=0;i<colors.Count;i++)
 {
  // Console.WriteLine("color:"+colors.Count);
  //   Console.WriteLine("weight:"+weights.Count);
  // Console.WriteLine("Inside here:"+sizes.Count);
  //   Console.WriteLine("Inside here:"+colors.Count);
  // Console.WriteLine("Inside here:"+versions.Count);
  //   Console.WriteLine("Inside here:"+mirrors.Count);
  //     Console.WriteLine("prices:"+prices.Count);

  string? color=colors[i];
  string? size=sizes[i];
  string? price_value=prices[i];
  Regex reg=new Regex("[^0-9]");

  var check_color_exist=new Models.Color();
  
  var check_size_exist=new Models.Size();

 if(!string.IsNullOrEmpty(color))
 {
   check_color_exist = await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);
 }

  Console.WriteLine("check color exist");
  
  if(!string.IsNullOrEmpty(size))
  {
   check_size_exist = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);
  }
 
  if(check_color_exist==null)
  {
    if(!string.IsNullOrEmpty(color))
  {
    var new_color = new Models.Color{Colorname=color};

    await this._context.Colors.AddAsync(new_color);
  }
  }
  
   if(check_size_exist==null)
  { 
   if(!string.IsNullOrEmpty(size))
  {
    var new_size = new Models.Size{Sizename=size};
    
    await this._context.Sizes.AddAsync(new_size);    
  }
  }

  await this.saveChanges();  
 
  var new_color_ob=await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);

  var new_size_ob = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);

  var new_varian_ob=new Variant{Colorid=new_color_ob!=null?new_color_ob.Id:null,Sizeid=new_size_ob!=null?new_size_ob.Id:null,Price=string.IsNullOrEmpty(price_value)?"":price_value};

  if(new_color_ob!=null || new_size_ob!=null)
  {
   variant.Add(new_varian_ob);
  } 
 }
}

Console.WriteLine("did here");

if(img_files!=null)
{ 
 Console.WriteLine("img avatar here");
 
 for(int i=0;i<img_files.Count;i++)
 { 
  var img=img_files[i];
   
  string extension=Path.GetExtension(img.FileName);

  string file_name=Guid.NewGuid().ToString()+extension;
  
  string file_path = Path.Combine(upload_path,file_name);
   
   if(i==0)
   {

   front_avatar=file_path;

   if(!string.IsNullOrEmpty(product_ob.Frontavatar))
   {
    temp_front_avatar=product_ob.Frontavatar;
   }
   }
   else
   {
    back_avatar=file_path;

    if(!string.IsNullOrEmpty(product_ob.Backavatar))
    {
      temp_back_avatar=product_ob.Backavatar;         
    }
   }
 
   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await img.CopyToAsync(fileStream);  
  }
 }
}
else
{   if(!string.IsNullOrEmpty(product_ob.Frontavatar))
   {
    temp_front_avatar=product_ob.Frontavatar;
   }
    if(!string.IsNullOrEmpty(product_ob.Backavatar))
    {
      temp_back_avatar=product_ob.Backavatar;
    }
}

List<ProductImage> list_img=new List<ProductImage>();

if(variant_files!=null)
{//Console.WriteLine("Lengh of variant file here is:"+variant_files.Count);
for(int i=0;i<variant_files.Count;i++)
{
  var img = variant_files[i];

 string extension=Path.GetExtension(img.FileName);

  string file_name=Guid.NewGuid().ToString()+extension;

  string file_path = Path.Combine(upload_path,file_name);

  var img_obj=new ProductImage{Avatar=file_path};

  list_img.Add(img_obj);

  using(var fileStream=new FileStream(file_path,FileMode.Create))
  {
    await img.CopyToAsync(fileStream);
  }
  
  foreach(var prod_img in product_ob.ProductImages )
  {
    if(!string.IsNullOrEmpty(prod_img.Avatar))
    {
      temp_list_img.Add(prod_img.Avatar);
    }
  }
}
}
else
{ Console.WriteLine("In this else case");
  foreach(var prod_img in product_ob.ProductImages)
  {
    if(!string.IsNullOrEmpty(prod_img.Avatar))
    {
      temp_list_img.Add(prod_img.Avatar);      
    }
  }
  Console.WriteLine("temp list img count:"+temp_list_img.Count);
}

 var product= new Product{ProductName=product_name,Discount=discount,CategoryId=category,Price=price,Quantity=quantity,Status=status,Description=description,Frontavatar=string.IsNullOrEmpty(front_avatar)?"":front_avatar,Backavatar=string.IsNullOrEmpty(back_avatar)?"":back_avatar,Small_Description=small_description,UpdatedDate=updated_date,ProductImages=list_img.Count==0?list_img:list_img,Variants=variant};

  if(product_ob!=null)
  {
    product_ob.ProductName=product.ProductName;
    product_ob.Price=product.Price;
    product_ob.Quantity=product.Quantity;
    product_ob.CategoryId=product.CategoryId;
    product_ob.BrandId=product.BrandId;
    product_ob.SubCatId=product.SubCatId;
    product_ob.Discount=product.Discount;
   if(!string.IsNullOrEmpty(front_avatar))
   {
    product_ob.Frontavatar=product.Frontavatar;
   }
   if(!string.IsNullOrEmpty(back_avatar))
   {
    product_ob.Backavatar=product.Backavatar;
   }
   if(list_img.Count>0)
   {
    product_ob.ProductImages=product.ProductImages;
  }
    product_ob.Variants=product.Variants;
    product_ob.Status=product.Status;
    product_ob.Description=product.Description;
    product_ob.Small_Description=product.Small_Description;
    product_ob.UpdatedDate=product.UpdatedDate;
    this._context.Products.Update(product_ob);
    await this.saveChanges();
    Console.WriteLine("DID STAY HERE");
    updated_res=1;
    if(!string.IsNullOrEmpty(temp_front_avatar))
    {
      await this._sp_services.removeFiles(temp_front_avatar);
    }
    if(!string.IsNullOrEmpty(temp_back_avatar))
    {
      await this._sp_services.removeFiles(temp_back_avatar);
    }
    if(temp_list_img.Count>0)
    {
      foreach(var img in temp_list_img)
      {
        await this._sp_services.removeFiles(img);
      }
    }
  }
}
catch(Exception er)
{ 
  Console.WriteLine("Update Product Exception:"+er.Message);
}

return updated_res;
}

public async Task saveChanges()
{
    await this._context.SaveChangesAsync();    
}



public async Task<IEnumerable<Product>> filterProductByPriceAndBrands(List<string>category,List<int>prices,List<string> color)
  {    
  
  var products = await this.getAllProductList();
  
  int min_price=prices[0];

  int max_price=prices[1];


  if(category.Count==0 &&  prices.Count!=0)
  { 
   products= products.Where(c=>double.Parse(c.Price.Replace(",","."),CultureInfo.InvariantCulture)>=min_price && double.Parse(c.Price.Replace(",","."),CultureInfo.InvariantCulture)<=max_price).OrderBy(c=>c.SortId).ToList();   
  }
 else if(category.Count!=0 && prices.Count!=0)
  {
    try
    {
   products= products.Where(c=>category.Contains(c.Category?.CategoryName.ToString()) &&double.Parse(c.Price.Replace(",","."),CultureInfo.InvariantCulture)>=min_price && double.Parse(c.Price.Replace(",","."),CultureInfo.InvariantCulture)<=max_price).OrderBy(c=>c.SortId).ToList();   
    }
    catch(Exception er)
    {
     Console.WriteLine("Filter Product By Price And Category Exception:"+er.Message);
    }
  }
  else
  {
    products=new List<Product>();
  }


   if(products.Count!=0 && color.Count!=0)
  { 
  products = products.Where(p=>color.All(c=>p.Variants.Any(v=>v.Color?.Colorname==c))).OrderBy(c=>c.SortId).ToList();
   
  return products;
  }

  return products;

  }

  public async Task<PageList<Product>> pagingProductByList(int page_size,int page,IEnumerable<Product> products)
  {
       var paging_prods_list =PageList<Product>.CreateItem(products.AsQueryable(),page,page_size);
       
       return paging_prods_list;                            
  }

  public async Task<int> updateProductStatus(int id,int status)
  {
    int res=0;

    try
    {
    var product=await this._context.Products.FirstOrDefaultAsync(p=>p.Id==id);
    
    if(product!=null)
    {  Console.WriteLine("Product name:"+product.ProductName);
      if(status==1)
      {
        product.Status="Còn hàng";
      }
      else if(status==0)
      {
        product.Status="Hết hàng";
    }
    res=1;

    this._context.Products.Update(product);

    await this.saveChanges();
    }
    else
    {
      res=-1;
    }
    }
    catch(Exception er)
    {
      Console.WriteLine("Update Product Status Exception:"+er.Message);
    }
    return res;
  }

  public async Task<PageList<Variant>> pagingVariant(int id,int page_size,int page)
  {
   try
   {
    var products=await this.findProductById(id);
    
    IEnumerable<Variant> all_variant= products.Variants;

    List<Variant> variants=all_variant.OrderByDescending(u=>u.Id).ToList(); 

   var variant_list =PageList<Variant>.CreateItem(variants.AsQueryable(),page,page_size);
   
   return variant_list;      
   }
   catch(Exception er)
   {
    Console.WriteLine("Paging Variant Exception:"+er.InnerException??er.Message);
   }
   return null;
  }
}