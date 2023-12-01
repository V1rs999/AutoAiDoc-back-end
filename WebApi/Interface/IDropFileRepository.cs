using WebApi.Models;

namespace WebApi.Interface
{
    public interface IDropFileRepository
    {
        Task<AppUser> GetUserById(string id); 
        bool Add(Errors errors);
        bool Save();
    }
}
