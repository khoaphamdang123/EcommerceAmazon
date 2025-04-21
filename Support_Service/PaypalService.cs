using PayPalCheckoutSdk.Core;
using Microsoft.Extensions.Configuration;

namespace Ecommerce_Product.Support_Serive;

public class PaypalService
{  
    private readonly IConfiguration _configuration;
    public PaypalService(IConfiguration configuration)
    {
        this._configuration=configuration;
    }
  
   public PayPalHttpClient getClient()
   {
 
  Console.WriteLine("Paypal Client ID:"+this._configuration["Paypal:Mode"]);
  
  PayPalEnvironment environment = this._configuration["Paypal:Mode"]=="sandbox" ? new SandboxEnvironment(this._configuration["Paypal:ClientId"],this._configuration["Paypal:ClientSecret"]):new LiveEnvironment(this._configuration["Paypal:ClientId"],this._configuration["Paypal:ClientSecret"]);  
  
  return new PayPalHttpClient(environment);  
   }

}