using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface ITrackDataRepository
{  
   public Task<int> getCurrentVisitedCount();

   public Task<int> updateCurrentVisitedCount(int count);

   public Task saveChanges();


}