using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IStaticFilesRepository
{

  public Task<IEnumerable<StaticFile>> getAllStaticFile();

  public Task<StaticFile> findStaticFileById(int id);

  public Task<StaticFile> findStaticFileByName(string name);

  public Task<PageList<StaticFile>> pagingStaticFiles(int page_size,int page);

  public Task<int> addPage(StaticFile file);

  public Task<int> deletePage(int id);

  public Task<int> updatePage(int id,StaticFile file);

  public Task saveChanges();


}