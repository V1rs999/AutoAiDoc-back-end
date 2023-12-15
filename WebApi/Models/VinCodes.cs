namespace WebApi.Models
{
    public class VinCodes
    {
        public int Id { get; set; }
        public string Vin { get; set; }
        public ICollection<Errors> Errors { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public VinCodes()
        {
            Errors = new List<Errors>();
        }
    }
}
