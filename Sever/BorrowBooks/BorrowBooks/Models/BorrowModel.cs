using System.ComponentModel.DataAnnotations;

namespace BorrowBooks.Models
{
    public class BorrowModel
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public bool IsTrue { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now; // Ngày mượn
        public DateTime? ReturnDate { get; set; } 

    }
}
