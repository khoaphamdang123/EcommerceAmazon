
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Repository;
namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class ErrorController:BaseAdminController
{

public ErrorController(IBannerListRepository banner):base(banner)
{
    
}
[Route("Error/404")]
public IActionResult NotFound()
{   ViewBag.Username=HttpContext.Session.GetString("Username");
    return View();
}

[Route("Error/502")]
 
public IActionResult ServerError()
{    
    return View();
}

// [Route("Maintainance")]
// public IActionResult Maintainance()
// {
//     return View();
// }
}