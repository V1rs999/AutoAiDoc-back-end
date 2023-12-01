using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class RegisterDto
    {
        [Required]
        [Display(Name = ("Електрона пошта"))]
        public string Email { get; set; }
        [Required]
        [Display(Name = ("І'мя користувача"))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
