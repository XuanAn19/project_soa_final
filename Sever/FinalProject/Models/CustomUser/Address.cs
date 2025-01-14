namespace FinalProject.Models.CustomUser
{
    public class Address : KeyEntities
    {
        public string? Provice { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Detail { get; set; }

        public User? Users { get; set; }
    }
}
