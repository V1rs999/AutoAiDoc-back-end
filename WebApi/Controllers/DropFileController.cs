using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Services;
using WebApi.Dto;
using WebApi.Models;
using System.Net;

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
        public async Task<IActionResult> Index([FromForm] string userId, IFormFile formFile,[FromForm] string? Vin)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (formFile == null) { return BadRequest("Прикріпіть файл"); }

            var ext = Path.GetExtension(formFile.FileName);

            if (ext != ".txt") { return BadRequest("Неправильний тип файла"); }

            var fileVin = _fileReader.FindVinCode(formFile);
            Vin = string.IsNullOrEmpty(fileVin) ? Vin : fileVin;

            if(string.IsNullOrEmpty(Vin) || Vin == "null") { return StatusCode(205); }

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
                        DateTime = DateTime.Now,
                    });
            }

            var user = await _dropFileRepository.GetUserById(userId);

            if (user == null) { return BadRequest("Користувача не існує"); }

            var vinCodes = new VinCodes
            {
                Vin = Vin,
                AppUser = user,
                AppUserId = user.Id,
                Errors = errors.ToList(),
            };

            if (!_dropFileRepository.Add(vinCodes))
            {
                ModelState.AddModelError("", "Щось пішло не так при збережені");
                return StatusCode(500, ModelState);
            }

            if (!errorsDto.Any()) { return BadRequest("Немає помилок або файли пусті"); }

            return Ok(new { page = "listoferror", vin = Vin });
        }
    }
}
