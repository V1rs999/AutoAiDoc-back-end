using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DropFileController : ControllerBase
    {
        private readonly IDropFileRepository _dropFileRepository;
        private readonly IFileReader _fileReader;

        public DropFileController(IDropFileRepository dropFileRepository, 
            IFileReader fileReader)
        {
            _dropFileRepository = dropFileRepository;
            _fileReader = fileReader;
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

            List<ErrorsDto> errorsDto = new List<ErrorsDto>();
            var vinCode = _fileReader.ReadFile(formFile);

            Vin = string.IsNullOrEmpty(vinCode.Vin) ? Vin : vinCode.Vin;

            if (string.IsNullOrEmpty(Vin) || Vin == "null") { return StatusCode(205); }

            foreach (var item in vinCode.Errors)
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

            if (!errorsDto.Any()) { return BadRequest("Немає помилок або файли пусті"); }

            var user = await _dropFileRepository.GetUserById(userId);

            if (user == null) { return BadRequest("Користувача не існує"); }

            vinCode.AppUser = user;
            vinCode.AppUserId = user.Id;

            if (!_dropFileRepository.AddVin(vinCode))
            {
                ModelState.AddModelError("", "Щось пішло не так при збережені");
                return StatusCode(500, ModelState);
            }

            return Ok(new { page = "listoferror", vin = Vin });
        }
    }
}
