using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using Ecommerce_Product.Support_Serive;
using Ecommerce_Product.Service;
using RazorLight;
using Org.BouncyCastle.Crypto.Engines;

namespace Ecommerce_Product.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class NewsLetterController : BaseAdminController
{
    private readonly ILogger<StaticFilesController> _logger;

    private readonly ISettingRepository _setting;

    private readonly IUserListRepository _user;
    private readonly SmtpService _smtpService;
    public NewsLetterController(IStaticFilesRepository static_files, ISettingRepository setting, IBannerListRepository banner, IUserListRepository user, ILogger<StaticFilesController> logger, SmtpService smtpService) : base(banner)
    {

        this._setting = setting;

        this._user = user;

        this._smtpService = smtpService;

        this._logger = logger;
    }
    [Route("news_letter")]
    [HttpGet]
    public async Task<IActionResult> NewsLetter()
    {
        try
        {
            var newsletter_setting = await this._setting.getSettingObjByName("newsletter");

            return View("~/Views/NewsLetterList/NewsLetter.cshtml", newsletter_setting);
        }
        catch (Exception er)
        {
            this._logger.LogTrace("Get Static File List Exception:" + er.Message);
        }
        return View("~/Views/NewsLetterList/NewsLetter.cshtml");
    }



    [Route("newsletter/update")]
    [HttpPost]
    public async Task<IActionResult> UpdateNewsLetter(string content)
    {
        try
        {

            int created_res = await this._setting.updateNewsLetterSetting(content);

            if (created_res == 0)
            {
                ViewBag.Status = 0;
                ViewBag.Created_Page = "Cập nhật nội dung newsletter thất bại";
            }

            else
            {
                ViewBag.Status = 1;

                ViewBag.Created_Page = "Cập nhat nội dung newsletter thành công";
            }

        }
        catch (Exception er)
        {
            this._logger.LogTrace("Add Page Exception:" + er.Message);
        }

        var newsletter_setting = await this._setting.getSettingObjByName("newsletter");

        return View("~/Views/NewsLetterList/NewsLetter.cshtml", newsletter_setting);
    }


    [HttpPost]
    [Route("newsletter/broadcast")]
    public async Task<JsonResult> BroadcastNewsLetter(string content)
    {
        try
        {
            var users = await this._user.getAllUserList();

            int success_count = 0;

            if (users.Count() > 0)
            {
                foreach (var user in users)
                {
                    string email = user?.Email;
                    string name = user?.UserName;
                    if (name == "admin123" || name == "company")
                    {
                        continue;
                    }
                    
                    var render_view = new RazorViewRenderer();

                    string mail_path = "MailTemplate/newsletter.cshtml";

                    UserInfo receipt = new UserInfo { UserName = email, Email = email, ContentNewsLetter = content };


                    string render_string = await render_view.RenderViewToStringAsync(mail_path, receipt);

                    Console.WriteLine("Render string here is:" + render_string);

                    bool is_sent = await this._smtpService.sendEmailGeneral(4, render_string, email);

                    if (is_sent)
                    {
                        success_count++;
                        this._logger.LogInformation("Send confirm newsletter successfully to " + email);

                        Console.WriteLine("Send confirm newsletter successfully to " + email);
                    }
                    else
                    {
                        this._logger.LogInformation("Send confirm newsletter failed to " + email);

                        Console.WriteLine("Send confirm newsletter failed to " + email);
                    }
                }

                if (success_count > 0)
                {
                    return Json(new { status = 1, message = "Đã gửi newsletter đến " + success_count + " subscribers" });
                }
                else
                {
                    return Json(new { status = 0, message = "Không có subscriber nào được gửi thành công" });
                }

            }

        }
        catch (Exception er)
        {
            this._logger.LogError("Broadcast NewsLetter Exception:" + er.Message);

            return Json(new { status = 0, message = "Đã xảy ra lỗi khi gửi newsletter đến subscriber" });
        }
        
         return Json(new { status = 0, message = "Gửi newsletter thất bại" });

    }


}
