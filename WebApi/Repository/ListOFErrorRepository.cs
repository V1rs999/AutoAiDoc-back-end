using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Interface;

namespace WebApi.Repository
{
    public class ListOFErrorRepository : IListOFErrorRepository
    {
        private readonly AppDbContext _context;

        public ListOFErrorRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ErrorsDto>> GetErrorsByUserIdAsync(string id)
        {
            var errors = await _context.Errors.Where(e => e.AppUserId == id).ToListAsync();
            List<ErrorsDto> errorsDto = new List<ErrorsDto>();
            foreach (var error in errors)
            {
                errorsDto.Add(new ErrorsDto
                {
                    Vin = error.Vin,
                    Code = error.Code,
                    Description = error.Description,
                });
            }
            return errorsDto;
        }
    }
}
