using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class ExternalLoginDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
