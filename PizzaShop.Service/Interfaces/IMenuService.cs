using System;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    Task<MenuWithItemsViewModel> GetAllCategory(int? categoryId = null, string? searchTerm = null, int pageNumber = 1, int pageSize = 5);
    Task<List<Category>> GetAllCategories();
    Task<List<Modifiergroup>> GetAllModifierGroup();
    Task<MenuWithItemsViewModel> GetModifiers(int? modifierGroupId = null, string? searchModifier = null, int pageNumber = 1, int pageSize = 5);
    Task AddCategoryService(MenuWithItemsViewModel model);
    Task AddModifierGroupService(MenuWithItemsViewModel model);
    Task EditCategoryService(MenuWithItemsViewModel model);
    Task DeleteCategoryService(MenuWithItemsViewModel model);
    Task DeleteModifierGroupService(MenuWithItemsViewModel model);
    Task DeleteItemService(int userid, int itemid);
    Task AddItemAsync(MenuWithItemsViewModel viewModel, IFormFile? uploadFile, int userId);
    Task AddModifierAsync(MenuWithItemsViewModel viewModel);
    Task UpdateItemAsync(MenuWithItemsViewModel viewModel, IFormFile? uploadFile, int userId);
    Task<Item?> GetItemById(int id);
    Task<Modifier?> GetModifierById(int id);
    Task<List<Modifier>> GetModifiersListAsync(List<int> id);
    Task<Item> IsAvailabeUpdateAsync(int id,bool available, int userid);
    Task DeleteMultipleItemsAsync(List<int>? itemIds, int userId);
    Task DeleteMultipleModifiersAsync(List<int>? modifierIds, int userId);
    Task DeleteModifierService(int userid, int modifierid);


    Task<Modifier?> GetModifierByIdAsync(int modifierId);
    Task<List<Modifiergroup>> GetAllModifierGroupsAsync();
    Task<List<int>> GetModifierGroupIdsAsync(int modifierId);
    Task EditModifierAsync(MenuWithItemsViewModel viewModel);
}
