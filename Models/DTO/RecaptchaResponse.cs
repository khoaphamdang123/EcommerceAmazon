
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
public class RecaptchaResponse
{
   public string SiteKey{get;set;}
   public string SecretKey{get;set;}
}