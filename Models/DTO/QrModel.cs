using System.Text.Json.Serialization;

namespace Ecommerce_Product.Models
{
public class QrModel
{
 [JsonPropertyName("code")]
 public string Code {get;set;}

 [JsonPropertyName("data")]
 public Qr Data {get;set;}
}

public class Qr
{
    [JsonPropertyName("qrCode")]
    public string QrCode{get;set;}
    
    [JsonPropertyName("qrDataURL")]
    public string QrDataURL{get;set;}
}
}