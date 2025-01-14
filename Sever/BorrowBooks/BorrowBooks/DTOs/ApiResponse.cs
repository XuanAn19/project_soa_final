namespace BorrowBooks.DTOs
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDTO Data { get; set; }  // Dữ liệu của user
    }
}
