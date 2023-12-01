using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    public class AppUser: IdentityUser
    {
        public ICollection<Errors> Errors { get; set; }
    }
}
