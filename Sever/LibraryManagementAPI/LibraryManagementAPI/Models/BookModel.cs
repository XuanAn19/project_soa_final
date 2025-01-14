using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models
{
    public class BookModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Author { get; set; }

        public DateTime? PublishedDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá phải là giá trị dương.")]
        public int Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Available copies must be non-negative.")]
        public int AvailableCopies { get; set; } // Số lượng hiện có

        public int GenreID { get; set; }
        public GenresModel Genre { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public bool IsAvailable { get; set; } // Loại bỏ thuộc tính tính toán
    }
}
