using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Ecommerce_Product.Support_Serive;
using iText.Commons.Utils;
using Newtonsoft.Json;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]

[Route("admin")]
public class DashboardController : BaseAdminController
{
    private readonly ILogger<DashboardController> _logger;

    private readonly IDashboardRepository _dashboard;

    private readonly ICategoryListRepository _category;

    private readonly ITrackDataRepository _trackData;


   public DashboardController(IDashboardRepository dashboard,ICategoryListRepository category,ITrackDataRepository trackData,IBannerListRepository banner,ILogger<DashboardController> logger):base(banner)
   {
  this._dashboard=dashboard;
  this._logger=logger;
  this._category=category;
  this._trackData=trackData;   
   }


  [Route("dashboard")]

  [HttpGet]
  public async Task<IActionResult> Dashboard()
  { 
    try
    {

   string id_user=this.HttpContext.Session.GetString("AdminId");
   
   if(string.IsNullOrEmpty(id_user))
   {
        return RedirectToAction("Index","LoginAdmin");
   }

    int total_orders=this._dashboard.countToTalOrder();    

    Console.WriteLine("Still come to here");
    
    double profit_in_day=this._dashboard.countProfitByDay(DateTime.Now.Day);

    
    int order_in_day=this._dashboard.countOrderByDay(DateTime.Now.Day);

    Console.WriteLine("Still come to here2");

    int order_in_year=this._dashboard.countOrderByYear(DateTime.Now.Year);

    Console.WriteLine("Still come to here3");

    decimal total_profit=this._dashboard.countToTalProfit();    

    Console.WriteLine("Still come to here4");

    double total_profit_previous_1_year=this._dashboard.countProfitByYear(DateTime.Now.Year-1);

    Console.WriteLine("Still come to here5");

    double total_profit_previous_2_year=this._dashboard.countProfitByYear(DateTime.Now.Year-2);

    Console.WriteLine("Still come to here6");

    var cat_list = await this._category.getAllCategory();
    
    int total_visitors=await this._trackData.getCurrentVisitedCount();
    
    Console.WriteLine("len of cat list:"+cat_list.Count().ToString());
    
    Dictionary<Category,double> profit_by_cats=new Dictionary<Category,double>();
    
    List<int> order_in_months=new List<int>();
    
    for(int i=1;i<=12;i++)
    {
        int order_in_month=this._dashboard.countOrderByMonth(i);

        order_in_months.Add(order_in_month);
    }

    foreach(var item in cat_list)
    {
     double profit=this._dashboard.countProfitByOrder(item.Id);

     profit_by_cats.Add(item,profit);
    }
    
    Console.WriteLine("Profit by cats here is:"+profit_by_cats.Count().ToString());

    Console.WriteLine("Category here is:" + JsonConvert.SerializeObject(profit_by_cats));


    var latest_orders=await this._dashboard.getLatestOrder(5);
    
    ViewData["total_orders"]=total_orders;
    
    ViewData["profit_in_day"]=profit_in_day;
    
    ViewData["order_in_day"]=order_in_day;
    
    ViewData["order_in_months"]=order_in_months;
    
    ViewData["order_in_year"]=order_in_year;
    
    ViewData["profit_by_cats"]=profit_by_cats;
    
    ViewData["latest_orders"]=latest_orders;
    
    ViewData["total_profit"]=total_profit;
    
    ViewData["total_profit_previous_1_year"]=total_profit_previous_1_year;
    
    ViewData["total_profit_previous_2_year"]=total_profit_previous_2_year;
    
    ViewData["total_visitors"]=total_visitors;
    
    var viewId="G-KHS83JFC5Y";
  
    DateTime startDate = DateTime.Now.AddDays(-30);
  
    DateTime endDate = DateTime.Now;
    
  }
    catch(Exception er)
    {   
        Console.WriteLine("Get Dashboard List Exception:"+er.Message);
        this._logger.LogTrace("Get Dashboard List Exception:"+er.Message);
    }
    return View();
  }


}
