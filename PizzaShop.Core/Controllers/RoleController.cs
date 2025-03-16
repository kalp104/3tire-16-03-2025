using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionRoles))]
public class RoleController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;
    private readonly IRoleService _roleService;
    public RoleController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService,
        IRoleService roleService)
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
        _roleService = roleService;
    }

    private async Task FetchData()
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
    }
    // private async Task<IActionResult?> CheckPermissionAsync()
    // {
    //     string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
    //     // List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);

    //     // if (rolefilter != null)
    //     // {
    //     //     foreach (var i in rolefilter)
    //     //     {
    //     //         if (i.PermissionId == 2 && i.Canview == false)
    //     //         {
    //     //             return RedirectToAction("Privacy", "Home");
    //     //         }
    //     //     }
    //     // }
    //     return null; // No redirection needed
    // }
    public async Task<IActionResult> Role()
    {
        await FetchData();
        // IActionResult? permissionResult = await CheckPermissionAsync();
        // if (permissionResult != null)
        // {
        //     return permissionResult;
        // }

        List<Role>? result = await _roleService.GetRoles();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Permission(int id)
    {
        await FetchData();
        // IActionResult? permissionResult = await CheckPermissionAsync();
        // if (permissionResult != null)
        // {
        //     return permissionResult;
        // }
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        List<RolePermissionModelView>? result = await _roleService.RoleBasePermission(id);
        int roleid = 0;
        if (role == "AccountManager") roleid = 1 ;
        if (role == "Chef") roleid = 2;
        if (role == "Admin") roleid = 3;
        List<RolePermissionModelView>? permissions = await _roleService.RoleBasePermission(roleid);
        bool Save = true;
        foreach (var i in permissions)
        {
            if (i.PermissionId == 2 && i.Canedit == false)
            {
                Save = false;
                break;
            }
        }
        if (id == 1) ViewBag.Rolename = "AccountManager";
        if (id == 2) ViewBag.Rolename = "Chef";
        if (id == 3) ViewBag.Rolename = "Admin";
        ViewBag.Save = Save;
        return View(result ?? new List<RolePermissionModelView>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePermission(List<RolePermissionModelView> model)
    {
        await FetchData();
        // IActionResult? permissionResult = await CheckPermissionAsync();
        // if (permissionResult != null)
        // {
        //     return permissionResult;
        // }

        if (ModelState.IsValid)
        {
            await _roleService.UpdatePermissions(model);
            TempData["SuccessPermission"] = "Permissions updated successfully";
            return RedirectToAction("Permission", new { id = model.FirstOrDefault()?.RoleId });
        }
        return View("Permission", model);
    }
}
