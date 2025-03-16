using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;


namespace PizzaShop.Service.Implementations;

public class LoginService : ILoginService
{
    private readonly IGenericRepository<Account> _account;
    private readonly IConfiguration _configuration;

    public LoginService(IGenericRepository<Account> account, IConfiguration configuration)
    {
        _configuration = configuration;
        _account = account;

    }
    enum Roles
    {
        AccountManager = 1,
        Chef = 2,
        Admin = 3
    }

    public async Task<ResponseTokenViewModel> GetLoginService(LoginViewModel model)
    {
        Account? account = await _account.GetAccountByEmail(model.Email);

        if (account != null)
        {
            string? rolename = account.Roleid != 0 ? ((Roles)account.Roleid).ToString() : null;
            if (rolename != null && BCrypt.Net.BCrypt.EnhancedVerify(model.Password, account.Password))
            {
                if (model?.Rememberme == null)
                {
                    model.Rememberme = false;
                }
                var TokenExpireTime = model.Rememberme ?
                                      DateTime.Now.AddDays(30) :
                                      DateTime.Now.AddDays(1);
                var token = GenerateJwtToken(model.Email, TokenExpireTime, rolename);

                if (token != null)
                {
                    return new ResponseTokenViewModel()
                    {
                        token = token,
                        response = "Login successful"
                    };
                }
            }
            else
            {
                return new ResponseTokenViewModel()
                {
                    token = "",
                    response = "please enter valid details"
                };
            }

        }
        return new ResponseTokenViewModel()
        {
            token = "",
            response = "please enter valid details"
        };

    }


    private string GenerateJwtToken(string email, DateTime expiryTime, string roleName)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Get user role from database
        Console.WriteLine(roleName);
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roleName) // Add roles as needed
            };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiryTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<Account?> GetAccoutAsync(string email)
    {
        Account? account = await _account.GetAccountByEmail(email);
        return account;
    }

    public async Task<string?> ResetPasswordService(ForgetPasswordViewModel model)
    {
        if (model.Email != null)
        {
            Account? account = await _account.GetAccountByEmail(model.Email);
            if (account == null)
            {
                return "1"; // account not exists
            }
            if (model.Password != model.ConfirmPassword)
            {
                return "2"; // password doesnot match
            }
            if (account != null && BCrypt.Net.BCrypt.EnhancedVerify(model.Password, account.Password))
            {
                return "3"; // password can not be same as previous one
            }

            if (account != null)
            {

                account.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);
                string? res = await _account.UpdateAsync(account);

                if (res == "saved")
                {
                    return "4";  // successfully changed password   
                }
            }
        }

        return ""; // invalid
    }




}