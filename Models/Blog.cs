using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Web;

namespace Ecommerce_Product.Models;

public partial class Blog
{
    public int Id { get; set; }

    public string Author { get; set; } = null!;

    public string Blogname { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Createddate { get; set; } = null!;

    public string Updateddate { get; set; } = null!;

    public string FeatureImage { get; set; } = null!;

    public int CategoryId { get; set; }

     public string GetFirstPContent()
    {
        if (string.IsNullOrEmpty(Content))
        { 
            return string.Empty;
        }
        var doc = new HtmlDocument();
        string clean_content=HttpUtility.HtmlDecode(Content);
        
        doc.LoadHtml(clean_content);

        var firstP = doc.DocumentNode.SelectNodes("//p")?[1];
     
        string fullText="";
        fullText= firstP != null ? firstP.InnerText : string.Empty;
        fullText=fullText.Replace("&nbsp;"," ");
        var words= fullText.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        if(words.Length<=30)
        {
            return fullText;            
        }
        return string.Join(" ",words.Take(30))+"...";
    }

    public virtual Category Category { get; set; } = null!;
}
