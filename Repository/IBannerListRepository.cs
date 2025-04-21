using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IBannerListRepository
{

  public Task<IEnumerable<Banner>> getAllBanner();

  public Task<int> deleteBanner(int id);

  public Task<Banner> findBannerById(int id);

  public Task<IEnumerable<Banner>> findBannerByName(string name);

  public Task<int> addBanner(BannerModel banner);

  public Task<int> updateBanner(int id,BannerModel banner);

  public Task saveChanges();
}