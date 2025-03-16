using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class UserTableService : IUserTableService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IGenericRepository<User> _User;
    private readonly IGenericRepository<Account> _Account;
    private readonly IGenericRepository<Image> _Image;
    private readonly IGenericRepository<Role> _Role;
    private readonly IGenericRepository<Country> _Country;
    private readonly IGenericRepository<State> _State;
    private readonly IGenericRepository<City> _City;
    public UserTableService(
        IWebHostEnvironment webHostEnvironment,
        IGenericRepository<City> City,
        IGenericRepository<State> State,
        IGenericRepository<Country> Country,
        IGenericRepository<Role> Role,
        IGenericRepository<Image> Image,
        IGenericRepository<User> User,
        IGenericRepository<Account> Account)
    {
        _User = User;
        _Account = Account;
        _Image = Image;
        _Role = Role;
        _Country = Country;
        _State = State;
        _City = City;
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task<List<UserTableViewModel>?> GetUsersDetails()
    {
        List<UserTableViewModel>? result = await _User.UserDetailAsync();
        if (result == null)
        {
            return null;
        }
        return result;
    }

    public async Task DeleteUser(int id)
    {
        User? user = await _User.GetByIdAsync(id);
        Account? account = await _Account.GetByIdAsync(id);
        user!.Isdeleted = true;
        account!.Isdeleted = true;
        user.Deletedat = DateTime.Now;
        account.Deletedat = DateTime.Now;
        user.Deletedbyid = id;
        account.Deletedbyid = id;
        await _User.UpdateAsync(user);
        await _Account.UpdateAsync(account);
    }

    public async Task<UserViewModel> EditUserById(int id)
    {
        User? user = await _User.GetByIdAsync(id);
        Account? account = (user != null) ? await _Account.GetByIdAsync(user.Accountid) : null;
        Image? image = (user != null) ? await _Image.GetByIdAsync(user.Userid) : null;
        Role? role = (account != null) ? await _Role.GetByIdAsync(account.Roleid) : null;
        Country? country = (user != null) ? await _Country.GetByIdAsync(user.Countryid) : null;
        State? state = (user != null) ? await _State.GetByIdAsync(user.Stateid) : null;
        City? city = (user != null) ? await _City.GetByIdAsync(user.Cityid) : null;
        List<Country>? countries = await _Country.GetAllAsync();
        List<Country1> country1List = countries.Select(c => new Country1
        {
            CountryId = c.Countryid,
            CountryName = c.Countryname
        }).ToList();

        UserViewModel Users = new UserViewModel()
        {
            Firstname = user?.Firstname,
            Lastname = user?.Lastname,
            Username = account?.Username,
            Email = account.Email,
            Status = user?.Status,
            Countryname = country?.Countryname,
            Statename = state?.Statename,
            cityname = city?.Cityname,
            Zipcode = user?.Zipcode,
            Address = user?.Address,
            phone = user?.Phone,
            cityId = user.Cityid,
            stateId = user.Stateid,
            countryId = user.Countryid,
            userId = user.Userid,
            Countries = country1List,
            accountId = account.Accountid,
            roleId = user.Roleid,
            Rolename = role?.Rolename

        };
        return Users;

    }

    public async Task EditUserPostAsync(UserViewModel model, [FromForm] IFormFile imageFile)
    {
        User? user = await _User.GetByIdAsync(model.userId);
        Account? account = (user != null) ? await _Account.GetByIdAsync(user.Accountid) : null;
        Image? image = await _Image.GetImageByIdAsync(model.userId);
        try
        {
            if (imageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                if (image != null)
                {
                    image.Userid = model.userId;
                    image.Imageurl = "/uploads/" + uniqueFileName;
                    await _Image.UpdateAsync(image);
                }
                if (image == null)
                {
                    Image i = new Image();

                    i.Userid = model.userId;
                    i.Imageurl = "/uploads/" + uniqueFileName;

                    await _Image.AddImageAsync(i);
                }
            }

            if (user != null && account != null)
            {
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.Phone = model.phone;
                user.Zipcode = model.Zipcode;
                user.Address = model.Address;
                user.Cityid = model.cityId;
                user.Stateid = model.stateId;
                user.Countryid = model.countryId;
                user.Status = model.Status;
                user.Roleid = model.roleId;
                account.Username = model.Username;
                account.Editedat = DateTime.Now;
                user.Editedat = DateTime.Now;

                await _Account.UpdateAsync(account);
                await _User.UpdateAsync(user);

            }
        }
        catch (Exception e)
        {
            Console.WriteLine("error while editing user" + e.Message);
        }
    }

    public async Task<string?> AddUserService(AddNewUserViewModel model, [FromForm] IFormFile imageFile)
    {
        if (model != null)
        {
            Account? account = model.Email != null ? await _Account.GetAccountByEmail(model.Email) : null;
            User? user = account != null ? await _User.GetByIdAsync(account.Accountid) : null;
            if (account != null)
            {
                return "email already exists";
            }

            Account? newAccount = new()
            {
                Username = model.Username,
                Email = model.Email ?? throw new ArgumentNullException(nameof(model.Email)),
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password),
                Roleid = model.roleId,
                Isdeleted = false,
                Rememberme = false,
                Createdat = DateTime.Now,
                Editedat = DateTime.Now,
            };
            await _Account.AddAsync(newAccount);

            User newUser = new()
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Phone = model.phone,
                Address = model.Address,
                Zipcode = model.Zipcode,
                Countryid = model.countryId,
                Stateid = model.stateId,
                Cityid = model.cityId,
                Accountid = newAccount.Accountid,
                Roleid = model.roleId,
                Status = 3,
                Isdeleted = false,
                Createdat = DateTime.Now,
                Editedat = DateTime.Now,
            };
            await _User.AddAsync(newUser);

            if (imageFile != null)
            {
                Image images = new()
                {
                    Userid = newUser.Userid,
                    Imageurl = "/uploads/" + Guid.NewGuid().ToString() + "_" + imageFile.FileName
                };
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, images.Imageurl.Substring(9));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                await _Image.AddImageAsync(images);
            }
        }

        return "";
    }

    
    
}
