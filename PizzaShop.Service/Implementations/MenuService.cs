using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class MenuService : IMenuService
{

    private readonly IGenericRepository<Category> _category;
    private readonly IGenericRepository<Item> _items;
    private readonly IGenericRepository<Modifiergroup> _modifierGroup;
    private readonly IGenericRepository<Modifier> _modifier;
    private readonly IGenericRepository<ItemModifiergroupMapping> _itemModifiergroupMapping;
    private readonly IGenericRepository<Modfierandgroupsmapping> _modfierandGroupsMapping;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public MenuService(
        IGenericRepository<Category> category,
        IGenericRepository<Item> items,
        IGenericRepository<Modifiergroup> modifierGroup,
        IGenericRepository<Modifier> modifier,
        IWebHostEnvironment webHostEnvironment,
        IGenericRepository<Modfierandgroupsmapping> modfierandGroupsMapping,
        IGenericRepository<ItemModifiergroupMapping> itemModifiergroupMapping)
    {
        _category = category;
        _items = items;
        _webHostEnvironment = webHostEnvironment;
        _modifierGroup = modifierGroup;
        _modifier = modifier;
        _modfierandGroupsMapping = modfierandGroupsMapping;
        _itemModifiergroupMapping = itemModifiergroupMapping;
    }

    public async Task<MenuWithItemsViewModel> GetAllCategory(int? categoryId = null, string? searchTerm = null, int pageNumber = 1, int pageSize = 5)
    {
        List<Category>? categories = await _category.GetAllCategoryAsync();
        List<Item>? items;

        // Filter by category first
        if (categoryId.HasValue)
        {
            items = await _items.GetItemsByCategoryAsync(categoryId.Value);
        }
        else
        {
            items = await _items.GetAllItemsAsync();
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            items = items.Where(i => i.Itemname.ToLower().Contains(searchTerm)).ToList();
        }

        // Get total count before pagination
        int totalItems = items.Count;

        // Apply pagination
        items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new MenuWithItemsViewModel
        {
            Categories = categories,
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems // Ensure totalItems is returned properly
        };
    }



    public async Task<List<Category>> GetAllCategories()
    {
        List<Category>? categories = await _category.GetAllCategoryAsync();
        return categories;
    }

    public async Task<List<Modifiergroup>> GetAllModifierGroup()
    {
        List<Modifiergroup>? mg = await _modifierGroup.GetAllAsync();
        mg = mg.Where(u => u.Isdeleted == false).ToList();
        return mg;
    }


    public async Task<MenuWithItemsViewModel> GetModifiers(int? modifierGroupId = null, string? searchModifier = null, int pageNumber = 1, int pageSize = 5)
    {
        List<Modifiergroup>? mg = await _modifierGroup.GetAllModifierGroupAsync();
        List<Modifier>? modifiers;

        // Filter items by category if specified
        if (modifierGroupId.HasValue)
        {
            modifiers = await _modifier.GetModifiersByMGAsync(modifierGroupId.Value);
        }
        else
        {
            modifiers = await _modifier.GetAllModifierAsync();
        }
        // Apply search filter if a search term is provided
        if (!string.IsNullOrEmpty(searchModifier))
        {
            searchModifier = searchModifier.ToLower();
            modifiers = modifiers.Where(i => i.Modifiername.ToLower().Contains(searchModifier)).ToList();
        }
        int totalItems = modifiers.Count;
        modifiers = modifiers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new MenuWithItemsViewModel
        {
            modifiergroups = mg,
            Modifiers = modifiers,
            CurrentPage1 = pageNumber,
            PageSize1 = pageSize,
            TotalItems1 = totalItems
        };
    }



    public async Task AddCategoryService(MenuWithItemsViewModel model)
    {
        Category category = new Category
        {
            Categorydescription = model.CategoryDescription,
            Categoryname = model.CategoryName,
            Createdat = DateTime.Now,
            Editedat = DateTime.Now,
            Createdbyid = model.Userid,
            Editedbyid = model.Userid,
            Isdeleted = false,
        };
        await _category.AddAsync(category);

    }
    public async Task AddModifierGroupService(MenuWithItemsViewModel model)
    {
        Modifiergroup modifiergroup = new Modifiergroup
        {
            Modifiergroupdescription = model.Modifiergroupdescription,
            Modifiergroupname = model.Modifiergroupname,
            Createdat = DateTime.Now,
            Editedat = DateTime.Now,
            Createdbyid = model.Userid,
            Editedbyid = model.Userid
        };
        await _modifierGroup.AddAsync(modifiergroup);

        List<int>? list = model.SelectedModifierIds;
        if (list != null)
        {
            foreach (int l in list)
            {
                Modifier? m = await _modifier.GetByIdAsync(l);
                if (m != null)
                {
                    Modfierandgroupsmapping? finding = await _modfierandGroupsMapping.GetMappingsById(modifiergroup.Modifiergroupid, l);
                    if (finding != null)
                    {
                        continue;
                    }
                    else
                    {
                        Modfierandgroupsmapping mapping = new Modfierandgroupsmapping
                        {
                            Modifiergroupid = modifiergroup.Modifiergroupid,
                            Modifierid = l,
                            Createdat = DateTime.Now,
                            Createdbyid = model.Userid
                        };
                        await _modfierandGroupsMapping.AddAsync(mapping);
                    }
                }
            }
        }
    }



    public async Task EditCategoryService(MenuWithItemsViewModel model)
    {
        Category? category = await _category.GetByIdAsync(model.Categoryid);
        if (category != null)
        {
            category.Categorydescription = model.CategoryDescription;
            category.Categoryname = model.CategoryName;
            category.Editedat = DateTime.Now;
            category.Editedbyid = model.Userid;
            await _category.UpdateAsync(category);
        }

    }

    public async Task DeleteCategoryService(MenuWithItemsViewModel model)
    {
        Category? category = await _category.GetByIdAsync(model.Categoryid);
        List<Item> items = await _items.GetAllItemsAsync();
        if (category != null)
        {
            category.Isdeleted = true;
            category.Deletedat = DateTime.Now;
            category.Deletedbyid = model.Userid;
            await _category.UpdateAsync(category);
        }
        foreach (var i in items)
        {
            if (i.Categoryid == category.Categoryid)
            {
                i.Isdeleted = true;
                i.Deletedat = DateTime.Now;
                i.Deletedbyid = model.Userid;
                await _items.UpdateAsync(i);
            }
        }

    }

    public async Task DeleteModifierGroupService(MenuWithItemsViewModel model)
    {
        Modifiergroup? mg = await _modifierGroup.GetByIdAsync(model.Modifiergroupid);
        List<Modfierandgroupsmapping> modifiers = await _modfierandGroupsMapping.GetAllAsync();
        if (mg != null)
        {
            mg.Isdeleted = true;
            mg.Deletedat = DateTime.Now;
            mg.Deletedbyid = model.Userid;
            await _modifierGroup.UpdateAsync(mg);
        }
        foreach (var i in modifiers)
        {
            if (i.Modifiergroupid == mg.Modifiergroupid)
            {
                i.Isdelete = true;
                i.Deletedat = DateTime.Now;
                i.Deletedbyid = model.Userid;
                await _modfierandGroupsMapping.UpdateAsync(i);
            }
        }

    }

    public async Task DeleteItemService(int userid, int itemid)
    {
        Item? item = await _items.GetByIdAsync(itemid);
        if (item != null)
        {
            item.Isdeleted = true;
            item.Deletedat = DateTime.Now;
            item.Deletedbyid = userid;
            await _items.UpdateAsync(item);
        }
    }
    public async Task DeleteModifierService(int userid, int modifierid)
    {
        Modifier? item = await _modifier.GetByIdAsync(modifierid);
        if (item != null)
        {
            item.Isdeleted = true;
            item.Deletedat = DateTime.Now;
            item.Deletedbyid = userid;
            await _modifier.UpdateAsync(item);
        }
    }



    public async Task DeleteMultipleItemsAsync(List<int>? itemIds, int userId)
    {
        if (itemIds == null || !itemIds.Any())
        {
            throw new ArgumentException("No item IDs provided for deletion.");
        }

        foreach (var itemId in itemIds)
        {
            var item = await _items.GetByIdAsync(itemId); // Assume an IItemsRepository
            if (item != null)
            {
                item.Isdeleted = true; // Soft delete
                item.Deletedbyid = userId;
                item.Deletedat = DateTime.Now;
                await _items.UpdateAsync(item);
            }
        }
    }

    public async Task DeleteMultipleModifiersAsync(List<int>? modifierIds, int userId)
    {
        if (modifierIds == null || !modifierIds.Any())
        {
            throw new ArgumentException("No modifier IDs provided for deletion.");
        }

        foreach (var modifierId in modifierIds)
        {
            var modifier = await _modifier.GetByIdAsync(modifierId); // Assume an IModifiersRepository
            if (modifier != null)
            {
                modifier.Isdeleted = true; // Soft delete
                modifier.Editedbyid = userId;
                modifier.Editedat = DateTime.Now;
                await _modifier.UpdateAsync(modifier);
            }
        }
    }

    public async Task AddModifierAsync(MenuWithItemsViewModel viewModel)
    {
        List<Category> c = await _category.GetAllCategoryAsync();
        viewModel.Categories = c;

        try
        {
            Modifier modifier = new Modifier
            {
                Modifiername = viewModel?.modifiersViewModel?.Modifiername,
                Modifierrate = viewModel?.modifiersViewModel?.Modifierrate,
                Modifierquantity = viewModel?.modifiersViewModel?.Modifierquantity,
                Modifierunit = viewModel?.modifiersViewModel?.Modifierunit,
                Modifierdescription = viewModel?.modifiersViewModel?.Modifierdescription,
                Createdat = DateTime.Now,
                Createdbyid = viewModel?.Userid,
                Editedat = DateTime.Now,
                Editedbyid = viewModel?.Userid,
                Isdeleted = false,
                Deletedat = null,
                Deletedbyid = 0,
                Taxdefault = false,
                Taxpercentage = 0

            };

            await _modifier.AddAsync(modifier);

            if (modifier.Modifierid == 0)
            {
                throw new Exception("Modifier ID was not generated.");
            }

            // Create mappings for each selected Modifiergroupid
            foreach (var groupId in viewModel.modifiersViewModel.Modifiergroupid)
            {
                Modfierandgroupsmapping modfierandgroupsmapping = new Modfierandgroupsmapping();
                modfierandgroupsmapping.Modifierid = modifier.Modifierid;
                modfierandgroupsmapping.Modifiergroupid = groupId;
                modfierandgroupsmapping.Createdat = DateTime.Now;
                modfierandgroupsmapping.Createdbyid = viewModel.Userid;
                modfierandgroupsmapping.Isdelete = false;


                await _modfierandGroupsMapping.AddAsync(modfierandgroupsmapping);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding modifier: " + ex.Message, ex);
        }
    }


    public async Task<Modifier?> GetModifierByIdAsync(int modifierId)
    {
        return await _modifier.GetByIdAsync(modifierId);
    }

    public async Task<List<Modifiergroup>> GetAllModifierGroupsAsync()
    {
        return await _modifierGroup.GetAllAsync(); // Adjust based on your repository
    }

    public async Task<List<int>> GetModifierGroupIdsAsync(int modifierId)
    {
        List<Modfierandgroupsmapping>? mappings = await _modfierandGroupsMapping.GetByModifierIdAsync(modifierId); // Adjust based on your repository     
        return mappings.Select(m => m.Modifiergroupid).ToList();
    }


    public async Task<List<ModifierGroupDataHelperViewModel>?> GetModifierGroupsByItemId(int itemId)
    {
        List<ItemModifiergroupMapping>? imm = await _itemModifiergroupMapping.GetByItemIdAsync(itemId);


        List<ModifierGroupDataHelperViewModel>? result = new List<ModifierGroupDataHelperViewModel>();
        foreach (var m in imm)
        {
            ModifierGroupDataHelperViewModel r = new ModifierGroupDataHelperViewModel();
            r.MinValue = m.Minvalue;
            r.MaxValue = m.Maxvalue;
            r.ModifierGroupid = m.Modifiergroupid;

            Modifiergroup? mg = await _modifierGroup.GetByIdAsync(m.Modifiergroupid);
            r.Modifiergroupname = mg.Modifiergroupname;
            List<Modifier>? modifiers = await _modfierandGroupsMapping.GetModifiersByMGAsync(m.Modifiergroupid);
            r.Modifiers = modifiers.Select(m => new ModifierData
            {
                ModifierId = m.Modifierid,
                Modifiername = m.Modifiername,
                Modifierrate = m.Modifierrate
            }).ToList();
            result.Add(r);
        }

        return result;
    }




    public async Task EditModifierAsync(MenuWithItemsViewModel viewModel)
    {
        try
        {
            Modifier? modifier = await _modifier.GetByIdAsync(viewModel?.modifiersViewModel?.Modifierid);
            if (modifier == null)
            {
                throw new Exception("Modifier not found.");
            }

            // Update modifier details
            modifier.Modifiername = viewModel?.modifiersViewModel?.Modifiername;
            modifier.Modifierrate = viewModel?.modifiersViewModel?.Modifierrate;
            modifier.Modifierquantity = viewModel?.modifiersViewModel?.Modifierquantity;
            modifier.Modifierunit = viewModel?.modifiersViewModel?.Modifierunit;
            modifier.Modifierdescription = viewModel?.modifiersViewModel?.Modifierdescription;
            modifier.Editedat = DateTime.Now;
            modifier.Editedbyid = viewModel?.Userid;

            await _modifier.UpdateAsync(modifier);

            // Update modifier group mappings
            List<Modfierandgroupsmapping>? existingMappings = await _modfierandGroupsMapping.GetByModifierIdAsync(modifier.Modifierid);
            List<int>? currentGroupIds = existingMappings != null ? existingMappings.Select(m => m.Modifiergroupid).ToList() : new List<int>();
            List<int>? newGroupIds = viewModel?.modifiersViewModel?.Modifiergroupid;

            // Remove mappings that are no longer selected
            if (existingMappings != null && newGroupIds != null)
            {
                foreach (var mapping in existingMappings)
                {
                    if (!newGroupIds.Contains(mapping.Modifiergroupid))
                    {
                        Modfierandgroupsmapping? entity = await _modfierandGroupsMapping.GetByIdAsync(mapping.Modfierandgroupsmappingid);
                        await _modfierandGroupsMapping.DeleteAsync(mapping);
                    }
                }


                // Add new mappings that weren’t previously selected
                foreach (var groupId in newGroupIds)
                {
                    if (!currentGroupIds.Contains(groupId))
                    {
                        var newMapping = new Modfierandgroupsmapping
                        {
                            Modifierid = modifier.Modifierid,
                            Modifiergroupid = groupId,
                            Createdat = DateTime.Now,
                            Createdbyid = viewModel?.Userid,
                            Isdelete = false
                        };
                        await _modfierandGroupsMapping.AddAsync(newMapping);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error editing modifier: " + ex.Message, ex);
        }
    }

    public async Task AddItemAsync(MenuWithItemsViewModel viewModel, IFormFile? uploadFile, int userId, Dictionary<string, ModifierGroupDataHelperViewModel> modifierGroups)
    {
        List<Category> c = await _category.GetAllCategoryAsync();
        viewModel.Categories = c;
        try
        {
            Item item = new Item
            {
                Itemname = viewModel.item?.Itemname,
                Categoryid = viewModel.item.Categoryid,
                Itemtype = viewModel.item?.Itemtype,
                Rate = viewModel.item?.Rate,
                Quantity = viewModel.item?.Quantity,
                Units = viewModel.item?.Units,
                Isavailabe = viewModel.item?.Isavailabe,
                DefaultTax = viewModel.item.Defaulttax,
                Taxpercentage = viewModel.item?.Taxpercentage,
                Shortcode = viewModel.item?.Shortcode,
                Description = viewModel.item?.Description,
                Createdat = DateTime.Now,
                Createdbyid = userId,
                Status = 1,
                Isdeleted = false,
                Editedat = DateTime.Now,
                Editedbyid = userId,
                Favourite = false,
                Deletedbyid = 0,
            };

            if (uploadFile != null && uploadFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadFile.FileName);
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "items");
                Directory.CreateDirectory(uploadFolder);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await uploadFile.CopyToAsync(fileStream);
                }

                item.Imageid = "/items/" + uniqueFileName;
            }
            await _items.AddAsync(item);


            // adding into mapping table of item and modifier groupid : 
            foreach (var g in modifierGroups)
            {
                ItemModifiergroupMapping m = new ItemModifiergroupMapping
                {
                    Itemid = item.Itemid,
                    Modifiergroupid = int.Parse(g.Key),
                    Minvalue = g.Value.MinValue,
                    Maxvalue = g.Value.MaxValue,
                    Isdeleted = false,
                    Createdat = DateTime.Now,
                    Createdbyid = userId,
                    Editedat = DateTime.Now,
                    Editedbyid = userId
                };
                await _itemModifiergroupMapping.AddAsync(m);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding item: " + ex.Message, ex);
        }


    }

    public async Task<List<ModifierDtoViewModel>> GetModifiersByModifierGroupId(int modifierGroupId)
    {
        List<Modfierandgroupsmapping>? mapping = await _modfierandGroupsMapping.GetByModifierGroupIdAsync(modifierGroupId);
        List<int>? modifierIds = (mapping != null) ? mapping.Select(u => u.Modifierid).ToList() : new List<int>();
        List<ModifierDtoViewModel> modifiers = new List<ModifierDtoViewModel>();

        if (modifierIds != null)
        {
            foreach (int id in modifierIds)
            {
                Modifier? m = await _modifier.GetByIdAsync(id);
                if (m != null)
                {
                    modifiers.Add(new ModifierDtoViewModel
                    {
                        Modifierid = m.Modifierid,
                        Modifiername = m.Modifiername,
                        Modifierrate = m.Modifierrate
                    });
                }
            }
        }
        return modifiers;
    }

    public async Task<Item?> GetItemById(int id)
    {
        return await _items.GetByIdAsync(id);
    }
    public async Task<Modifier?> GetModifierById(int id)
    {
        return await _modifier.GetByIdAsync(id);
    }

    public async Task<List<Modifier>> GetModifiersListAsync(List<int> id)
    {
        List<Modifier>? result = new List<Modifier>();
        foreach (int i in id)
        {
            Modifier? m = await _modifier.GetByIdAsync(i);
            if (m != null)
                result.Add(m);
        }
        return result;
    }

    public async Task<Item> IsAvailabeUpdateAsync(int id, bool available, int userid)
    {
        Item? item = await _items.GetByIdAsync(id);
        if (item != null)
        {
            item.Isavailabe = available;
            item.Editedat = DateTime.Now;
            item.Editedbyid = userid;
        }
        return item;
    }

    public async Task UpdateItemAsync(MenuWithItemsViewModel viewModel, IFormFile? uploadFile, int userId, Dictionary<string, ModifierGroupDataHelperViewModel> modifierGroups)
    {
        try
        {
            if (viewModel.item == null)
            {
                throw new ArgumentNullException(nameof(viewModel.item), "Item details are required.");
            }

            Item? item = await _items.GetByIdAsync(viewModel.item.Itemid);
            if (item == null)
            {
                throw new Exception("Item not found.");
            }

            // Update item properties
            item.Categoryid = viewModel.item.Categoryid;
            item.Itemname = viewModel.item.Itemname;
            item.Itemtype = viewModel.item.Itemtype;
            item.Rate = viewModel.item.Rate;
            item.Quantity = viewModel.item.Quantity;
            item.Units = viewModel.item.Units;
            item.Isavailabe = viewModel.item.Isavailabe;
            item.DefaultTax = viewModel.item.Defaulttax;
            item.Taxpercentage = viewModel.item.Taxpercentage;
            item.Shortcode = viewModel.item.Shortcode;
            item.Description = viewModel.item.Description;
            item.Editedbyid = userId;
            item.Editedat = DateTime.Now;

            // Handle file upload
            if (uploadFile != null && uploadFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadFile.FileName);
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "items");
                Directory.CreateDirectory(uploadFolder);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await uploadFile.CopyToAsync(fileStream);
                }

                item.Imageid = "/items/" + uniqueFileName;
            }
            else
            {
                item.Imageid = item.Imageid;
            }

            string? result = await _items.UpdateAsync(item);

            List<ItemModifiergroupMapping>? imm = await _itemModifiergroupMapping.GetByItemIdAsync(item.Itemid);
            foreach (var i in imm)
            {
                await _itemModifiergroupMapping.DeleteAsync(i);
            }

            foreach (var g in modifierGroups)
            {
                ItemModifiergroupMapping m = new ItemModifiergroupMapping
                {
                    Itemid = item.Itemid,
                    Modifiergroupid = int.Parse(g.Key),
                    Minvalue = g.Value.MinValue,
                    Maxvalue = g.Value.MaxValue,
                    Isdeleted = false,
                    Createdat = DateTime.Now,
                    Createdbyid = userId,
                    Editedat = DateTime.Now,
                    Editedbyid = userId
                };
                await _itemModifiergroupMapping.AddAsync(m);
            }


            System.Console.WriteLine("result: " + result);
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding item: " + ex.Message, ex);
        }

    }


}
