namespace Ecommerce_Product.Models;

public class CountryInfoModel
{
    public bool Error { get; set; }

    public string? Msg { get; set; }

    public List<CountryInfo>? Data { get; set; }
}

public class CountryInfo
{
    public string? Iso3 { get; set; }

    public string? Name { get; set; }

    public List<StateInfo>? States { get; set; }
}

public class StateInfo
{
    public string Name { get; set; }
    public string State_Code{ get; set; }
}

