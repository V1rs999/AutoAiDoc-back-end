using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;

namespace WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ListOFErrorController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IListOFErrorRepository _listOFErrorRepository;

        public ListOFErrorController(IHttpContextAccessor httpContextAccessor, IListOFErrorRepository listOFErrorRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _listOFErrorRepository = listOFErrorRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();

                var models = await _listOFErrorRepository.GetErrorsByUserIdAsync(curUserId);
                return Ok(models);
            }
            return Ok("Немає помилок");
        }
    }
}
