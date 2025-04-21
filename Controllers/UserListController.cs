using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using System.Drawing.Text;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]
[Route("admin")]
public class UserListController : BaseAdminController
{
    private readonly ILogger<UserListController> _logger;

    private readonly IUserListRepository _userList;

    private readonly IConfiguration _configure;

    private readonly Support_Serive.Service _sp_service;

    private readonly IWebHostEnvironment _webHostEnv;


    public UserListController(ILogger<UserListController> logger,IBannerListRepository banner,IUserListRepository userList,IConfiguration configure,Support_Serive.Service sp_service,IWebHostEnvironment webHost):base(banner)
    {
        _logger = logger;
        this._userList=userList;
        this._configure=configure;
        this._sp_service=sp_service;
        this._webHostEnv=webHost;
    }
   [HttpGet("user_list")]
    public async Task<IActionResult> UserList()
    {  Console.WriteLine("gere");
          try
        {             
         var users=await this._userList.pagingUser(10,1);
         
         string url=this._configure["Connect_Dns"];
         
         ViewBag.URL=url;
         
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
 [Route("user_list/page")]
   [HttpGet]
    public async Task<IActionResult> UserListPaging([FromQuery]int page_size,[FromQuery] int page=1,string username="",string email="",string phonenumber="",string datetime="",string endtime="")
    {
      Console.WriteLine("task here");
       try
        { 
          var users=await this._userList.pagingUser(page_size,page);

          if(!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(phonenumber) || !string.IsNullOrEmpty(datetime) || string.IsNullOrEmpty(endtime))
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
          return View("~/Views/UserList/UserList.cshtml",users);
        }
        catch(Exception er)
        {
            this._logger.LogTrace("Get User List Exception:"+er.Message);
        }
  return RedirectToAction("UserList","UserList");
    }

    


    [Authorize(Roles ="Admin")]
    [Route("user_list")]
    [HttpPost]
    public async Task<IActionResult> UserList(string username,string email,string phonenumber,string datetime,string endtime)
    {

     List<string> options=new List<string>(){"10","25","50","100"};
     
      string select_size="10";
      
      ViewBag.select_size=select_size;
     
      ViewBag.options=options;
     try
  {   
 if(!string.IsNullOrEmpty(datetime))
 {
   string[] reformatted=datetime.Trim().Split('-');

   datetime=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }
if(!string.IsNullOrEmpty(endtime))
 {
   string[] reformatted=endtime.Trim().Split('-');

   endtime=reformatted[1]+"/"+reformatted[2]+"/"+reformatted[0];
 }


    FilterUser user_list=new FilterUser(username,email,phonenumber,datetime,endtime);
    
    var users=await this._userList.filterUserList(user_list);

    var user_paging=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),1,10);

    ViewBag.filter_user=user_list;

    return View(user_paging);     
    }
     catch(Exception er)
     {  Console.WriteLine("Exception Filter User here:"+er.Message);
        this._logger.LogTrace("Filter User List Exception:"+er.Message);
     }
     return View();
    }

      [Authorize(Roles ="Admin")]
    [Route("user_list/add")]
   [HttpGet]
  public IActionResult AddUserList()
  {
    return View();        
  }


  [Authorize(Roles ="Admin")]
  [Route("user_list/add")]
  [Authorize(Roles ="Admin")]
   [HttpPost]
   public async Task<IActionResult> AddUserList(Register user)
   {
  try
 {
  string username=user.UserName;
  string email=user.Email;
  string password= user.Password;
  string gender= user.Gender;
  //string avatar="https://cdn-icons-png.flaticon.com/128/3135/3135715.png";
string folder_name="UploadImageUser";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

   if(!Directory.Exists(upload_path))
   {
    Directory.CreateDirectory(upload_path);
   }
  var avatar=user.Avatar;
  if(avatar!=null)
  {
   string file_name=Guid.NewGuid()+"_"+Path.GetFileName(avatar.FileName);
  
   string file_path=Path.Combine(upload_path,file_name);

   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await avatar.CopyToAsync(fileStream);
   } 
  }
  Console.WriteLine(username);
  Console.WriteLine(email);
  Console.WriteLine(password);
  Console.WriteLine(gender);
  int res=await this._userList.createUser(user,"User");
     if(res==1)
     {
      ViewBag.Status=1;
      ViewBag.Created_User="Đã thêm user thành công";
     }
     else if(res==-1)
     {
      ViewBag.Status=-1;
      ViewBag.Created_User="Username hoặc Email này đã tồn tại trong hệ thống";
     }
     else
     {
      ViewBag.Status=0;
      ViewBag.Created_User="Thêm user thất bại";
     }
    
    }
    catch(Exception er)
    {
      Console.WriteLine("Add User Exception:"+er.InnerException?.Message??er.Message);
      this._logger.LogTrace("Add User Exception:"+er.InnerException?.Message??er.Message);
      ViewBag.Status=0;
      ViewBag.Created_User="Thêm user thất bại";
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
  [Route("user_list/user_info")]
  [HttpGet]
  public async Task<IActionResult> UserInfo(string email)
  {
   try
   {
     var user=await this._userList.findUserByEmail(email);
     if(user!=null)
     {
     return View("~/Views/UserList/UserInfo.cshtml",user);
     }
     else
     {
  return RedirectToAction("UserList","UserList");
     }
   }
   catch(Exception er)
   {
     Console.WriteLine("User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("User Info Exception:"+er.InnerException?.Message??er.Message); 
   }
  return RedirectToAction("UserList","UserList");
  }


[Authorize(Roles ="Admin")]
[Route("user_list/user_info")]
[HttpPost]
public async Task<IActionResult> UserInfo(UserInfo user)
{ int res_update=0;
  try
  {
    res_update=await this._userList.updateUser(user);
    if(res_update==1)
    {
      ViewBag.Status=1;
      ViewBag.Update_Message="Cập nhật User thành công";
    }
    else
    {
      ViewBag.Status=0;
      ViewBag.Update_Message="Cập nhật User thất bại";
    }
    var user_after=await this._userList.findUserById(user.Id);

    return View("~/Views/UserList/UserInfo.cshtml",user_after);

  }
  catch(Exception er)
  {
     Console.WriteLine("Update User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Update User Info Exception:"+er.InnerException?.Message??er.Message); 
  }
  return RedirectToAction("UserList","UserList");
} 
[Authorize(Roles ="Admin")]
[Route("user_list/user_info/delete")]
[HttpGet] 
public async Task<IActionResult> UserInfoDelete(string email)
{ 
  try
  {
   int res_delete=await this._userList.deleteUser(email);

   if(res_delete==1)
   {
    this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete user account {email}");

    TempData["Status_Delete"]=1;
    TempData["Message_Delete"] = "Xóa User thành công";
   }
   else
   {
  TempData["Status_Delete"]=0;
  TempData["Message_Delete"] = "Xóa User thất bại";
   }
  }
  catch(Exception er)
  {
     Console.WriteLine("Delete User Info Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Delete User Info Exception:"+er.InnerException?.Message??er.Message);    
  }
  return RedirectToAction("UserList","UserList");
}
[Authorize(Roles ="Admin")]
[Route("user_list/user_info/change_password")]
[HttpGet]
public async Task<IActionResult> ResetPasswordUser(string email)
{
  try
  {
    int res_change= await this._userList.changeUserPassword(email);
    if(res_change==1)
    {
    TempData["change_res"]=1;
    TempData["message_change"]="Mật khẩu mới của User là Ecommerce123@";
    }
   else
   {
    TempData["change_res"]=0;
    TempData["message_change"]="Đổi mật khẩu User thất bại";
   }
  }
  catch(Exception er)
  {
         Console.WriteLine("Reset User Password Exception:"+er.InnerException?.Message??er.Message);
     this._logger.LogTrace("Reset User Password Exception:"+er.InnerException?.Message??er.Message);    
  }
  return RedirectToAction("UserList","UserList");
}
[Authorize(Roles ="Admin")]
[Route("user_list/export_excel")]
[HttpGet]
public async Task<IActionResult> ExportExel()
{
  try
  {
  var content= await this._userList.exportToExcel();
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Users.xlsx");
  }
  catch(Exception er)
  {
   Console.WriteLine("Export Excel Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Export Excel Exception:"+er.InnerException?.Message??er.Message);      
  }
  return RedirectToAction("UserList","UserList");
}
[Authorize(Roles ="Admin")]
[Route("user_list/export_pdf")]
[HttpGet]
public async Task<IActionResult> ExportPdf()
{
  try
  {
  var content = await this._userList.exportToPDF();
  
  return File(content,"application/pdf","User_List.pdf");
  }
  catch(Exception er)
  {
     Console.WriteLine("Export Pdf Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Export Pdf Exception:"+er.InnerException?.Message??er.Message);     
  }
  return RedirectToAction("UserList","UserList");
}



[Route("{username}/info")]

[HttpGet]

public async Task<IActionResult> GetUserByName(string username)
{
  try
  {
    var user=await this._userList.findUserByName(username);
    if(user!=null)
    {
    return View("~/Views/UserList/UserInfo.cshtml",user);
    }
    else
    {
      return RedirectToAction("UserList","UserList");
    }
  }
  catch(Exception er)
  {
    Console.WriteLine("Get User By Name Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Get User By Name Exception:"+er.InnerException?.Message??er.Message);     
  }
  return RedirectToAction("UserList","UserList");
}

[Authorize(Roles ="Admin")]
[Route("user_list/export_csv")]
[HttpGet]

public async Task<IActionResult> ExportCsv()
{
  try
  {
  var content = await this._userList.exportToCSV();
  return File(content,"application/csv;charset=utf-8","User_List.csv");
  }
  catch(Exception er)
  {
  Console.WriteLine("Export Csv Exception:"+er.InnerException?.Message??er.Message);
    this._logger.LogTrace("Export Csv Exception:"+er.InnerException?.Message??er.Message);     
  }
  return RedirectToAction("UserList","UserList");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
