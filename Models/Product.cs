using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? ProductName { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCatId { get; set; }

    public int? BrandId { get; set; }

    public string? Price { get; set; }

    public string? Description { get; set; }

    public string? DiscountDescription { get; set; }

    public string? InboxDescription { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }

    public int? Quantity { get; set; }

    public string? Status { get; set; }

    public string? Frontavatar { get; set; }

    public string? Backavatar { get; set; }

    public string? Statdescription { get; set; }

    public int? Discount { get; set; }

    public int? SortId{get;set;}

    public int? SortProminentId{get;set;}

    public string? Manufacturer{get;set;}

    public string? Small_Description{get;set;}

    public string? Asin{get;set;}

    public string? Date_First_Available{get;set;}

    public string? Package_Dimensions{get;set;}

    public string? Model{get;set;}

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Manual> Manuals { get; set; } = new List<Manual>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Ratingdetail> Ratingdetails { get; set; } = new List<Ratingdetail>();

    public virtual ICollection<Reviewdetail> Reviewdetails { get; set; } = new List<Reviewdetail>();

    public virtual SubCategory? SubCat { get; set; }

    public virtual ICollection<Variant> Variants { get; set; } = new List<Variant>();

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
