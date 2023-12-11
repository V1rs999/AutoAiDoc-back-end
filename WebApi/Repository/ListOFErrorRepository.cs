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
        public async Task<IEnumerable<ListOfErrorsOutputDto>> GetErrorsByVinAsync(string vin)
        {
            var errors = await _context.Errors.Where(e => e.VinCodes.Vin == vin).ToListAsync();
            List<ListOfErrorsOutputDto> errorsDto = new List<ListOfErrorsOutputDto>();
            foreach (var error in errors)
            {
                errorsDto.Add(new ListOfErrorsOutputDto
                {
                    Code = error.Code,
                    Description = error.Description,
                    DateTime = error.DateTime.ToString("dd\\MM\\yy HH:mm"),
                });
            }
            return errorsDto;
        }
    }
}
