namespace Ecommerce_Product.Models
{
    public class BlogModel
    {
    public string Author { get; set; } = null!;

    public string Blogname { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Createddate { get; set; } = null!;

    public string Updateddate { get; set; } = null!;

    public IFormFile FeatureImage { get; set; } = null!;

    public int CategoryId { get; set; }    }
}