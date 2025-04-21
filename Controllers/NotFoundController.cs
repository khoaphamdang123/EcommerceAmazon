
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce_Product.Controllers;
public class NotFoundController:BaseController
{
private readonly ILogger<NotFoundController> _logger;
private readonly ICategoryListRepository _category;

public NotFoundController(ICategoryListRepository category,IUserListRepository user,IBannerListRepository banner,ILogger<NotFoundController> logger):base(category,user,banner)
{
    this._category=category;
    this._logger=logger;
}

[Route("404")]
[HttpGet]
public IActionResult NotFound()
{   
    return View("~/Views/ClientSide/NotFound/NotFound.cshtml");
}

[Route("maintainance")]
[HttpGet]
public IActionResult Maintainance()
{ 
    Console.WriteLine("maintian here");
    return View("~/Views/ClientSide/NotFound/Maintainance.cshtml");
}
}