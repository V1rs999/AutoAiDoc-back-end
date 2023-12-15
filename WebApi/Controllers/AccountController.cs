using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(IAccountRepository accountRepository, UserManager<AppUser> userManager)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string userId)
        {
            var user = await _accountRepository.GetUserByIdAsync(userId);

            if (user == null) { return BadRequest("Користувача не існує"); }
            var model = new AccountDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                LastName = user.LastName,
                FirstName = user.FirstName,
                PhoneNumber = user.PhoneNumber,
            };
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody]AccountDto model) 
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await _accountRepository.GetUserByIdAsync(model.Id);
            if(user == null) { return BadRequest("Користувача не існує"); }
            var LoginProvider = await _accountRepository.isLoginProvider(model.Id);

            if (LoginProvider && user.Email != model.Email)
            {
                return BadRequest("Користувачам авторизованим через Google не можна міняти електрону пошту");
            };

            user.Email = model.Email;
            user.ImageUrl = model.ImageUrl;
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if(!result.Succeeded) { return BadRequest(model); }

            if (_accountRepository.Update(user)) { return Ok("Успіх"); }

            return BadRequest(model);
        }

    }
}
