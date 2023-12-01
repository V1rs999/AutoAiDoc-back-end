using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LogoutController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LogoutController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            await _signInManager.SignOutAsync();
            return Ok("Success logout");
        }
    }
}
