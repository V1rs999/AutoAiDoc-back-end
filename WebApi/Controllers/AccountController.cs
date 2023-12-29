using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
        private readonly IPhotoService _photoService;

        public AccountController(IAccountRepository accountRepository, 
            UserManager<AppUser> userManager, 
            IPhotoService photoService)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string userId)
        {
            var user = await _accountRepository.GetUserByIdAsync(userId);

            var LoginProvider = await _accountRepository.isLoginProvider(userId);

            if (user == null) { return BadRequest("Користувача не існує"); }
            var model = new AccountDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                LastName = user.LastName ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ImageUrl = user.ImageUrl,
                auth_Google = LoginProvider,
            };
            string jsUser = JsonConvert.SerializeObject(model);
            return Ok(jsUser);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UpdateAccountDto model) 
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await _accountRepository.GetUserByIdAsync(model.userId);
            if(user == null) { return BadRequest("Користувача не існує"); }
            var LoginProvider = await _accountRepository.isLoginProvider(model.userId);
            var validate = await _userManager.CheckPasswordAsync(user, model.passwordR ?? string.Empty);

            if (LoginProvider && (model.emailR != user.Email || !validate))
            {
                    return BadRequest("Користувачам авторизованим через Google не можна міняти електрону пошту та пароль");
            }

            user.Email = model.emailR.IsNullOrEmpty() ? user.Email : model.emailR;
            user.UserName = model.userName.IsNullOrEmpty() ? user.UserName : model.userName;
            user.PhoneNumber = model.phone.IsNullOrEmpty() ? user.PhoneNumber : model.phone;
            user.LastName = model.lastName.IsNullOrEmpty() ? user.LastName : model.lastName;
            user.FirstName = model.firstName.IsNullOrEmpty() ? user.FirstName : model.firstName;
            await _userManager.UpdateNormalizedUserNameAsync(user);
            await _userManager.UpdateNormalizedEmailAsync(user);

            if (!model.passwordR.IsNullOrEmpty())
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, model.passwordR);
                if (!result.Succeeded) { return BadRequest(model); }
            }

            if (_accountRepository.Update(user)) { return Ok("Успіх"); }

            return BadRequest("Помилка при оновленні даних");
        }

        [HttpPost("ChangeImage")]
        public async Task<IActionResult> ChangeImage([FromForm] ChangeImageDto model)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await _accountRepository.GetUserByIdAsync(model.Id);
            if (user == null) { return BadRequest("Користувача не існує"); }

            if (model.Image != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ImageUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Сервер не зміг оновти фото");
                }

                var photoResult = await _photoService.AddPhotoAsync(model.Image);
                user.ImageUrl = photoResult.Url.ToString() ?? user.ImageUrl;
            }

            if (_accountRepository.Update(user)) { return Ok("Успіх"); }

            return BadRequest("Помилка при оновленні даних");
        }
    }
}
