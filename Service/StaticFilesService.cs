using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Service;

public class StaticFilesService:IStaticFilesRepository
{
    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public StaticFilesService(EcommerceshopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<StaticFile>> getAllStaticFile()
  {
    var static_files=this._context.StaticFile.ToList();
    
    return static_files;
  }

  public async Task<StaticFile> findStaticFileById(int id)
  {
    var static_file=await this._context.StaticFile.FirstOrDefaultAsync(s=>s.Id==id);
    return static_file;
  }

  public async Task<StaticFile> findStaticFileByName(string name)
  {
    var static_file=await this._context.StaticFile.FirstOrDefaultAsync(s=>s.Filename==name);    
    return static_file;
  }
  public async Task<PageList<StaticFile>> pagingStaticFiles(int page_size,int page)
  {
    IEnumerable<StaticFile> all_files= await this.getAllStaticFile();

   List<StaticFile> list_file=all_files.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_file=PageList<StaticFile>.CreateItem(list_file.AsQueryable(),page,page_size);
   
   return paging_list_file;
  }

  public async Task<int> addPage(StaticFile file)
  {
  int created_res=0;
    try
    {
  var check_page_exist=await this.findStaticFileByName(file.Filename);
  if(check_page_exist!=null)
  {
    created_res=-1;
    return created_res;
  }
 string created_date=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
 
 string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");    
 
var page=new StaticFile{Filename=file.Filename,Content=file.Content,Createddate=created_date,Updateddate=updated_date};
await this._context.StaticFile.AddAsync(page);
await this.saveChanges();
created_res=1;
 }
    catch(Exception er)
    {
        Console.WriteLine(er.Message);
    }
return created_res;
  }

    public async Task<int> deletePage(int id)
    {  
        int delete_res=0;
        try
        {
          var page=await this.findStaticFileById(id);

          if(page!=null)
          {
            this._context.StaticFile.Remove(page);
            await this.saveChanges();
            delete_res=1;            
          }
        }
        catch(Exception er)
        {
            Console.WriteLine(er.Message);
        }
        return delete_res;
    }

      public async Task<int> updatePage(int id,StaticFile file)
      {
        int updated_res=0;
        var page=await this.findStaticFileById(id);
        if(page!=null)
        {   updated_res=1;
            page.Filename=file.Filename;
            page.Content=file.Content;
         string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");    
            page.Updateddate=updated_date;
        this._context.StaticFile.Update(page);
        await this.saveChanges();
        }
    return updated_res;
      }
  



  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}