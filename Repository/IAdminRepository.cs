using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
namespace Ecommerce_Product.Repository;

public interface IAdminRepository
{
public Task<IEnumerable<ApplicationUser>> filterUserList(FilterUser user);

public Task<IEnumerable<ApplicationUser>> getAllUserList();

public Task<PageList<ApplicationUser>> pagingUser(int page_size,int page);

public Task<bool> checkUserExist(string email,string username);

public Task<int> createUser(Register user);

public Task<ApplicationUser> findUserByEmail(string email);

public Task<ApplicationUser> findUserById(string id);

public Task<int> updateUser(UserInfo user);

public Task<int> deleteUser(string email);

public Task<int> changeUserPassword(string email);

public  Task<MemoryStream> exportToExcel();

public Task<byte[]> exportToPDF();

public  Task<byte[]> exportToCSV();
}