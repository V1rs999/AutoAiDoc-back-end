using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Dto;
using WebApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthorizationController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Index([FromBody] LoginDto model)
        {
            var curUser = await _userManager.FindByEmailAsync(model.Email);
            if (curUser != null && await _userManager.CheckPasswordAsync(curUser, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(curUser);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, curUser.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);
                var userId = new {userId = curUser.Id};
                var user = new { username = curUser.UserName };
                var result = await _signInManager.PasswordSignInAsync(curUser, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        userId,
                        user,
                    });
                }
            }
            return Unauthorized();
        }

        [HttpGet("Google")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string returnUrl = "http://localhost:5173/authorization";
            string provider = "Google";
            var redirectUrl = Url.Action(nameof(Callback), "Authorization", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback(string returnUrl = "/", string remoteError = null)
        {

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Index));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var userInfo = new ExternalLoginDto { Email = email, Name = name };

            //Sign in the user with this external login provider, if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                //update any authentication tokens
                return Ok(new { returnUrl , userInfo });
                //return Ok("Logged in");
            }
            else
            {
                //If the user does not have account, then we will ask the user to create an account.
                var userExist = await _userManager.FindByEmailAsync(email);
                if (userExist != null)
                {
                    ModelState.AddModelError("", "Користувач з тикою електроною почтою вже існує");
                    return StatusCode(422, ModelState);
                }
                //return Ok( new ExternalLoginDto { Email = email, Name = name});
                var user = new AppUser { UserName = name, Email = email };
                var createRresult = await _userManager.CreateAsync(user);
                if (createRresult.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "User");
                    createRresult = await _userManager.AddLoginAsync(user, info);
                    if (createRresult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return Ok(new { returnUrl = "http://localhost:5173/authorization", userInfo });
                        //return Ok(new ExternalLoginDto { Email = email, Name = name });
                    }
                }
                ModelState.AddModelError("Email", "Error occuresd");
            return BadRequest();
            }
        }

        [HttpPost("ExternalLoginConfirmation")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginDto model, string? returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //get the info about the user from external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return BadRequest();
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "User");
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return Ok();
                    }
                }
                ModelState.AddModelError("Email", "Error occuresd");
            }
            return BadRequest();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
