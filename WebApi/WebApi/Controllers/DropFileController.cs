using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Services;
using WebApi;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DropFileController : ControllerBase
    {
        private readonly IDropFileRepository _dropFileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DropFileController(IDropFileRepository dropFileRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dropFileRepository = dropFileRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Ok");
        }

        [HttpPost()]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Index(IFormFile formFile)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            FileReader file = new FileReader();
            var errors = file.ReadAsList(formFile);
            List<ErrorsDto> errorsDto = new List<ErrorsDto>();
            foreach (var item in errors)
            {
                errorsDto.Add(
                    new ErrorsDto
                    {
                        Code = item.Code,
                        Description = item.Description,
                    });
            }
            if (!User.Identity.IsAuthenticated) { return Ok(errorsDto); }
  
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _dropFileRepository.GetUserById(curUserId);

            foreach (var item in errors)
            {
                item.AppUser = user;
                item.AppUserId = user.Id;
                if (!_dropFileRepository.Add(item))
                {
                    ModelState.AddModelError("", "Щось пішло не так при збережені");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok(errorsDto);  
        }
    }
}
