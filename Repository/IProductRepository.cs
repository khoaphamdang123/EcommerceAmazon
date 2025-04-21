using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IProductRepository
{

  public Task<IEnumerable<Product>> getAllProduct();

  public Task<Product> findProductById(int id);

  public Task<Product> findProductByName(string name);

  public Task<IEnumerable<Product>> getProductBySubCategory(int id);

  public Task<PageList<Product>> pagingProduct(int page_size,int page);

  public Task<PageList<Product>> pagingProminentProduct(int page_size,int page);

  public Task<PageList<Product>> pagingProductByList(int page_size,int page,IEnumerable<Product> products);

  public Task<IEnumerable<Product>> getProductList();

  public Task<IEnumerable<Product>> filterProductByNameAndCategory(string name,string cat);

  public Task<int> countProductRatingByStar(int star,int product_id);

  public Task<Dictionary<string,List<string>>> getProductVariant(Product product);

  public Task<int> addRatingStar(int product_id,string user_id,int start);

  public Task<int> getSingleProductRating(int product_id);

  public Task<Dictionary<string,int>> countAllReview(List<Product> produtcs);

  public Task<List<Reviewdetail>> getProductReviewList(int product_id);

  public Task<int> addReviews(int product_id,string user_id,string comment);

  public Task<List<Product>> getListProductRating(int star);
  
  public Task<IEnumerable<Product>> getProductByCategory(string cat);

  public Task<IEnumerable<Product>> getProductByBrand(string brand);

  public Task<Manual> findManualByLanguage(string language,Product product);

  public Task<PageList<Variant>> pagingVariant(int id,int page_size,int page);

  public Task<IEnumerable<Product>> filterProduct(FilterProduct product);

  public Task<IEnumerable<Product>> filterProductByPriceAndBrands(List<string> brands,List<int> prices,List<string> stars);

  public Task<int> deleteProduct(int id);

  public Task<MemoryStream> exportToExcelProduct();

  public Task<int> addNewProduct(AddProductModel product);

  public Task<int> updateProduct(int id,AddProductModel product);
  
  public Task<int> addNewProductByLink(string link);
  
  public Task<List<Variant>> getVariantByProductId(int id);

  public Task<List<Product>> getProductRedis();

  public Task<List<Product>> getAllProductList();

  public Task<List<Product>> getAllProminentProductList();

  public Task<List<Product>> getProminentProductRedis();


  public Task SaveProduct(List<Product> products);

  public Task saveProductRedis(List<Product> products);

    public Task saveProminentProduct(List<Product> products);


  public Task saveProminentProductRedis(List<Product> products);

  public Task saveChanges();
  
}