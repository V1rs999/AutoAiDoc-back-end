using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    public class AppUser: IdentityUser
    {
        public string ImageUrl { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ICollection<VinCodes> VinCodes { get; set; }
    }
}
