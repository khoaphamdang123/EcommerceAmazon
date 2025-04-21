using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]

[Route("admin")]
public class AdminListController : BaseAdminController
{
    private readonly ILogger<AdminListController> _logger;

    private readonly IAdminRepository _userList;

    public AdminListController(ILogger<AdminListController> logger,IBannerListRepository banner,IAdminRepository userList):base(banner)
    {
        _logger = logger;
        this._userList=userList;
    }
  [Authorize(Roles ="Admin")]
   [HttpGet("admin_list")]
    public async Task<IActionResult> AdminList()
    { 
          try
        {         
          var users=await this._userList.pagingUser(10,1);

          string select_size="10";

          ViewBag.select_size=select_size;
          
          List<string> options=new List<string>(){"10","25","50","100"};
          
            ViewBag.options=options;
            FilterUser filter_obj=new FilterUser("","","","","");
            ViewBag.filter_user=filter_obj;
          return View(users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
        return View();
    }
  [Authorize(Roles ="Admin")]
 [Route("admin_list/page")]
   [HttpGet]
    public async Task<IActionResult> UserListPaging([FromQuery]int page_size,[FromQuery] int page=1,string username="",string email="",string phonenumber="",string datetime="",string endtime="")
    {
       try
        { 
          var users=await this._userList.pagingUser(page_size,page);

          if(!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(phonenumber) || !string.IsNullOrEmpty(datetime))
          {
          FilterUser filter_obj=new FilterUser(username,email,phonenumber,datetime,endtime);
          var filtered_user_list=await this._userList.filterUserList(filter_obj);
          users=PageList<ApplicationUser>.CreateItem(filtered_user_list.AsQueryable(),page,page_size);
          ViewBag.filter_user=filter_obj;
          }
         

          List<string> options=new List<string>(){"10","25","50","100"};
          ViewBag.options=options;
          string select_size=page_size.ToString();
          ViewBag.select_size=select_size;
          return View("~/Views/AdminList/AdminList.cshtml",users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get Admin List Exception:"+er.Message);
        }
        return RedirectToAction("AdminList","AdminList");
    }

    


  [Authorize(Roles ="Admin")]
    [Route("admin_list")]
    [HttpPost]
    public async Task<IActionResult> AdminList(string username,string email,string phonenumber,string datetime,string endtime)
    {
 Console.WriteLine("username:"+username);

     List<string> options=new List<string>(){"10","25","50","100"};
     
      string select_size="10";
      
      ViewBag.select_size=select_size;
     
      ViewBag.options=options;
     try
     {
    //  {string username=Model?.UserName;
    //  string email=Model?.Email;
    //  string phonenumber=Model?.PhoneNumber; 

    FilterUser user_list=new FilterUser(username,email,phonenumber,datetime,endtime);
    
    var users=await this._userList.filterUserList(user_list);

    var user_paging=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),1,10);

    ViewBag.filter_user=user_list;

    return View(user_paging);     
     }
     catch(Exception er)
     {  Console.WriteLine("Exception here:"+er.Message);
        this._logger.LogTrace("Filter Admin List Exception:"+er.Message);
     }
     return View();
    }
    [Authorize(Roles ="Admin")]
   [Route("admin_list/add")]
   [HttpGet]
  public IActionResult AddAdminList()
  {
    return View();
  }
   [Route("admin_list/add")]
   [HttpPost]
   public async Task<IActionResult> AddAdminList(Register user)
   {
    try
    { 
  string username=user.UserName;
  string email=user.Email;
  string password= user.Password;
  string gender= user.Gender;

  int res=await this._userList.createUser(user);
     if(res==1)
     {
      ViewBag.Status=1;
      ViewBag.Created_User="Đã thêm tài khoản admin thành công";
     }
     else if(res==-1)
     {
      ViewBag.Status=-1;
      ViewBag.Created_User="Username hoặc Email này đã tồn tại trong hệ thống";
     }
     else
     {
      ViewBag.Status=0;
      ViewBag.Created_User="Thêm tài khoản admin thất bại";
     }
    }
    catch(Exception er)
    {
      Console.WriteLine("Add Admin Exception:"+er.InnerException?.Message??er.Message);
        this._logger.LogTrace("Add Admin Exception:"+er.InnerException?.Message??er.Message);
    ViewBag.Status=0;
    ViewBag.Created_User="Thêm admin thất bại";
    }
    return View();
   }
  //  [Route("user_list")]
  //  [HttpGet]
  //  public async Task<IActionResult> handleNumberItem(int page_size)
  //  {
  // try
  // {
  //   Console.WriteLine("page size here is:"+page_size);
  //  var users=await this._userList.pagingUser(page_size,1);
  //  return View("~/Views/UserList/UserList.cshtml",users);
  // }
  // catch(Exception er)
  // {
  //   this._logger.LogTrace("Handle Page Size Exception:"+er.Message);
  // }
  // return View("~/Views/UserList/UserList.cshtml");
  //  }
  [Authorize(Roles ="Admin")]
  [Route("admin_list/admin_info")]
  [HttpGet]
  public async Task<IActionResult> AdminInfo(string email)
  {
   try
   {
     var user=await this._userList.findUserByEmail(email);
     if(user!=null)
     {
     return View("~/Views/AdminList/AdminInfo.cshtml",user);
     }
     else
     {
        return RedirectToAction("AdminList","AdminList");
     }
   }
   catch(Exception er)
   {
     Console.WriteLine("Admin Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Admin Info Exception:"+er.InnerException?.Message??er.Message); 
   }
        return RedirectToAction("AdminList","AdminList");
  }
  [Authorize(Roles ="Admin")]
[Route("admin_list/admin_info")]
[HttpPost]
public async Task<IActionResult> AdminInfo(UserInfo user)
{ int res_update=0;
  try
  {
    res_update=await this._userList.updateUser(user);
    if(res_update==1)
    {
      ViewBag.Status=1;
      ViewBag.Update_Message="Cập nhật Admin thành công";
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Update_Message="Cập nhật Admin thất bại";
    }
    var user_after=await this._userList.findUserById(user.Id);

    return View("~/Views/AdminList/AdminInfo.cshtml",user_after);

  }
  catch(Exception er)
  {
     Console.WriteLine("Update Admin Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Update Admin Info Exception:"+er.InnerException?.Message??er.Message); 
  }
  return RedirectToAction("AdminList","AdminList");
} 

[Route("admin_list/admin_info/delete")]
[HttpGet] 
public async Task<IActionResult> AdminInfoDelete(string email)
{ 
  try
  {
   int res_delete=await this._userList.deleteUser(email);
  this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete admin account {email}");

   if(res_delete==1)
   {
    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = "Xóa tài khoản Admin thành công";
   }
   else
   {
   TempData["Status_Delete"]=0;
   TempData["Message_Delete"]="Xóa tài khoản Admin thất bại";
   }
  }
  catch(Exception er)
  {
     Console.WriteLine("Delete Admin Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Delete Admin Info Exception:"+er.InnerException?.Message??er.Message);    
  }
  return RedirectToAction("AdminList","AdminList");
}

  [Authorize(Roles ="Admin")]
[Route("admin_list/admin_info/change_password")]
[HttpGet]
public async Task<IActionResult> ResetPasswordUser(string email)
{
  try
  {
    int res_change= await this._userList.changeUserPassword(email);
    
    if(res_change==1)
    {
    TempData["change_res"]=1;
    TempData["message_change"]="Mật khẩu mới của tài khoản admin này là Ecommerce123@";
    }
   else
   {
   TempData["change_res"]=0;
   TempData["message_change"] = "Đổi mật khẩu tài khoản admin thất bại";
   }
  }
  catch(Exception er)
  {
         Console.WriteLine("Reset Admin Password Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Reset Admin Password Exception:"+er.InnerException?.Message??er.Message);    
  }
   return RedirectToAction("AdminList","AdminList");
}
  [Authorize(Roles ="Admin")]
[Route("admin_list/export_excel")]
[HttpGet]
public async Task<IActionResult> ExportExel()
{
  try
  {
  var content= await this._userList.exportToExcel();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Admin_List.xlsx");
  }
  catch(Exception er)
  {
   Console.WriteLine("Export Excel Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Export Excel Exception:"+er.InnerException?.Message??er.Message);      
  }
  return RedirectToAction("AdminList","AdminList");
}
  [Authorize(Roles ="Admin")]
[Route("admin_list/export_pdf")]

[HttpGet]
public async Task<IActionResult> ExportPdf()
{
  try
  {
  var content = await this._userList.exportToPDF();
  
  return File(content,"application/pdf","Admin_List.pdf");
  }
  catch(Exception er)
  {
     Console.WriteLine("Export Pdf Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Export Pdf Exception:"+er.InnerException?.Message??er.Message);     
  }
  return RedirectToAction("AdminList","AdminList");
}
  [Authorize(Roles ="Admin")]
[Route("admin_list/export_csv")]
[HttpGet]

public async Task<IActionResult> ExportCsv()
{
  try
  {
  var content = await this._userList.exportToCSV();
  return File(content,"application/csv;charset=utf-8","Admin_List.csv");
  }
  catch(Exception er)
  {
  Console.WriteLine("Export Csv Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Export Csv Exception:"+er.InnerException?.Message??er.Message);     
  }
  return RedirectToAction("AdminList","AdminList");

}  
}
