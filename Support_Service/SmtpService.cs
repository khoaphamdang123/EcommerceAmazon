using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Ecommerce_Product.Models;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using MimeKit.Cryptography;
using System.Security.Policy;
using System.Runtime.InteropServices;
namespace Ecommerce_Product.Support_Serive;
public class SmtpService
{
private readonly SmtpModel _smtpClient;

private readonly Service _spService;

private readonly ILogger<SmtpService> _logger;


public SmtpService(IOptions<SmtpModel> smtpClient,Service spService,ILogger<SmtpService> logger)
{
    this._smtpClient=smtpClient.Value;
    this._spService=spService;
    this._logger=logger;
}

public string GenerateHmac(string timestamp)
    {   
      string serect_key=Environment.GetEnvironmentVariable("SECRET_KEY");

      Console.WriteLine("Secret Key:"+serect_key);
      
      using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(serect_key)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(timestamp));
            
            return Convert.ToBase64String(hash);
        }
    }


public string ConvertModelToQueryString<T>(T model)
{
    var properties = typeof(T).GetProperties();
    
    var queryString = HttpUtility.ParseQueryString(string.Empty);

    foreach (var property in properties)
    {
        var value = property.GetValue(model)?.ToString() ?? string.Empty;
        queryString[property.Name] = value;
    }

    return queryString.ToString();
}




public string RegisterContent(string url)
{
    string htmlContent="";
    
    string path=this._spService.GetCurrentFilePath("Views/MailTemplate/Register.html");
    
    using(StreamReader sr= new StreamReader(path))
    {
        htmlContent=sr.ReadToEnd();
    }

    htmlContent=htmlContent.Replace("_cur_url_",url);

    return htmlContent;
}

public string htmlContent(string receiver,string operating_system,string random_password,int role=1)
{
    string htmlContent="";

    string dns=Environment.GetEnvironmentVariable("DNS");
    
    string path=this._spService.GetCurrentFilePath("Views/MailTemplate/SendMailTemplate.html");
    
    using(StreamReader sr=new StreamReader(path))
    {
        htmlContent=sr.ReadToEnd();
    }
 string url="";
if(role==1)
{
 url=$"{dns}/admin/reset_password?email="+receiver+"&password="+random_password;
}
else
{
 url=$"{dns}/reset_password?email="+receiver+"&password="+random_password;
}

    htmlContent=htmlContent.Replace("{name}",receiver);
    htmlContent=htmlContent.Replace("{operating_system}",operating_system);
    //htmlContent=htmlContent.Replace("{browser_name}",browser_name);
    htmlContent=htmlContent.Replace("{new_password}",random_password);
    htmlContent=htmlContent.Replace("{email_value}",receiver);
    htmlContent=htmlContent.Replace("_cur_url_",url);
    Console.WriteLine(url);

    return htmlContent;
}

public string loginNotify(string username)
{
   string htmlContent="";
    
    string path=this._spService.GetCurrentFilePath("Views/MailTemplate/loginnotify.html");
    
    using(StreamReader sr=new StreamReader(path))
    {
        htmlContent=sr.ReadToEnd();
    }
    htmlContent=htmlContent.Replace("{account_name}",username);

    htmlContent=htmlContent.Replace("{username}","Admin");

    string datetime=DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    htmlContent=htmlContent.Replace("{date_time}",datetime);

    return htmlContent;
}

public async Task<bool> sendEmailGeneral(int type,string htmlContent,string receiver="")
{
  bool send_mail=false;
  try
  {
var emailMessage = new MimeMessage();

if(receiver=="")
{
 receiver="huynhkiengquan123@gmail.com";
}

string subject="";

if(type==1)
{
  subject="Thông báo người dùng đổi mật khẩu";
}
else if(type==2)
{
  subject="Order Successfully";
}
else if(type==3)
{
  subject="Đăng ký tài khoản";
}

   emailMessage.From.Add(new MailboxAddress(this._smtpClient.SenderName,this._smtpClient.SenderEmail));

   emailMessage.To.Add(new MailboxAddress("",receiver));

   emailMessage.Subject=subject;

   var bodyBuilder =new BodyBuilder{HtmlBody=htmlContent};
   
   emailMessage.Body=bodyBuilder.ToMessageBody();
   
   using(var client = new SmtpClient())
   {
      await client.ConnectAsync(_smtpClient.Host,this._smtpClient.Port,this._smtpClient.UseSsl);
      
      await client.AuthenticateAsync(this._smtpClient.Username,this._smtpClient.Password);
      
      await client.SendAsync(emailMessage);

      await client.DisconnectAsync(true);
   }

   send_mail=true;
   
   Console.WriteLine("Send Email Success");
  }
  catch(Exception er)
  { 
    Console.WriteLine("Send Email General Exception:"+er.Message);

    this._logger.LogTrace("Send Email General Exception:"+er.Message);
  }

  return send_mail;
}


// public string getCurrentBrowser()
// {
//     string web_brower=HttpContext..Headers["User-Agent"].ToString();
// }
public async Task<bool> sendEmail(string new_password,string receiver,string subject,int role=1)
{   
  bool is_sent=false;
  Console.WriteLine("did in this send email function");
  try
  {
    // Console.WriteLine("Port:"+this._smtpClient.Port);
    // Console.WriteLine("Username:"+this._smtpClient.Username);
    //     Console.WriteLine("Password:"+this._smtpClient.Password);

    // Console.WriteLine("SenderName:"+this._smtpClient.SenderName);

    // Console.WriteLine("SenderEmail:"+this._smtpClient.SenderEmail);

    // Console.WriteLine("Usessl:"+this._smtpClient.UseSsl);

    // Console.WriteLine("Host:"+this._smtpClient.Host);

    string currentOs=this._spService.getCurrentOs();
    
    string htmlValue=htmlContent(receiver,currentOs,new_password,role);
    
    Console.WriteLine(currentOs);

   var emailMessage = new MimeMessage();

   emailMessage.From.Add(new MailboxAddress(this._smtpClient.SenderName,this._smtpClient.SenderEmail));

   emailMessage.To.Add(new MailboxAddress("",receiver));

   emailMessage.Subject=subject;

   var bodyBuilder =new BodyBuilder{HtmlBody=htmlValue};
   
   emailMessage.Body=bodyBuilder.ToMessageBody();
   
   using(var client = new SmtpClient())
   {
      await client.ConnectAsync(_smtpClient.Host,this._smtpClient.Port,this._smtpClient.UseSsl);
      //Console.WriteLine("did here");
      
      await client.AuthenticateAsync(this._smtpClient.Username,this._smtpClient.Password);
      //Console.WriteLine("Did come to here");
      await client.SendAsync(emailMessage);
      await client.DisconnectAsync(true);
   }
   Console.WriteLine("finish send email");
   is_sent=true;
  }
  catch(Exception er)
  { this._logger.LogTrace("Smtp Exception:"+er.Message);
    Console.WriteLine("Send Smtp Exception:"+er.Message);
  }
  return is_sent;
}
}