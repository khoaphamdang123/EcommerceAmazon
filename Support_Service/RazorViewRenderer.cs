using System;
using RazorLight;
using Ecommerce_Product.Models;

namespace Ecommerce_Product.Support_Serive;

public class RazorViewRenderer
{
    private readonly IRazorLightEngine _engine;

    public RazorViewRenderer()
    {
     string basePath = Directory.GetCurrentDirectory();
     
     string viewsPath = Path.Combine(basePath, "Views");

     Console.WriteLine("View Path:"+viewsPath); 

        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(viewsPath)
            .UseMemoryCachingProvider()
            .Build();        
    }

    public async Task<string> RenderViewToStringAsync<T>(string viewName, T model)
    {   
        return await _engine.CompileRenderAsync(viewName, model);
    }
}