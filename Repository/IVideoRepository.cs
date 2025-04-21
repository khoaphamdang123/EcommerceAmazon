using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IVideoRepository
{
 public Task<IEnumerable<Video>> getAllVideo();

 public Task<PageList<Video>> pagingVideo(int page_size,int page,IEnumerable<Video> videos);

 public Task<int> addVideo(Video video);

 public Task<int> deleteVideo(int id);
 
 public Task<int> updateVideo(int id,Video video);

 public Task<Video> findVideoById(int id);

 public Task<IEnumerable<Video>> findVideoByProductId(int product_id);

 public Task<IEnumerable<Video>> findVideoByProductName(string product_name);

 public Task saveChanges(); 

}