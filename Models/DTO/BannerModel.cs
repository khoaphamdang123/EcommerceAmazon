namespace Ecommerce_Product.Models
{
    public class BannerModel
    {
     public int Id { get; set; }

    public string? BannerName { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public IFormFile Image { get; set; }   
    }
}