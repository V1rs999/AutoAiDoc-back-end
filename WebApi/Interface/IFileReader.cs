using WebApi.Models;

namespace WebApi.Interface
{
    public interface IFileReader
    {
        string FindVinCode(IFormFile file);
        IEnumerable<Errors> FindErrors(IFormFile file);
    }
}
