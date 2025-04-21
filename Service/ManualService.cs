using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Service;

public class ManualService:IManualRepository
{
    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public ManualService(EcommerceshopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<Manual>> getAllManual()
  {
    var manual_files =this._context.Manuals.Include(c=>c.Product).ToList();
    return manual_files;
  }

    public async Task<PageList<Manual>> pagingManualFiles(int page_size,int page,IEnumerable<Manual> manual)
  {

   List<Manual> list_file=manual.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_file=PageList<Manual>.CreateItem(manual.AsQueryable(),page,page_size);
   
   return paging_list_file;
  }

  public async Task<int> addManual(ManualModel manual,string scheme,string host)
  { int add_res=0;
    try
    {
    string manual_name=manual.ManualName;
    string pdf_link="";
    string web_link="";

    string language=manual.Language;
    int product_id=manual.ProductId;
    string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
    string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");

    if(manual.PdfLink!=null)
    {
       var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadPdf");
    
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }
    
    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(manual.PdfLink.FileName);

    var filePath = Path.Combine(uploadPath, fileName);
    
    using(var stream = new FileStream(filePath, FileMode.Create))
    {
        await manual.PdfLink.CopyToAsync(stream);
    }

    pdf_link = $"{scheme}://{host}/UploadPdf/{fileName}";
    
    }

    if(manual.WebLink!=null)
    {
 var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadLink");
    
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }
    
    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(manual.WebLink.FileName);

    var filePath = Path.Combine(uploadPath, fileName);
    
    using(var stream = new FileStream(filePath, FileMode.Create))
    {
        await manual.WebLink.CopyToAsync(stream);
    }

    web_link  = $"{scheme}://{host}/UploadLink/{fileName}";    
    }

    string manual_link_str=pdf_link+","+web_link;    



    var product=await this._context.Products.FirstOrDefaultAsync(p=>p.Id==product_id);
    
    await this._context.Manuals.AddAsync(new Manual{ManualName=manual_name,ManualLink=manual_link_str,Language=language,CreatedDate=created_date,UpdatedDate=updated_date,Product=product});
    
    await this.saveChanges();
  
    add_res=1;
    }
    catch(Exception er)
    { 
      Console.WriteLine("Add Manual Exception:"+er.InnerException??er.Message);
    }
    return add_res;
  }
  
  public async Task<Manual> findManualById(int id)
  {
    var manual=await this._context.Manuals.FirstOrDefaultAsync(m=>m.Id==id);
    
    return manual;
  }

   public async Task<IEnumerable<Manual>> filterManualByProductName(string product_name)
   { 
    
    var manual_all=await this.getAllManual();

    if(!string.IsNullOrEmpty(product_name))
    {
      product_name=product_name.ToLower();
     
     manual_all=manual_all.Where(m=>m.Product.ProductName.ToLower().Contains(product_name)).ToList();
    }
    
    return manual_all;

   }


 public async Task<int> deleteManual(int id)
 {
    int delete_res=0;
    try
    {
   var manual = await this.findManualById(id);
   if(manual!=null)
   {
    this._context.Manuals.Remove(manual);
    await this.saveChanges();
    delete_res=1;
   }
    }
    catch(Exception er)
    {
      Console.WriteLine("Delete Manual Exception:"+er.InnerException??er.Message);
    }
    return delete_res;
 }


 public async Task<IEnumerable<Manual>> findManualByProductId(int product_id)
 {
  var manual = await this._context.Manuals.Include(c=>c.Product).Where(m=>m.ProductId==product_id).ToListAsync();
  return manual;
 }



 public async Task<int>updateManual(int id,ManualModel manual,string scheme,string host)
 {
  int update_res=0;
  try
  {
  var manual_ob = await this.findManualById(id);
  if(manual_ob!=null)
  {
    string manual_name=manual.ManualName;
   string pdf_link="";
    string web_link="";
    string manual_link_ob=manual_ob.ManualLink;
    string[] link_details=manual_link_ob.Split(",");
    string old_pdf_link=link_details[0];
    string old_web_link=link_details[1];
    // List<string> manual_link_list = new List<string>{pdf_link,web_link};
    // manual_link_list=manual_link_list.Where(c=>!string.IsNullOrEmpty(c)).Distinct().ToList();
    
    string language=manual.Language;
    
    int product_id=manual.ProductId;
    
    string manual_link="";
  //  if(manual_link_list.Count>1)
  //  {  
  //    manual_link=string.Join(",",manual_link_list);
  //  }
  //  else
  //  {
  // manual_link=manual_link_list[0];
  //  }

  if(manual.PdfLink!=null)
    {
       var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadPdf");
    
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }
    
    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(manual.PdfLink.FileName);

    var filePath = Path.Combine(uploadPath, fileName);
    
    using(var stream = new FileStream(filePath, FileMode.Create))
    {
        await manual.PdfLink.CopyToAsync(stream);
    }
    pdf_link = $"{scheme}://{host}/UploadPdf/{fileName}";
   string[] old_pdf_link_details=old_pdf_link.Split("UploadPdf/");
   string full_pdf_link=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadPdf",old_pdf_link_details[1]);
     Console.WriteLine("Old Pdf Link:"+full_pdf_link);

  if (System.IO.File.Exists(full_pdf_link))
{
    System.IO.File.Delete(full_pdf_link);
}
    }

    if(manual.WebLink!=null)
    {
 var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadLink");
    
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }
    
    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(manual.WebLink.FileName);

    var filePath = Path.Combine(uploadPath, fileName);
    
    using(var stream = new FileStream(filePath, FileMode.Create))
    {
        await manual.WebLink.CopyToAsync(stream);
    }

    web_link  = $"{scheme}://{host}/UploadPdf/{fileName}";
 string[] old_web_link_details=old_web_link.Split("UploadFile/");
   string full_web_link=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadLink",old_web_link_details[1]);
    if (System.IO.File.Exists(full_web_link))
    {
        System.IO.File.Delete(full_web_link);
    }

    }

    manual_link=pdf_link+","+web_link;

    string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");

    var product=await _context.Products.FirstOrDefaultAsync(p=>p.Id==product_id);
    
    manual_ob.ManualName=manual_name;
    
    manual_ob.ManualLink=manual_link;
    
    manual_ob.Language=language;
    
    manual_ob.UpdatedDate=updated_date;
    
    manual_ob.Product=product;
    
    this._context.Manuals.Update(manual_ob);
    
    await this.saveChanges();
    
    update_res=1;
  }
  }
  catch(Exception er)
  {
    Console.WriteLine("Update Manual Exception:"+er.InnerException??er.Message);
  }
  return update_res;
 }



//   public async Task<StaticFile> findStaticFileById(int id)
//   {
//     var static_file=await this._context.StaticFile.FirstOrDefaultAsync(s=>s.Id==id);
//     return static_file;
//   }

//   public async Task<StaticFile> findStaticFileByName(string name)
//   {
//     var static_file=await this._context.StaticFile.FirstOrDefaultAsync(s=>s.Filename==name);
//     return static_file;
//   }
//   public async Task<PageList<StaticFile>> pagingStaticFiles(int page_size,int page)
//   {
//     IEnumerable<StaticFile> all_files= await this.getAllStaticFile();

//    List<StaticFile> list_file=all_files.OrderByDescending(u=>u.Id).ToList(); 

//    //var users=this._userManager.Users;   
//    var paging_list_file=PageList<StaticFile>.CreateItem(list_file.AsQueryable(),page,page_size);
   
//    return paging_list_file;
//   }

//   public async Task<int> addPage(StaticFile file)
//   {
//   int created_res=0;
//     try
//     {
//   var check_page_exist=await this.findStaticFileByName(file.Filename);
//   if(check_page_exist!=null)
//   {
//     created_res=-1;
//     return created_res;
//   }
//  string created_date=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
 
//  string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");    
 
// var page=new StaticFile{Filename=file.Filename,Content=file.Content,Createddate=created_date,Updateddate=updated_date};
// await this._context.StaticFile.AddAsync(page);
// await this.saveChanges();
// created_res=1;
//  }
//     catch(Exception er)
//     {
//         Console.WriteLine(er.Message);
//     }
// return created_res;
//   }

//     public async Task<int> deletePage(int id)
//     {  
//         int delete_res=0;
//         try
//         {
//           var page=await this.findStaticFileById(id);

//           if(page!=null)
//           {
//             this._context.StaticFile.Remove(page);
//             await this.saveChanges();
//             delete_res=1;            
//           }
//         }
//         catch(Exception er)
//         {
//             Console.WriteLine(er.Message);
//         }
//         return delete_res;
//     }

//       public async Task<int> updatePage(int id,StaticFile file)
//       {
//         int updated_res=0;
//         var page=await this.findStaticFileById(id);
//         if(page!=null)
//         {   updated_res=1;
//             page.Filename=file.Filename;
//             page.Content=file.Content;
//          string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");    
//             page.Updateddate=updated_date;
//         this._context.StaticFile.Update(page);
//         await this.saveChanges();
//         }
//     return updated_res;
//       }
  



  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}