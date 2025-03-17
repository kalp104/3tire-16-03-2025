using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Models;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;


public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILoginService _loginService;

    public HomeController(ILoginService loginService, IEmailService emailService, IConfiguration configuration)
    {
        _emailService = emailService;
        _configuration = configuration;
        _loginService = loginService;
    }


    #region Login
    public IActionResult Index()
    {
        if (HttpContext.Request.Cookies["auth_token"] != null && HttpContext.Items["UserRole"] != null)
        {
            var role = HttpContext.Items["UserRole"] as string;
            if (role == "Admin")
            {
                TempData["success"] = "logged in";
                return RedirectToAction("Admin", "Users");
            }
            if (role == "AccountManager")
            {
                TempData["success"] = "logged in";
                return RedirectToAction("AccountManager", "Users");
            }
            if (role == "Chef")
            {
                TempData["success"] = "logged in";
                return RedirectToAction("Chef", "Users");
            }
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            ResponseTokenViewModel? response = await _loginService.GetLoginService(model);
            if (response != null && response.token.Length > 0)
            {
                SetJwtCookie(response.token, model.Rememberme);
                return RedirectToAction("RoleWiseBack", "Users");
            }
            TempData["EmailWrong"] = response.response;
        }
        return View(model);
    }

    private void SetJwtCookie(string token, bool isPersistent)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = isPersistent ? DateTime.Now.AddDays(30) : null // Session cookie if not persistent
        };
        Response.Cookies.Append("auth_token", token, cookieOptions);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        TempData["logout"] = "Logout Successful!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult UpdateEmail(string email)
    {
        TempData["Email"] = email;
        TempData.Keep("Email");
        return Ok();
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {
        TempData.Keep("Email");
        ViewBag.Email = TempData["Email"];
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(EmailViewModel Email)
    {
        if (Email == null || string.IsNullOrEmpty(Email.ToEmail))
        {
            TempData["ErrorMessage"] = "Email address is required.";
        }
        else
        {
            Account? forgetpassword = await _loginService.GetAccoutAsync(Email.ToEmail);
            if (forgetpassword != null)
            {
                try
                {
                    string? passwordResetLink = Url.Action("ResetPassword", "Home", new { Email = Email.ToEmail }, protocol: HttpContext.Request.Scheme);
                    string resetLink = HtmlEncoder.Default.Encode(passwordResetLink);
                    string emailBody = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <meta charset='UTF-8'>
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                    <title>Email Template</title>
                                </head>
                                <body style='font-family: Arial, sans-serif; background-color: #f4f4f9; padding: 20px;'>
                                    <header>
                                    <h1 style='color:#fff;height:5rem;background-color:#0565a1;width:100%;display:flex;align-items:center;justify-content:center;'>PIZZASHOP</h1>
                                </header>
                                <p>Pizza shop,</p><p>Please click <a href='{resetLink}'>here</a> for reset your password.</p>
                                <p>If you encounter any issue or have any question. please do not hesitate to contact our support team.</p>
                                <p><span style='color:#8B8000'>Important note:</span> For security purpose, the link will expire in 24 hours. If you did not request a reset password, please ignore this email or contact our support team immediately.</p>
                                </body>
                                </html>";
                    await _emailService.SendEmailAsync(
                            Email.ToEmail,
                            "Password Reset Request",
                            emailBody

                    );
                    TempData["validEmail"] = Email.ToEmail;
                    TempData["SuccessMessage"] = "Password reset instructions have been sent to your email.";
                    TempData.Keep("validEmail");
                    return View(Email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    TempData["ErrorMessage"] = "Failed to send email. Please try again later.";
                    return View(Email);
                }
            }
        }
        return View(Email);
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string Email)
    {
        if (Email == null)
        {
            ViewBag.ErrorTitle = "Invalid Password Reset Token";
            ViewBag.ErrorMessage = "The Link is Expired or Invalid";
            return View("Error");
        }
        else
        {
            ForgetPasswordViewModel model = new();
            model.Email = Email;
            return View(model);
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            string? response = await _loginService.ResetPasswordService(model);

            switch (response)
            {
                case "1":
                    TempData["password"] = "account does not exist";
                    break;
                case "2":
                    TempData["EmailNotMatch"] = "email doesnot match";
                    break;
                case "3":
                    TempData["password"] = "password can not be same as previous one";
                    break;
                case "4":
                    TempData["success"] = "successfully changed password";
                    return RedirectToAction("Index");
                default:
                    TempData["password"] = "invalid";
                    break;
            }



        }
        TempData["error"] = "please confirm password first";
        return View(model);
    }

    #endregion
    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Error404()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}