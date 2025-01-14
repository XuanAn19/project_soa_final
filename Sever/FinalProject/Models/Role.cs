using System.Text.Json.Serialization;

namespace FinalProject.Models
{
    public class Role : KeyEntities
    {
        public string roleName { get; set; }
        public string description { get; set; }
        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
