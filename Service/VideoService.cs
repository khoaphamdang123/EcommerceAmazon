using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Service;

public class VideoService:IVideoRepository
{
    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public VideoService(EcommerceshopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<Video>> getAllVideo()
  {
    var video_files =this._context.Videos.Include(c=>c.Product).ToList();
    return video_files;
  }

    public async Task<PageList<Video>> pagingVideo(int page_size,int page,IEnumerable<Video> video)
  {

   List<Video> list_file=video.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_file=PageList<Video>.CreateItem(video.AsQueryable(),page,page_size);
   
   return paging_list_file;
  }

  public async Task<int> addVideo(Video video)
  { int add_res=0;
    try
    {
    string link=video.Link;
    int product_id=video.ProductId;
    string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
    string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
        


    var product=await this._context.Products.FirstOrDefaultAsync(p=>p.Id==product_id);
    
    await this._context.Videos.AddAsync(new Video{Link=link,CreatedDate=created_date,UpdatedDate=updated_date,Product=product});
    
    await this.saveChanges();
  
    add_res=1;
    }
    catch(Exception er)
    { 
      Console.WriteLine("Add Video Exception:"+er.InnerException??er.Message);
    }
    return add_res;
  }
  
  public async Task<Video> findVideoById(int id)
  {
    var video=await this._context.Videos.FirstOrDefaultAsync(m=>m.Id==id);
    return video;
  }

   public async Task<IEnumerable<Video>> findVideoByProductName(string product_name)
   {
    var video_all=await getAllVideo();

    if(!string.IsNullOrEmpty(product_name))
    {
      product_name=product_name.ToLower();
      video_all=video_all.Where(m=>m.Product.ProductName.ToLower().Contains(product_name)).ToList();
    }

    return video_all;
   }


 public async Task<int> deleteVideo(int id)
 {
    int delete_res=0;
    try
    {
   var video = await this.findVideoById(id);
   if(video!=null)
   {
    this._context.Videos.Remove(video);
    await this.saveChanges();
    delete_res=1;
   }
    }
    catch(Exception er)
    {
      Console.WriteLine("Delete Video Exception:"+er.InnerException??er.Message);
    }
    return delete_res;
 }


 public async Task<IEnumerable<Video>> findVideoByProductId(int product_id)
 {
  var video = await this._context.Videos.Include(c=>c.Product).Where(m=>m.ProductId==product_id).ToListAsync();
  return video;
 }



 public async Task<int>updateVideo(int id,Video video)
 {
  int update_res=0;
  try
  {
  var video_ob = await this.findVideoById(id);
  if(video_ob!=null)
  {
    string link=video.Link;;
 
    string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");

    var product=await this._context.Products.FirstOrDefaultAsync(p=>p.Id==id);

    video_ob.Link=link;
    
    
    video_ob.UpdatedDate=updated_date;
    
    this._context.Videos.Update(video_ob);
    
    await this.saveChanges();
    
    update_res=1;
  }
  }
  catch(Exception er)
  {
    Console.WriteLine("Update Video Exception:"+er.InnerException??er.Message);
  }
  return update_res;
 }

  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}