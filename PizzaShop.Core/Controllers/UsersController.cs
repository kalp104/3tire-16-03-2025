using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;
    public UsersController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
    }

    public async Task FetchData(){
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
    }
    public IActionResult RoleWiseBack()
    {
        var role = HttpContext.Items["UserRole"] as string;
        if (role == "Admin")
        {
            return RedirectToAction("Admin", "Users");
        }
        if (role == "AccountManager")
        {
            return RedirectToAction("AccountManager", "Users");
        }
        if (role == "Chef")
        {
            return RedirectToAction("Chef", "Users");
        }
        return RedirectToAction("index", "Home");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin()
    {
        await FetchData();
        return View();
    }

    [Authorize(Roles = "AccountManager")]
    public async Task<IActionResult> AccountManager()
    {
        await FetchData();
        return View();
    }

    [Authorize(Roles = "Chef")]
    public async Task<IActionResult> Chef()
    {
        await FetchData();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UserProfile()
    {
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
        UserViewModel? user = userBag != null ? await _userTableService.EditUserById(userBag.UserId) : null;

        return View(user);

    }

    [HttpPost]
    public async Task<IActionResult> UserProfile(UserViewModel model)
    {
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }



        string? result = await _userService.UpdateUserProfile(model);
        if (result == "")
        {
            TempData["updated"] = "user updated successfully";
            UserViewModel? user = userBag != null ? await _userTableService.EditUserById(userBag.UserId) : null;
            return View(user);
        }

        TempData["ERROR"] = "Profile Update Failed";
        return RedirectToAction("UserProfile", model);
    }

    public async Task<IActionResult> UpdateImage(int userId, IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            TempData["ERROR"] = "Please select the a image.";
            return RedirectToAction("UserProfile");
        }
        string? result = await _userService.UpdateImage(userId, imageFile);

        if (result == "") return RedirectToAction("UserProfile");
        ViewBag.ImageUrl = result;
        TempData["updated"] = "image updated successfully.";
        return RedirectToAction("UserProfile");

    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        await FetchData();
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model)
    {
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }

        if (!ModelState.IsValid)
        {
            return View(model); // Return view if input validation fails
        }
        string? result = await _userService.ChangePassword(model, email);
        switch (result)
        {
            case "0":
                TempData["changepassword"] = "Password updated successfully";
                return View();
            case "1":
                ModelState.AddModelError("", "Account not found.");
                return View(model);
            case "2":
                ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                return View(model);
            case "3":
                ModelState.AddModelError("Password", "New password cannot be the same as the old one.");
                return View(model);
            case "4":
                ModelState.AddModelError("ConfirmPassword", "Password does not match.");
                return View(model);

        }
        TempData["changePasswordError"] = "Somthing went wrong";
        return View();

    }


}
