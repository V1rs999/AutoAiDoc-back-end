using WebApi.Models;

namespace WebApi.Interface
{
    public interface IDropFileRepository
    {
        Task<AppUser> GetUserById(string id);
        bool AddVin(VinCodes errors);
        bool Save();
    }
}
