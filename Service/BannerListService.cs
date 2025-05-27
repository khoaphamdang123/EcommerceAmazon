using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce_Product.Service;

public class BannerListService : IBannerListRepository
{
  private readonly EcommerceshopContext _context;

  private readonly IWebHostEnvironment _webHostEnv;

  private readonly ILogger<BannerListService> _logger;



  private readonly Support_Serive.Service _sp_services;
  public BannerListService(EcommerceshopContext context, Support_Serive.Service sp_services, ILogger<BannerListService> logger, IWebHostEnvironment webHostEnv)
  {
    this._context = context;
    this._sp_services = sp_services;
    this._webHostEnv = webHostEnv;
    this._logger = logger;
  }

  public async Task<IEnumerable<Banner>> getAllBanner()
  {
    var banner = this._context.Banners.ToList();
    return banner;
  }

  public async Task<Banner> findBannerById(int id)
  {
    var banner = await this._context.Banners.FirstOrDefaultAsync(s => s.Id == id);
    return banner;
  }

  public async Task<int> deleteBanner(int id)
  {
    int deleted_res = 0;
    try
    {
      var banner = await this.findBannerById(id);
      if (banner != null)
      {
        string curr_image = banner.Image;
        this._context.Banners.Remove(banner);
        await this.saveChanges();
        if (!string.IsNullOrEmpty(curr_image))
        {
          await this._sp_services.removeFiles(curr_image);
        }
        deleted_res = 1;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Delete Banner Exception:" + ex.Message);
      deleted_res = 0;
    }
    return deleted_res;
  }

  public async Task<int> addBanner(BannerModel banner)
  {

    int created_res = 0;
    try
    {
      var check_banner_exist = await this.findBannerById(banner.Id);

      if (check_banner_exist != null)
      {
        created_res = -1;

        return created_res;
      }

      string folder_name = "UploadImageBanner";

      string upload_path = Path.Combine(this._webHostEnv.WebRootPath, folder_name);

      if (!Directory.Exists(upload_path))
      {
        Directory.CreateDirectory(upload_path);
      }
      string avatar_url = "";
      var avatar_obj = banner.Image;
      if (avatar_obj != null)
      {
        string file_name = Guid.NewGuid() + "_" + Path.GetFileName(avatar_obj.FileName);

        Console.WriteLine("banner name:" + file_name);

        string file_path = Path.Combine(upload_path, file_name);

        using (var fileStream = new FileStream(file_path, FileMode.Create))
        {
          await avatar_obj.CopyToAsync(fileStream);
        }
        avatar_url = file_path;
      }
      else
      {
        Console.WriteLine("banner is null");
      }
      Console.WriteLine(avatar_url);
      var banner_ob = new Banner { Bannername = banner.BannerName, Image = avatar_url };
      this._context.Banners.Add(banner_ob);
      await this.saveChanges();
      created_res = 1;
      if (banner.BannerName == "logo")
      {
        Environment.SetEnvironmentVariable("Logo", avatar_url);
      }
    }
    catch (Exception ex)
    {
      created_res = 0;
      Console.WriteLine("Add Banner Exception:" + ex.Message);
    }
    return created_res;
  }


  public async Task<IEnumerable<Banner>> findBannerByName(string name)
  {
    var banner = await this._context.Banners.Where(s => s.Bannername == name).ToListAsync();
    return banner;
  }


  public async Task<int> updateBanner(int id, BannerModel banner)
  {
    int updated_res = 0;

    var banner_ob = await this.findBannerById(id);

    string avatar_url = "";

    this._logger.LogInformation("Update Banner Id:" + id);

    this._logger.LogInformation("Update Banner Name:" + banner.BannerName);
    
    if (banner_ob != null)
    {
      updated_res = 1;

      banner_ob.Bannername = banner.BannerName;

      string folder_name = "UploadImageBanner";

      string upload_path = Path.Combine(this._webHostEnv.WebRootPath, folder_name);

      this._logger.LogInformation("Upload Path Banner:" + upload_path);

      if (!Directory.Exists(upload_path))
      {
        Directory.CreateDirectory(upload_path);
      }
      this._logger.LogInformation("Upload Banner:come to here");

      var avatar_obj = banner.Image;
      if (avatar_obj != null)
      {
        Console.WriteLine("banner is not null");

        this._logger.LogInformation("Banner is not null");

        string file_name = Guid.NewGuid() + "_" + Path.GetFileName(avatar_obj.FileName);

        string file_path = Path.Combine(upload_path, file_name);

        this._logger.LogInformation("File path here is:" + file_path);


        using (var fileStream = new FileStream(file_path, FileMode.Create))
        {
          await avatar_obj.CopyToAsync(fileStream);
        }
        avatar_url = file_path;

        string curr_image = banner_ob.Image;

        this._logger.LogInformation("Current Image is:" + curr_image);

        if (!string.IsNullOrEmpty(curr_image))
        {
          await this._sp_services.removeFiles(curr_image);
        }
        banner_ob.Image = avatar_url;
      }
      this._context.Banners.Update(banner_ob);
      await this.saveChanges();

      this._logger.LogInformation("Banner updated success");

    }

    else
    {
      this._logger.LogInformation("Banner not found for update");
    }

    return updated_res;
  }

  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }


}