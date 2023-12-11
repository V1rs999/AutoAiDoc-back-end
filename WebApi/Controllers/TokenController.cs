using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Middleware;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IToken _token;

        public TokenController(IToken token)
        {
            _token = token;
        }

        [HttpGet("isExpired")]
        public IActionResult Index(string token)
        {
            if (_token.isExpired(token)) { return BadRequest("Token is Expired"); }
            return Ok();
        }
    }
}
