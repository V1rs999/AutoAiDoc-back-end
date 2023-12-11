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

        [HttpGet]
        public async Task<IActionResult> Index(string Vin)
        {
            var models = await _listOFErrorRepository.GetErrorsByVinAsync(Vin);

            if (models.Count() <= 0) { return Ok("Немає помилок"); }

            return Ok(new { Vin, errors = models.OrderByDescending(date => date.DateTime).ToList() });
        }
    }
}

