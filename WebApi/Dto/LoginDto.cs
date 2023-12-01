using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class LoginDto
    {
        [Required]
        [Display(Name = ("Електрона пошта"))]
        public string Email { get; set; }
        [Required]
        [Display(Name = ("Пароль"))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
