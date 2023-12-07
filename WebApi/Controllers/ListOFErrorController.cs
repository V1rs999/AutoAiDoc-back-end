using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Middleware;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ListOFErrorController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IListOFErrorRepository _listOFErrorRepository;
        private readonly IToken _token;

        public ListOFErrorController(IHttpContextAccessor httpContextAccessor, 
            IListOFErrorRepository listOFErrorRepository,
            IToken token)
        {
            _httpContextAccessor = httpContextAccessor;
            _listOFErrorRepository = listOFErrorRepository;
            _token = token;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string token)
        {
            if (_token.isExpired(token)) { return BadRequest(new { msg = "Token is expired" }); }

            if (User.Identity.IsAuthenticated)
            {
                var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();

                var models = await _listOFErrorRepository.GetErrorsByUserIdAsync(curUserId);

                if (models.Count() <= 0) { return Ok(new { msg =  "Немає помилок" }); }

                return Ok(models.GroupBy(m => m.Vin));
            }
            return Ok(new { msg = "Немає помилок" });
        }
    }
}
