namespace WebApi.Models
{
    public class Errors
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public VinCodes VinCodes { get; set; }
        public DateTime DateTime { get; set; }
    }
}
