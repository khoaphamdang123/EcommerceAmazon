using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;
using System.Globalization;

namespace Ecommerce_Product.Service;

public class DashboardService:IDashboardRepository
{
    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public DashboardService(EcommerceshopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;    
  }
 
  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();    
  }

  public int countToTalOrder()
  {
    var total_order=this._context.Orders.Count();    
    
    return total_order;    
  }

public decimal countToTalProfit()
{
    var total_profit = this._context.Orders.Include(c=>c.User).Include(c=>c.Payment).Sum(c=>c.Total);
    return total_profit;
}

  public int  countOrderByDay(int day)
  {
    var total_order=this._context.Orders.AsEnumerable().Where(s=>!string.IsNullOrEmpty(s.Createddate)&&DateTime.ParseExact(s.Createddate, "MM/dd/yyyy HH:mm:ss", null).Day==day).Count();
    
    return total_order;
  }

 public async Task<IEnumerable<Order>> getLatestOrder(int number)
 {
    var orders=await this._context.Orders.Include(c=>c.User).Include(c=>c.Payment).OrderByDescending(c=>c.Id).Take(number).ToListAsync();
    return orders;
}
    
      public int countOrderByMonth(int month)
      {
     var total_order=this._context.Orders.AsEnumerable().Where(s=>!string.IsNullOrEmpty(s.Createddate)&&DateTime.ParseExact(s.Createddate, "MM/dd/yyyy HH:mm:ss", null).Month==month).Count();
     return total_order;
      }
    
      public int countOrderByYear(int year)
      {
     var total_order=this._context.Orders.AsEnumerable().Where(s=>!string.IsNullOrEmpty(s.Createddate)&&DateTime.ParseExact(s.Createddate, "MM/dd/yyyy HH:mm:ss", null).Year==year).Count();
     return total_order;
      }
    
      public int countOrderByStatus(string status)
      {
     var total_order=this._context.Orders.Where(s=>s.Status==status).Count();
     return total_order;
 }


  public double countProfitByOrder(int cat_id)
  {
    var total_profit = this._context.OrderDetails.Include(c => c.Product).Include(c => c.Order).AsEnumerable().Where(s => s.Product.CategoryId == cat_id).Sum(s =>
    {
      string price_value = s.Product.Price.Replace("$", "").Replace(",", ".");
      if (double.TryParse(price_value, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
      {
        return s.Quantity * price;
      }
      return 0;
    });

    return total_profit;

  }


  public double countProfitByMonth(int month)
  {
    var total_profit = this._context.OrderDetails
        .Include(c => c.Product)
        .Include(c => c.Order).AsEnumerable()
        .Where(s => !string.IsNullOrEmpty(s.Order.Createddate) && DateTime.ParseExact(s.Order.Createddate, "MM/dd/yyyy HH:mm:ss", null).Month == month)
        .Sum(s =>
        {
          string price_value = s.Product.Price.Replace("$", "").Replace(",", ".");
          if (double.TryParse(price_value, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
          {
            return s.Quantity * price;
          }
          return 0;
        });
    return total_profit; 
}

  public double countProfitByYear(int year)
  {
    var total_profit = this._context.OrderDetails
        .Include(c => c.Product)
        .Include(c => c.Order).AsEnumerable()
        .Where(s => !string.IsNullOrEmpty(s.Order.Createddate) && DateTime.ParseExact(s.Order.Createddate, "MM/dd/yyyy HH:mm:ss", null).Year == year)
        .Sum(s =>
        {
          string price_value = s.Product.Price.Replace("$", "").Replace(",", ".");
          if (double.TryParse(price_value, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
          {
            return s.Quantity * price;
          }
          return 0;
        });
    return total_profit;  
  }

  public double countProfitByDay(int day)
  {
    var total_profit = this._context.OrderDetails
            .Include(c => c.Product)
            .Include(c => c.Order).AsEnumerable()
            .Where(s => !string.IsNullOrEmpty(s.Order.Createddate) && DateTime.ParseExact(s.Order.Createddate, "MM/dd/yyyy HH:mm:ss", null).Day == day)
            .Sum(s =>
            {
              string price_value = s.Product.Price.Replace("$", "").Replace(",", ".");
              if (double.TryParse(price_value, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
              {
                return s.Quantity * price;
              }
              return 0;
            });
    return total_profit;  
  }
}