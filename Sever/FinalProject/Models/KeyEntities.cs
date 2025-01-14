using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class KeyEntities
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public string Id { get; set; }
        public KeyEntities()
        {
 
            Id = GenerateRandomString();
        }

        private string GenerateRandomString()
        {
            // Tạo chuỗi ngẫu nhiên
            return Guid.NewGuid().ToString("N");
        }
    }
}
