using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Middleware;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ListOFErrorController : ControllerBase
    {
        private readonly IListOFErrorRepository _listOFErrorRepository;

        public ListOFErrorController(IListOFErrorRepository listOFErrorRepository)
        {
            _listOFErrorRepository = listOFErrorRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] ListOfErrorsDto listOfErrorsDto)
        {
                var models = await _listOFErrorRepository.GetErrorsByVinAsync(listOfErrorsDto.Vin);

                if (models.Count() <= 0) { return Ok(new { msg =  "Немає помилок" }); }

                return Ok(new {listOfErrorsDto.Vin, errors = models });
        }
    }
}
