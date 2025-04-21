using Ecommerce_Product.Repository;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Support_Serive;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Text;
namespace Ecommerce_Product.Service;
public class UserListService:IUserListRepository
{
  
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _support_service;

    private readonly SmtpService _smtpService;

    private readonly ILogger<LoginService> _logger;

    private readonly IWebHostEnvironment _webHostEnv;

    public UserListService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,EcommerceshopContext context,Support_Serive.Service service,SmtpService smtpService,ILogger<LoginService> logger,IWebHostEnvironment webHost)
    {
        this._userManager=userManager;
        this._roleManager=roleManager;
        this._support_service=service;
        this._smtpService=smtpService;
        this._context=context;
        this._logger=logger;
        this._webHostEnv=webHost;
    }
 
    public async Task<IEnumerable<ApplicationUser>> filterUserList(FilterUser user)
    {
    string username=user.UserName;
    string email=user.Email;
    string phonenumber=user.PhoneNumber;
    string datetime=user.DateTime;
    string start_date=user.DateTime;
    string end_date=user.EndTime;
    var users=this._userManager.GetUsersInRoleAsync("User").Result.ToList();
    if(!string.IsNullOrEmpty(username))
    {
        users=users.Where(u=>u.UserName==username).ToList();
    }
    if(!string.IsNullOrEmpty(email))
    {
        users=users.Where(u=>u.Email==email).ToList();
    }
    if(!string.IsNullOrEmpty(phonenumber))
    {
        users=users.Where(u=>u.PhoneNumber==phonenumber).ToList();
    }
    if(!string.IsNullOrEmpty(start_date) && string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    users= users.Where(c=>DateTime.TryParse(c.Created_Date,out var startDate) && DateTime.TryParse(start_date,out var lowerDate) && startDate.Date==lowerDate.Date).ToList();
   }
   else if(string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   { 
    
    end_date=end_date.Trim();
   
    users= users.Where(c=>DateTime.TryParse(c.Created_Date,out var startDate) && DateTime.TryParse(end_date,out var upperDate) && startDate.Date==upperDate.Date).ToList();
   }
   else if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    end_date=end_date.Trim();    
    users=users.Where(c=>DateTime.TryParse(c.Created_Date,out var createdDate)&& DateTime.TryParse(start_date,out var startDate) && DateTime.TryParse(end_date,out var endDate) && createdDate>=startDate && createdDate<=endDate).ToList();
   }
    return users;
    }

    public async Task<IEnumerable<ApplicationUser>> getAllUserList()
    {
      string role="User";
      string sub_role="Anonymous";       
      var users=this._userManager.Users.ToList();
      List<ApplicationUser> userList=new List<ApplicationUser>();
      foreach(var user in users)
      {
        if(await this._userManager.IsInRoleAsync(user,role) || await this._userManager.IsInRoleAsync(user,sub_role))
        {
            userList.Add(user);
        }
      }
      return userList;
    }

   public async Task<PageList<ApplicationUser>> pagingUser(int page_size,int page)
   { 

    if(page_size<page)
    {
        return PageList<ApplicationUser>.CreateItem(new List<ApplicationUser>().AsQueryable(),0,0);
    }
 
   IEnumerable<ApplicationUser> all_user= await this.getAllUserList();

   List<ApplicationUser> users=all_user.ToList(); 

   //var users=this._userManager.Users;
   
   var user_list=PageList<ApplicationUser>.CreateItem(users.AsQueryable(),page,page_size);
   
   return user_list;
   }

   public async Task<ApplicationUser> findUserByName(string name)
   {
    var user=await this._userManager.FindByNameAsync(name);

   if(await this._userManager.IsInRoleAsync(user,"User"))
    {
    return user;
    }
    return null;
   }

   

   
public async Task<bool> checkUserExist(string email,string username)
{
    bool res=false;
    var check_user_email_exist=await this._userManager.FindByEmailAsync(email);
     
    if(check_user_email_exist!=null)
     {
         res=true;
     }
   
    return res;
}

public async Task<bool> createRole(string role)
{
  bool create_role=false;
  if(!string.IsNullOrEmpty(role))
  {
    if(!await this._roleManager.RoleExistsAsync(role))
    {
      var new_role=new IdentityRole(role);
      
      var res=await this._roleManager.CreateAsync(new_role);
      
      if(res.Succeeded)
      {
        create_role=true;
      }
    }
  }
  return create_role;
}





   public async Task<int> createUser(Register user,string role)
   { 
     int res_created=0;

    bool is_existed=await checkUserExist(user.Email,user.UserName);

    if(is_existed)
    {   res_created=-1;
        
        return res_created;
    }
     var users=this._userManager.Users;
     int seq=1;
      var latestUser = await users
            .OrderByDescending(u => u.Seq)  // Replace with the correct field to order by
            .FirstOrDefaultAsync();
    if(latestUser!=null)
    {
      seq=(latestUser.Seq??0)+1;
    }
  string folder_name="UploadImageUser";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

   if(!Directory.Exists(upload_path))
   {
    Directory.CreateDirectory(upload_path);
   }
  //  string avatar_url="";
  // var avatar_obj=user.Avatar;
  // if(avatar_obj!=null)
  // {
  //  string file_name=Guid.NewGuid()+"_"+Path.GetFileName(avatar_obj.FileName);
  
  //  string file_path=Path.Combine(upload_path,file_name);      

  //  using(var fileStream=new FileStream(file_path,FileMode.Create))
  //  {
  //   await avatar_obj.CopyToAsync(fileStream);    
  //  } 
  //  avatar_url=file_path;
  // }
     string avatar="https://demos.themeselection.com/sneat-bootstrap-html-admin-template-free/assets/img/avatars/1.png";
     
     string created_date=DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
     
     var new_user=new ApplicationUser{UserName = user.UserName.Replace(" ",""),Email=user.Email,Address1=user.Address1,Address2=user.Address2,Gender=user.Gender,PhoneNumber=user.PhoneNumber,Created_Date=created_date,Seq=seq,Avatar=avatar};
     
     var res=await this._userManager.CreateAsync(new_user,user.Password);
     if(res.Succeeded)
     {  
        await this._userManager.AddToRoleAsync(new_user,role);
        res_created=1;
     }
     else
     {
      foreach (var error in res.Errors)
    {
        Console.WriteLine("Create User:"+error.Description);
    }
     }
     return res_created;
   }
  
  public async Task<ApplicationUser> findUserByEmail(string email)
  {
    var user=await this._userManager.FindByEmailAsync(email);
    return user;
  }

  public async Task<int> updateUser(UserInfo user_info)
  {
   int res=0;
   try{
   string id=user_info.Id;
   string cur_avatar="";
   var user=await this._userManager.FindByIdAsync(id);
   if(user!=null)
   {
     if(user_info.UserName=="company")
     {
      user.UserName=user_info.UserName.Replace(" ","").Trim();
      
      
      user.PhoneNumber=user_info.PhoneNumber;
      
      user.Address1=user_info.Address1;
      
      user.Address2=user_info.Address2;

      string extra_info="";

      if(!string.IsNullOrEmpty(user_info.BankName))
      {
        Environment.SetEnvironmentVariable("bank",user_info.BankName);
      }


 string qr_code_img=this._support_service.generateQRCode(user_info.BankName,user_info.AccountNum,user_info.AccountName);

 
 
  Console.WriteLine("QR Code Image:"+qr_code_img);
    
     Dictionary<string,string> dict_info=new Dictionary<string,string>
     {
      {"bank_name",user_info.BankName},
      {"account_num",user_info.AccountNum},
      {"account_name",user_info.AccountName},
      {"facebook",user_info.Facebook},
      {"zalo",user_info.Zalo},
      {"youtube",user_info.Youtube},
      {"instagram",user_info.Instagram},
      {"telegram",user_info.Telegram},
     };
      
     extra_info=string.Join(Environment.NewLine,dict_info.Select(x=>$"{x.Key}~{x.Value}"));
     
     user_info.Email=user_info.Email+"#"+extra_info;
     
     user.Email=user_info.Email;
     
     Console.WriteLine("Extra Info convert:"+extra_info);    


     var res_update_company=await this._userManager.UpdateAsync(user);
      
      if(!res_update_company.Succeeded)
      {
        Console.WriteLine("Error update user");
        
        foreach(var err in res_update_company.Errors)
        {
            Console.WriteLine("Error update user:"+err.Description);
        }

      }
  if(qr_code_img=="ERROR")
 {
  return -1;
 } 
   Environment.SetEnvironmentVariable("qr_code",qr_code_img);
      res=1;
      return res;
     }
      user.UserName=user_info.UserName.Replace(" ","").Trim();
      
      user.Email=user_info.Email.Trim();
      
      user.PhoneNumber=user_info.PhoneNumber.Trim();
      
      user.Address1=user_info.Address1;
      
      user.Address2=user_info.Address2;
      
      user.Gender=user_info.Gender;
      
      cur_avatar=user.Avatar;  
    
   string folder_name="UploadImageUser";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

   if(!Directory.Exists(upload_path))
   {
    Directory.CreateDirectory(upload_path);
   }
   string avatar_url="";
  var avatar=user_info.Avatar;

  if(avatar!=null)
  {
    string filename=Path.GetFileName(avatar.FileName);
    if(filename.Contains("___"))
    {
    var arr_value=filename.Split("___");
    filename=arr_value[arr_value.Length-1];
    }
    
   string file_name=Guid.NewGuid()+"___"+filename;

   Console.WriteLine("File name here is:"+file_name);
  
   string file_path=Path.Combine(upload_path,file_name);

   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await avatar.CopyToAsync(fileStream);
   } 
   
   avatar_url=file_path;
   
   user.Avatar=avatar_url;
  }
  else
  {
    user.Avatar="https://cdn-icons-png.flaticon.com/128/3135/3135715.png";
  }
  

      var res_update=await this._userManager.UpdateAsync(user);
      if(!res_update.Succeeded)
      {
        foreach(var err in res_update.Errors)
        {
            Console.WriteLine("Error update user:"+err.Description);
        }
      }
      else
      {
        res=1;
       if(!string.IsNullOrEmpty(cur_avatar))
        await this._support_service.removeFiles(cur_avatar);
      }
   }
   }
   catch(Exception er)
   {
    Console.WriteLine("Update User Info Exception:"+er.InnerException);
   }
   return res;
  }

  public async Task<ApplicationUser> findUserById(string id)
  {
    var user=await this._userManager.FindByIdAsync(id);
    return user;
  }

  public async Task<int> deleteUser(string email)
  {  int res_delete=0;
     var user=await this._userManager.FindByEmailAsync(email);
     string curr_avatar=user.Avatar;
     if(user!=null)
     {
        var deleted_user=await this._userManager.DeleteAsync(user);
        if(deleted_user.Succeeded)
        {
            res_delete=1;
            if(!string.IsNullOrEmpty(curr_avatar))
            {
                await this._support_service.removeFiles(curr_avatar);
            }
        }
        else
        {   
            foreach(var err in deleted_user.Errors)
            {
                Console.WriteLine("Exception in deleting user:"+err.Description);
            }
        }
     }
     return res_delete;
  }
  
 public async Task<int> changeUserPassword(string email)
 {  
    int change_pass_res=0;
    var user = await this._userManager.FindByEmailAsync(email);
    if(user!=null)
    {
        string new_password=Environment.GetEnvironmentVariable("RESET_PASSWORD");
        string change_pass_token=await this._userManager.GeneratePasswordResetTokenAsync(user);
        var change_res=await this._userManager.ResetPasswordAsync(user,change_pass_token,new_password);
        if(change_res.Succeeded)
        {
            change_pass_res=1;
        }
        else{
            foreach(var error in change_res.Errors)
            {
                Console.WriteLine("Change User Password exception:"+error.Description);
            }
        }
    }
    return change_pass_res;
 }

  public async Task<MemoryStream> exportToExcel()
 {
  using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("User");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên User";
    worksheet.Cells[1,3].Value = "Email";
    worksheet.Cells[1,4].Value="Giới tính";
    worksheet.Cells[1,5].Value="Số điện thoại";
    worksheet.Cells[1,6].Value="Địa chỉ 1";
    worksheet.Cells[1,7].Value="Địa chỉ 2";
    worksheet.Cells[1,8].Value="Ngày tạo";
    var user=await this.getAllUserList();
    if(user!=null)
    {
Console.WriteLine("this user list is not null");
    List<ApplicationUser> list_user=user.ToList();
    Console.WriteLine("Length of user list here is:"+list_user.Count);
    for(int i=0;i<list_user.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_user[i].UserName;
    
    worksheet.Cells[i+2,3].Value=list_user[i].Email;
    
    worksheet.Cells[i+2,4].Value=list_user[i].Gender;

     worksheet.Cells[i+2,5].Value=list_user[i].PhoneNumber;

     worksheet.Cells[i+2,6].Value=list_user[i].Address1;

     worksheet.Cells[i+2,7].Value=list_user[i].Address2;

     worksheet.Cells[i+2,8].Value=list_user[i].Created_Date;
    Console.WriteLine("UserName:"+list_user[i].UserName);
        Console.WriteLine("UserName:"+list_user[i].Email);

    Console.WriteLine("UserName:"+list_user[i].PhoneNumber);

    Console.WriteLine("UserName:"+list_user[i].Gender);
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  Console.WriteLine("content here is:"+stream);
  return stream;
  }
 }

  public async Task<byte[]> exportToPDF()
 { 

 MemoryStream ms=new MemoryStream();
 try
 {
  using(PdfWriter writer=new PdfWriter(ms))
  {
    PdfDocument pdfDoc=new PdfDocument(writer);
    Document dc=new Document(pdfDoc);
    dc.Add(new Paragraph("User List").SetFontSize(20).SetBold());
    Table table = new Table(8);
    table.AddCell("STT");
    table.AddCell("Tên User");
    table.AddCell("Email");
    table.AddCell("Giới tính");
    table.AddCell("Số điện thoại");
    table.AddCell("Địa chỉ 1");
    table.AddCell("Địa chỉ 2");
    table.AddCell("Ngày tạo");
    var users=await this.getAllUserList();
  if(users!=null)
  {
    List<ApplicationUser> list_user = users.ToList();
    Console.WriteLine("User count:"+list_user.Count);
    int count_user=0;
    foreach(var user in list_user)
    {   count_user+=1;
         table.AddCell(count_user.ToString());
         table.AddCell(user.UserName);
         table.AddCell(user.Email);
         table.AddCell(user.Gender);
         table.AddCell(user.PhoneNumber);
         table.AddCell(user.Address1);
         table.AddCell(user.Address2);
         table.AddCell(user.Created_Date);
    }
  }
   dc.Add(table);
   dc.Close();
  }
 }
 catch(Exception er)
 {
    Console.WriteLine("PDF Exception:"+er.Message);
 }
   ms.Position=0;
  byte[] content = ms.ToArray();
  return content;
 }

 public async Task<AspNetUser> getAspUser(string id)
 {
  var user=await this._context.AspNetUsers.FindAsync(id);
  return user;
 }





public async Task<byte[]> exportToCSV()
{
  StringBuilder csv=new StringBuilder();

  

  csv.AppendLine("STT,Tên User,Email,Giới tính,Số điện thoại,Địa chỉ 1,Địa chỉ 2,Ngày tạo");
  var users=await this.getAllUserList();
  if(users!=null)
  {
    int count_user=0;
    List<ApplicationUser> list_user= users.ToList();
    foreach(var user in list_user)
    {
     count_user+=1;
    
     csv.AppendLine($"{count_user},{user.UserName},{user.Email},{user.Gender},{user.PhoneNumber},{user.Address1},{user.Address2},{user.Created_Date}");
    }
  }
   byte[] bytes=Encoding.UTF8.GetBytes(csv.ToString());

    var bom = Encoding.UTF8.GetPreamble();
    var fileBytes = new byte[bom.Length + bytes.Length];
    System.Buffer.BlockCopy(bom, 0, fileBytes, 0, bom.Length);
    System.Buffer.BlockCopy(bytes, 0, fileBytes, bom.Length, bytes.Length);
   return bytes;
}

 }