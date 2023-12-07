using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;

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

        [HttpPost("isExpired")]
        public IActionResult Index(string token)
        {
            if (_token.isExpired(token)) { return BadRequest(new { msg = "Token is Expired" }); }
            return Ok();
        }
    }
}
