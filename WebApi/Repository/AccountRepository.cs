using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<AppUser> GetUserByIdAsync(string id)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> isLoginProvider(string id)
        {
            var loginProvider = await _context.UserLogins.FirstOrDefaultAsync(u => u.UserId == id);
            return loginProvider != null ? true : false;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser appUser)
        {
            _context.Update(appUser);
            return Save();
        }
    }
}
