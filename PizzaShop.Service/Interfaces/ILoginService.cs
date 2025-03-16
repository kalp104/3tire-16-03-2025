using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface ILoginService
{
    public Task<ResponseTokenViewModel> GetLoginService(LoginViewModel model);
    public Task<Account?> GetAccoutAsync(string email);
    public Task<string?> ResetPasswordService(ForgetPasswordViewModel model);

    
}
