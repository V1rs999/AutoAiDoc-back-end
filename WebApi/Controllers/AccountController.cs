using Microsoft.AspNetCore.Http;
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

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
            };
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody]AccountDto model) 
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await _accountRepository.GetUserByIdAsync(model.Id);

            if(user == null) { return BadRequest("Користувача не існує"); }

            user.Email = model.Email;
            user.ImageUrl = model.ImageUrl;
            user.UserName = model.UserName;

            if (_accountRepository.Update(user)) { return Ok("Успіх"); }

            return BadRequest(model);
        }

    }
}
