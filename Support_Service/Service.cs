using System.Security.Cryptography;
using System.Text;
using System.Management;
using Microsoft.AspNetCore.Identity;
using Ecommerce_Product.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Ecommerce_Product.Models;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Ecommerce_Product.Support_Serive;

public class Service
{
    private readonly ILogger<Service> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;



    public Service(ILogger<Service> logger,SignInManager<ApplicationUser> signInManager)
    {
        this._logger=logger;
        this._signInManager=signInManager;
    }


public async Task<string> convertVNDToUSD(string value)
{   string ussd_value="";
    try
    {
    string url="https://api.exchangerate-api.com/v4/latest/VND";
    using(var client=new HttpClient())
    {
        client.BaseAddress=new Uri(url);
        HttpResponseMessage response=await client.GetAsync(client.BaseAddress);
        if(response.IsSuccessStatusCode)
        {
            string data=await response.Content.ReadAsStringAsync();
            var json_data=JsonConvert.DeserializeObject<ExchangeRateModel>(data);
            if(json_data!=null)
            {   
                decimal exchange_rate=json_data.Rates.USD;
                decimal vnd_value=Convert.ToDecimal(value);
                decimal usd_value=vnd_value/exchange_rate;
                Console.WriteLine("Usd value:"+usd_value.ToString("N0"));
                return usd_value.ToString("N0");
            }
        }
    }
}
    catch(Exception er)
    {
        Console.WriteLine("Convert VND to USD Exception:"+er.Message);
    }
    return ussd_value;
}

public string extractASIN(string link)
{ try
{
   string pattern = @"/dp/([A-Z0-9]{10})";

    Match match = Regex.Match(link, pattern);
    
    if (match.Success)
    {
        string asin = match.Groups[1].Value;
        
        return asin;
    }
}
catch(Exception er)
{
    Console.WriteLine("Extract ASIN Exception:"+er.Message);
}
    return null;
}

public string generateQRCode(string data,string account_num,string account_name)
{  
    string qr_code_img="";

    try
    {
    string url="https://api.vietqr.io/v2/generate";
    
    using(var client = new HttpClient())
    {
     client.BaseAddress=new Uri(url);

     string[] bank_data_detail=data.Split("*");

     string bank_name=bank_data_detail[0];

     string bank_code = bank_data_detail[1];

     Console.WriteLine("Bank Name:"+bank_name);

     Console.WriteLine("Bank Code:"+bank_code);

     Console.WriteLine("Account Number:"+account_num);

     Console.WriteLine("Account Name:"+account_name);
         
     FormUrlEncodedContent content=new FormUrlEncodedContent(new[]
     {
         new KeyValuePair<string,string>("accountNo",account_num),
         new KeyValuePair<string, string>("accountName",account_name),
         new KeyValuePair<string,string>("acqId",bank_code),
         new KeyValuePair<string,string>("template","compact"),
     });

        HttpResponseMessage response=client.PostAsync(client.BaseAddress,content).Result;

        if(response.IsSuccessStatusCode)
        {  Console.WriteLine("did success here");
            string qr_data=response.Content.ReadAsStringAsync().Result;
            var qr_code=JsonConvert.DeserializeObject<QrModel>(qr_data);

          if(qr_code!=null)
          { if(qr_code.Code=="00")
          {
            qr_code_img=qr_code.Data.QrDataURL;
          }
          else
          {
            qr_code_img="ERROR";
          }
          }
    }
    }
    }
    catch(Exception er)
    {  Console.WriteLine("Generate QR Code Exception:"+er.Message);
        this._logger.LogTrace("Generate QR Code Exception:"+er.Message);
    }
    return qr_code_img;
}

 public BankModel getListBank()
 {  
    BankModel bankModel=new BankModel();
    
    try
    {
    using(HttpClient client=new HttpClient())
    {
        client.BaseAddress=new Uri("https://api.vietqr.io/v2/banks");
        HttpResponseMessage response=client.GetAsync(client.BaseAddress).Result;
        if(response.IsSuccessStatusCode)
        {
            string data=response.Content.ReadAsStringAsync().Result;            
            
            bankModel=JsonConvert.DeserializeObject<BankModel>(data);
        }
    }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get List Bank Exception:"+er.Message);
    }
    return bankModel;
 }
public string convertToVND(string value)
{
    return Convert.ToInt32(value).ToString("N0");    
}
public string AddSha256(string data)
 {  
    StringBuilder sha_hash=new StringBuilder();
 try
 {
    using(SHA256 hash=SHA256.Create())
    {   
        byte[] bytes=hash.ComputeHash(Encoding.UTF8.GetBytes(data));

        for(int i=0;i<bytes.Length;i++)
        {
            sha_hash.Append(bytes[i].ToString("x2"));
        }
    }
 }
 catch(Exception er)
 {
    this._logger.LogTrace("AddSha256:"+er.Message);
 }
    return sha_hash.ToString();

 }

public async Task<int> removeFiles(string filepath)
{ int delete_res=0;
try
{ 
  if(File.Exists(filepath))
  { 
    File.Delete(filepath);
    delete_res=1;
  }
}
catch(IOException er)
{
    Console.WriteLine("Remove File Exception:"+er.Message);
}
return delete_res;
}



  public string GetCurrentFilePath(string file_name)
 {
    string full_file_path=Path.Combine(Directory.GetCurrentDirectory(),file_name);
    return full_file_path;
 }

public void writeCsvFile(string file_name,string data)
{
    try
    {
    
    string file_path=GetCurrentFilePath(file_name);
    
    using(var writer=new StreamWriter(file_path,true,Encoding.UTF8))
    {
        writer.WriteLine(data);
    }
    }
    catch(Exception ex)
    {
     this._logger.LogTrace("Write CSV Exception:"+ex.Message);
    }
}

public string generateRandomPassword()
{
  string random_password="Acb@";
  Random r= new Random();
  for(int i=1;i<=5;i++)
  {
    random_password+=r.Next(0,9);
  }
  return random_password;
}

public async Task<RecaptchaResponse> ValidateRecaptcha(string recaptchaToken)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={"6LeQYl0qAAAAAGMswsbJBdkpb_anbatHza9Be52a"}&response={recaptchaToken}");

        return JsonConvert.DeserializeObject<RecaptchaResponse>(response);        
    }

public string getCurrentOs()
{
//     var os_name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
//                       select x.GetPropertyValue("Caption")).FirstOrDefault();


// return os_name != null ? os_name.ToString() : "Unknown";
 string os_name="";

 if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
 {
     os_name="Windows";
 }
 else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
 {
     os_name="Linux";     
 }
 else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
 {
     os_name="OSX";
 }
 else
 {
     os_name="Unknown";
 }
 return os_name;
}



public async Task logoutUser()
{
   await this._signInManager.SignOutAsync();
}


public List<string> getListOfLanguage()
{
    List<string> languages=new List<string>();

    foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
    {
        languages.Add(culture.DisplayName);
    }

    return languages;    

}
}