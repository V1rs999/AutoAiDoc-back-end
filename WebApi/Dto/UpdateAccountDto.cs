namespace WebApi.Dto
{
    public class UpdateAccountDto
    {
        public string userId { get; set; }
        public string? userName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? emailR { get; set; }
        public string? passwordR { get; set; }
        public string? phone { get; set; }
    }
}
