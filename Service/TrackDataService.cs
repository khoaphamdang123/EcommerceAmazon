using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Service;

public class TrackdataService:ITrackDataRepository
{
    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public TrackdataService(EcommerceshopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<int> getCurrentVisitedCount()
  {
    var track_data=await this._context.Trackdata.FirstOrDefaultAsync(p=>p.Trackname=="visitor");
    return track_data.Totalcount??0;
  }

     public async Task<int> updateCurrentVisitedCount(int count)
     { int update_res=0;
        try
        {
            var track_data=await this._context.Trackdata.FirstOrDefaultAsync(p=>p.Trackname=="visitor");
            if(track_data!=null)
            {
                track_data.Totalcount=count;
                this._context.Trackdata.Update(track_data);
                this._context.SaveChanges();                
                update_res=1;
            }

        }
        catch(Exception er)
        {
            Console.WriteLine("Update Current Visited Count Exception:"+er.Message);
        }
    return update_res;
     }




  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}