using System.Text.Json.Serialization;

namespace Ecommerce_Product.Models
{
public class BankModel
{
 [JsonPropertyName("code")]
 public string Code {get;set;}

 [JsonPropertyName("data")]
 public List<Bank> Data {get;set;}
}

public class Bank
{
    [JsonPropertyName("id")]
    public int Id{get;set;}

    [JsonPropertyName("name")]
    
    public string Name{get;set;}

    [JsonPropertyName("bin")]

    public string Bin {get;set;}

    [JsonPropertyName("shortName")]
    public string ShortName{get;set;}

    [JsonPropertyName("swift_code")]

    public string Swift_Code{get;set;}

}
}