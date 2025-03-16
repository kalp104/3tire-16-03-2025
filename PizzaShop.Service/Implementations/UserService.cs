using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class UserService : IUserService
{
    private readonly IGenericRepository<Account> _account;
    private readonly IGenericRepository<User> _User;
    private readonly IGenericRepository<Image> _Image;
    private readonly IGenericRepository<State> _State;
    private readonly IGenericRepository<City> _City;
    private readonly IGenericRepository<Country> _Country;
    private readonly IGenericRepository<List<RolePermissionModelView>> _Role;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserService(
        IWebHostEnvironment webHostEnvironment,
        IGenericRepository<State> State,
        IGenericRepository<Country> Country,
        IGenericRepository<City> City,
        IGenericRepository<List<RolePermissionModelView>> Role,
        IGenericRepository<Account> account,
        IGenericRepository<User> User,
        IGenericRepository<Image> Image)
    {
        _account = account;
        _User = User;
        _Image = Image;
        _Role = Role;
        _State = State;
        _City = City;
        _Country = Country;
        _webHostEnvironment = webHostEnvironment;
    }
    enum Roles
    {
        AccountManager = 1,
        Chef = 2,
        Admin = 3
    }
    public async Task<UserBagViewModel?> UserDetailBag(string email)
    {
        if (email != null)
        {
            Account? account = await _account.GetAccountByEmail(email);
            User? user = (account == null) ? null : await _User.GetUserByIdAsync(account.Accountid);
            Image? image = (user == null) ? null : await _Image.GetImageByIdAsync(user.Userid);

            if (account != null && user != null && image != null)
            {
                return new UserBagViewModel
                {
                    UserName = account.Username,
                    UserId = user.Userid,
                    ImageUrl = image.Imageurl,
                };
            }
        }
        return null;
    }

    public async Task<List<RolePermissionModelView>?> RoleFilter(string rolename)
    {
        int roleid = 0;
        switch (rolename)
        {
            case "Admin":
                roleid = 3;
                break;
            case "AccountManager":
                roleid = 1;
                break;
            case "Chef":
                roleid = 2;
                break;
            default:
                return null;
        }
        ;
        List<RolePermissionModelView>? result = await _Role.GetRolePermissionModelViewAsync(roleid);
        if (result != null)
        {
            return result;
        }
        return null;
    }

    
    public async Task<RolePermissionModelView?> PermissionFilter(string rolename, int permission)
    {
        int roleid = 0;
        switch (rolename)
        {
            case "Admin":
                roleid = 3;
                break;
            case "AccountManager":
                roleid = 2;
                break;
            case "Chef":
                roleid = 1;
                break;
            default:
                return null;
        }
        ;
        // repo call : permission = 1 for users bar
        RolePermissionModelView? result = await _Role.GetPermissionAsync(roleid, permission);

        if (result != null)
        {
            return result;
        }
        return null;

    }

    public async Task<List<Country1>?> GetCountries()
    {
        List<Country> countries = await _Country.GetAllAsync();
        List<Country1> country1List = countries.Select(c => new Country1
        {
            CountryId = c.Countryid,
            CountryName = c.Countryname
        }).ToList();
        return country1List;
    }
    public async Task<List<State>?> GetStates(int id)
    {
        List<State>? state = await _State.GetStateListByIdAsync(id);
        if (state != null) return state;
        return null;
    }

    public async Task<List<City>?> GetCities(int id)
    {
        List<City>? city = await _City.GetCityListByIdAsync(id);
        if (city != null) return city;
        return null;
    }

    public async Task<string?> UpdateUserProfile(UserViewModel model)
    {
        User? user = await _User.GetByIdAsync(model.userId);
        Account? account = (model != null) ? await _account.GetByIdAsync(model.accountId) : null;

        if (user == null && account == null)
        {
            return "profile update failed";
        }
        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        account.Username = model.Username;
        user.Phone = model.phone;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;
        user.Countryid = model.countryId;
        user.Stateid = model.stateId;
        user.Cityid = model.cityId;
        user.Editedat = DateTime.Now;
        account.Editedat = DateTime.Now;
        await _account.UpdateAsync(account);
        await _User.UpdateAsync(user);

        return "";
    }

    public async Task<string?> UpdateImage(int userId, IFormFile imageFile)
    {
        User? user = await _User.GetByIdAsync(userId);
        if (user != null)
        {
            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
                Image? image = await _Image.GetImageByIdAsync(userId);
                if (image != null)
                {
                    image.Userid = userId;
                    image.Imageurl = "/uploads/" + uniqueFileName;
                    await _Image.UpdateAsync(image);
                    return image.Imageurl;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("image uplode failed : " + e.Message);
            }
        }
        return "";

    }

    public async Task<string?> ChangePassword(ResetPasswordViewModel model, string email) {
        Account? account = await _account.GetAccountByEmail(email);
        if(account == null)
            return "1"; //"user doesnot exist"
        if (!BCrypt.Net.BCrypt.EnhancedVerify(model.CurrentPassword, account.Password)) 
            return "2"; //"password does not match"
        if(model.Password == model.CurrentPassword) 
            return "3"; // New password cannot be the same as the old one.
        if(model.Password != model.ConfirmPassword)
            return "4"; // Passwords do not match.

        account.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);
        account.Editedat = DateTime.Now;
        await _account.UpdateAsync(account);
        return "0";
    }
}
