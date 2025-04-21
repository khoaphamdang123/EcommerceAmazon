using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IDashboardRepository
{  
   public int countToTalOrder();

   public decimal countToTalProfit();

   public int countOrderByDay(int day);

   public Task<IEnumerable<Order>>getLatestOrder(int number);

   public int countOrderByMonth(int month);

   public int countOrderByYear(int month);

   public int countProfitByOrder(int cat_id);

  public int countProfitByMonth(int month);

  public int countProfitByYear(int year);

  public int countProfitByDay(int day);


   public Task saveChanges();


}