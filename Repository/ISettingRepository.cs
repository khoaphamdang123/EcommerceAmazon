using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface ISettingRepository
{
  public Task<IEnumerable<Setting>> getAllSetting();

   public Task<int> updateSetting(SettingModel model);

   public Task<int> getStatusByName(string name);

   public Task<string> getContentByName(string name);
   

   public Task<Setting> getSettingObjByName(string name);

   public Task<int> updateFirebaseSetting(FirebaseSettingModel setting);

  public Task<int> updateNewsLetterSetting(string content);

   public Task saveChanges();
}