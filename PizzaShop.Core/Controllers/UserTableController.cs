using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionUserTable))]
public class UserTableController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserTableService _userTableService;

    public UserTableController(IUserService userService, IUserTableService userTableService)
    {
        _userTableService = userTableService;
        _userService = userService;
    }

    public async Task FetchData()
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
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Request.Cookies["auth_token"] != null)
        {

            string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
            await FetchData();
            if (email == null)
            {
                return RedirectToAction("index", "Home");
            }
            return View();
        }
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 5, string searchTerm = "")
    {
        string? role = HttpContext.Items["UserRole"] as string;
        if (role == null)
        {
            return RedirectToAction("Index", "Home");
        }
        // List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        // if (rolefilter != null)
        // {
        //     foreach (var i in rolefilter)
        //     {
        //         if (i.PermissionId == 1 && i.Canview == false)
        //         {
        //             return RedirectToAction("Privacy", "Home");
        //         }
        //     }
        // }

        //role and permission vise edit and delelete permission
        // 1 for users 
        // 2 for roles and permission etc...
        RolePermissionModelView? roleFilter = await _userService.PermissionFilter(role, 1);
        List<UserTableViewModel>? result = await _userTableService.GetUsersDetails();
        if (result == null) return View();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();

                result = result.Where(u =>
                    u.Firstname.ToLower().Contains(searchTerm) ||
                    u.Lastname.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.Phone.ToString().Contains(searchTerm) ||
                    (searchTerm.ToLower() == "admin" && u.Role == 3) ||
                    (searchTerm.ToLower() == "chef" && u.Role == 2) ||
                    (searchTerm.ToLower() == "accountmanager" && u.Role == 1) ||
                    (searchTerm.ToLower() == "active" && u.Status == 1) ||
                    (searchTerm.ToLower() == "inactive" && u.Status == 2) ||
                    (searchTerm.ToLower() == "pending" && u.Status == 3)
                ).ToList();
            }

        }

        int totalUsers = result.Count();
        var users = result
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Json(new { data = users, totalUsers, roleFilter?.Canedit, roleFilter?.Candelete });

    }

    public async Task<IActionResult> DeleteUserById(int id)
    {
        await _userTableService.DeleteUser(id);
        TempData["SUCCESSDELETE"] = "User deleted successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> EditUserById(int id)
    {
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        // List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        // if (rolefilter != null)
        // {
        //     foreach (var i in rolefilter)
        //     {
        //         if (i.PermissionId == 1 && i.Canview == false)
        //         {
        //             return RedirectToAction("Privacy", "Home");
        //         }
        //     }
        // }
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        await FetchData();
        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }
        UserViewModel user = await _userTableService.EditUserById(id);
        return View(user);

    }

    [HttpPost]
    public async Task<IActionResult> EditUserById(UserViewModel model, [FromForm] IFormFile imageFile)
    {
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        // List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        // if (rolefilter != null)
        // {
        //     foreach (var i in rolefilter)
        //     {
        //         if (i.PermissionId == 1 && i.Canview == false)
        //         {
        //             return RedirectToAction("Privacy", "Home");
        //         }
        //     }
        // }
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        await FetchData();
        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }
        if (model.userId != 0)
        {
            await _userTableService.EditUserPostAsync(model, imageFile);
            return RedirectToAction("Index");
        }

        return View(model);

    }

    [HttpGet]
    public async Task<IActionResult> GetStates(int countryId)
    {
        List<State>? states = await _userService.GetStates(countryId);
        return Json(states);
    }

    [HttpGet]
    public async Task<IActionResult> GetCities(int stateId)
    {
        List<City>? city = await _userService.GetCities(stateId);
        return Json(city);
    }

    public async Task<IActionResult> AddUser()
    {
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        // List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        // if (rolefilter != null)
        // {
        //     foreach (var i in rolefilter)
        //     {
        //         if (i.PermissionId == 1 && i.Canview == false)
        //         {
        //             return RedirectToAction("Privacy", "Home");
        //         }
        //     }
        // }
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        await FetchData();

        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }

        List<Country1>? countries = await _userService.GetCountries();
        ViewBag.Country = countries;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddNewUserViewModel obj, [FromForm] IFormFile imageFile)
    {
        string role = HttpContext.Items["UserRole"] as string ?? string.Empty;
        // List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        // if (rolefilter != null)
        // {
        //     foreach (var i in rolefilter)
        //     {
        //         if (i.PermissionId == 1 && i.Canview == false)
        //         {
        //             return RedirectToAction("Privacy", "Home");
        //         }
        //     }
        // }
        string email = HttpContext.Items["UserEmail"] as string ?? string.Empty;
        await FetchData();
        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }


        if (ModelState.IsValid)
        {
            string? error = await _userTableService.AddUserService(obj, imageFile);
            if (error != "")
            {
                TempData["error"] = error;
                List<Country1>? c = await _userService.GetCountries();
                ViewBag.Country = c;
                return View(obj);
            }

            return RedirectToAction("Index");
        }
        List<Country1>? countries = await _userService.GetCountries();
        ViewBag.Country = countries;
        return View(obj);
    }


}
