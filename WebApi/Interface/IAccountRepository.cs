using WebApi.Models;

namespace WebApi.Interface
{
    public interface IAccountRepository
    {
        Task<AppUser> GetUserByIdAsync(string id);
        bool Update(AppUser appUser);
        bool Save();
    }
}
