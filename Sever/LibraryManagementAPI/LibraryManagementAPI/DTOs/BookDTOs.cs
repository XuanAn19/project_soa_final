using LibraryManagementAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class BookDTO
    {
       
        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime PublishedDate { get; set; } = DateTime.Now;


  
        public int Price { get; set; }
        public int AvailableCopies { get; set; }
        public int GenreID { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
