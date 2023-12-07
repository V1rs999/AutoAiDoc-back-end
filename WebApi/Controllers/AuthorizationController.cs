using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Middleware;
using WebApi.Models;


namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IToken _token;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IToken token,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _token = token;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
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
                var authClaims = _token.GetClaimsForJwt();

                var myToken = _token.GetToken(authClaims);
                var result = await _signInManager.PasswordSignInAsync(curUser, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok(ReturnUrl(string.Empty, curUser.Id, curUser.Email, curUser.UserName, myToken));
                }
            }
            return Unauthorized();
        }

        [HttpGet("Google")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string returnUrl = "https://localhost:5173";
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

            var authClaims = _token.GetClaimsForJwt();
            var myToken = _token.GetToken(authClaims);

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                var curUser = await _userManager.FindByEmailAsync(email);
                return Redirect(ReturnUrl(returnUrl, curUser.Id, curUser.Email, curUser.UserName, myToken));
            }
            else
            {
                var curUser = await _userManager.FindByEmailAsync(email);
                if (curUser != null)
                {
                    ModelState.AddModelError("", "Користувач з тикою електроною почтою вже існує");
                    return StatusCode(422, ModelState);
                }
                
                var user = new AppUser { UserName = name, Email = email };
                var createRresult = await _userManager.CreateAsync(user);
                if (createRresult.Succeeded)
                {
                    createRresult = await _userManager.AddLoginAsync(user, info);
                    if (createRresult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return Redirect(ReturnUrl(returnUrl, curUser.Id, curUser.Email, curUser.UserName, myToken));
                    }
                }

                ModelState.AddModelError("Email", "Error occuresd");
                return BadRequest();
            }
        }

        private string ReturnUrl(string returnUrl, string userId, string email, string _userName, JwtSecurityToken myToken)
        {
            var userName = WebUtility.UrlEncode(_userName);
            var user = new {userId, email, userName };
            var token = new { token = new JwtSecurityTokenHandler().WriteToken(myToken), tokenId = myToken.Id };
            string finalUrl = $"{returnUrl}?token={token}&user={user}";
            return finalUrl;
        }
    }
}
