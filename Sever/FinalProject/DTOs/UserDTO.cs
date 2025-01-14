namespace FinalProject.DTOs
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }

        public DateTime LastLoggedIn { get; set; }
        public DateTime CreateDateAccessToken { get; set; }
        public bool isActive { get; set; }
    }
}
