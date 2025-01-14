using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models.Token
{
    public class UserToken : KeyEntities
    {
        public string UserId { get; set; }
        public string Accesstoken { get; set; }
        public DateTime ExpiredAccess { get; set; }
        public string CodeRefreshToken { get; set; }
        public string Refreshtoken { get; set; }
        public DateTime ExpiredRefresh { get; set; }
        public DateTime CreateDateRefresh { get; set; }
        public bool IsRevoked { get; set; }
    }
}
