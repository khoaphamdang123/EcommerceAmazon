using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using Org.BouncyCastle.Math.EC.Rfc8032;
using System.ComponentModel;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Ecommerce_Product.Controllers;
[Authorize(Roles ="Admin")]

[Route("admin")]
public class PaymentListController : BaseAdminController
{
  private readonly ILogger<PaymentListController> _logger;

   private readonly IPaymentRepository _payment;

   public PaymentListController(IPaymentRepository payment,IBannerListRepository banner,ILogger<PaymentListController> logger):base(banner)
   {
  this._payment=payment;

  this._logger=logger;   
   }
  //[Authorize(Roles ="Admin")]
  [Route("payment")]
  [HttpGet]
  public async Task<IActionResult> PaymentList()
  {       
          string select_size="7";
          ViewBag.select_size=select_size;
          List<string> options=new List<string>(){"7","10","20","50"};
          ViewBag.options=options;
    try
    {  
        var payments=await this._payment.pagingPayment(7,1);

        return View(payments);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Static File List Exception:"+er.Message);
    }
    
    return View();

  }


//[Authorize(Roles ="Admin")]
  [Route("payment/paging")]
   [HttpGet]
  public async Task<IActionResult> PaymentPaging([FromQuery]int page_size,[FromQuery] int page=1)
  {
    try{
         var payments=await this._payment.pagingPayment(page_size,page);
      
          List<string> options=new List<string>(){"7","10","20","50"};
          
          ViewBag.options=options;
                  
          string select_size=page_size.ToString();
          
          ViewBag.select_size=select_size;
          
          return View("~/Views/PaymentList/PaymentList.cshtml",payments);
        }
     
        catch(Exception er)
        {
            this._logger.LogTrace("Paging Static File List Exception:"+er.Message);
        }
    return View();
  }

  [Route("payment/add_payment")]
  
  [HttpGet]
  public IActionResult AddPayment()
  {
    return View();
  }

  [Route("payment/add_payment")]
  [HttpPost]
  public async Task<IActionResult> AddPayment(Payment payment)
  {
    try
    {
      int created_res=await this._payment.addPaymentMethod(payment);
      
      if(created_res==0)
      {
        ViewBag.Status=0;
        ViewBag.Created_Payment="Thêm phương thức thanh toán thất bại";
      }
      else if(created_res==-1)
      {
        ViewBag.Status=-1;
        ViewBag.Created_Payment="Phương thức thanh toán này đã tồn tại trong hệ thống";
      }
      else
      {
        ViewBag.Status=1;
        ViewBag.Created_Payment="Thêm phương thức thanh toán thành công";        
      }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Add Payment Exception:"+er.Message);
    }
    return View();
  }

//   [Route("file_list/add_page")]
//   [HttpPost]
//   public async Task<IActionResult> AddStaticFiles(StaticFile file)
//   {  try{
//       string file_name=file.Filename;
//       string content= file.Content;
//       int created_res=await this._static_files.addPage(file);
//       if(created_res==0)
//       {
//         ViewBag.Status=0;
//         ViewBag.Created_Page="Thêm trang thất bại";
//       }
//       else if(created_res==-1)
//       {
//         ViewBag.Status=-1;
//         ViewBag.Created_Page="Trang này đã tồn tại trong hệ thống";
//       }
//       else{
//         ViewBag.Status=1;
//         ViewBag.Created_Page="Thêm trang thành công";
//       }
//   }
//   catch(Exception er)
//   {
//     this._logger.LogTrace("Add Page Exception:"+er.Message);
//   }
//       return View();
//   }

  [Route("payment/delete")]
  [HttpGet]
  public async Task<IActionResult> DeletePayment(int id)
  {
    try
  {
   int remove_res=await this._payment.deletePaymentMethod(id);
   if(remove_res==0)
   {
   TempData["Status_Delete"]=0;
   TempData["Message_Delete"]="Xóa phương thức thanh toán thất bại";
   }
   else
   {
   TempData["Status_Delete"]=1;
   TempData["Message_Delete"]="Xóa phương thức thanh toán thành công";
   }
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Remove Payment Method Exception:"+er.Message);
    }
    return RedirectToAction("PaymentList","PaymentList");
  }

  [Route("payment/update/{id}")]
  [HttpGet]
  public async Task<IActionResult> UpdatePayment(int id)
  {
    int payment=await this._payment.updatePaymentMethod(id);

    if(payment!=1)
    {
        TempData["Status_Update"]=0;
        
        TempData["Message_Update"]="Cập nhật trạng thái phương thức thanh toán thất bại";
    }
    else
    {
       TempData["Status_Update"]=1;
        
       TempData["Message_Update"]="Cập nhật trạng thái phương thức thanh toán thành công";
    }
    return RedirectToAction("PaymentList","PaymentList");
  }

//   [Route("file_list/{id}/page_info")]
//   [HttpGet]
//   public async Task<IActionResult> StaticFilesInfo(int id)
//   {
//     var page=await this._static_files.findStaticFileById(id);
//     return View(page);
//   }

//     [Route("file_list/{id}/page_info")]
//   [HttpPost]
//   public async Task<IActionResult> StaticFilesInfo(int id,StaticFile file)
//   {  
//     Console.WriteLine("id:"+id);

//     int updated_res=await this._static_files.updatePage(id,file);
//     if(updated_res==0)
//     {
//         ViewBag.Status=0;
//         ViewBag.Updated_Message="Cập nhật trang thất bại";
//     }
//     else
//     {
//   ViewBag.Status=1;
//   ViewBag.Updated_Message="Cập nhật trang thành công";
//     }
//     Console.WriteLine(updated_res);
//   var page=await this._static_files.findStaticFileById(id);
//     return View(page);
//   }


}
