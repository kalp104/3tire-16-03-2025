using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IUserTableService
{
    Task<List<UserTableViewModel>?> GetUsersDetails();
    Task DeleteUser(int id);
    Task<UserViewModel> EditUserById(int id);
    Task EditUserPostAsync(UserViewModel model, [FromForm] IFormFile imageFile);
    Task<string?> AddUserService(AddNewUserViewModel model, [FromForm] IFormFile imageFile);
}
