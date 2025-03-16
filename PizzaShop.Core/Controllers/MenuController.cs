using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionMenu))]
public class MenuController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;
    private readonly IRoleService _roleService;
    private readonly IMenuService _menuService;

    public MenuController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService,
        IRoleService roleService,
        IMenuService menuService)
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
        _roleService = roleService;
        _menuService = menuService;
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

    public async Task<IActionResult> Index(int? categoryId = null, string? searchTerm = null, int? modifierGroupId = null, string? searchModifier = null, int pageNumber = 1, int pageSize = 5)
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetAllCategory(categoryId, searchTerm, pageNumber, pageSize);
        MenuWithItemsViewModel menu2 = await _menuService.GetModifiers(modifierGroupId, searchTerm, pageNumber, pageSize);
        ViewBag.SelectedCategoryId = categoryId;
        MenuWithItemsViewModel result = new MenuWithItemsViewModel
        {
            Categories = menu.Categories,
            Items = menu.Items,
            CurrentPage = menu.CurrentPage,
            PageSize = menu.PageSize,
            TotalItems = menu.TotalItems,
            modifiergroups = menu2.modifiergroups,
            Modifiers = menu2.Modifiers,
            CurrentPage1 = menu2.CurrentPage1,
            PageSize1 = menu2.PageSize1,
            TotalItems1 = menu2.TotalItems1
        };
        ViewBag.SelectedModifierId = modifierGroupId;

        return View(result);
    }

    public async Task<IActionResult> FilterItems(int? categoryId = null, string? searchTerm = null, int pageNumber = 1, int pageSize = 5)
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetAllCategory(categoryId, searchTerm, pageNumber, pageSize);
        return PartialView("_ItemsPartial", menu);
    }

    public async Task<IActionResult> FilterModifiers(int? modifierGroupId = null, string? searchTerm = null, int pageNumber = 1, int pageSize = 5)
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetModifiers(modifierGroupId, searchTerm, pageNumber, pageSize);
        return PartialView("_ModifiersPartial", menu);
    }

    public async Task<IActionResult> FilterModifiersAtAddCategory(int? modifierGroupId = null, string? searchTerm = null, int pageNumber = 1, int pageSize = 5)
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetModifiers(modifierGroupId, searchTerm, pageNumber, pageSize);
        return PartialView("_ModifiersAtAddCategoryPartail", menu);
    }


    [HttpPost]
    public async Task<IActionResult> AddCategory(MenuWithItemsViewModel model)
    {
        await _menuService.AddCategoryService(model);
        await FetchData();
        TempData["CategoryAdd"] = "Category added successfully";
        return RedirectToAction("Index");
    }

    // [HttpPost]
    // public async Task<IActionResult> AddModifierGroup(MenuWithItemsViewModel model)
    // {
    //     await _menuService.AddModifierGroupService(model);
    //     await FetchData();
    //     TempData["ModifierGroupAdd"] = "ModifierGroup added successfully";
    //     return RedirectToAction("Index");
    // }



    [HttpPost]
    public async Task<IActionResult> AddModifierGroup(MenuWithItemsViewModel model)
    {
        // Log the received selectedIds for debugging
        Console.WriteLine($"Received selectedIds: {model.selectedIds}");

        List<int> modifierIds = string.IsNullOrEmpty(model.selectedIds)
            ? new List<int>()
            : model.selectedIds.Split(',').Select(int.Parse).ToList();

        model.SelectedModifierIds = modifierIds; // Assuming this property exists in your model
        await _menuService.AddModifierGroupService(model);
        await FetchData();
        TempData["ModifierGroupAdd"] = "ModifierGroup added successfully";
        return RedirectToAction("Index");
    }





    [HttpPost]
    public async Task<IActionResult> EditCategory(MenuWithItemsViewModel model)
    {
        await _menuService.EditCategoryService(model);
        await FetchData();
        TempData["CategoryAdd"] = "Category Edited successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(MenuWithItemsViewModel model)
    {
        await _menuService.DeleteCategoryService(model);
        await FetchData();
        TempData["CategoryAdd"] = "Modifier Group Deleted successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteModifierGroup(MenuWithItemsViewModel model)
    {
        await _menuService.DeleteModifierGroupService(model);
        await FetchData();
        TempData["ModifierGroupAdd"] = "Category Deleted successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(MenuWithItemsViewModel viewModel)
    {
        if (viewModel.item == null)
        {
            // Handle null item case early
            MenuWithItemsViewModel menu2 = await _menuService.GetAllCategory(0, "", 1, 5);
            menu2.item = viewModel.item;
            await FetchData();
            ModelState.AddModelError("", "Item details are required.");
            return View("Index", menu2);
        }
        if (viewModel.item != null)
        {
            try
            {
                await FetchData();
                await _menuService.AddItemAsync(viewModel, viewModel.item?.UploadFiles, ViewBag.Userid);

                TempData["SuccessMessage"] = "Item added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("error :" + ex.Message);
                MenuWithItemsViewModel menu1 = await _menuService.GetAllCategory(0, "", 1, 5);
                menu1.item = viewModel.item;
                return RedirectToAction("Index", menu1);
            }
        }
        MenuWithItemsViewModel menu = await _menuService.GetAllCategory(0, "", 1, 5);
        menu.item = viewModel.item;
        return RedirectToAction("Index", menu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddModifier(MenuWithItemsViewModel viewModel)
    {
        if (viewModel.modifiersViewModel == null || viewModel.modifiersViewModel.Modifiergroupid == null || !viewModel.modifiersViewModel.Modifiergroupid.Any())
        {
            MenuWithItemsViewModel menu2 = await _menuService.GetModifiers(0, "", 1, 5);
            menu2.modifiersViewModel = viewModel.modifiersViewModel;
            ModelState.AddModelError("", "Modifier details and at least one modifier group are required.");
            return View("Index", menu2);
        }

        try
        {
            await FetchData(); // Assuming this sets ViewBag.Userid
            viewModel.Userid = ViewBag.Userid;
            await _menuService.AddModifierAsync(viewModel);
            TempData["ModifierGroupAdd"] = "Modifier added successfully";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding modifier: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while adding the modifier.");
            return View("Index", viewModel);
        }
    }


    [HttpPost("DeleteItem")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(int userid, int itemid)
    {
        await _menuService.DeleteItemService(userid, itemid);
        TempData["CategoryAdd"] = "Item Deleted successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteModifier(int modifierid, int Userid)
    {
        // Assuming this is handled elsewhere as per your original setup
        // Left unchanged as requested
        try
        {
            await _menuService.DeleteModifierService(modifierid, Userid);
            return RedirectToAction("Index"); // Or adjust as per your original flow
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting modifier: {ex.Message}");
            return RedirectToAction("Index"); // Or adjust as per your original flow
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultipleItems(string selectedItemIds)
    {
        if (string.IsNullOrEmpty(selectedItemIds))
        {
            TempData["ErrorMessage"] = "No items selected for deletion.";
            return RedirectToAction("Index");
        }

        try
        {
            // Parse the comma-separated string into an array of integers
            List<int>? itemsIds = selectedItemIds.Split(',')
                .Select(id => int.Parse(id))
                .ToList();

            if (!itemsIds.Any())
            {
                TempData["ErrorMessage"] = "Invalid item IDs provided.";
                return RedirectToAction("Index");
            }

            await FetchData();
            int userId = ViewBag.Userid;
            await _menuService.DeleteMultipleItemsAsync(itemsIds, userId);
            TempData["SuccessMessage"] = "Selected items deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to delete items: {ex.Message}";
        }

        return RedirectToAction("Index");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultipleModifiers(string selectedModifierIds)
    {
        if (string.IsNullOrEmpty(selectedModifierIds))
        {
            TempData["ErrorMessage"] = "No modifiers selected for deletion.";
            return RedirectToAction("Index");
        }

        try
        {
            List<int>? modifierIds = selectedModifierIds.Split(',')
                .Select(id => int.Parse(id))
                .ToList();

            if (!modifierIds.Any())
            {
                TempData["ErrorMessage"] = "Invalid modifier IDs provided.";
                return RedirectToAction("Index");
            }

            await FetchData();
            int userId = ViewBag.Userid;
            await _menuService.DeleteMultipleModifiersAsync(modifierIds, userId);
            TempData["SuccessMessage"] = "Selected modifiers deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to delete modifiers: {ex.Message}";
        }

        return RedirectToAction("Index");
    }



    [HttpGet]
    public async Task<IActionResult> EditItemPartial(int id)
    {
        var item = await _menuService.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        var viewModel = new MenuWithItemsViewModel
        {
            item = new ItemsViewModel
            {
                Itemid = item.Itemid,
                Categoryid = item.Categoryid,
                Itemname = item.Itemname,
                Itemtype = item.Itemtype,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Units = item.Units,
                Isavailabe = (bool)item.Isavailabe,
                Defaulttax = (bool)item.DefaultTax,
                Taxpercentage = item.Taxpercentage,
                Shortcode = item.Shortcode,
                Description = item.Description
            },
            Categories = await _menuService.GetAllCategories()
        };
        await FetchData();
        return PartialView("_EditItemPartial", viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> GetModifierData(int modifierId)
    {
        try
        {
            var modifier = await _menuService.GetModifierByIdAsync(modifierId);
            if (modifier == null)
            {
                return NotFound();
            }

            var currentGroupIds = await _menuService.GetModifierGroupIdsAsync(modifierId);
            var data = new
            {
                modifierid = modifier.Modifierid,
                modifiername = modifier.Modifiername,
                modifierrate = modifier.Modifierrate,
                modifierquantity = modifier.Modifierquantity,
                modifierunit = modifier.Modifierunit,
                modifierdescription = modifier.Modifierdescription,
                modifiergroupid = currentGroupIds
            };

            return Json(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching modifier data: {ex.Message}");
            return StatusCode(500, "Error fetching modifier data.");
        }
    }

    // POST: Edit Modifier
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditModifier(MenuWithItemsViewModel viewModel)
    {
        try
        {
            if (viewModel.modifiersViewModel == null || string.IsNullOrEmpty(viewModel.modifiersViewModel.Modifiername) ||
                !viewModel.modifiersViewModel.Modifiergroupid.Any())
            {
                return Json(new { success = false, message = "Name and at least one modifier group are required." });
            }

            await FetchData(); // Assuming this sets ViewBag.Userid
            viewModel.Userid = ViewBag.Userid;
            await _menuService.EditModifierAsync(viewModel);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating modifier: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(MenuWithItemsViewModel viewModel)
    {
        if (viewModel.item == null)
        {
            return Json(new { success = false, message = "Item details are required." });
        }

        try
        {
            await FetchData();
            await _menuService.UpdateItemAsync(viewModel, viewModel.item.UploadFiles, viewModel.Userid);

            return Json(new { success = true, redirectUrl = Url.Action("Index") });
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Error: " + ex.Message);
            return Json(new { success = false, message = "Error updating item." });
        }
    }



    [HttpPost]
    public async Task<IActionResult> IsAvailableUpdate(int itemId, bool available)
    {
        try
        {
            await FetchData();
            int userId = ViewBag.Userid; // Consider dependency injection instead
            Item? item = await _menuService.IsAvailabeUpdateAsync(itemId, available, userId);

            if (item?.Isavailabe == available) // Added null check
            {
                return Json(new
                {
                    success = true,
                    data = "Update completed successfully"
                });
            }

            return Json(new
            {
                success = false,
                data = "Update failed to apply"
            });
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Error in IsAvailableUpdate: " + e.Message);
            return Json(new
            {
                success = false,
                data = "An error occurred: " + e.Message
            });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddModifierGroupDetails(string selectedIds)
    {
        try
        {
            List<int>? modifierIds = selectedIds.Split(',')
                .Select(id => int.Parse(id))
                .ToList();
            List<Modifier>? modifiers = await _menuService.GetModifiersListAsync(modifierIds); // Fetch modifiers from service

            return Json(new
            {
                success = true,
                modifiers = modifiers
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = "Error adding modifiers: " + ex.Message
            });
        }
    }

}