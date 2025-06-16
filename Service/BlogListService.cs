using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Ecommerce_Product.Service;

public class BlogListService:IBlogRepository
{
    private readonly EcommerceshopContext _context;

    private readonly IWebHostEnvironment _webHostEnv;


    private readonly Support_Serive.Service _sp_services;
  public BlogListService(EcommerceshopContext context,Support_Serive.Service sp_services,IWebHostEnvironment webHostEnv)
  {
    this._context=context;
    this._sp_services=sp_services;
    this._webHostEnv=webHostEnv;
  }

  public async Task<IEnumerable<Blog>>getAllBlog()
  {
    var blogs=this._context.Blogs.ToList();

    return blogs;    
  }

   public async Task<PageList<Blog>> pagingBlogFiles(int page_size,int page,IEnumerable<Blog> blog)
  {

   List<Blog> list_file=blog.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_file=PageList<Blog>.CreateItem(blog.AsQueryable(),page,page_size);
   
   return paging_list_file;   
  }

  public async Task<IEnumerable<Blog>> findBlogByCategory(int cat_id)
  {
    var blogs=await this._context.Blogs.Where(s=>s.CategoryId==cat_id).ToListAsync();
    return blogs;
  }

    public async Task<IEnumerable<Blog>> filterBlogByCreatedDate()
    {   

        var blogs = await this._context.Blogs.OrderByDescending(s => DateTime.ParseExact(s.Createddate,"MM/dd/yyyy HH:mm:ss",CultureInfo.InvariantCulture)).ToListAsync();

        return blogs;
    }



    public async Task<Blog> findBlogById(int id)
    {
        var blog = await this._context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
        return blog;
    }


  public async Task<int> addBlog(BlogModel blog)
  { int add_res=0;
    try
    {
    string author=blog.Author;
    string blog_name=blog.Blogname;
    string blog_content=blog.Content;
    string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
    string updated_date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
    int cat_id=blog.CategoryId;
    string folder_name="BlogImages";
    string path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);
    var feature_image=blog.FeatureImage;
    string url_img="";
    if(!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }
    if(feature_image!=null)
    {
         string file_name=Guid.NewGuid()+"_"+Path.GetFileName(feature_image.FileName);
         
         string file_path=Path.Combine(path,file_name);
         
         using(var fileStream=new FileStream(file_path,FileMode.Create))
         {
             await feature_image.CopyToAsync(fileStream);
         }
         url_img=file_path;
    }
    else
    {
        url_img="https://cdn-icons-png.flaticon.com/128/16955/16955062.png";
    }

 
    var check_blog_exist=await this._context.Blogs.FirstOrDefaultAsync(p=>p.Blogname==blog_name);
    if(check_blog_exist!=null)
    {
        add_res=-1;
        return add_res;
    }
    
    await this._context.Blogs.AddAsync(new Blog{Author=author,Blogname=blog_name,FeatureImage=url_img,Content=blog_content,Createddate=created_date,Updateddate=updated_date,CategoryId=cat_id==-1?12:cat_id});
    
    await this.saveChanges();
    
    add_res=1;
    }
    catch(Exception er)
    {  
        Console.WriteLine("Add Blog Exception:"+er.InnerException??er.Message);
    }
    return add_res;    
  }

    public async Task<int> updateBlog(int id,BlogModel blog)
    {  int update_res=0;
        try
        {
            var blog_update=await this._context.Blogs.FirstOrDefaultAsync(s=>s.Id==id);
            if(blog_update!=null)
            {
                blog_update.Author=blog.Author;
                blog_update.Blogname=blog.Blogname;
                blog_update.Content=blog.Content;
                blog_update.Updateddate=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
                blog_update.CategoryId=blog.CategoryId;
                var feature_image=blog.FeatureImage;
                string folder_name="BlogImages";
                string path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);
                string url_img="";
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if(feature_image!=null)
                {
                    string file_name=Guid.NewGuid()+"_"+Path.GetFileName(feature_image.FileName);
                    string file_path=Path.Combine(path,file_name);
                    using(var fileStream=new FileStream(file_path,FileMode.Create))
                    {
                        await feature_image.CopyToAsync(fileStream);
                    }
                    url_img=file_path;
                }
                else
                {
                    url_img="https://cdn-icons-png.flaticon.com/128/16955/16955062.png";
                }
                blog_update.FeatureImage=url_img;
                this._context.Blogs.Update(blog_update);
                await this.saveChanges();
                update_res=1;
            }
        }
        catch(Exception er)
        {
            Console.WriteLine("Update Blog Exception:"+er.InnerException??er.Message);
        }
        return update_res;
    }




    public async Task<int> deleteBlog(int id)
    {  int delete_res=0;
        try
        {
            var blog=await this._context.Blogs.FirstOrDefaultAsync(s=>s.Id==id);
            if(blog!=null)
            {
                this._context.Blogs.Remove(blog);
                await this.saveChanges();
                delete_res=1;            
            }
        }
        catch(Exception er)
        {
            Console.WriteLine("Delete Blog Exception:"+er.InnerException??er.Message);
        }
        return delete_res;
    }


  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }


}