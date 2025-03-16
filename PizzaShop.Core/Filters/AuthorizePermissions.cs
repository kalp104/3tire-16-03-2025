using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Filters;

public class AuthorizePermissionUserTable : ActionFilterAttribute
{
    private readonly IUserService _userService;

        public AuthorizePermissionUserTable(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string role = context.HttpContext.Items["UserRole"] as string ?? string.Empty;
            var rolefilter = await _userService.RoleFilter(role);

            if (rolefilter != null)
            {
                foreach (var i in rolefilter)
                {
                    if (i.PermissionId == 1 && i.Canview == false)
                    {
                        context.Result = new RedirectToActionResult("Privacy", "Home", null);
                        return;
                    }
                }
            }
            await next();
        }
}
