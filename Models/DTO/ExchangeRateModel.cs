namespace Ecommerce_Product.Models
{
    public class ExchangeRateModel
    {   
        public Rate Rates{get;set;}
    }

    public class Rate
    {
        public decimal USD {get;set;}
        public decimal AUD{get;set;}
    }
}