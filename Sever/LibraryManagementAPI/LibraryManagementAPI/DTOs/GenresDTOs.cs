using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class GenresDTO
    {
        [Key]
        public int GenreID { get; set; }

        [Required]
        [MaxLength(255)]
        public string GenreName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
    }
}
