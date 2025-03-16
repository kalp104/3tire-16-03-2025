using System;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class RoleService : IRoleService
{
    private readonly IGenericRepository<Role> _Role;
    private readonly IGenericRepository<PermissionsRole> _permissionsRole;
    public RoleService(
        IGenericRepository<Role> Role,
        IGenericRepository<PermissionsRole> permissionsRole)
    {
        _Role = Role;
        _permissionsRole = permissionsRole;
    }

    public async Task<List<Role>?> GetRoles()
    {
        return await _Role.GetAllAsync();
    }

    public async Task<List<RolePermissionModelView>?> RoleBasePermission(int id)
    {
        List<RolePermissionModelView>? result = await _permissionsRole.GetPermissionAsync(id);
        return result;
    }

    public async Task UpdatePermissions(List<RolePermissionModelView> models)
{
    foreach (var model in models)
    {
        PermissionsRole? permissionsRole = await _permissionsRole.GetRoleAndPermissionAsync(model.RoleId, model.PermissionId);
        if (permissionsRole != null)
        {
            permissionsRole.Canview = model.Canview;
            permissionsRole.Canedit = model.Canedit;
            permissionsRole.Candelete = model.Candelete;
            await _permissionsRole.UpdateAsync(permissionsRole);
        }
    }
}
}
