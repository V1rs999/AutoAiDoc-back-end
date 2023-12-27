namespace WebApi.Dto
{
    public class ChangeImageDto
    {
        public string Id { get; set; }
        public IFormFile Image { get; set; }
    }
}
