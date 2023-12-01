using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Interface
{
    public interface IListOFErrorRepository
    {
        Task<IEnumerable<ErrorsDto>> GetErrorsByUserIdAsync(string id);
    }
}
