using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<VinCodes> VinCodes { get; set; }
        public DbSet<Errors> Errors { get; set; }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    // Configure identity to use Unicode for UserNames
        //    builder.Entity<AppUser>(entity =>
        //    {
        //        entity.Property(e => e.UserName).IsUnicode(true);
        //    });
        //}
    }

}
