using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RegistrationController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var regsterVM = new RegisterDto();
            return Ok(regsterVM);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Index([FromBody] RegisterDto registerVM)
        {
            if (!ModelState.IsValid) return BadRequest(registerVM);

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                ModelState.AddModelError("", "Користувач з тикою електроною почтою вже існує");
                return StatusCode(422, ModelState);
            }

            var newUser = new AppUser()
            {
                UserName = registerVM.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = registerVM.Email,
                ImageUrl = "http://16.170.236.235:5000/img/user.webp",
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (!newUserResponse.Succeeded)
            {
                ModelState.AddModelError("", "Щось пішло не так при збережені");
                return StatusCode(500, ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
            if (result.Succeeded)
            {
                return Ok("Success");
            }

            return StatusCode(500, ModelState);
        }
    }
}
