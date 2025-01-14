using FinalProject.Models.CustomUser;
using FinalProject.Models.Token;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace FinalProject.Models
{
    public class User : KeyEntities
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirm { get; set; } = false;
        public string? CodeEmailConfirm { get; set; }
        [ForeignKey("role")]
        public string RoleId { get; set; }
        [ForeignKey("Addresss")]
        public string? IdAddress { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public DateTime CreateDateAt { get; set; }
        public bool isActive { get; set; }
        public Role role { get; set; }
        public Address? Addresss { get; set; }
    }
}
 