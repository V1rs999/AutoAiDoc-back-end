namespace WebApi.Dto
{
    public class DropFileDto
    {
        [AllowedExtensions(new string[] { ".txt" })]
        public IFormFile File { get; set; }
    }
}
