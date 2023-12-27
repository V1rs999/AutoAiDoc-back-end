using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

            if (user == null) { return BadRequest("Користувача не існує"); }
            var model = new AccountDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
            };
            string jsUser = JsonConvert.SerializeObject(model);
            return Ok(jsUser);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UpdateAccountDto model) 
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await _accountRepository.GetUserByIdAsync(model.Id);
            if(user == null) { return BadRequest("Користувача не існує"); }

            var LoginProvider = await _accountRepository.isLoginProvider(model.Id);

            if (LoginProvider && user.Email != model.Email)
            {
                return BadRequest("Користувачам авторизованим через Google не можна міняти електрону пошту");
            };

            user.Email = model.Email ?? user.Email;
            user.UserName = model.UserName ?? user.UserName;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.LastName = model.LastName ?? user.LastName;
            user.FirstName = model.FirstName ?? user.FirstName;

            if (model.Password != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
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
