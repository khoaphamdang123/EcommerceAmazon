using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.ComponentModel;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class OrderListController : BaseAdminController
{
    private readonly ILogger<OrderListController> _logger;

    // private readonly ICategoryRepository _categoryList;

    // public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    // {
    //     _logger = logger;
    //    this._categoryList=categoryList; 
    // }

   private readonly IOrderRepository _order;


   
   public OrderListController(IOrderRepository order,IBannerListRepository banner,ILogger<OrderListController> logger):base(banner)
   {
  this._order=order;
  this._logger=logger;   
   }
  [Authorize(Roles ="Admin")]
  [Route("order")]
  [HttpGet]
  public async Task<IActionResult> OrderList()
  {       
    
    string id_user=this.HttpContext.Session.GetString("AdminId");

if(string.IsNullOrEmpty(id_user))
{
  return RedirectToAction("Index","LoginAdmin");
}
    string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          int processing_count=this._order.countOrderStatus("Processing");
            int completed_count=this._order.countOrderStatus("Finished");
            int cancelled_count=this._order.countOrderStatus("Cancelled");
            int refund_count=this._order.countOrderStatus("Refund");


            ViewBag.processing_count=processing_count; 
            ViewBag.completed_count=completed_count;
            ViewBag.cancelled_count=cancelled_count;
            ViewBag.refund_count=refund_count;
    try
    {  
        var order=await this._order.pagingOrderList(7,1);
        return View(order);
        
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Static File List Exception:"+er.Message);
    }
    return View();
  }
  
  [Route("order/paging")]
  [HttpGet]
  public async Task<IActionResult> OrderListPaging([FromQuery]int page_size,[FromQuery] int page=1,string status="")
  {
    try
    {
        var order=await this._order.pagingOrderList(page_size,page);
        if(!string.IsNullOrEmpty(status))
         {
            var filtered_order = await this._order.filterOrderList(status);
            var filtered_order_paging=PageList<Order>.CreateItem(filtered_order.AsQueryable(),page,page_size);
            ViewBag.filter_obj=filtered_order_paging;
         }
         string select_size=page_size.ToString();
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          
          int processing_count=this._order.countOrderStatus("Processing");
          
          int completed_count=this._order.countOrderStatus("Finished");
          
          int cancelled_count=this._order.countOrderStatus("Cancelled");
            
          int refund_count=this._order.countOrderStatus("Refund");

            ViewBag.processing_count=processing_count; 
            ViewBag.completed_count=completed_count;
            ViewBag.cancelled_count=cancelled_count;
            ViewBag.refund_count=refund_count;
        return View("OrderList",order);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Paging Order List Exception:"+er.Message);
    }
    return View("OrderList");
  }



  [Route("order/filter")]
  [HttpGet]
  public async Task<IActionResult>filterOrderList(string status)
  {
   try
   {
    var order=await this._order.filterOrderList(status);
      this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} filter Order List");

        var filtered_order_paging=PageList<Order>.CreateItem(order.AsQueryable(),1,7);
         string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          ViewBag.filter_obj=status;
          int processing_count=this._order.countOrderStatus("Processing");
            int completed_count=this._order.countOrderStatus("Finished");
            int cancelled_count=this._order.countOrderStatus("Cancelled");
            int refund_count=this._order.countOrderStatus("Refund");
                        
            ViewBag.processing_count=processing_count; 
            ViewBag.completed_count=completed_count;
            ViewBag.cancelled_count=cancelled_count;
            ViewBag.refund_count=refund_count;
    return View("OrderList",filtered_order_paging);
   }
   catch(Exception er)
   {
         this._logger.LogTrace("Filter Order List Exception:"+er.Message);
   }
  return null;
  }

  [Route("order/export")]
  [HttpGet]
  public async Task<IActionResult> ExportToExcel()
  {
    try
    {
  var content= await this._order.exportToExcel();
  
  this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Export Order List");
  
  return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Orders.xlsx");
  
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Export Order List Exception:"+er.Message);
    }
    return RedirectToAction("OrderList");
  }
  

  [Route("order/{id}/delete/{detail_id}")]
  [HttpGet]

  public async Task<IActionResult> DeleteOrderDetailByProduct(int id,int detail_id)
  {    var order=await this._order.findOrderById(id);
       
       var orderDetail = await this._order.deleteProductOrderDetail(detail_id);

    try
    {     

     if(orderDetail==0)
     { 
        ViewBag.Status=0;
        ViewBag.Delete_Message="Xóa sản phẩm trong đơn hàng thất bại";
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete Order Detail Id:{detail_id} from order id:{id} Failed");

     }
     else
     {
        ViewBag.Status=1;
        ViewBag.Delete_Message="Xóa sản phẩm trong đơn hàng thành công";
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete Order Detail Id:{detail_id} from order id:{id} Success");
     }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Delete Order Detail Exception:"+er.Message);
    }
    return View("~/Views/OrderList/OrderDetail.cshtml",order);
  }

  [Route("order/{id}/delete")]

  [HttpGet]

  public async Task<IActionResult> DeleteOrder(int id)
  { Console.WriteLine("did come to delete order");
    try
    {
    var order=await this._order.deleteOrder(id);
    if(order==0)
    {
        ViewBag.Delete_Status=0;
        ViewBag.Delete_Message="Xóa đơn hàng thất bại";
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete Order Id:{id} Failed");
    }
    else
    {
        ViewBag.Delete_Status=1;
        ViewBag.Delete_Message="Xóa đơn hàng thành công";
    this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete Order Id:{id}");
    }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Delete Order Exception:"+er.Message);
    }
  
    string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
          int processing_count=this._order.countOrderStatus("Processing");
            int completed_count=this._order.countOrderStatus("Finished");
            int cancelled_count=this._order.countOrderStatus("Cancelled");
            int refund_count=this._order.countOrderStatus("Refund");

            ViewBag.processing_count=processing_count; 
            ViewBag.completed_count=completed_count;
            ViewBag.cancelled_count=cancelled_count;
            ViewBag.refund_count=refund_count;
            var orders=await this._order.pagingOrderList(7,1);

    return View("~/Views/OrderList/OrderList.cshtml",orders);
  }


  [Route("order/{id}/status")]
  [HttpGet]
  public async Task<IActionResult> OrderStatus(int id)
  {
    var order=await this._order.findOrderById(id);
  
    return View("~/Views/OrderList/UpdateOrder.cshtml",order);
  }

  [Route("order/update_status")]
  
  [HttpPost]
  public async Task<IActionResult> UpdateOrderStatus(int id,string status)
  {Console.WriteLine("did come to update order status");
    try
    {
    var updated_order=await this._order.updateOrderStatus(id,status);
    if(updated_order==0)
    {
        ViewBag.Status=0;
        ViewBag.Message="Cập nhật trạng thái đơn hàng thất bại";
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Update Order Status Failed");
    }
    else
    {
        ViewBag.Status=1;
        ViewBag.Message="Cập nhật trạng thái đơn hàng thành công";
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Update Order Status Success");
    }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Update Order Status Exception:"+er.Message);
    }
    
    var order=await this._order.findOrderById(id);    
    
    return View("~/Views/OrderList/UpdateOrder.cshtml",order);

  }

  [Route("order/{id}/detail/delete")]
  
  [HttpGet]
  public async Task<IActionResult> DeleteOrderDetailByProducts(int id,int[] ids)
  { Console.WriteLine("Did come here");
  Console.WriteLine(ids.Length);
    var order=await this._order.findOrderById(id);
    try
    {
    foreach(var detail_id in ids)
    {   Console.WriteLine("Detail Id here is:"+detail_id);
        var orderDetail = await this._order.deleteProductOrderDetail(detail_id);
    }
        ViewBag.Delete_Multiple_Status=1;
    ViewBag.Delete_Multiple_Message="Xóa các sản phẩm đã chọn trong đơn hàng thành công";
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} Delete Multiple Order Detail");
    }
 
    catch(Exception er)
    {
        this._logger.LogTrace("Delete Order Detail Exception:"+er.Message);
        this._logger.LogInformation($"{this.HttpContext.Session.GetString("Username")} delete Order Detail Failed Exception:{er.Message}");
    }

    return View("~/Views/OrderList/OrderDetail.cshtml",order);
  }


  [Route("order/{id}")]
  [HttpGet]
  public async Task<IActionResult> OrderDetail(int id)
  {
    try
    {

    var order=await this._order.filterOrderDetail(id);

    var user_id=order.Userid;

    var order_count=this._order.countOrder(user_id);

    ViewBag.OrderCount=order_count;
        
    return View(order);

    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Order Detail Exception:"+er.Message);
    }
    return View(); 
  }
}
