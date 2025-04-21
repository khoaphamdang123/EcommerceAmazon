namespace Ecommerce_Product.Models
{
    public class AddCategoryModel
    {
     public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public IFormFile Avatar { get; set; }   
 }
}