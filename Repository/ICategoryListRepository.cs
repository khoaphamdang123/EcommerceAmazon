using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface ICategoryListRepository
{
public Task<IEnumerable<Category>> getAllCategory();

public Task<IEnumerable<Category>> filterCategoryList(FilterCategory category);

public Task<PageList<Category>> pagingCategory(int page_size,int page);

public Task<int> createCategory(AddCategoryModel user);

public Task<bool> checkCategoryExist(string categoryname);

public Task<int> deleteCategory(int id);

public Task<int> updateCategory(AddCategoryModel category);

public Task<Category> findCategoryById(int id);

public Task<Category> findCategoryByName(string categoryname);

public Task<IEnumerable<SubCategory>> findSubCategoryByCat(string category);

public Task<PageList<SubCategory>> pagingSubCategory(int category,int page_size,int page);

public Task<int> createSubCategory(string subcategoryname,int categoryid);

public Task<bool> checkSubCatExist(string sub_cat);

public Task<IEnumerable<Brand>> getAllBrandList();


public Task<PageList<CategoryBrandDetail>> pagingBrand(int category,int page_size,int page);

public Task<int> createBrand(int category,string brand_name,IFormFile avatar);

public Task<int> deleteBrand(int brand_category);

public Task<int> deleteSubCategory(int sub_category);

public Task<int> updateSubCategory(int id,SubCategory sub_cat);

public Task<PageList<CategoryBrandDetail>> pagingAllBrand(int page_size,int page);

public Task<MemoryStream> exportToExcelCategory();

public Task<MemoryStream> exportToExcelSubCategory(int category);

public Task<MemoryStream> exportToExcelBrandCategory();


public Task saveChange();

}