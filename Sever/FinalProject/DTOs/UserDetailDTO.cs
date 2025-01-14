namespace FinalProject.DTOs
{
    public class UserDetailDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; } // Lấy tên role thay vì ID
        public string Address { get; set; } // Địa chỉ đầy đủ
        public DateTime LastLoggedIn { get; set; }
        public DateTime CreateDateAt { get; set; }
       
    }
}
