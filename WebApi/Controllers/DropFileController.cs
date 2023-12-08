using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Services;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DropFileController : ControllerBase
    {
        private readonly IDropFileRepository _dropFileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileReader _fileReader;
        private readonly IToken _token;

        public DropFileController(IDropFileRepository dropFileRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IFileReader fileReader,
            IToken token)
        {
            _dropFileRepository = dropFileRepository;
            _httpContextAccessor = httpContextAccessor;
            _fileReader = fileReader;
            _token = token;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Ok");
        }

        [HttpPost()]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Index([FromForm] string userId, IFormFile formFile, string? Vin)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (formFile == null) { return BadRequest(new { msg = "Прикріпіть файл" }); }

            var ext = Path.GetExtension(formFile.FileName);
            if (ext != ".txt")
            {
                return BadRequest(new { msg = "Неправильний тип файла" });
            }

            Vin = _fileReader.FindVinCode(formFile);
            if(string.IsNullOrEmpty(Vin)) { return BadRequest(new { msg = "У файлі немає Vin коду" }); }

            List<ErrorsDto> errorsDto = new List<ErrorsDto>();


            var errors = _fileReader.FindErrors(formFile);

            foreach (var item in errors)
            {
                errorsDto.Add(
                    new ErrorsDto
                    {
                        Vin = Vin,
                        Code = item.Code,
                        Description = item.Description,
                    });
            }

            var user = await _dropFileRepository.GetUserById(userId);

            foreach (var item in errors)
            {
                item.Vin = Vin;
                item.AppUser = user;
                item.AppUserId = user.Id;
                if (!_dropFileRepository.Add(item))
                {
                    ModelState.AddModelError("", "Щось пішло не так при збережені");
                    return StatusCode(500, ModelState);
                }
            }

            if (!errorsDto.Any())
            {
                return BadRequest(new { msg = "Немає помилок або файли пусті" });
            }

            return Ok("listoferror");
        }
    }
}
