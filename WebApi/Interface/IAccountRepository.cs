using WebApi.Models;

namespace WebApi.Interface
{
    public interface IAccountRepository
    {
        Task<AppUser> GetUserByIdAsync(string id);
        Task<bool> isLoginProvider(string id);
        bool Update(AppUser appUser);
        bool Save();
    }
}
