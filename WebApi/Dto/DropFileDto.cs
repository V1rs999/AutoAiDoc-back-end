namespace WebApi.Dto
{
    public class DropFileDto
    {
        public IFormFile File { get; set; }
        public string userId { get; set; }
        public string Vin { get; set; }
    }
}
