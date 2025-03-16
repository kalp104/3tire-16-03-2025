using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<Account?> GetAccountByEmail(string email);
    Task<string?> UpdateAsync(T model);
    Task<User?> GetUserByIdAsync(int id);
    Task<Image?> GetImageByIdAsync(int id);
    Task AddImageAsync(Image image);
    Task<List<RolePermissionModelView>?> GetRolePermissionModelViewAsync(int roleid);
    Task<RolePermissionModelView?> GetPermissionAsync(int roleid, int permission);
    Task<List<UserTableViewModel>?> UserDetailAsync();
    Task<T?> GetByIdAsync(int? id);
    Task<List<T>> GetAllAsync();
    Task<List<City>> GetCityListByIdAsync(int id);
    Task<List<State>> GetStateListByIdAsync(int id);
    Task AddAsync(T model);
    Task<List<RolePermissionModelView>?> GetPermissionAsync(int id);
    Task<PermissionsRole?> GetRoleAndPermissionAsync(int roleid, int permissionid);
    Task<List<Category>> GetAllCategoryAsync();
    Task<List<Modifiergroup>> GetAllModifierGroupAsync();
    Task<List<Item>> GetAllItemsAsync();
    Task<List<Modifier>> GetAllModifierAsync();
    Task<List<Item>> GetItemsByCategoryAsync(int? id);
    Task<List<Modifier>> GetModifiersByMGAsync(int? id);
    Task<Modfierandgroupsmapping?> GetMappingsById(int Modifiergroupid, int modifierid);
    Task<List<Modfierandgroupsmapping>?> GetByModifierIdAsync(int? modifiersid);
    Task DeleteAsync(T entity);
}
