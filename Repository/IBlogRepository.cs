using Ecommerce_Product.Models;
namespace Ecommerce_Product.Repository;

public interface IBlogRepository
{

  public Task<IEnumerable<Blog>> getAllBlog();

  public Task<PageList<Blog>> pagingBlogFiles(int page_size,int page,IEnumerable<Blog> blog);

  public Task<Blog> findBlogById(int id);

  public Task<IEnumerable<Blog>> findBlogByCategory(int cat_id);

  public Task<int> addBlog(BlogModel blog);

  public Task<int> updateBlog(int id,BlogModel blog);

  public Task<int> deleteBlog(int id);

  public Task<IEnumerable<Blog>> filterBlogByCreatedDate();

  public Task saveChanges();
}