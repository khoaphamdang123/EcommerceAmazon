using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IManualRepository
{
 public Task<IEnumerable<Manual>> getAllManual();

 public Task<PageList<Manual>> pagingManualFiles(int page_size,int page,IEnumerable<Manual> manual);

 public Task<int> addManual(ManualModel manual,string scheme,string host);

 public Task<int> deleteManual(int id);
 
 public Task<int> updateManual(int id,ManualModel manual,string scheme,string host);

 public Task<Manual> findManualById(int id);

 public Task<IEnumerable<Manual>> findManualByProductId(int product_id);

 public Task<IEnumerable<Manual>> filterManualByProductName(string product_name);

 public Task saveChanges();
}