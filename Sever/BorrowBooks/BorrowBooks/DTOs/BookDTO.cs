using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BorrowBooks.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime? PublishedDate { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int Price { get; set; }
        public int AvailableCopies { get; set; }

        public int GenreID { get; set; }

        public string? Image { get; set; }

    }
}
