using WebApi.Models;

namespace WebApi.Interface
{
    public interface IDropFileRepository
    {
        Task<AppUser> GetUserById(string id); 
        bool Add(VinCodes errors);
        bool Save();
    }
}
