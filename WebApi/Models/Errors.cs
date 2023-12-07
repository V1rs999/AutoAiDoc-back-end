namespace WebApi.Models
{
    public class Errors
    {
        public int Id { get; set; }
        public string Vin { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
