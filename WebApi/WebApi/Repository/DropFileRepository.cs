using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Repository
{
    public class DropFileRepository : IDropFileRepository
    {
        private readonly AppDbContext _context;

        public DropFileRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool Add(Errors errors)
        {
            _context.Add(errors);
            return Save();
        }

        public async Task<AppUser> GetUserById(string id)
        { 
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
