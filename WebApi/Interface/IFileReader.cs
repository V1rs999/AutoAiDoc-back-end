using WebApi.Models;

namespace WebApi.Interface
{
    public interface IFileReader
    {
        VinCodes ReadFile(IFormFile file);
    }
}
